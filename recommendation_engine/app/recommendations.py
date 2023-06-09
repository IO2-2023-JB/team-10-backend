import os

import numpy as np
import pandas as pd
import pymongo
from bson.objectid import ObjectId

from app.models import Recommendation
from app.recommendations_parameters import (
    DEPTH_PENALTY,
    HISTORY_COLLECTION_NAME,
    HISTORY_DEPTH,
    REACTION_COLLECTION_NAME,
    SUBSCRIPTION_COLLECTION_NAME,
    USER_COLLECTION_NAME,
    VIDEO_COLLECTION_NAME,
)


class Recommendations:
    def __init__(self):
        self.db_client = pymongo.MongoClient(os.getenv("DATABASE_URL"))
        self.db = self.db_client[os.getenv("DATABASE_NAME")]

    def generate_recommendations(self, user_id: str) -> list[Recommendation]:
        user_id = ObjectId(user_id)
        self.collect_data()
        if (
            len(
                self.history_collection[
                    self.history_collection["_id"] == ObjectId(user_id)
                ].reset_index()["WatchedVideos"][0]
            )
            == 0
        ):
            return []
        self.preprocess_user_history_data(user_id)
        self.calculate_movie_similarity_matrix()
        recommendations = self.find_recommendations_by_tags_similarity()
        result = list()
        for i in range(len(recommendations)):
            result.append(Recommendation(video_id=str(recommendations["VideoId"][i])))
        return result

    def collect_data(self):
        self.video_collection = pd.DataFrame(
            list(self.db[VIDEO_COLLECTION_NAME].find())
        )
        self.user_collection = pd.DataFrame(list(self.db[USER_COLLECTION_NAME].find()))
        self.reaction_collection = pd.DataFrame(
            list(self.db[REACTION_COLLECTION_NAME].find())
        )
        self.subscription_collection = pd.DataFrame(
            list(self.db[SUBSCRIPTION_COLLECTION_NAME].find())
        )
        self.history_collection = pd.DataFrame(
            list(self.db[HISTORY_COLLECTION_NAME].find())
        )

    def preprocess_user_history_data(self, user_id: ObjectId):
        user_history_index = self.history_collection["WatchedVideos"][
            self.history_collection["_id"] == user_id
        ].index[0]
        user_subscriptions = self.subscription_collection[
            self.subscription_collection["SubscriberId"] == user_id
        ]
        user_reactions = self.reaction_collection[
            self.reaction_collection["UserId"] == user_id
        ]
        watched_video = pd.DataFrame(
            self.history_collection["WatchedVideos"][user_history_index].copy()
        )
        user_watched_videos = watched_video.merge(
            self.video_collection, left_on="VideoId", right_on="_id"
        )
        user_watched_videos = user_watched_videos.merge(
            user_reactions, left_on="VideoId", right_on="VideoId", how="left"
        )
        user_watched_videos = user_watched_videos.merge(
            user_subscriptions, left_on="AuthorId", right_on="CreatorId", how="left"
        )
        user_watched_videos = user_watched_videos.loc[
            :, ["VideoId", "Date", "Tags", "ReactionType", "SubscriberId"]
        ]

        user_watched_videos["Subscribes"] = user_watched_videos.loc[
            :, "SubscriberId"
        ].transform(lambda x: not pd.isnull(x))
        user_watched_videos = user_watched_videos.drop(columns=["SubscriberId"])
        self.user_data = user_watched_videos

    def calculate_movie_similarity_matrix(self):
        video_count = len(self.video_collection["AuthorId"])
        movie_matrix = np.zeros((video_count, video_count), float)
        for i in range(video_count):
            for j in range(video_count):
                tags1, tags2 = (
                    self.video_collection["Tags"][i],
                    self.video_collection["Tags"][j],
                )
                min_length = min(len(tags1), len(tags2))
                if i == j or min_length == 0:
                    continue
                movie_matrix[i, j] = len(np.intersect1d(tags1, tags2)) / min_length
        self.video_matrix = movie_matrix

    def find_recommendations_by_tags_similarity(self):
        user_history = (
            self.user_data.drop_duplicates(subset="VideoId", keep="last")
            .reset_index()
            .drop_duplicates(subset="VideoId", keep="first")
        ).reset_index()
        recommended = pd.DataFrame(columns=["VideoId", "score"])
        history_length = len(user_history)
        recently_watched_indexes = self.video_collection[
            self.video_collection["_id"].isin(
                user_history.head(HISTORY_DEPTH)["VideoId"]
            )
        ].index
        for i in range(min(history_length, HISTORY_DEPTH)):
            video_index = self.video_collection[
                self.video_collection["_id"] == user_history["VideoId"][i]
            ].index
            recently_watched_indexes.append(video_index)
            for j in range(len(self.video_matrix)):
                if (
                    self.video_matrix[video_index, j] > 0
                    and not j in recently_watched_indexes
                ):
                    recommended.loc[len(recommended)] = {
                        "VideoId": self.video_collection["_id"][j],
                        "score": self.video_matrix[video_index, j]
                        / ((i + 1) * DEPTH_PENALTY),
                    }
        return (
            recommended.drop_duplicates(subset="VideoId", keep="first")
            .sort_values(by="score", ascending=False)
            .reset_index()
        )

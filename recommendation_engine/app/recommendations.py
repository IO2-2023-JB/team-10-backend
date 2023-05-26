from app.models import Recommendation
import numpy as np
import pandas as pd
import pymongo
from app.const import (
    DATABASE_URL,
    DATABASE_NAME,
    USER_COLLECTION_NAME,
    VIDEO_COLLECTION_NAME,
    REACTION_COLLECTION_NAME,
    SUBSCRIPTION_COLLECTION_NAME,
    HISTORY_COLLECTION_NAME,
)
from bson.objectid import ObjectId

class Recommendations:
    def __init__(self):
        self.db_client = pymongo.MongoClient(DATABASE_URL)
        self.db = self.db_client[DATABASE_NAME]

    def generate_recommendations(self, user_id: str) -> list[Recommendation]:
        self.collect_data()
        self.preprocess_user_history_data(user_id)
        print(self.user_data)
        return [
            Recommendation(video_id="test id 1"),
            Recommendation(video_id="test id 2"),
            Recommendation(video_id="test id 3"),
        ]

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

    def preprocess_user_history_data(
        self,
        user_id: str,
    ):
        user_id = ObjectId("6425dbadc7611003a102a7ab")
        user_history_index = self.history_collection["WatchedVideos"][
            self.history_collection["_id"] == user_id
        ].index[0]
        # todo: do not use subscriptions? we already implemented subs page... (maybe hide subscribed channels?)
        user_subscriptions = self.subscription_collection[
            self.subscription_collection["SubscriberId"] == user_id
        ]
        user_reactions = self.reaction_collection[
            self.reaction_collection["UserId"] == user_id
        ]
        watched_video = pd.DataFrame(
            self.history_collection["WatchedVideos"][user_history_index].copy()
        )
        user_watched_videos = (
            watched_video.merge(
                self.video_collection, left_on="VideoId", right_on="_id"
            )
            .merge(user_reactions, left_on="VideoId", right_on="VideoId", how="left")
            .merge(
                user_subscriptions, left_on="AuthorId", right_on="CreatorId", how="left"
            )
            .loc[:, ["Date", "Tags_x", "ReactionType", "SubscriberId"]]
        )
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
                    print(min_length)
                    continue
                movie_matrix[i, j] = len(np.intersect1d(tags1, tags2)) / min_length
        self.movie_matrix = movie_matrix

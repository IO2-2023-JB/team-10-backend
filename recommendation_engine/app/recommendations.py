from recommendation_engine.app.models import Recommendation


def generate_recommendations(user_id: str) -> list[Recommendation]:
    return [
        Recommendation(video_id="test id 1"),
        Recommendation(video_id="test id 2"),
        Recommendation(video_id="test id 3"),
    ]

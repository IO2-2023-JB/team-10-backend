import uvicorn
from fastapi import FastAPI

from app.models import Recommendation
from app.recommendations import generate_recommendations

app = FastAPI()


@app.get("/recommendations/{user_id}")
async def get_recommendations(user_id: str) -> list[Recommendation]:
    return generate_recommendations(user_id)


def main():
    uvicorn.run(app, port=8000)


if __name__ == "__main__":
    main()

import uvicorn
from dotenv import load_dotenv
from fastapi import FastAPI
import os
from app.models import Recommendation
from app.recommendations import Recommendations

app = FastAPI()
if os.getenv("IO2_ENVIRONMENT") == "Production":
    load_dotenv("./.env.production")
else:
    load_dotenv("./.env.development")


@app.get("/recommendations/{user_id}")
async def get_recommendations(user_id: str) -> list[Recommendation]:
    recommendations = Recommendations()
    return recommendations.generate_recommendations(user_id)


def main():
    uvicorn.run(app, port=8000)


if __name__ == "__main__":
    main()

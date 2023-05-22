import uvicorn
from fastapi import FastAPI

app = FastAPI()


@app.get("/")
async def hello_world():
    return {"message": "Hello World"}


def main():
    uvicorn.run(app, port=8000)


if __name__ == "__main__":
    main()

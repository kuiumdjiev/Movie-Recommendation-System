# Movie Recommendation System (MRS)

## Description
An intelligent movie recommendation system based on machine learning.

## Technologies
- **C# (ASP.NET Core)** – backend and database
- **Python** – machine learning
- **SQL Server** – database
- **TMDb API** – movie information

## Structure
```
MovieRecommendationSystem/
│── CSharp_Backend/  # Backend (C#)
│── Python_ML/       # Recommendations (Python)
│── README.md        # Description
```

## Database Setup
It is recommended to create two separate profiles for the database – one for C# and one for Python.

## API Endpoints
- **C# API**: `GET /api/movies`, `POST /api/ratings`, `GET /api/recommendations/{userId}`
- **Python API**: `GET /recommend?user_id=X&movie_id=Y`

## Usage
1. Register in the web application
2. Rate movies
3. Get recommendations

## Future Improvements
- Improved model accuracy
- More user settings
- Expansion to other types of content

## Author
Stefan Ivanov Kuyumdjiev – PMG "Vasil Drumev", Veliko Tarnovo

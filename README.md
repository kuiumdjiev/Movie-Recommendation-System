# Movie Recommendation System
This repository contains the code for a movie recommendation system implemented in Python. The system combines content-based filtering using the Jaccard metric and a machine learning model based on user ratings.

# Overview
The goal of this project is to create a movie recommendation system that suggests movies to users based on their preferences. The system incorporates two main components:

# 1 Content-Based Filtering using Jaccard Metric:

  The system utilizes the Jaccard metric to measure the similarity between movies based on their genres. When a user provides a favorite movie, the system recommends similar movies by calculating Jaccard distances.
# 2 Machine Learning Model for Rating Prediction:

  A machine learning model is employed to predict user ratings for movies. The model takes into account various features, including movie popularity, vote average, vote count, runtime, revenue, and language. DecisionTreeRegressor is used as the primary model.
# Data Sources
The project uses the MovieLens 20M dataset, which consists of 20 million ratings given to 27,000 movies by 138,000 users. The relevant files used are ratings.csv, movies.csv, and links.csv. Additionally, movie details are fetched from The Movie Database (TMDB) using the TMDB API.
Shield: [![CC BY-NC-ND 4.0][cc-by-nc-nd-shield]][cc-by-nc-nd]

This work is licensed under a
[Creative Commons Attribution-NonCommercial-NoDerivs 4.0 International License][cc-by-nc-nd].

[![CC BY-NC-ND 4.0][cc-by-nc-nd-image]][cc-by-nc-nd]

[cc-by-nc-nd]: http://creativecommons.org/licenses/by-nc-nd/4.0/
[cc-by-nc-nd-image]: https://licensebuttons.net/l/by-nc-nd/4.0/88x31.png
[cc-by-nc-nd-shield]: https://img.shields.io/badge/License-CC%20BY--NC--ND%204.0-lightgrey.svg
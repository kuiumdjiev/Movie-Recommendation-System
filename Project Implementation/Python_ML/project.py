from sqlalchemy import ForeignKey
from sqlalchemy.orm import relationship
from sqlalchemy import Column, Integer, String, Float, DateTime, Boolean
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker
from datetime import datetime
from sqlalchemy import MetaData
from sqlalchemy import Table
from sqlalchemy import select
from sqlalchemy import and_
import numpy as np
from sqlalchemy import create_engine, MetaData
from sqlalchemy.engine import URL
import pandas as pd
from sklearn.preprocessing import StandardScaler
from scipy.spatial.distance import pdist
from scipy.sparse import csr_matrix
from sklearn.metrics import pairwise_distances
from sklearn.model_selection import train_test_split
from sklearn.tree import DecisionTreeRegressor
from flask import Flask, request, jsonify
from apscheduler.schedulers.background import BackgroundScheduler
import pandas as pd
import numpy as np
import threading

class DatabaseConnector:
    def __init__(self, host, db, driver="ODBC Driver 17 for SQL Server"):
        connection_string = (
            f"DRIVER={driver};"
            f"SERVER={host};"
           # f"PORT=1433;"
            f"DATABASE={db};"
            f"Trusted_Connection=yes;"
            f"autocommit=true;"
            f"User Id=ML;"
            f"Password=123;"
        )
        
        connection_url = URL.create("mssql+pyodbc", query={"odbc_connect": connection_string})
        
        self.engine = create_engine(connection_url, use_setinputsizes=False, echo=False)
        self.metadata = MetaData()

    def reflect_tables(self):
        """Отразява съществуващите таблици от базата данни."""
        self.metadata.reflect(bind=self.engine)
        return self.metadata.tables

    def print_table_info(self):
        """Извежда информация за таблиците и колоните."""
        tables = self.reflect_tables()
        for table_name, table in tables.items():
            print(f"Таблица: {table_name}")
            for column in table.columns:
                print(f"  Колона: {column.name}, Тип: {column.type}")

if __name__ == "__main__":
    host = "localhost"
    db = "MRS"

    connector = DatabaseConnector(host, db)
    connector.print_table_info()

def get_recommendation_by_movie(movie_id):
    temp_movies = movies.copy()  
    temp_movies = temp_movies.drop(['Title'], axis=1)
    temp_movies.iloc[:,0:] = temp_movies.iloc[:,0:].astype('int')

    ids = temp_movies['Id'].copy()
    movie  = temp_movies[temp_movies['Id']==movie_id].copy()

    movie.drop(columns=['Id'],inplace=True)
    temp_movies.drop(columns=['Id'],inplace=True)
    
    genre_cols = ['Adventure', 'Animation', 'Children', 'Comedy', 'Fantasy',
                  'Romance', 'Drama', 'Action', 'Crime', 'Thriller', 'Horror',
                  'Mystery', 'SciFi', 'IMAX', 'Documentary', 'War', 'Musical',
                  'Western', 'FilmNoir']
    
    movie_genres = movie[genre_cols].copy().values.astype(bool)
    target_genres = temp_movies[genre_cols].copy().values.astype(bool)

    similarity_matrix_genre = pairwise_distances(target_genres, movie_genres, metric='jaccard')
    similarity_score_genre = pd.Series(similarity_matrix_genre.flatten())

    movie_num = movie[['Popularity', 'VoteAverage', 'Runtime','Year','VoteCount','Revenue']]
    target_num = temp_movies[['Popularity', 'VoteAverage', 'Runtime','Year','VoteCount','Revenue']]

    similarity_matrix_num = pairwise_distances(target_num.values, movie_num.values, metric='euclidean')
    similarity_score_num = pd.Series(similarity_matrix_num.flatten())

    temp_movies['similarity_score_genre'] = similarity_score_genre
    temp_movies['similarity_score_num'] = similarity_score_num
    temp_movies['Id'] =ids

    temp_movies = temp_movies.sort_values(['similarity_score_genre','Popularity','similarity_score_num'], ascending=[True,False,True]).reset_index(drop=True)
    temp_movies.drop(columns=['similarity_score_genre','similarity_score_num'], inplace=True)

    temp_movies['Id'] = temp_movies['Id'].astype(int)

    first = temp_movies[temp_movies['Id']==movie_id].copy()
    temp_movies.drop(first.index,inplace=True)
    temp_movies.reset_index(drop=True, inplace=True)
    print(first)
    temp_movies=pd.concat([ first,temp_movies ],ignore_index=True)

    return temp_movies.head(11), temp_movies['Id'].head(11)


movies = pd.read_csv('movies.csv')
movies['Popularity'].apply(np.ceil)
movies['VoteCount'].apply(np.ceil)
movies['Runtime'].apply(np.ceil)
movies['VoteAverage'].apply(np.ceil)
scaler_popularity = StandardScaler()
scaler_vote_count = StandardScaler()
scaler_runtime = StandardScaler()
scaler_vote_average = StandardScaler()


movies['Popularity'] = scaler_popularity.fit_transform(movies[['Popularity']])
movies['VoteCount'] = scaler_vote_count.fit_transform(movies[['VoteCount']])
movies['Runtime'] = scaler_runtime.fit_transform(movies[['Runtime']])
movies['VoteAverage'] = scaler_vote_average.fit_transform(movies[['VoteAverage']])

data = pd.read_sql_table('UserMovies', connector.engine)
data=data.join(movies.set_index('Id'), on='MovieId')
data.drop(['MovieId'], axis=1, inplace=True)

data['Popularity'].apply(np.ceil)
data['VoteCount'].apply(np.ceil)
data['Runtime'].apply(np.ceil)
data['VoteAverage'].apply(np.ceil)
data['Score'].apply(np.ceil)

app = Flask(__name__)



data = None
last_date = None
model = DecisionTreeRegressor(max_depth=14, min_samples_split=900)
lock = threading.Lock()
processed_data= None

def fetch_new_data():
    global data, scaler_popularity, scaler_vote_count, scaler_runtime, scaler_vote_average, model, movies, last_date, processed_data, dtype_mapping

    locked_acquired = lock.acquire(blocking=False)
    if not locked_acquired:
        print("fetch_new_data is already running. Skipping execution.")
        return

    try:
        # Зареждаме данните от таблицата UserMovies
        fetched_data = pd.read_sql_table('UserMovies', connector.engine)
        # Обединяваме с movies по MovieId = Id. Използваме merge за коректно съвпадение.
        fetched_data = pd.merge(fetched_data, movies, left_on='MovieId', right_on='Id', how='left')
        # Ако вече нямате нужда от оригиналния идентификатор, премахваме го.
        fetched_data.drop(['MovieId'], axis=1, inplace=True)

        if data is None or data.empty:
            data = fetched_data.copy()
            last_date = data['CreationTime'].max()
            new_data = fetched_data
        else:
            # Използваме последната дата от data, за да филтрираме новите записи
            last_date = data['CreationTime'].max()
            new_data = fetched_data[fetched_data['CreationTime'] > last_date]

        if len(new_data) > 0:
            numeric_columns = ['Popularity', 'VoteCount', 'Runtime', 'VoteAverage']
            # Приложение на np.ceil върху числовите колони
            new_data[numeric_columns] = new_data[numeric_columns].apply(np.ceil)

            scalers = {
                'Popularity': scaler_popularity,
                'VoteCount': scaler_vote_count,
                'Runtime': scaler_runtime,
                'VoteAverage': scaler_vote_average
            }

            for col, scaler in scalers.items():
                if scaler is not None:
                    new_data[col] = scaler.transform(new_data[[col]])
            
            # Допълнително прилагаме np.ceil (ако е нужно)
            new_data[numeric_columns] = new_data[numeric_columns].apply(np.ceil)
            print(new_data.columns)
            data = pd.concat([data, new_data], ignore_index=True)
            #print(data)
            new_train_x = data.drop(columns=['Score', 'CreationTime', 'Title','Id'])
            new_train_y = data['Score']
            model.fit(new_train_x.values, new_train_y.values)
            print(f"Processed {len(new_data)} new records")
        else:
            print("No new data to process")
    finally:
        if locked_acquired:
            lock.release()

fetch_new_data()

# Scheduler to fetch new data every 2 minutes
scheduler = BackgroundScheduler()
scheduler.add_job(fetch_new_data, 'interval', minutes=2, max_instances=1, coalesce=True, replace_existing=True)
scheduler.start()

@app.route('/recommend', methods=['GET'])
def recommend():
    user_id = request.args.get('user_id')
    movie_id = request.args.get('movie_id')

    if user_id is None or movie_id is None:
        return jsonify({"error": "Please provide both user_id and movie_id"}), 400
    
    try:
        user_id = int(user_id)
        movie_id = int(movie_id)
    except ValueError:
        return jsonify({"error": "user_id and movie_id must be integers"}), 400
    
    recommendations, indexs = get_recommendation_by_movie(movie_id)
    recommendations.insert(0, 'UserId', user_id)
    #print(recommendations)
    print(recommendations.columns)

    if user_id != -1:        
        recommendations['Score'] = model.predict(recommendations.drop(columns=['Id']).values)
    else :
        recommendations['Score'] = 0
 
    recommendations['Id'] = indexs
    recommendations['Title'] = movies.set_index('Id').loc[indexs, 'Title'].values
    return recommendations.to_json(orient='records')

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5059)

using Microsoft.EntityFrameworkCore;
using MRS.Core.Contracts;
using MRS.Core.Model.Movie;
using MRS.Infrastructure.Data;
using MRS.Infrastructure.Data.Identity;
using MRS.Infrastructure.Data.Repositories;
using System.Net.Http.Headers;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace MRS.Core.Service;

public class MovieService : IMovieService
{
    private readonly IApplicatioDbRepository repository;
    private readonly ILoadService loadService;
    public MovieService(IApplicatioDbRepository repository, ILoadService loadService)
    {
        this.repository = repository;
        this.loadService = loadService;
    }

    // Fetches detailed information about a movie, including recommendations and user-specific data.
    public async Task<DetailsMovie> DetailsMovie(int id, string userName)
    {
        int? userId = -1;
        var movies = await loadService.LoadMovies();
        var  movie = movies.FirstOrDefault(x=>x.Id==id);

        var detaildMovie = new DetailsMovie
        {
            Id = movie.Id,
            Title = movie.Title,
            Movies = new List<MovieRecommendation>()
        };

        var users =await this.loadService.LoadUsers();
        var user =users
            .FirstOrDefault(x => x.UserName == userName);
            

        if (user!=null)
        {
            userId = user.Id;
        }


        string url = $"http://127.0.0.1:5059/recommend?user_id={userId}&movie_id={id}";

        try
        {
            HttpClient client = new HttpClient();
            string responseBody = await client.GetStringAsync(url);
            List<MovieRecommendation> recommendations = JsonSerializer.Deserialize<List<MovieRecommendation>>(responseBody);

            foreach (var recommendation in recommendations)
            {
                detaildMovie.Movies.Add(recommendation);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching recommendations: {ex.Message}");
        }

         url = $"https://api.themoviedb.org/3/movie/{id}?language=bg-BG";
        try
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiIyODNhZmZkN2JjNGU4MmVlMzIwMGRjNTllMDgyMjBmMyIsIm5iZiI6MTcwMDcyNjUwMC40NTgsInN1YiI6IjY1NWYwNmU0MmIxMTNkMDBlYmE2OWU2NyIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.-0mqnsu74PpAsaR7oY83RAIzHTx5it7Mjh_Rm1-MsM0");
            var responseBody = await client.GetStringAsync(url);
            TheMovieDBMovieDetails details = JsonSerializer.Deserialize<TheMovieDBMovieDetails>(responseBody);
            detaildMovie.Img = "https://image.tmdb.org/t/p/w300_and_h450_bestv2/" + details.backdrop_path;
            detaildMovie.Overview = details.overview;

        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching img and overview: {e.Message}");
        }
        detaildMovie.Score = detaildMovie.Movies.First(x=>x.Title == detaildMovie.Title).Score;
        detaildMovie.Movies.RemoveAll(x => x.Title == detaildMovie.Title);
        if (userId == null)
        {
            detaildMovie.Score = 0;
            foreach(var recommendation in detaildMovie.Movies)
            {
                recommendation.Score =0;
            }
        }
        else
        {
            if (await CheckIfUserAlreadyRateMovie(id, userName))
            {
                detaildMovie.IsAlreadyLiked = true;
                var userMovies = await this.loadService.LoadUserMovies();
               
                var userMovie = userMovies
                    .FirstOrDefault(x => x.UserId == user.Id && x.MovieId == id);
                detaildMovie.Score = userMovie.Score;
            }
        }

        return detaildMovie;
    }

    // Checks if a user has already rated a specific movie.
    public async Task<bool> CheckIfUserAlreadyRateMovie(int id, string username)
    {
        var users = await this.loadService.LoadUsers();
        var user = users
            .FirstOrDefault(x => x.Email == username);
        var movies = await this.loadService.LoadMovies();
        var movie = movies
            .FirstOrDefault(x => x.Id == id);
        if (user==null)
        {
            return false;   
        }
        var userMovie = await this.loadService.LoadUserMovies();
        var userMovieExists = userMovie
            .Any(x => x.UserId == user.Id && x.MovieId == movie.Id);
        if (userMovieExists)
        {
            return true;
        }
        return false;
    }

    // Retrieves movie details for rating purposes, ensuring the user hasn't already rated it.
    public async Task<(LikeMovie,bool)> GetMovieDetailsForRate(int id, string username)
    {
        var movies = await this.loadService.LoadMovies();
        var movie = movies
            .FirstOrDefault(x => x.Id == id);
        var likeMovie = new LikeMovie
        {
            Id = movie.Id,
            Name = movie.Title
        };

        if (await this.CheckIfUserAlreadyRateMovie(id,username))
        {
            return (null, false);
        }

        return (likeMovie,true);
    }

    // Allows a user to initially like a movie with a default score.
    public async Task LikeInitialMovie(int id, string userName)
    {

        var users = await this.loadService.LoadUsers();
        var user = users
            .FirstOrDefault(x => x.UserName == userName);
        var movies = await this.loadService.LoadMovies();
        var movie = movies
            .FirstOrDefault(x => x.Id == id);

        if (await this.CheckIfUserAlreadyRateMovie(id, userName))
        {
            throw new ArgumentException();
        }

        var recommendedMovie = new UserMovie
        {
            MovieId = movie.Id,
            UserId = user.Id,
            Score = 5,
            CreationTime = DateTime.Now
        };
        await this.loadService.AddUserMovie(recommendedMovie);
    }

    // Allows a user to like a movie with a specific rating.
    public async Task<bool> LikeMovie(LikeMovie likeMovie, string userName)
    {
        var users = await this.loadService.LoadUsers();
        var user = users
            .FirstOrDefault(x => x.UserName == userName);
       var movies = await this.loadService.LoadMovies();
        var movie = movies
            .FirstOrDefault(x => x.Id == likeMovie.Id);

        if (await this.CheckIfUserAlreadyRateMovie(likeMovie.Id, userName))
        {
            return false;
        }

        var recommendedMovie = new UserMovie
        {
            MovieId = movie.Id,
            UserId = user.Id,
            Score = likeMovie.Raiting,
            CreationTime = DateTime.Now
        };
            await this.loadService.AddUserMovie(recommendedMovie);
        return true;
    }

    // Retrieves a list of available movie genres.
    public MovieGenresViewModel[] GetGenres()
    {
        MovieGenresViewModel[] genres = new MovieGenresViewModel[]
        {
            new MovieGenresViewModel { Name = "Adventure" },
            new MovieGenresViewModel { Name = "Animation" },
            new MovieGenresViewModel { Name = "Children" },
            new MovieGenresViewModel { Name = "Comedy" },
            new MovieGenresViewModel { Name = "Fantasy" },
            new MovieGenresViewModel { Name = "Romance" },
            new MovieGenresViewModel { Name = "Drama" },
            new MovieGenresViewModel { Name = "Action" },
            new MovieGenresViewModel { Name = "Crime" },
            new MovieGenresViewModel { Name = "Thriller" },
            new MovieGenresViewModel { Name = "Horror" },
            new MovieGenresViewModel { Name = "Mystery" },
            new MovieGenresViewModel { Name = "SciFi" },
            new MovieGenresViewModel { Name = "IMAX" },
            new MovieGenresViewModel { Name = "Documentary" },
            new MovieGenresViewModel { Name = "War" },
            new MovieGenresViewModel { Name = "Musical" },
            new MovieGenresViewModel { Name = "Western" },
            new MovieGenresViewModel { Name = "FilmNoir" }
        };
        return genres;
    }

    // Retrieves movies of a specific genre, paginated.
    public async Task<MoviesPerPageGenre> GetMoviesPerPageGenre(string genre, int page)
    {
        var movies = await this.loadService.LoadMovies();

        // Dictionary to map genre names to their corresponding properties
        var genreProperties = new Dictionary<string, Func<Movie, bool>>
        {
            { "Adventure", x => x.Adventure },
            { "Animation", x => x.Animation },
            { "Children", x => x.Children },
            { "Comedy", x => x.Comedy },
            { "Fantasy", x => x.Fantasy },
            { "Romance", x => x.Romance },
            { "Drama", x => x.Drama },
            { "Action", x => x.Action },
            { "Crime", x => x.Crime },
            { "Thriller", x => x.Thriller },
            { "Horror", x => x.Horror },
            { "Mystery", x => x.Mystery },
            { "SciFi", x => x.SciFi },
            { "IMAX", x => x.IMAX },
            { "Documentary", x => x.Documentary },
            { "War", x => x.War },
            { "Musical", x => x.Musical },
            { "Western", x => x.Western },
            { "FilmNoir", x => x.FilmNoir }
        };

        if (!genreProperties.ContainsKey(genre))
        {
            throw new ArgumentException("Invalid genre", nameof(genre));
        }

        var filteredMovies = movies
            .Where(genreProperties[genre])
            .OrderByDescending(x => x.Popularity)
            .ThenByDescending(x => x.VoteAverage)
            .Skip((page - 1) * 20)
            .Take(20)
            .Select(x => new GenresMovie
            {
                Id = x.Id,
                Name = x.Title
            })
            .ToList();

        int totalPages = (int)Math.Ceiling((double)movies.Count(genreProperties[genre]) / 20);

        var moviesPerPageGenre = new MoviesPerPageGenre
        {
            Genre = genre,
            Movies = filteredMovies,
            Page = page,
            TotalPages = totalPages
        };

        return moviesPerPageGenre;
    }

    // Retrieves the ID of a movie by its name.
    public async Task<int> GetByName(string name)
    {
        var movies = await this.loadService.LoadMovies();
        var movie = movies
            .FirstOrDefault(x=>x.Title.Contains(name));

        
        return movie.Id;
    }

    // Retrieves a list of movie names and their IDs.
    public async Task<SearchMovie[]> GetNames()
    {

        var movies = await this.loadService.LoadMovies();
        var names = movies
            .Select(x => new SearchMovie { Id = x.Id, Name = x.Title })
            .ToArray();
            
        return names;
    }
    
}
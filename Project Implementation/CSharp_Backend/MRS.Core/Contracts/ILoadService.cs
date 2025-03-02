using MRS.Infrastructure.Data;
using MRS.Infrastructure.Data.Identity;

namespace MRS.Core.Contracts;

public interface ILoadService
{
    // Loads all movies from the database or cache.
    Task<List<Movie>> LoadMovies();
    
    // Loads all user-movie relationships from the database or cache.
    Task<List<UserMovie>> LoadUserMovies();
    
    // Loads all users from the database or cache.
    Task<List<ApplicationUser>> LoadUsers();
    
    // Adds a user-movie relationship to the database and updates the cache.
    Task AddUserMovie(UserMovie userMovie);
    
    // Loads a specific movie by its ID from the database or cache.
    Task<Movie> LoadMovieById(int id);
    
    // Reloads all cached data.
    Task Reload();
}
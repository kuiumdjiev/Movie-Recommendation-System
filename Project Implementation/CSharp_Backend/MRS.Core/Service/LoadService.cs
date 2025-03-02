using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MRS.Core.Contracts;
using MRS.Infrastructure.Data;
using MRS.Infrastructure.Data.Identity;
using MRS.Infrastructure.Data.Repositories;

namespace MRS.Core.Service;

public class LoadService: ILoadService
{

    private readonly IApplicatioDbRepository repository;
    private readonly IMemoryCache _memoryCache;

    public LoadService(IApplicatioDbRepository repository, IMemoryCache memoryCache)
    {
        this.repository = repository;
        _memoryCache = memoryCache;
    }

    // Loads all movies from the database or cache.
    public async Task<List<Movie>> LoadMovies()
    {
        List<Movie> movies;
        if (!_memoryCache.TryGetValue("Movies", out movies))
        {
            movies =await this.repository.All<Movie>().ToListAsync();
            _memoryCache.Set("Movies", movies, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
        }

        return movies;
    }

    // Loads all user-movie relationships from the database or cache.
    public async Task<List<UserMovie>> LoadUserMovies()
    {
        List<UserMovie> userMovies;
        if (!_memoryCache.TryGetValue("UserMovies", out userMovies))
        {
            userMovies = await this.repository.All<UserMovie>().ToListAsync();
            _memoryCache.Set("UserMovies", userMovies, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
        }
        return userMovies;
    }

    // Loads all users from the database or cache.
    public async Task<List<ApplicationUser>> LoadUsers()
    {
        List<ApplicationUser> users;
        if (!_memoryCache.TryGetValue("Users", out users))
        {
            users = await this.repository.All<ApplicationUser>().ToListAsync();
            _memoryCache.Set("Users", users, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
        }
        return users;
    }

    // Adds a user-movie relationship to the database and updates the cache.
    public async Task AddUserMovie(UserMovie userMovie)
    {
        var userMovies = await LoadUserMovies();
        userMovies.Add(userMovie);
        _memoryCache.Set("UserMovies", userMovies, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });

        await this.repository.AddAsync(userMovie);
        await this.repository.SaveChangesAsync();
    }

    // Reloads all cached data.
    public async Task Reload()
    {
        await LoadUserMovies();
        await LoadMovies();
        await LoadUsers();
    }

    // Loads a specific movie by its ID from the database or cache.
    public async Task<Movie> LoadMovieById(int id)
    {
        var movies = await LoadMovies();
        return movies.FirstOrDefault(x => x.Id == id);
    }
}
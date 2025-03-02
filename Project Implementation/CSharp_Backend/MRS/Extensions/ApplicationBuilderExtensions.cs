

using MRS.Core.Model;
using MRS.Infrastructure.Data;
using MRS.Infrastructure.Data.Identity;

namespace MRS.Extensions;
using CsvHelper.Configuration;

using CsvHelper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder PrepareDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var services = serviceScope.ServiceProvider;
        SeedData(services);
        return app;
    }

    private static void SeedData(IServiceProvider services)
    {
        var data = services.GetRequiredService<MRS.Infrastructure.Data.ApplicationDbContext>();
        var movies = LoadMovies();

        using (var transaction = data.Database.BeginTransaction())
        {

            var userMovies = LoadUserMovies();
            var users = LoadUsers(userMovies);
            data.Database.ExecuteSqlRaw("SET IDENTITY_INSERT AspNetUsers ON");
            data.Users.AddRange(users);
            data.Database.ExecuteSqlRaw("SET IDENTITY_INSERT AspNetUsers OFF");
            data.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Movies ON");

            data.Movies.AddRange(movies);

              data.SaveChanges();
             movies = data.Movies.ToList(); // Ensure Movies are loaded first
             users = data.Users.ToList();

            var userMoviesFixed = new List<UserMovie>();

            foreach (var userMovie in userMovies)
            {
                userMovie.Movie = movies.FirstOrDefault(x => x.Id == userMovie.MovieId);
                userMovie.User = users.FirstOrDefault(x => x.Id == userMovie.UserId);



                if (userMovie.Movie != null && userMovie.User != null)
                {
                    userMoviesFixed.Add(userMovie);
                }

            }

            data.Database.ExecuteSqlRaw("SET IDENTITY_INSERT Movies OFF");

            data.UserMovies.AddRange(userMoviesFixed);

            data.SaveChanges();

            transaction.Commit();
        }

        ;
    }



    private static List<ApplicationUser> LoadUsers(List<UserMovie> usersRaitings)
    {
        var users = usersRaitings.Select(x => x.UserId)
            .Distinct()
            .Select(x => new ApplicationUser
            {
                UserName = x.ToString(),
            });

        return users.ToList();
    }

    private static List<Movie> LoadMovies()
    {
        using var reader = new StreamReader("C:\\Users\\Stefan\\Desktop\\Project\\Example\\MRS\\Data\\movies.csv");
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
           
        });

        var records = csv.GetRecords<Movie>().ToList();
        records = records.GroupBy(m => m.Id).Select(g => g.First()).ToList();

        return records;
    }

    private static List<UserMovie> LoadUserMovies()
    {
        using var reader = new StreamReader("C:\\Users\\Stefan\\Desktop\\Project\\Example\\MRS\\Data\\ratings.csv");
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {

        });

        var records = csv.GetRecords<ParserMovie>().ToList();
        records = records.GroupBy(m => new { m.UserId, m.MovieId })
            .Select(g => g.First())
            .ToList();


        int id = 1;
        var userMovies = records.Select(x => new UserMovie
        {
            MovieId = x.MovieId,
            UserId = x.UserId,
            Score = x.Score,
            CreationTime = DateTime.Now,
        }).ToList();

        return userMovies;
    }

}
using MRS.Core.Model.Movie;

namespace MRS.Core.Contracts;

public interface IMovieService
{
    Task<DetailsMovie> DetailsMovie(int id, string userName);
    Task<(LikeMovie,bool)> GetMovieDetailsForRate(int id, string username);
    Task LikeInitialMovie(int id, string userName);
    Task<bool> LikeMovie(LikeMovie likeMovie, string userName);
    Task<SearchMovie[]> GetNames();
    Task<bool> CheckIfUserAlreadyRateMovie(int id, string username);
    MovieGenresViewModel[] GetGenres();
    Task<MoviesPerPageGenre> GetMoviesPerPageGenre(string genre, int page);
    Task<int> GetByName(string name);
}
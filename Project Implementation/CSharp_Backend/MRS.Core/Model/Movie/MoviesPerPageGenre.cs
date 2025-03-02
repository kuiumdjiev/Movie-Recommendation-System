namespace MRS.Core.Model.Movie;

public class MoviesPerPageGenre
{
    public string Genre { get; set; }
    public int TotalPages { get; set; }
    public int Page { get; set; }
    public List<GenresMovie> Movies { get; set; }
}
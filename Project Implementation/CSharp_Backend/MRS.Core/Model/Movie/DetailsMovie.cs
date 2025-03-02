namespace MRS.Core.Model.Movie;

public class DetailsMovie
{
    public int Id { get; set; }

    public bool IsAlreadyLiked { get; set; }
    public string Title { get; set; }

    public double Score  { get; set; }

    public string Overview { get; set; }

    public string Img { get; set; }
    public List<MovieRecommendation> Movies { get; set; }
}
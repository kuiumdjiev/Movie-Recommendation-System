using System.ComponentModel.DataAnnotations;

namespace MRS.Infrastructure.Data
{
    public class Movie
    {
        [Key]
        public int Id { get; set; } 

        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public int Year { get; set; }
        [Required]
        public bool Adventure { get; set; }
        [Required]

        public bool Animation { get; set; }
        [Required]

        public bool Children { get; set; }
        [Required]

        public bool Comedy { get; set; }
        [Required]

        public bool Fantasy { get; set; }
        [Required]

        public bool Romance { get; set; }
        [Required]

        public bool Drama { get; set; }
        [Required]

        public bool Action { get; set; }
        [Required]

        public bool Crime { get; set; }
        [Required]

        public bool Thriller { get; set; }
        [Required]

        public bool Horror { get; set; }
        [Required]

        public bool Mystery { get; set; }
        [Required]

        public bool SciFi { get; set; }
        [Required]

        public bool IMAX { get; set; }
        [Required]

        public bool Documentary { get; set; }
        [Required]

        public bool War { get; set; }
        [Required]

        public bool Musical { get; set; }
        [Required]

        public bool Western { get; set; }
        [Required]

        public bool FilmNoir { get; set; }
        [Required]

        public bool NoGenresListed { get; set; }
        [Required]

        public double Popularity { get; set; }
        [Required]

        public double VoteAverage { get; set; }
        [Required]

        public double VoteCount { get; set; }
        [Required]

        public double Runtime { get; set; }
        public decimal Revenue { get; set; }
        [Required]

        public bool Language { get; set; } 

        public List<UserMovie> UserMovies { get; set; } = new List<UserMovie>();
    }
}
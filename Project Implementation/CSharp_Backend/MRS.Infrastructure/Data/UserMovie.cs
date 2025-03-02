using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.VisualBasic;
using MRS.Infrastructure.Data.Identity;

namespace MRS.Infrastructure.Data;

public class UserMovie
{

    public int MovieId { get; set; }
    [ForeignKey(nameof(MovieId))]
    public Movie Movie { get; set; }

    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }

    public double Score { get; set; }

    public DateTime CreationTime { get; set; }
}
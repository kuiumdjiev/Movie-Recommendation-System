using System.ComponentModel.DataAnnotations.Schema;

namespace MRS.Core.Model;

public class ParserMovie
{

    public int MovieId { get; set; }


    public int UserId { get; set; }


    public double Score { get; set; }
}
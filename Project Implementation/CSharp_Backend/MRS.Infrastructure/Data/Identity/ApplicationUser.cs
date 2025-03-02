using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MRS.Infrastructure.Data.Identity
{
 
    public class ApplicationUser : IdentityUser<int>
    {
        public List<UserMovie> UserMovies { get; set; } = new List<UserMovie>();
    }
}
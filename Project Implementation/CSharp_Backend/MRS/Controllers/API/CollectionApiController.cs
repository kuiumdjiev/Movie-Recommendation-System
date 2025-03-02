using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MRS.Core.Contracts;

namespace MRS.Controllers.API
{
    [ApiController]
    [Route("api/movie")]
    public class MovieApiController : ControllerBase
    {
        private readonly IMovieService movieService;

        public MovieApiController(IMovieService movieService)
        {
            this.movieService = movieService;
        }
        [HttpGet("all")]
        public async Task<IActionResult> All()
        {
            try
            {
                var data = await this.movieService.GetNames();
            return this.Ok(data);
            
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
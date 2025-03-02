using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MRS.Core.Contracts;
using MRS.Core.Model.Movie;
using MRS.Models;
using System.Diagnostics;

namespace MRS.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        public async Task<IActionResult> Details(string name)
        {
            try
            {
                int id = await _movieService.GetByName(name);
                string userId = User.Identity?.Name ?? "-1";

                var movie = await _movieService.DetailsMovie(id, userId);
                return View(movie);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult InitailLikes()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> InitailLikes(LikeMovie movie)
        {
            try
            {
                string username = User.Identity?.Name ?? throw new InvalidOperationException("User is not authenticated.");

                if (!await _movieService.LikeMovie(movie, username))
                {
                    return this.RedirectToAction("AlreadyLiked");

                }
                return View();
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Like(int id)
        {
            try
            {
                string name = User.Identity?.Name ?? throw new InvalidOperationException("User is not authenticated.");
                var (movie,check) = await _movieService.GetMovieDetailsForRate(id, name);
                if (!check)
                {
                    return this.RedirectToAction("AlreadyLiked");

                }
                return View(movie);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Like(LikeMovie likeMovie)
        {
            try
            {
                string name = User.Identity?.Name ?? throw new InvalidOperationException("User is not authenticated.");
                if (!await _movieService.LikeMovie(likeMovie, name))
                {
                    return this.RedirectToAction("AlreadyLiked");
                }
                name = likeMovie.Name;
                return Redirect($"/Movie/Details/?name={Uri.EscapeDataString(likeMovie.Name)}");


            }
            catch (Exception ex)
            {
                // Log the exception (ex) here
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public IActionResult Genres()
        {
            try
            {
                var genres = _movieService.GetGenres();
                return View(genres);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Genre(string genre, int page)
        {
            try
            {
                var movies = await _movieService.GetMoviesPerPageGenre(genre, page);
                return View(movies);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here
                return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AlreadyLiked()
        {
            return this.View();
        }
    }
}

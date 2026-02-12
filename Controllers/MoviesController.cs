using System;
using ApiMovies.Data;
using ApiMovies.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMovies.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController : ControllerBase
{
    // private readonly AppDbContext _context;

    // public MoviesController(AppDbContext context)
    // {
    //     _context = context;
    // }
    
    [HttpGet(Name = "GetMovies")]
    public async Task<ActionResult<IEnumerable<Movie>>> Get()
    {
        // var movies = await _context.Movies
        //     .AsNoTracking()
        //     .ToListAsync();

        // return Ok(movies);

        List<Movie> movies = new List<Movie>
        {
            new Movie { Id = 1, Title = "O homem que copiava", Director = "Jorge Furtado", ReleaseYear = 2003 },
            new Movie { Id = 2, Title = "O homem do futuro", Director = "Cláudio Torres", ReleaseYear = 2011 },
            new Movie { Id = 3, Title = "Carandiru", Director = "Héctor Babenco", ReleaseYear = 2003 }
        };

        return Ok(movies);
    }

    // [HttpGet("{id:int}")]
    // public async Task<ActionResult<Movie>> GetById(int id)
    // {
    //     var movie = await _context.Movies.FindAsync(id);

    //     if (movie == null)
    //         return NotFound();

    //     return Ok(movie);
    // }

    // // POST /movies
    // [HttpPost(Name = "PostMovie")]
    // public async Task<ActionResult<Movie>> Post(Movie movie)
    // {
    //     _context.Movies.Add(movie);
    //     await _context.SaveChangesAsync();

    //     return CreatedAtAction(
    //         nameof(GetById),
    //         new { id = movie.Id },
    //         movie
    //     );
    // }
}

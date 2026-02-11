using System;
using System.ComponentModel.DataAnnotations;

namespace ApiMovies.Models;

public class Movie
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; } = string.Empty;
    [Required]
    public string Director { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
}

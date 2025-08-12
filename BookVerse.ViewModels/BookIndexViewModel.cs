using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookVerse.ViewModels
{
    public class BookIndexViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? CoverImageUrl { get; set; }

        public string Genre { get; set; } = null!;

        public int SavedCount { get; set; }

        public bool IsAuthor { get; set; }

        public bool IsSaved { get; set; } 
    }
}

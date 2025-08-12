using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookVerse.GCommon.ValidationConstants;

namespace BookVerse.DataModels
{
    public class Genre
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(GenreNameMaxLength)]
        public string Name { get; set; } = null!;

        [Required]
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}

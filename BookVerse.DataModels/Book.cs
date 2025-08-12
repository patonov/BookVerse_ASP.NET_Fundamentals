using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookVerse.GCommon.ValidationConstants;

namespace BookVerse.DataModels
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(TitleNameMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; } = null!;

        public string? CoverImageUrl { get; set; }

        [Required]
        public string PublisherId { get; set; } = null!;

        [Required]
        public IdentityUser Publisher { get; set; } = null!;

        public DateTime PublishedOn { get; set; }

        public int GenreId { get; set; } 

        [Required]
        [ForeignKey(nameof(GenreId))]
        public Genre Genre { get; set; } = null!;

        public bool IsDeleted { get; set; } = false;

        [Required]
        public ICollection<UserBook> UsersBooks { get; set; } = new HashSet<UserBook>();
    }
}

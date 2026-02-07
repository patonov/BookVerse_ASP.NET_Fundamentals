using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BookVerse.GCommon.ValidationConstants;

namespace BookVerse.ViewModels
{
    public class BookEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(TitleNameMaxLength, MinimumLength = TitleNameMinLength)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength)]
        public string Description { get; set; } = null!;

        public string? CoverImageUrl { get; set; }

        [Required]
        //[RegularExpression(@"^\d{2}-\d{2}-\d{4}$")]
        public string PublishedOn { get; set; } = null!;

        public int GenreId { get; set; }

        public string PublisherId { get; set; } = null!;

        public virtual IEnumerable<GenreViewModel>? Genres { get; set; } 
    }
}

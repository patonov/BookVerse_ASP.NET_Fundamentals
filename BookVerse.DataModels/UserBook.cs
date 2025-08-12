using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookVerse.DataModels
{
    [PrimaryKey(nameof(UserId), nameof(BookId))]
    public class UserBook
    {
        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; } = null!;

        public int BookId { get; set; }

        [Required]
        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; } = null!;

    }
}

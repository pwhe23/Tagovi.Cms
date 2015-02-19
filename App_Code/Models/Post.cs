using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Site
{
    [Table("Post")]
    public class Post
    {
        public int Id { get; set; }

        [StringLength(250), Required]
        public string Title { get; set; }

        [StringLength(250)]
        public string Slug { get; set; }

        [StringLength(8000)]
        public string Summary { get; set; }

        [MaxLength]
        public string Body { get; set; }

        public DateTime Created { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }
    };
}

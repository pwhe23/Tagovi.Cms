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

        [Index("IX_Post_Slug", IsUnique = true)]
        [StringLength(250)]
        public string Slug { get; set; }

        [StringLength(8000)]
        public string Summary { get; set; }

        [MaxLength]
        public string Body { get; set; }

        public DateTime Created { get; set; }

        [ForeignKey("Page")]
        public int? PageId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual Page Page { get; set; }
        public virtual User User { get; set; }

        [NotMapped]
        public string Url
        {
            get { return (Page == null ? "" : Page.Url + "/") + Slug; }
        }
    };
}

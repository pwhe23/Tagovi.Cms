using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Site
{
    [Table("Page")]
    public class Page
    {
        public int Id { get; set; }

        [Index("IX_Page_Url", IsUnique = true)]
        [StringLength(250)]
        public string Url { get; set; }

        [ForeignKey("Parent")]
        public int? ParentId { get; set; }

        public virtual Page Parent { get; set; }
    };
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Site
{
    [Table("Part")]
    public class Part
    {
        public int Id { get; set; }

        [StringLength(25)]
        public string Parent { get; set; }

        [StringLength(25)]
        public string Section { get; set; }

        [StringLength(25), Required]
        public string Type { get; set; }

        [MaxLength]
        public string Data { get; set; }
    };
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalendrierAventCore.Data.Models
{
    [Table("Box")]
    public partial class Box
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }

        [Required]
        [StringLength(32)]
        public string Path { get; set; }

        public virtual List<Calendar> Calendars { get; set; }
    }
}

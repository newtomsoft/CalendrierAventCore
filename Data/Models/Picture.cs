using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalendrierAventCore.Data.Models
{
    [Table("Picture")]
    public partial class Picture
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CalendarId { get; set; }

        [Required]
        [StringLength(37)]
        public string Name { get; set; }

        [Required]
        [Range(0, 24)]
        public int DayNumber { get; set; }

        public virtual Calendar Calendar { get; set; }
    }
}

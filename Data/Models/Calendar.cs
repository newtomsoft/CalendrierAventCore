using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CalendrierAventCore.Data.Models
{
    [Table("Calendar")]
    public class Calendar : IValidatableObject
    {
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage ="Merci de renseigner le nom")]
        [StringLength(20)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(27)]
        public string PublicName { get; set; }

        [Required]
        [StringLength(31)]
        public string PrivateName { get; set; }

        [Required]
        public int BoxId { get; set; }

        public virtual List<Picture> Pictures { get; set; }
        public virtual Box Box { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(PublicName))
                yield return new ValidationResult("Vous devez saisir au moins un caractère", new[] { nameof(PublicName) });
        }
    }
}

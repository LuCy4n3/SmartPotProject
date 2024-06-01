
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;

namespace SqlServerVer2.Models
{
    public class User 
    {
        [Key]
        [Required]
        public int UserId { get; set; }
      
        [Required]
        public int UserType { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserPassword { get; set; }

        /*public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var userTypes = Enumerable.Range(0, 1);
            if(!userTypes.Contains(UserType))
            {
                yield return
                    new ValidationResult("Invalid type");
            }
        }*/
    }
}

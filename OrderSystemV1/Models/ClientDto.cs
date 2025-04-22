using System.ComponentModel.DataAnnotations;

namespace OrderSystemV1.Models
{
    public class ClientDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }
    }
}
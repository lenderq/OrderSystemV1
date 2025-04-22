using System.ComponentModel.DataAnnotations;

namespace OrderSystemV1.Models
{
    public class OrderDto
    {
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public int ClientId { get; set; }
    }
}

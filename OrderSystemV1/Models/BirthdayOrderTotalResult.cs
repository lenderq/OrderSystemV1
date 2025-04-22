using System.ComponentModel.DataAnnotations.Schema;

namespace OrderSystemV1.Models
{
    public class BirthdayOrderTotalResult
    {
        [Column("client_id")]
        public int ClientId { get; set; }

        [Column("total_amount")]
        public decimal TotalAmount { get; set; }
    }
}

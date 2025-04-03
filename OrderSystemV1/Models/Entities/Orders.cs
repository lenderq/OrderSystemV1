using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderSystemV1.Models.Entities
{
    public class Orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("Сумма", TypeName = "decimal(10, 2)")]
        public Decimal Amount { get; set; }

        [Required]
        [Column("Дата_и_время")]
        public DateTime OrderDate { get; set; }

        [Required]
        [Column("Статус")]
        public OrderStatus Status { get; set; }

        [ForeignKey("clients")]
        [Column("ClientID")]
        public int ClientId { get; set; }
        public Clients Client { get; set; }
    }
}

public enum OrderStatus
{
    NotProcessed,
    Cancelled,
    Completed
}

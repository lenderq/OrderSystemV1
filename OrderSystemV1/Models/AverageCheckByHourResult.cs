using System.ComponentModel.DataAnnotations.Schema;

namespace OrderSystemV1.Models
{
    public class AverageCheckByHourResult
    {
        [Column("Час")]
        public int Hour { get; set; }
        [Column("Средний_чек")]
        public decimal AverageCheck { get; set; }
    }
}

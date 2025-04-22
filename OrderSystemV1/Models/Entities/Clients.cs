using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderSystemV1.Models.Entities
{
    public class Clients
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("Имя")]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [Column("Фамилия")]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [Column("Дата_рождения")]
        public DateTime BirthDate { get; set; }
        public List<Orders> Orders { get; set; } = new();
    }
}

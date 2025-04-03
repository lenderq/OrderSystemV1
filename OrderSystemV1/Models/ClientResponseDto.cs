namespace OrderSystemV1.Models
{
    public class ClientResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public int OrderCount { get; set; }
    }
}

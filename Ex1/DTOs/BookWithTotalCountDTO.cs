namespace Ex1.DTOs
{
    public class BookWithTotalCountDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime? PublishedDate { get; set; }
        public int? AuthorId { get; set; }
        public string AuthorName { get; set; }  
        public string AuthorBio { get; set; }   
        public int TotalCount { get; set; }   
    }
}

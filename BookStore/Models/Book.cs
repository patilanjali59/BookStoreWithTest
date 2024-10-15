using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required,Range(0.01, 5000)]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; }

        [Required,Range(0,10000)]
        public int Stock { get; set; }
        public string CoverImageUrl { get; set; }
        

        


    }
}

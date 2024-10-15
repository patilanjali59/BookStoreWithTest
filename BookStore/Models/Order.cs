using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required,Range(1,100)]
        public int Quantity { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        //[ForeignKey("BookId")]
        public Book? Book { get; set; }

        //[ForeignKey("UserId")
        public User? User { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
       
        public string ?Role { get; set; }

        public ICollection<Order>? Orders { get; set; }  // Navigation property
    }
}

using System.ComponentModel.DataAnnotations; 
using System.ComponentModel.DataAnnotations.Schema; 

namespace ProductService.Models
{
    public class Product 
    {
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Name {get; set;} = string.Empty; 
        public string? Description {get; set;}
        [Column(TypeName="numeric(18, 2)")]
        public decimal Price {get; set;}
        public int Stock {get; set;}



        //Clé étrangére sur les fournisseurs 
        public Guid SupplierId {get; set;}
        public Supplier? Supplier { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.UtcNow; 
    }
}
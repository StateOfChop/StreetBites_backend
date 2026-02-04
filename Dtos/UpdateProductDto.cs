using System.ComponentModel.DataAnnotations;

namespace StreetBites.Dtos
{
    public class UpdateProductDto
    {
        [Required]
        [MaxLength(120)]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be >= 0")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock must be >= 0")]
        public int Stock { get; set; }

        public bool IsActive { get; set; }
    }
}

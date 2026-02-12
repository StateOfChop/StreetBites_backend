using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StreetBites.Models
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [MaxLength(120)]
        public required string Name { get; set; }
        
        public string? Description { get; set; }
        
        [Precision(10, 2)]
        public decimal Price { get; set; }
        
        public int Stock { get; set; } = 0;
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

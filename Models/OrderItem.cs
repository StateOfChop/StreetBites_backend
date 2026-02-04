using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StreetBites.Models
{
    public class OrderItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
        
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        
        public int Quantity { get; set; }
        
        [Precision(10, 2)]
        public decimal Price { get; set; } // Precio al momento de la compra
    }
}

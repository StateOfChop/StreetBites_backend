using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace StreetBites.Models
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid UserId { get; set; }
        public User? User { get; set; } // Propiedad de navegación
        
        public OrderStatus Status { get; set; } = OrderStatus.PENDING;
        
        [Precision(10, 2)]
        public decimal Total { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relación: Un pedido tiene muchos detalles
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}

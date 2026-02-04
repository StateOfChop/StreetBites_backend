using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;


namespace StreetBites.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [MaxLength(100)]
        public required string Name { get; set; }
        
        [MaxLength(150)]
        public required string Email { get; set; }
        
        [MaxLength(255)]
        public required string PasswordHash { get; set; }
        
        public UserRole Role { get; set; } = UserRole.USER;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Relación: Un usuario tiene muchos pedidos
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
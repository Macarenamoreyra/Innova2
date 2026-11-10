using System;
using System.ComponentModel.DataAnnotations;

namespace ClasesInnovaStock.Models
{
    public class Tinta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Color { get; set; }

        public decimal PrecioPorLitro { get; set; }

        public decimal Stock { get; set; }
    }
}
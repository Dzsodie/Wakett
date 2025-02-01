using System;
using System.ComponentModel.DataAnnotations;

namespace RatesService.Model
{
    public class Rate
    {
        [Key]
        public string InstrumentId { get; set; }
        public decimal Price { get; set; }
        public DateTime Timestamp { get; set; }
    }
}


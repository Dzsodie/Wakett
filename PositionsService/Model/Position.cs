using System;
using System.ComponentModel.DataAnnotations;

namespace PositionsService.Model
{
    public class Position
    {
        [Key]
        public string InstrumentId { get; set; }
        public decimal Quantity { get; set; }
        public decimal InitialRate { get; set; }
        public string Side { get; set; } // "BUY" or "SELL"
        public decimal CurrentRate { get; set; }
        public decimal ProfitLoss { get; set; }
    }
}


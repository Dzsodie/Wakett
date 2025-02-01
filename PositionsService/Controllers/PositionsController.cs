using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PositionsService.Data;
using PositionsService.Messaging;

namespace PositionsService.Controllers
{
    [ApiController]
    [Route("api/positions")]
    public class PositionsController : ControllerBase
    {
        private readonly PositionsDbContext _context;
        private readonly PositionUpdatePublisher _publisher;

        public PositionsController(PositionsDbContext context, PositionUpdatePublisher publisher)
        {
            _context = context;
            _publisher = publisher;
        }

        [HttpGet]
        public async Task<IActionResult> GetPositions()
        {
            return Ok(await _context.Positions.ToListAsync());
        }

        [HttpPut("{instrumentId}")]
        public async Task<IActionResult> UpdatePosition(string instrumentId, decimal newRate)
        {
            var position = await _context.Positions.FirstOrDefaultAsync(p => p.InstrumentId == instrumentId);
            if (position == null) return NotFound();

            position.CurrentRate = newRate;
            position.ProfitLoss = position.Quantity * (newRate - position.InitialRate) * Convert.ToDecimal(position.Side);
            await _context.SaveChangesAsync();

            await _publisher.PublishPositionUpdate(position);
            return Ok(position);
        }
    }
}


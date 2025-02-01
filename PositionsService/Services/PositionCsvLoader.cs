using System;
using CsvHelper;
using PositionsService.Data;
using PositionsService.Model;
using System.Globalization;

namespace PositionsService.Services
{
    public class PositionCsvLoader
    {
        private readonly PositionsDbContext _context;

        public PositionCsvLoader(PositionsDbContext context)
        {
            _context = context;
        }

        public async Task LoadPositionsFromCsv(string filePath)
        {
            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<Position>().ToList();

            foreach (var record in records)
            {
                if (record.Side.ToUpper() == "BUY") record.Side = "+1";
                else if (record.Side.ToUpper() == "SELL") record.Side = "-1";
            }

            _context.Positions.AddRange(records);
            await _context.SaveChangesAsync();
        }
    }
}


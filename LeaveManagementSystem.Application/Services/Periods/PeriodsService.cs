﻿using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Application.Services.Periods
{
    public class PeriodsService(ApplicationDbContext context) : IPeriodsService
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<Period> GetCurrentPeriod()
        {
            var currentDate = DateTime.Now;
            var period = await _context.Periods.SingleAsync(q => q.EndDate.Year == currentDate.Year);
            return period;
        }
    }
}

﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LeaveManagementSystem.Application.Services.LeaveTypes
{
    public class LeaveTypesService(ApplicationDbContext context, IMapper mapper, ILogger<LeaveTypesService> logger) : ILeaveTypesService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<LeaveTypesService> _logger = logger;

        public async Task<List<LeaveTypeReadOnlyVM>> GetAll()
        {
            var data = await _context.LeaveTypes.ToListAsync();
            var viewData = _mapper.Map<List<LeaveTypeReadOnlyVM>>(data);
            return viewData;
        }

        public async Task<T?> Get<T>(int id) where T : class
        {
            var data = await _context.LeaveTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (data == null)
                return null;

            var viewData = _mapper.Map<T>(data);
            return viewData;
        }

        public async Task Remove(int id)
        {
            var data = await _context.LeaveTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (data != null)
            {
                _context.LeaveTypes.Remove(data);
                await _context.SaveChangesAsync();
            }
        }

        public async Task Edit(LeaveTypeEditVM model)
        {
            var leaveType = _mapper.Map<LeaveType>(model);
            _context.Update(leaveType);
            await _context.SaveChangesAsync();
        }

        public async Task Create(LeaveTypeCreateVM model)
        {
            _logger.LogInformation("Creating a new leave type with name: {Name} - {Days}", model.Name, model.NumberOfDays);

            var leaveType = _mapper.Map<LeaveType>(model);
            _context.Add(leaveType);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckIfLeaveTypeNameExists(string name)
        {
            var lowerCaseName = name.ToLower();
            return await _context.LeaveTypes.AnyAsync(q => q.Name.ToLower().Equals(lowerCaseName));
        }

        public async Task<bool> CheckIfLeaveTypeNameExistsForEdit(LeaveTypeEditVM leaveType)
        {
            var lowerCaseName = leaveType.Name.ToLower();
            return await _context.LeaveTypes.AnyAsync(q => q.Name.ToLower().Equals(lowerCaseName) && q.Id != leaveType.Id);
        }

        public bool LeaveTypeExists(int id)
        {
            return _context.LeaveTypes.Any(e => e.Id == id);
        }

        public async Task<bool> DaysExceedMaximum(int leaveTypeId, int days)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(leaveTypeId);
            return leaveType != null && days > leaveType.NumberOfDays;
        }
    }
}

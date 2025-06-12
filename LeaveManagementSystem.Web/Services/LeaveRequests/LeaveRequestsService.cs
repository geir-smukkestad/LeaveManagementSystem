using AutoMapper;
using LeaveManagementSystem.Web.Models.LeaveRequests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace LeaveManagementSystem.Web.Services.LeaveRequests
{
    public class LeaveRequestsService(
        IMapper mapper,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext context) : ILeaveRequestsService
    {
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ApplicationDbContext _context = context;

        public async Task CancelLeaveRequest(int leaveRequestId)
        {
            var leaveRequest = _context.LeaveRequests.Find(leaveRequestId);
            leaveRequest.LeaveRequestStatusId = (int)LeaveRequestStatusEnum.Canceled;

            var numberOfDays = leaveRequest.EndDate.DayNumber - leaveRequest.StartDate.DayNumber;
            var allocationToRefund = await _context.LeaveAllocations.FirstAsync(q => q.EmployeeId == leaveRequest.EmployeeId && q.LeaveTypeId == leaveRequest.LeaveTypeId);
            allocationToRefund.Days += numberOfDays;

            await _context.SaveChangesAsync();
        }

        public async Task CreateLeaveRequest(LeaveRequestCreateVM model)
        {
            var leaveRequest = _mapper.Map<LeaveRequest>(model);

            var user = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);
            leaveRequest.EmployeeId = user.Id;

            leaveRequest.LeaveRequestStatusId = (int)LeaveRequestStatusEnum.Pending;
            _context.LeaveRequests.Add(leaveRequest);

            // Deduct allocated leave days from the employee's leave type balance
            var numberOfDays = model.EndDate.DayNumber - model.StartDate.DayNumber;
            var allocationToDeduct = await _context.LeaveAllocations.FirstAsync(q => q.EmployeeId == user.Id && q.LeaveTypeId == model.LeaveTypeId);
            allocationToDeduct.Days -= numberOfDays;

            await _context.SaveChangesAsync();
        }

        public Task<LeaveRequestReadOnlyVM> GetAllLeaveRequests()
        {
            throw new NotImplementedException();
        }

        public async Task<List<LeaveRequestReadOnlyVM>> GetEmployeeLeaveRequests()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);
            var leaveRequests = await _context.LeaveRequests
                .Include(q => q.LeaveType)
                .Where(q => q.EmployeeId == user.Id)
                .ToListAsync();

            // Note: The Select statement isn't done within the db query above, since we need
            // to do a C# casting of the leave request status enum, and that would fail
            // in the EF Core query translation.
            var models = leaveRequests.Select(q => new LeaveRequestReadOnlyVM
            {
               Id = q.Id,
               StartDate = q.StartDate,
               EndDate = q.EndDate,
               LeaveType = q.LeaveType.Name,
               LeaveRequestStatus = (LeaveRequestStatusEnum)q.LeaveRequestStatusId,
               NumberOfDays = q.EndDate.DayNumber - q.StartDate.DayNumber
            }).ToList();

            return models;
        }

        public async Task<bool> RequestDatesExceedAllocation(LeaveRequestCreateVM model)
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);
            var numberOfDays = model.EndDate.DayNumber - model.StartDate.DayNumber;
            var allocation = await _context.LeaveAllocations.FirstAsync(q => q.EmployeeId == user.Id && q.LeaveTypeId == model.LeaveTypeId);

            return numberOfDays > allocation.Days;
        }

        public Task ReviewLeaveRequest(ReviewLeaveRequestVM model)
        {
            throw new NotImplementedException();
        }
    }
}
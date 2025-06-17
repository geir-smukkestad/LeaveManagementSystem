using AutoMapper;
using LeaveManagementSystem.Web.Data;
using LeaveManagementSystem.Web.Models.LeaveAllocations;
using LeaveManagementSystem.Web.Models.LeaveRequests;
using LeaveManagementSystem.Web.Services.LeaveAllocations;
using LeaveManagementSystem.Web.Services.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace LeaveManagementSystem.Web.Services.LeaveRequests
{
    public class LeaveRequestsService(
        IMapper mapper,
        IUserService userService,
        ApplicationDbContext context,
        ILeaveAllocationsService leaveAllocationsService) : ILeaveRequestsService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IUserService _userService = userService;
        private readonly ApplicationDbContext _context = context;
        private readonly ILeaveAllocationsService _leaveAllocationsService = leaveAllocationsService;

        public async Task CancelLeaveRequest(int leaveRequestId)
        {
            var leaveRequest = _context.LeaveRequests.Find(leaveRequestId);
            leaveRequest.LeaveRequestStatusId = (int)LeaveRequestStatusEnum.Canceled;

            // Restore allocation days
            await UpdateAllocationDays(leaveRequest, false);

            await _context.SaveChangesAsync();
        }

        public async Task CreateLeaveRequest(LeaveRequestCreateVM model)
        {
            var leaveRequest = _mapper.Map<LeaveRequest>(model);

            var user = await _userService.GetLoggedInUser();
            leaveRequest.EmployeeId = user.Id;

            leaveRequest.LeaveRequestStatusId = (int)LeaveRequestStatusEnum.Pending;
            _context.LeaveRequests.Add(leaveRequest);

            // Deduct allocated leave days from the employee's leave type balance
            await UpdateAllocationDays(leaveRequest, true);

            await _context.SaveChangesAsync();
        }

        public async Task<EmployeeLeaveRequestListVM> AdminGetAllLeaveRequests()
        {
            var leaveRequests = await _context.LeaveRequests
                .Include(q => q.LeaveType)
                .ToListAsync();

            var leaveRequestVMs = leaveRequests.Select(q => new LeaveRequestReadOnlyVM
            {
                Id = q.Id,
                StartDate = q.StartDate,
                EndDate = q.EndDate,
                NumberOfDays = q.EndDate.DayNumber - q.StartDate.DayNumber,
                LeaveType = q.LeaveType?.Name ?? string.Empty,
                LeaveRequestStatus = (LeaveRequestStatusEnum)q.LeaveRequestStatusId
            }).ToList();
             
            var model = new EmployeeLeaveRequestListVM
            {
                TotalRequests = leaveRequests.Count,
                ApprovedRequests = leaveRequests.Count(q => q.LeaveRequestStatusId == (int)LeaveRequestStatusEnum.Approved),
                PendingRequests = leaveRequests.Count(q => q.LeaveRequestStatusId == (int)LeaveRequestStatusEnum.Pending),
                DeclinedRequests = leaveRequests.Count(q => q.LeaveRequestStatusId == (int)LeaveRequestStatusEnum.Declined),
                LeaveRequests = leaveRequestVMs
            };

            return model;
        }

        public async Task<List<LeaveRequestReadOnlyVM>> GetEmployeeLeaveRequests()
        {
            var user = await _userService.GetLoggedInUser();
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
            var user = await _userService.GetLoggedInUser();
            var currentDate = DateTime.Now;
            var period = await _context.Periods.SingleAsync(q => q.EndDate.Year == currentDate.Year);
            var numberOfDays = model.EndDate.DayNumber - model.StartDate.DayNumber;
            var allocation = await _context.LeaveAllocations.FirstAsync(q => q.EmployeeId == user.Id &&
                q.LeaveTypeId == model.LeaveTypeId && q.PeriodId == period.Id);

            return numberOfDays > allocation.Days;
        }

        public async Task<ReviewLeaveRequestVM> GetLeaveRequestForReview(int id)
        {
            var leaveRequest = await _context.LeaveRequests
                .Include(q => q.LeaveType)
                .FirstAsync(q => q.Id == id); // Note: Have to use FirstNnn, since FindNnnn doesn't work with Include

            var user = await _userService.GetUserById(leaveRequest.EmployeeId);

            var model = new ReviewLeaveRequestVM
            {
                Id = leaveRequest.Id,
                StartDate = leaveRequest.StartDate,
                EndDate = leaveRequest.EndDate,
                NumberOfDays = leaveRequest.EndDate.DayNumber - leaveRequest.StartDate.DayNumber,
                LeaveType = leaveRequest.LeaveType.Name,
                LeaveRequestStatus = (LeaveRequestStatusEnum)leaveRequest.LeaveRequestStatusId,
                RequestComments = leaveRequest.RequestComments,
                Employee = new EmployeeListVM
                {
                    Id = leaveRequest.EmployeeId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                }
            };
            return model;
        }

        public async Task ReviewLeaveRequest(int leaveRequestId, bool approved)
        {
            var user = await _userService.GetLoggedInUser();

            var leaveRequest = await _context.LeaveRequests.FindAsync(leaveRequestId);
            leaveRequest.LeaveRequestStatusId = approved ? (int)LeaveRequestStatusEnum.Approved : (int)LeaveRequestStatusEnum.Declined;
            leaveRequest.ReviewerId = user.Id;

            if (!approved)
            {
                UpdateAllocationDays(leaveRequest, false);
            }

            await _context.SaveChangesAsync();
        }

        private async Task UpdateAllocationDays(LeaveRequest leaveRequest, bool deductDays)
        {
            var allocation = await _leaveAllocationsService.GetCurrentAllocation(leaveRequest.LeaveTypeId, leaveRequest.EmployeeId);
            var numberOfDays = CalculateDays(leaveRequest.StartDate, leaveRequest.EndDate);
            if (deductDays)
                allocation.Days -= numberOfDays;
            else
                allocation.Days += numberOfDays;
            _context.Entry(allocation).State = EntityState.Modified;
        }

        private int CalculateDays(DateOnly start, DateOnly end)
        {
            return end.DayNumber - start.DayNumber;
        }
    }
}
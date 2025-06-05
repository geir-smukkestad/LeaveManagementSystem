
using AutoMapper;
using LeaveManagementSystem.Web.Models.LeaveAllocations;
using LeaveManagementSystem.Web.Models.LeaveTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Web.Services.LeaveAllocations
{
    public class LeaveAllocationsService(
        ApplicationDbContext context,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager,
        IMapper mapper
    ) : ILeaveAllocationsService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;

        public async Task AllocateLeave(string employeeId)
        {
            var leaveTypes = await _context.LeaveTypes.ToListAsync();

            // Get the current period based on the year (assuming a period starts at the beginning of the year and ends at the end of the year)
            var currentDate = DateTime.Now;
            var period = await _context.Periods.SingleAsync(p => p.EndDate.Year == currentDate.Year);
            var monthsRemaining = period.EndDate.Month - currentDate.Month;

            // for each leave type, create a new allocation for the employee
            foreach (var leaveType in leaveTypes)
            {
                var accrualRate = decimal.Divide(leaveType.NumberOfDays, 12); // Assuming leave is accrued monthly

                // Note: Either initialize the navigation property or the foreign key property, not both.
                var leaveAllocation = new LeaveAllocation
                {
                    EmployeeId = employeeId,
                    LeaveTypeId = leaveType.Id,
                    PeriodId = period.Id,
                    Days = (int)Math.Ceiling(accrualRate * monthsRemaining) // Allocate remaining days based on the number of months left in the year
                };
                _context.Add(leaveAllocation);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<LeaveAllocation>> GetAllocations()
        {
            // var userName = await _httpContextAccessor?.HttpContext?.User?.Identity?.Name;
            // var user = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);
            var user = await GetUserAsync();

            var currentDate = DateTime.Now;
#if true
            var allocations = await _context.LeaveAllocations
                .Include(x => x.LeaveType) // Include navigation property for LeaveType
                .Include(x => x.Period) // Include navigation property for Period
                .Where(x => x.EmployeeId == user.Id && x.Period.EndDate.Year == currentDate.Year)
                .ToListAsync();
#else // Alternative approach using SingleAsync to get the period that matches the current year
            var period = await _context.Periods.SingleAsync(p => p.EndDate.Year == currentDate.Year);
            var allocations = await _context.LeaveAllocations
                .Include(x => x.LeaveType) // Include navigation property for LeaveType
                .Include(x => x.Period) // Include navigation property for Period
                .Where(x => x.EmployeeId == user.Id && x.Period.Id == period.Id)
                .ToListAsync();
#endif
            return allocations;
        }

        public async Task<EmployeeAllocationVM> GetEmployeeAllocations()
        {
            var allocations = await GetAllocations();
            var allocationVmList = _mapper.Map<List<LeaveAllocation>, List<LeaveAllocationVM>>(allocations);

            // var user = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);
            var user = await GetUserAsync();
            var employeeVM = new EmployeeAllocationVM
            {
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                LeaveAllocations = allocationVmList
            };

            return employeeVM;
        }

        private async Task<ApplicationUser?> GetUserAsync()
        {
            return await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);
        }
    }
}

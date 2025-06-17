
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using LeaveManagementSystem.Application.Services.Periods;
using LeaveManagementSystem.Application.Services.Users;

namespace LeaveManagementSystem.Application.Services.LeaveAllocations
{
    public class LeaveAllocationsService(
        ApplicationDbContext context,
        IUserService userService,
        IMapper mapper,
        IPeriodsService periodsService
    ) : ILeaveAllocationsService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IUserService _userService = userService;
        private readonly IMapper _mapper = mapper;
        private readonly IPeriodsService _periodsService = periodsService;

        public async Task AllocateLeave(string employeeId)
        {
            var leaveTypes = await _context.LeaveTypes
                .Where(x => !x.LeaveAllocations.Any(y => y.EmployeeId == employeeId))
                .ToListAsync();

            // Get the current period based on the year (assuming a period starts at the beginning of the year and ends at the end of the year)
            var period = await _periodsService.GetCurrentPeriod();
            var monthsRemaining = period.EndDate.Month - DateTime.Now.Month;

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

        public async Task<EmployeeAllocationVM> GetEmployeeAllocations(string? userId)
        {
            var user = string.IsNullOrEmpty(userId) ? await _userService.GetLoggedInUser() : await _userService.GetUserById(userId);

            var allocations = await GetAllocations(user.Id);
            var allocationVmList = _mapper.Map<List<LeaveAllocation>, List<LeaveAllocationVM>>(allocations);
            var leaveTypesCount = await _context.LeaveTypes.CountAsync();

            var employeeVM = new EmployeeAllocationVM
            {
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                LeaveAllocations = allocationVmList,
                IsCompletedAllocation = leaveTypesCount == allocations.Count,
            };

            return employeeVM;
        }

        public async Task<List<EmployeeListVM>> GetEmployees()
        {
            var users = await _userService.GetEmployees();
            var employees = _mapper.Map<List<ApplicationUser>, List<EmployeeListVM>>(users.ToList());
            return employees;
        }

        public async Task<LeaveAllocationEditVM> GetEmployeeAllocation(int allocationId)
        {
            var allocation = await _context.LeaveAllocations
                .Include(x => x.LeaveType) // Include navigation property for LeaveType
                .Include(x => x.Employee) // Include navigation property for Employee
                .FirstOrDefaultAsync(x => x.Id == allocationId);

            var model = _mapper.Map<LeaveAllocationEditVM>(allocation);
            return model;
        }

        public async Task EditAllocation(LeaveAllocationEditVM allocationEditVm)
        {
#if false
            var leaveAllocation = await GetEmployeeAllocation(allocationEditVm.Id);
            if (leaveAllocation == null)
            {
                throw new Exception("Leave allocation record does not exist.");
            }
            leaveAllocation.Days = allocationEditVm.Days;
            // Option 1: _context.Update(leaveAllocation);
            // Option 2: _context.Entry(leaveAllocation).State = EntityState.Modified; // Explicitly set the state to Modified
            // await _context.SaveChangesAsync();
#else
            await _context.LeaveAllocations
                .Where(x => x.Id == allocationEditVm.Id)
                .ExecuteUpdateAsync(x => x.SetProperty(y => y.Days, allocationEditVm.Days));
#endif
        }

        public async Task<LeaveAllocation> GetCurrentAllocation(int leaveTypeId, string employeeId)
        {
            var currentPeriod = await _periodsService.GetCurrentPeriod();
            var allocation = await _context.LeaveAllocations.FirstAsync(q => q.EmployeeId == employeeId &&
                q.LeaveTypeId == leaveTypeId && q.PeriodId == currentPeriod.Id);
            return allocation;
        }

        private async Task<List<LeaveAllocation>> GetAllocations(string userId)
        {
            var period = await _periodsService.GetCurrentPeriod();
            var allocations = await _context.LeaveAllocations
                .Include(x => x.LeaveType) // Include navigation property for LeaveType
                .Include(x => x.Period) // Include navigation property for Period
                .Where(x => x.EmployeeId == userId && x.PeriodId == period.Id)
                .ToListAsync();
            return allocations;
        }


        private async Task<bool> AllocationExists(string userId, int periodId, int leaveTypeId)
        {
            var exists = await _context.LeaveAllocations.AnyAsync(q =>
                q.EmployeeId == userId &&
                q.PeriodId == periodId &&
                q.LeaveTypeId == leaveTypeId
            );
            return exists;
        }
    }
}

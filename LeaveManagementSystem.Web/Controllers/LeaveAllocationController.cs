using LeaveManagementSystem.Web.Services.LeaveAllocations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.Web.Controllers
{
    [Authorize]
    public class LeaveAllocationController(ILeaveAllocationsService leaveAllocationService) : Controller
    {
        private readonly ILeaveAllocationsService _leaveAllocationService = leaveAllocationService;

        public async Task<IActionResult> Details()
        {
            var employeeVM = await _leaveAllocationService.GetEmployeeAllocations();
            return View(employeeVM);
        }
    }
}

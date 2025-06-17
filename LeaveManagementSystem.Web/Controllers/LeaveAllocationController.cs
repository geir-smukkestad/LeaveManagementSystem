using LeaveManagementSystem.Application.Services.LeaveAllocations;
using LeaveManagementSystem.Application.Services.LeaveTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagementSystem.Web.Controllers
{
    [Authorize]
    public class LeaveAllocationController(ILeaveAllocationsService leaveAllocationService, ILeaveTypesService leaveTypesService) : Controller
    {
        private readonly ILeaveAllocationsService _leaveAllocationService = leaveAllocationService;
        private readonly ILeaveTypesService _leaveTypesService = leaveTypesService;

        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Index()
        {
            var employees = await _leaveAllocationService.GetEmployees();
            return View(employees);
        }

        [Authorize(Roles = Roles.Administrator)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateLeave(string? id)
        {
            await _leaveAllocationService.AllocateLeave(id);
            return RedirectToAction(nameof(Details), new { userId = id });
        }

        public async Task<IActionResult> Details(string? userId)
        {
            var employeeVM = await _leaveAllocationService.GetEmployeeAllocations(userId);
            return View(employeeVM);
        }

        public async Task<IActionResult> EditAllocation(int? id)
        {
            if (id == null)
                return NotFound();

            var allocation = await _leaveAllocationService.GetEmployeeAllocation(id.Value);
            if (allocation == null)
            {
                return NotFound();
            }
            return View(allocation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAllocation(LeaveAllocationEditVM allocation)
        {
            if (await _leaveTypesService.DaysExceedMaximum(allocation.LeaveType.Id, allocation.Days))
            {
                ModelState.AddModelError(nameof(allocation.Days), "The number of days exceeds the maximum allowed for this leave type.");
            }

            if (ModelState.IsValid)
            {
                await _leaveAllocationService.EditAllocation(allocation);
                return RedirectToAction(nameof(Details), new { userId = allocation.Employee.Id });
            }

            // If we reach here, something failed; redisplay form with validation errors
            // Note: We need to reload the allocation from the database, otherwise reloading the page won't fill the needed properties.
            var days = allocation.Days;
            allocation = await _leaveAllocationService.GetEmployeeAllocation(allocation.Id);
            allocation.Days = days; // Preserve the original days value in case of validation failure

            return View(allocation);
        }
    }
}

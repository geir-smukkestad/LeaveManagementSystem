using LeaveManagementSystem.Web.Models.LeaveRequests;
using LeaveManagementSystem.Web.Services.LeaveRequests;
using LeaveManagementSystem.Web.Services.LeaveTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveManagementSystem.Web.Controllers
{
    [Authorize]
    public class LeaveRequestsController(ILeaveTypesService leaveTypesService, ILeaveRequestsService leaveRequestsService) : Controller
    {
        private readonly ILeaveTypesService _leaveTypesService = leaveTypesService;
        private readonly ILeaveRequestsService _leaveRequestsService = leaveRequestsService;

        // Employee view requests
        public async Task<IActionResult> Index()
        {
            var models = await _leaveRequestsService.GetEmployeeLeaveRequests();
            return View(models);
        }

        // Employee create requests
        public async Task<IActionResult> Create(int? leaveTypeId)
        {
            var leaveTypes = await _leaveTypesService.GetAll();
            var leaveTypesList = new SelectList(leaveTypes, "Id", "Name", leaveTypeId);
            var model = new LeaveRequestCreateVM
            {
                StartDate = DateOnly.FromDateTime(DateTime.Now),
                EndDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), // Default to next day
                LeaveTypes = leaveTypesList
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRequestCreateVM model)
        {
            // Validate that the days don't exceed the allocated leave days
            if (await _leaveRequestsService.RequestDatesExceedAllocation(model))
            {
                ModelState.AddModelError(string.Empty, "Yoy have exceeded your allocation.");
                ModelState.AddModelError(nameof(model.EndDate), "The requested leave dates exceed your allocated leave days for this type.");
            }

            if (ModelState.IsValid)
            {
                await _leaveRequestsService.CreateLeaveRequest(model);
                return RedirectToAction(nameof(Index));
            }

            // Re-populate the select list, since it wasn't bound in the form submission
            var leaveTypes = await _leaveTypesService.GetAll();
            model.LeaveTypes = new SelectList(leaveTypes, "Id", "Name");

            return View(model);
        }

        // Employee cancel requests
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            await _leaveRequestsService.CancelLeaveRequest(id);
            return RedirectToAction(nameof(Index));
        }

        // Admin/Supe review requests
        [Authorize(Policy = "AdminSupervisorOnly")]
        public async Task<IActionResult> ListRequests()
        {
            var model = await _leaveRequestsService.AdminGetAllLeaveRequests();
            return View(model);
        }

        // Admin/supervisor review requests
        public async Task<IActionResult> Review(int id)
        {
            var model = await _leaveRequestsService.GetLeaveRequestForReview(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(int id, bool approved)
        {
            await _leaveRequestsService.ReviewLeaveRequest(id, approved);
            return RedirectToAction(nameof(ListRequests));
        }
    }
}

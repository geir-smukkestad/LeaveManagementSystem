using System.ComponentModel;

namespace LeaveManagementSystem.Application.Models.LeaveRequests
{
    public class EmployeeLeaveRequestListVM
    {
        [DisplayName("Total Number of Requests")]
        public int TotalRequests { get; set; }

        [DisplayName("Approved Requests")]
        public int ApprovedRequests { get; set; }

        [DisplayName("Pending Requests")]
        public int PendingRequests { get; set; }

        [DisplayName("Rejected Requests")]
        public int DeclinedRequests { get; set; }

        public List<LeaveRequestReadOnlyVM> LeaveRequests { get; set; } = [];
    }
}
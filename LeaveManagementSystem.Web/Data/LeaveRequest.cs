namespace LeaveManagementSystem.Web.Data
{
    public class LeaveRequest : BaseEntity
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public LeaveType? LeaveType { get; set; }
        public int LeaveTypeId { get; set; }

        public LeaveRequestStatus? LeaveRequestStatus { get; set; }
        public int LeaveRequestStatusId { get; set; }

        public ApplicationUser? Employee { get; set; }
        public string EmployeeId { get; set; }

        public ApplicationUser? Reviewer { get; set; }
        public string? ReviewerId { get; set; } // Must be nullable, since we don't know the reviewer at the time of creation

        public string? RequestComments { get; set; }
    }
}
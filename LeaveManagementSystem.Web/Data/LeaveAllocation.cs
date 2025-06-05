namespace LeaveManagementSystem.Web.Data
{
    public class LeaveAllocation : BaseEntity
    {
        public LeaveType? LeaveType { get; set; } // Navigation property
        public int LeaveTypeId { get; set; } // LeaveType foreign key (same as the Id of the navigation property). EF Core will deduce that this is a foreign key based on the naming convention.

        public ApplicationUser? Employee { get; set; } // ApplicationUser navigation property
        public string EmployeeId { get; set; } // ApplicationUser foreign key

        public Period? Period { get; set; } // Navigation property for Period
        public int PeriodId { get; set; }

        public int Days { get; set; }
    }
}

namespace LeaveManagementSystem.Application.Services.LeaveTypes
{
    public interface ILeaveTypesService
    {
        Task Create(LeaveTypeCreateVM model);
        Task Edit(LeaveTypeEditVM model);
        Task<T?> Get<T>(int id) where T : class;
        Task<List<LeaveTypeReadOnlyVM>> GetAll();
        Task<bool> CheckIfLeaveTypeNameExists(string name);
        Task<bool> CheckIfLeaveTypeNameExistsForEdit(LeaveTypeEditVM leaveType);
        bool LeaveTypeExists(int id);
        Task Remove(int id);
        Task<bool> DaysExceedMaximum(int leaveTypeId, int days);
    }
}
namespace LeaveManagementSystem.Application.Services.Users
{
    public interface IUserService
    {
        Task<ApplicationUser> GetLoggedInUser();
        Task<ApplicationUser> GetUserById(string userId);
        Task<List<ApplicationUser>> GetEmployees();
    }
}
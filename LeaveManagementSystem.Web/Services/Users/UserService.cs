using Microsoft.AspNetCore.Identity;

namespace LeaveManagementSystem.Web.Services.Users
{
    public class UserService(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<ApplicationUser> GetLoggedInUser()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User);
            return user;
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }

        public async Task<List<ApplicationUser>> GetEmployees()
        {
            var employees = await _userManager.GetUsersInRoleAsync(Roles.Employee);
            return employees.ToList();
        }
    }
}

using role_api.Data.Models;

namespace role_api.Data.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<RoleData>> GetAllRole(GetRoleRequest request);
        Task<RoleData> GetDetailRole(int id);
        Task<RoleData> CreateRole(CreateRoleRequest request);
        Task<RoleData> UpdateRole(UpdateRoleRequest request);
        Task<int> DeleteRole(int id);
        Task<int> CreateUserRole(CreateUserRoleRequest request);
        Task<IEnumerable<UserRoleData>> GetAllUserRole(GetUserRoleRequest request);
    }
}
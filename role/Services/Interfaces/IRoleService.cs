using role_api.Data.Models;

namespace role_api.Services.Interfaces
{
    public interface IRoleService
    {
        Task<GetRoleResponse> GetAllRole(GetRoleRequest request);
        Task<GetDetailRoleResponse> GetDetailRole(int id);
        Task<CreateRoleResponse> CreateRole(CreateRoleRequest request);
        Task<CreateRoleResponse> UpdateRole(UpdateRoleRequest request);
        Task<DeleteRoleResponse> DeleteRole(int id);
        Task<CreateUserRoleResponse> CreateUserRole(CreateUserRoleRequest request);
        Task<GetUserRoleResponse> GetAllUserRole(GetUserRoleRequest request);
    }
}
using user_api.Data.Models;

namespace user_api.Services.Interfaces
{
    public interface IUserService
    {
        Task<GetUserResponse> GetAllUser(GetUserRequest request);
        Task<GetDetailUserResponse> GetDetailUser(string id);
        Task<CreateUserResponse> CreateUser(CreateUserRequest request);
        Task<CreateUserResponse> UpdateUser(UpdateUserRequest request);
        Task<DeleteUserResponse> DeleteUser(string id);
    }
}
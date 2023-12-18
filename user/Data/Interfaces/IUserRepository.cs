using System.Collections.Generic;
using user_api.Data.Models;

namespace user_api.Data.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserData>> GetAllUser(GetUserRequest request);
        Task<UserData> GetDetailUser(string id);
        Task<UserData> CreateUser(CreateUserRequest request);
        Task<UserData> UpdateUser(UpdateUserRequest request);
        Task<string> DeleteUser(string id);
    }
}
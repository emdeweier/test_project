using System;
using System.Collections.Generic;

namespace user_api.Data.Models
{
    public class UserModel
    {
    }

    public class UserData
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class GetUserRequest
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

    public class GetUserResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public IEnumerable<UserData> Data { get; set; }
    }

    public class GetDetailUserResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public UserData Data { get; set; }
        public IEnumerable<RoleData> Roles { get; set; }
    }
    
    public class CreateUserRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public IEnumerable<int> RoleIds { get; set; }
    }
    
    public class UpdateUserRequest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
    }

    public class CreateUserResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public UserData Data { get; set; }
    }

    public class DeleteUserResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string Id { get; set; }
    }

    #region Role
    
    public class RoleData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }
    
    public class GetRoleResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public IEnumerable<RoleData> Data { get; set; }
    }

    public class GetRoleRequest
    {
        public string Name { get; set; }
    }

    public class GetDetailRoleResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public RoleData Data { get; set; }
    }
    
    public class UserRoleData
    {
        public string UserId { get; set; }
        public int RoleId { get; set; }
    }

    public class CreateUserRoleRequest
    {
        public string UserId { get; set; }
        public IEnumerable<int> RoleId { get; set; }
    }

    public class CreateUserRoleResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public int AffectedRow { get; set; }
    }

    public class GetUserRoleRequest
    {
        public string UserId { get; set; }
        public IEnumerable<int> RoleId { get; set; }
    }

    public class GetUserRoleResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public IEnumerable<UserRoleData> Data { get; set; }
    }

    #endregion
}
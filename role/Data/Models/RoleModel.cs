using System;
using System.Collections.Generic;

namespace role_api.Data.Models
{
    public class RoleModel
    {
    }

    public class RoleData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdate { get; set; }
    }

    public class GetRoleRequest
    {
        public string Name { get; set; }
    }

    public class GetRoleResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public IEnumerable<RoleData> Data { get; set; }
    }

    public class GetDetailRoleResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public RoleData Data { get; set; }
    }
    
    public class CreateRoleRequest
    {
        public string Name { get; set; }
    }
    
    public class UpdateRoleRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CreateRoleResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public RoleData Data { get; set; }
    }

    public class DeleteRoleResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public int Id { get; set; }
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

    public class UserRoleData
    {
        public string UserId { get; set; }
        public int RoleId { get; set; }
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
}
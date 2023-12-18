using System.Net;
using Newtonsoft.Json;
using role_api.Data.Interfaces;
using role_api.Services.Interfaces;
using role_api.Data.Models;
using Serilog;

namespace role_api.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _repository;
        private readonly IConfiguration _configuration;
        
        public RoleService(IRoleRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }
        
        public async Task<GetRoleResponse> GetAllRole(GetRoleRequest request)
        {
            GetRoleResponse response = new GetRoleResponse();
            try
            {
                IEnumerable<RoleData> roles = new List<RoleData>();
                roles = await _repository.GetAllRole(request);

                response.ResponseCode = HttpStatusCode.OK.ToString();
                response.ResponseMessage = "Success Get Role Data";
                response.Data = roles;
                
                Log.Information(String.Format("[API] SUCCESS GET LIST ROLE : \n REQUEST : {0} \n RESPONSE : {1}", JsonConvert.SerializeObject(request),
                    JsonConvert.SerializeObject(response)));
                return response;
            }
            catch (Exception ex)
            {
                response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                response.ResponseMessage = "Error to Fetch Data";
                response.Data = null;
                
                Log.Error(String.Format("[API] ERROR GET LIST ROLE : \n REQUEST : {0} \n RESPONSE : {1} \n EXCEPTION : {2} \n STACK TRACE : {3}", JsonConvert.SerializeObject(request),
                    JsonConvert.SerializeObject(response), ex.Message, ex.StackTrace));
                return response;
            }
        }
        
        public async Task<GetDetailRoleResponse> GetDetailRole(int id)
        {
            GetDetailRoleResponse response = new GetDetailRoleResponse();
            try
            {
                RoleData role = new RoleData();
                if (id == 0)
                {
                    response.ResponseCode = HttpStatusCode.BadRequest.ToString();
                    response.ResponseMessage = "Id Cannot Empty";
                    response.Data = null;
                    return response;
                }
                
                role = await _repository.GetDetailRole(id);
                if (role == null)
                {
                    response.ResponseCode = HttpStatusCode.NotFound.ToString();
                    response.ResponseMessage = "Data Role Not Found";
                    response.Data = role;
                    return response;
                }

                response.ResponseCode = HttpStatusCode.OK.ToString();
                response.ResponseMessage = "Success Get Role Data";
                response.Data = role;
                
                Log.Information(String.Format("[API] SUCCESS GET DETAIL ROLE : \n REQUEST : {0} \n RESPONSE : {1}", id,
                    JsonConvert.SerializeObject(response)));
                return response;
            }
            catch (Exception ex)
            {
                response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                response.ResponseMessage = "Error to Fetch Data";
                response.Data = null;
                
                Log.Error(String.Format("[API] ERROR GET DETAIL ROLE : \n REQUEST : {0} \n RESPONSE : {1} \n EXCEPTION : {2} \n STACK TRACE : {3}", id,
                    JsonConvert.SerializeObject(response), ex.Message, ex.StackTrace));
                return response;
            }
        }

        public async Task<CreateRoleResponse> CreateRole(CreateRoleRequest request)
        {
            CreateRoleResponse response = new CreateRoleResponse();
            try
            {
                if (request.Name.Trim() == "")
                {
                    response.ResponseCode = HttpStatusCode.BadRequest.ToString();
                    response.ResponseMessage = "Role Name Cannot Empty";
                    response.Data = null;
                    return response;
                }
                
                RoleData role = new RoleData();
                GetRoleResponse listRoles = new GetRoleResponse();
                GetRoleRequest listRoleRequest = new GetRoleRequest
                {
                    Name = request.Name
                };
                
                listRoles = await GetAllRole(listRoleRequest);
                if (listRoles.Data.Count() != 0)
                {
                    response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                    response.ResponseMessage = "Data Role Already Exist";
                    response.Data = null;
                    return response;
                }
                role = await _repository.CreateRole(request);

                response.ResponseCode = HttpStatusCode.OK.ToString();
                response.ResponseMessage = "Success Create Role Data";
                response.Data = role;
                
                Log.Information(String.Format("[API] SUCCESS CREATE ROLE : \n REQUEST : {0} \n RESPONSE : {1}", JsonConvert.SerializeObject(request),
                   JsonConvert.SerializeObject(response)));
                return response;
            }
            catch (Exception ex)
            {
                response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                response.ResponseMessage = "Error to Create Data";
                response.Data = null;
                
                Log.Error(String.Format("[API] ERROR CREATE ROLE : \n REQUEST : {0} \n RESPONSE : {1} \n EXCEPTION : {2} \n STACK TRACE : {3}", JsonConvert.SerializeObject(request),
                    JsonConvert.SerializeObject(response), ex.Message, ex.StackTrace));
                return response;
            }
        }

        public async Task<CreateRoleResponse> UpdateRole(UpdateRoleRequest request)
        {
            CreateRoleResponse response = new CreateRoleResponse();
            try
            {
                if (request.Id == 0)
                {
                    response.ResponseCode = HttpStatusCode.BadRequest.ToString();
                    response.ResponseMessage = "Id Cannot Empty";
                    response.Data = null;
                    return response;
                } else if (request.Name.Trim() == "")
                {
                    response.ResponseCode = HttpStatusCode.BadRequest.ToString();
                    response.ResponseMessage = "Role Name Cannot Empty";
                    response.Data = null;
                    return response;
                }
                
                RoleData role = new RoleData();
                GetDetailRoleResponse detailRole = new GetDetailRoleResponse();
                GetRoleResponse listRoles = new GetRoleResponse();
                GetRoleRequest listRoleRequest = new GetRoleRequest
                {
                    Name = request.Name
                };

                detailRole = await GetDetailRole(request.Id);
                if (detailRole.Data == null)
                {
                    response.ResponseCode = HttpStatusCode.NotFound.ToString();
                    response.ResponseMessage = "Data Role Not Found";
                    response.Data = null;
                    return response;
                }
                
                listRoles = await GetAllRole(listRoleRequest);
                if (detailRole.Data.Name.ToLower() != request.Name.ToLower() && listRoles.Data.Count() != 0)
                {
                    response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                    response.ResponseMessage = "Data Role Already Exist";
                    response.Data = null;
                    return response;
                }
                
                role = await _repository.UpdateRole(request);

                response.ResponseCode = HttpStatusCode.OK.ToString();
                response.ResponseMessage = "Success Update Role Data";
                response.Data = role;
                
                Log.Information(String.Format("[API] SUCCESS UPDATE ROLE : \n REQUEST : {0} \n RESPONSE : {1}", JsonConvert.SerializeObject(request),
                   JsonConvert.SerializeObject(response)));
                return response;
            }
            catch (Exception ex)
            {
                response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                response.ResponseMessage = "Error to Create Data";
                response.Data = null;
                
                Log.Error(String.Format("[API] ERROR UPDATE ROLE : \n REQUEST : {0} \n RESPONSE : {1} \n EXCEPTION : {2} \n STACK TRACE : {3}", JsonConvert.SerializeObject(request),
                    JsonConvert.SerializeObject(response), ex.Message, ex.StackTrace));
                return response;
            }
        }
        
        public async Task<DeleteRoleResponse> DeleteRole(int id)
        {
            DeleteRoleResponse response = new DeleteRoleResponse();
            try
            {
                RoleData role = new RoleData();
                role = await _repository.GetDetailRole(id);
                if (role == null)
                {
                    response.ResponseCode = HttpStatusCode.NotFound.ToString();
                    response.ResponseMessage = "Data Role Not Found";
                    response.Id = id;
                    return response;
                }

                var deleteRole = await _repository.DeleteRole(id);
                if (deleteRole == 0)
                {
                    response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                    response.ResponseMessage = "Error to Delete Data";
                    response.Id = id;
                }

                response.ResponseCode = HttpStatusCode.OK.ToString();
                response.ResponseMessage = "Success Delete Data";
                response.Id = id;

                Log.Information(String.Format("[API] SUCCESS DELETE ROLE : \n REQUEST : {0} \n RESPONSE : {1}", id,
                    JsonConvert.SerializeObject(response)));
                return response;
            }
            catch (Exception ex)
            {
                response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                response.ResponseMessage = "Error to Delete Data";
                response.Id = id;

                Log.Error(String.Format(
                    "[API] ERROR DELETE ROLE : \n REQUEST : {0} \n RESPONSE : {1} \n EXCEPTION : {2} \n STACK TRACE : {3}",
                    id,
                    JsonConvert.SerializeObject(response), ex.Message, ex.StackTrace));
                return response;
            }
        }
        
        public async Task<CreateUserRoleResponse> CreateUserRole(CreateUserRoleRequest request)
        {
            CreateUserRoleResponse response = new CreateUserRoleResponse();
            try
            {
                if (request.UserId.Trim() == "")
                {
                    response.ResponseCode = HttpStatusCode.BadRequest.ToString();
                    response.ResponseMessage = "User Id Cannot Empty";
                    response.AffectedRow = 0;
                    return response;
                }
                if (request.RoleId.Count() <= 0)
                {
                    response.ResponseCode = HttpStatusCode.BadRequest.ToString();
                    response.ResponseMessage = "Role Id Cannot Empty";
                    response.AffectedRow = 0;
                    return response;
                }

                GetUserRoleResponse listUserRoles = new GetUserRoleResponse();
                GetUserRoleRequest listUserRoleRequest = new GetUserRoleRequest
                {
                    RoleId = request.RoleId,
                    UserId = request.UserId,
                };
                
                listUserRoles = await GetAllUserRole(listUserRoleRequest);
                if (listUserRoles.Data.Count() != 0)
                {
                    response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                    response.ResponseMessage = "Data User Role Already Exist";
                    response.AffectedRow = 0;
                    return response;
                }
                
                int userRole = await _repository.CreateUserRole(request);

                response.ResponseCode = HttpStatusCode.OK.ToString();
                response.ResponseMessage = "Success Create User Role Data";
                response.AffectedRow = userRole;
                
                Log.Information(String.Format("[API] SUCCESS CREATE USER ROLE : \n REQUEST : {0} \n RESPONSE : {1}", JsonConvert.SerializeObject(request),
                   JsonConvert.SerializeObject(response)));
                return response;
            }
            catch (Exception ex)
            {
                response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                response.ResponseMessage = "Error to Create Data";
                response.AffectedRow = 0;
                
                Log.Error(String.Format("[API] ERROR CREATE USER ROLE : \n REQUEST : {0} \n RESPONSE : {1} \n EXCEPTION : {2} \n STACK TRACE : {3}", JsonConvert.SerializeObject(request),
                    JsonConvert.SerializeObject(response), ex.Message, ex.StackTrace));
                return response;
            }
        }
        
        public async Task<GetUserRoleResponse> GetAllUserRole(GetUserRoleRequest request)
        {
            GetUserRoleResponse response = new GetUserRoleResponse();
            try
            {
                IEnumerable<UserRoleData> userRole = new List<UserRoleData>();
                userRole = await _repository.GetAllUserRole(request);

                response.ResponseCode = HttpStatusCode.OK.ToString();
                response.ResponseMessage = "Success Get User Role Data";
                response.Data = userRole;
                
                Log.Information(String.Format("[API] SUCCESS GET LIST USER ROLE : \n REQUEST : {0} \n RESPONSE : {1}", JsonConvert.SerializeObject(request),
                    JsonConvert.SerializeObject(response)));
                return response;
            }
            catch (Exception ex)
            {
                response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                response.ResponseMessage = "Error to Fetch Data";
                response.Data = null;
                
                Log.Error(String.Format("[API] ERROR GET LIST USER ROLE : \n REQUEST : {0} \n RESPONSE : {1} \n EXCEPTION : {2} \n STACK TRACE : {3}", JsonConvert.SerializeObject(request),
                    JsonConvert.SerializeObject(response), ex.Message, ex.StackTrace));
                return response;
            }
        }
    }
}
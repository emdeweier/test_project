using System.Net;
using System.Text;
using Newtonsoft.Json;
using user_api.Data.Interfaces;
using user_api.Data.Models;
using user_api.Services.Interfaces;
using Serilog;

namespace user_api.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;
        private const string roleClientURL = "https://localhost:44381/role";

        public UserService(IUserRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        public async Task<GetUserResponse> GetAllUser(GetUserRequest request)
        {
            GetUserResponse response = new GetUserResponse();
            try
            {
                IEnumerable<UserData> users = new List<UserData>();
                users = await _repository.GetAllUser(request);

                response.ResponseCode = HttpStatusCode.OK.ToString();
                response.ResponseMessage = "Success Get User Data";
                response.Data = users;

                Log.Information(String.Format("SUCCESS GET LIST USER : \n REQUEST : {0} \n RESPONSE : {1}",
                    JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response)));
                return response;
            }
            catch (Exception ex)
            {
                response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                response.ResponseMessage = "Error to Fetch Data";
                response.Data = null;

                Log.Error(String.Format(
                    "ERROR GET LIST USER : \n REQUEST : {0} \n RESPONSE : {1} \n EXCEPTION : {2} \n STACK TRACE : {3}",
                    JsonConvert.SerializeObject(request), JsonConvert.SerializeObject(response), ex.Message,
                    ex.StackTrace));
                return response;
            }
        }

        public async Task<GetDetailUserResponse> GetDetailUser(string id)
        {
            GetDetailUserResponse response = new GetDetailUserResponse();
            try
            {
                UserData user = new UserData();
                if (String.IsNullOrEmpty(id))
                {
                    response.ResponseCode = HttpStatusCode.BadRequest.ToString();
                    response.ResponseMessage = "Id Cannot Empty";
                    response.Data = null;
                    return response;
                }
                
                user = await _repository.GetDetailUser(id);
                if (user == null)
                {
                    response.ResponseCode = HttpStatusCode.NotFound.ToString();
                    response.ResponseMessage = "Data User Not Found";
                    response.Data = user;
                    return response;
                }

                response.ResponseCode = HttpStatusCode.OK.ToString();
                response.ResponseMessage = "Success Get User Data";
                response.Data = user;
                
                GetUserRoleResponse? userRoles = new GetUserRoleResponse();
                IList<RoleData> roleDatas = new List<RoleData>();
                GetUserRoleRequest userRoleRequest = new GetUserRoleRequest()
                {
                    UserId = user.Id,
                    RoleId = new int[]{}
                };

                var getUserRoleRequest =
                    (HttpWebRequest)WebRequest.Create(String.Format("{0}{1}", roleClientURL, "/get-user-role"));
                var dataUserRole = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(userRoleRequest));
                getUserRoleRequest.Method = "POST";
                getUserRoleRequest.ContentType = "application/json";
                getUserRoleRequest.ContentLength = dataUserRole.Length;
                using (var stream = getUserRoleRequest.GetRequestStream())
                {
                    stream.Write(dataUserRole, 0, dataUserRole.Length);
                }

                var getUserRoleResponse = (HttpWebResponse)getUserRoleRequest.GetResponse();
                var responseUserRoleString = new StreamReader(getUserRoleResponse.GetResponseStream()).ReadToEnd();

                userRoles = JsonConvert.DeserializeObject<GetUserRoleResponse>(responseUserRoleString);
                if (userRoles.ResponseCode != HttpStatusCode.OK.ToString())
                {
                    Log.Information("[API] FAILED TO GET USER ROLE");
                    return response;
                }

                foreach (var roleId in userRoles.Data)
                {
                    GetDetailRoleResponse? roles = new GetDetailRoleResponse();
                    
                    var getRoleRequest =
                        (HttpWebRequest)WebRequest.Create(String.Format("{0}{1}", roleClientURL, "/get-detail-role"));
                    var dataRole = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(roleId.RoleId));
                    getRoleRequest.Method = "POST";
                    getRoleRequest.ContentType = "application/json";
                    getRoleRequest.ContentLength = dataRole.Length;
                    using (var stream = getRoleRequest.GetRequestStream())
                    {
                        stream.Write(dataRole, 0, dataRole.Length);
                    }

                    var getRoleResponse = (HttpWebResponse)getRoleRequest.GetResponse();
                    var responseRoleString = new StreamReader(getRoleResponse.GetResponseStream()).ReadToEnd();

                    roles = JsonConvert.DeserializeObject<GetDetailRoleResponse>(responseRoleString);
                    if (roles.ResponseCode != HttpStatusCode.OK.ToString())
                    {
                        Log.Information("[API] FAILED TO GET USER ROLE");
                        continue;
                    }
                    
                    roleDatas.Add(roles.Data);
                }

                response.Roles = roleDatas;

                Log.Information(String.Format("[API] SUCCESS GET DETAIL USER : \n REQUEST : {0} \n RESPONSE : {1}", id,
                    JsonConvert.SerializeObject(response)));
                return response;
            }
            catch (Exception ex)
            {
                response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                response.ResponseMessage = "Error to Fetch Data";
                response.Data = null;

                Log.Error(String.Format(
                    "[API] ERROR GET DETAIL USER : \n REQUEST : {0} \n RESPONSE : {1} \n EXCEPTION : {2} \n STACK TRACE : {3}",
                    id,
                    JsonConvert.SerializeObject(response), ex.Message, ex.StackTrace));
                return response;
            }
        }

        public async Task<CreateUserResponse> CreateUser(CreateUserRequest request)
        {
            CreateUserResponse response = new CreateUserResponse();
            try
            {
                if (request.Name.Trim() == "")
                {
                    response.ResponseCode = HttpStatusCode.BadRequest.ToString();
                    response.ResponseMessage = "User Name Cannot Empty";
                    response.Data = null;
                    return response;
                }
                if (request.RoleIds.Count() <= 0)
                {
                    response.ResponseCode = HttpStatusCode.BadRequest.ToString();
                    response.ResponseMessage = "Data Role Cannot Empty";
                    response.Data = null;
                    return response;
                }

                UserData user = new UserData();
                GetUserResponse listUsers = new GetUserResponse();
                GetUserRequest listUserRequest = new GetUserRequest
                {
                    Name = request.Name,
                    Phone = "",
                    Address = ""
                };

                listUsers = await GetAllUser(listUserRequest);
                if (listUsers.Data.Count() != 0)
                {
                    response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                    response.ResponseMessage = "Data User Already Exist";
                    response.Data = null;
                    return response;
                }

                user = await _repository.CreateUser(request);

                if (user == null)
                {
                    response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                    response.ResponseMessage = "Error to Create Data";
                    response.Data = null;
                    return response;
                }
                response.ResponseCode = HttpStatusCode.OK.ToString();
                response.ResponseMessage = "Success Create User Data";
                response.Data = user;

                if (request.RoleIds.Count() > 0)
                {
                    GetRoleResponse? listRoles = new GetRoleResponse();
                    GetRoleRequest listRoleRequest = new GetRoleRequest()
                    {
                        Name = ""
                    };

                    var getRoleRequest =
                        (HttpWebRequest)WebRequest.Create(String.Format("{0}{1}", roleClientURL, "/get-role"));
                    var data = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(listRoleRequest));
                    getRoleRequest.Method = "POST";
                    getRoleRequest.ContentType = "application/json";
                    getRoleRequest.ContentLength = data.Length;
                    using (var stream = getRoleRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }

                    var getRoleResponse = (HttpWebResponse)getRoleRequest.GetResponse();
                    var responseString = new StreamReader(getRoleResponse.GetResponseStream()).ReadToEnd();

                    listRoles = JsonConvert.DeserializeObject<GetRoleResponse>(responseString);
                    if (listRoles.Data.Count() <= 0)
                    {
                        Log.Information("[API] DATA ROLE NOT FOUND");
                        return response;
                    }

                    int totalRoles = 0;
                    IList<int> roleIds = new List<int>();
                    foreach (var roleId in request.RoleIds)
                    {
                        foreach (var role in listRoles.Data)
                        {
                            if (roleId == role.Id)
                            {
                                totalRoles += 1;
                                roleIds.Add(roleId);
                            }
                        }
                    }

                    if (totalRoles == 0)
                    {
                        Log.Information("[API] ROLE IS EMPTY");
                        return response;
                    }
                    
                    Log.Information(String.Format("[API] ROLE IDS : {0}", String.Join(",", roleIds)));
                    
                    CreateUserRoleResponse? userRoles = new CreateUserRoleResponse();
                    CreateUserRoleRequest userRoleRequest = new CreateUserRoleRequest()
                    {
                        UserId = user.Id,
                        RoleId = roleIds
                    };

                    var createUserRoleRequest =
                        (HttpWebRequest)WebRequest.Create(String.Format("{0}{1}", roleClientURL, "/create-user-role"));
                    var dataUserRole = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(userRoleRequest));
                    createUserRoleRequest.Method = "POST";
                    createUserRoleRequest.ContentType = "application/json";
                    createUserRoleRequest.ContentLength = dataUserRole.Length;
                    using (var stream = createUserRoleRequest.GetRequestStream())
                    {
                        stream.Write(dataUserRole, 0, dataUserRole.Length);
                    }

                    var getUserRoleResponse = (HttpWebResponse)createUserRoleRequest.GetResponse();
                    var responseUserRoleString = new StreamReader(getUserRoleResponse.GetResponseStream()).ReadToEnd();

                    userRoles = JsonConvert.DeserializeObject<CreateUserRoleResponse>(responseUserRoleString);
                    if (userRoles.ResponseCode != HttpStatusCode.OK.ToString())
                    {
                        Log.Information("[API] FAILED TO CREATE USER ROLE");
                        return response;
                    }
                }

                Log.Information(String.Format("[API] SUCCESS CREATE USER : \n REQUEST : {0} \n RESPONSE : {1}",
                    JsonConvert.SerializeObject(request),
                    JsonConvert.SerializeObject(response)));
                return response;
            }
            catch (Exception ex)
            {
                response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                response.ResponseMessage = "Error to Create Data";
                response.Data = null;

                Log.Error(String.Format(
                    "[API] ERROR CREATE USER : \n REQUEST : {0} \n RESPONSE : {1} \n EXCEPTION : {2} \n STACK TRACE : {3}",
                    JsonConvert.SerializeObject(request),
                    JsonConvert.SerializeObject(response), ex.Message, ex.StackTrace));
                return response;
            }
        }

        public async Task<CreateUserResponse> UpdateUser(UpdateUserRequest request)
        {
            CreateUserResponse response = new CreateUserResponse();
            try
            {
                if (request.Id == "")
                {
                    response.ResponseCode = HttpStatusCode.BadRequest.ToString();
                    response.ResponseMessage = "Id Cannot Empty";
                    response.Data = null;
                    return response;
                }
                else if (request.Name.Trim() == "")
                {
                    response.ResponseCode = HttpStatusCode.BadRequest.ToString();
                    response.ResponseMessage = "User Name Cannot Empty";
                    response.Data = null;
                    return response;
                }

                UserData user = new UserData();
                GetDetailUserResponse detailUser = new GetDetailUserResponse();
                GetUserResponse listUsers = new GetUserResponse();
                GetUserRequest listUserRequest = new GetUserRequest
                {
                    Name = request.Name,
                    Phone = request.Phone,
                    Address = request.Address
                };

                detailUser = await GetDetailUser(request.Id);
                if (detailUser.Data == null)
                {
                    response.ResponseCode = HttpStatusCode.NotFound.ToString();
                    response.ResponseMessage = "Data User Not Found";
                    response.Data = null;
                    return response;
                }

                listUsers = await GetAllUser(listUserRequest);
                if (detailUser.Data.Name.ToLower() != request.Name.ToLower() && listUsers.Data.Count() != 0)
                {
                    response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                    response.ResponseMessage = "Data User Already Exist";
                    response.Data = null;
                    return response;
                }

                user = await _repository.UpdateUser(request);

                response.ResponseCode = HttpStatusCode.OK.ToString();
                response.ResponseMessage = "Success Update User Data";
                response.Data = user;

                Log.Information(String.Format("[API] SUCCESS UPDATE USER : \n REQUEST : {0} \n RESPONSE : {1}",
                    JsonConvert.SerializeObject(request),
                    JsonConvert.SerializeObject(response)));
                return response;
            }
            catch (Exception ex)
            {
                response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                response.ResponseMessage = "Error to Create Data";
                response.Data = null;

                Log.Error(String.Format(
                    "[API] ERROR UPDATE USER : \n REQUEST : {0} \n RESPONSE : {1} \n EXCEPTION : {2} \n STACK TRACE : {3}",
                    JsonConvert.SerializeObject(request),
                    JsonConvert.SerializeObject(response), ex.Message, ex.StackTrace));
                return response;
            }
        }

        public async Task<DeleteUserResponse> DeleteUser(string id)
        {
            DeleteUserResponse response = new DeleteUserResponse();
            try
            {
                UserData user = new UserData();
                user = await _repository.GetDetailUser(id);
                if (user == null)
                {
                    response.ResponseCode = HttpStatusCode.NotFound.ToString();
                    response.ResponseMessage = "Data User Not Found";
                    response.Id = id;
                    return response;
                }

                var deleteUser = await _repository.DeleteUser(id);
                if (String.IsNullOrEmpty(deleteUser))
                {
                    response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                    response.ResponseMessage = "Error to Delete Data";
                    response.Id = id;
                }

                response.ResponseCode = HttpStatusCode.OK.ToString();
                response.ResponseMessage = "Success Delete Data";
                response.Id = id;

                Log.Information(String.Format("[API] SUCCESS DELETE USER : \n REQUEST : {0} \n RESPONSE : {1}", id,
                    JsonConvert.SerializeObject(response)));
                return response;
            }
            catch (Exception ex)
            {
                response.ResponseCode = HttpStatusCode.InternalServerError.ToString();
                response.ResponseMessage = "Error to Delete Data";
                response.Id = id;

                Log.Error(String.Format(
                    "[API] ERROR DELETE USER : \n REQUEST : {0} \n RESPONSE : {1} \n EXCEPTION : {2} \n STACK TRACE : {3}",
                    id,
                    JsonConvert.SerializeObject(response), ex.Message, ex.StackTrace));
                return response;
            }
        }
    }
}
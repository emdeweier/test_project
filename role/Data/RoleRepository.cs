using Dapper;
using Newtonsoft.Json;
using role_api.Contexts;
using role_api.Data.Interfaces;
using role_api.Data.Models;
using Serilog;

namespace role_api.Data
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DbContext _context;

        public RoleRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoleData>> GetAllRole(GetRoleRequest request)
        {
            IEnumerable<RoleData> roles = new List<RoleData>();
            var parameters = new DynamicParameters();
            var condition = "";
            if (!String.IsNullOrEmpty(request.Name))
            {
                condition += " AND name = @Name";
                parameters.Add("@Name", request.Name);
            }

            string query = String.Format("SELECT * FROM roles WHERE 1=1 {0}", condition);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using var trans = connection.BeginTransaction();
                    var roleDatas = await connection.QueryAsync<RoleData>(query, parameters, trans);
                    trans.Commit();
                    connection.Close();
                    roles = roleDatas;

                    Log.Information(String.Format("[REPO] SUCCESS GET LIST ROLE : \n REQUEST : {0} \n RESPONSE : {1}",
                        JsonConvert.SerializeObject(request),
                        JsonConvert.SerializeObject(roles)));
                }
            }
            catch (Exception ex)
            {
                Log.Information(String.Format(
                    "[REPO] ERROR GET LIST ROLE : \n REQUEST : {0} \n EXCEPTION : {1} \n STACK TRACE : {2}",
                    JsonConvert.SerializeObject(request),
                    ex.Message, ex.StackTrace));

                if (ex is TimeoutException)
                {
                    throw ex;
                }

                roles = null;
            }

            return roles;
        }

        public async Task<RoleData> GetDetailRole(int id)
        {
            RoleData roles = new RoleData();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            string query = "SELECT * FROM roles WHERE id = @Id";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using var trans = connection.BeginTransaction();
                    var roleData = await connection.QueryFirstOrDefaultAsync<RoleData>(query, parameters, trans);
                    trans.Commit();
                    connection.Close();
                    roles = roleData;

                    Log.Information(String.Format("[REPO] SUCCESS GET DETAIL ROLE : \n REQUEST : {0} \n RESPONSE : {1}",
                        id,
                        JsonConvert.SerializeObject(roles)));
                }
            }
            catch (Exception ex)
            {
                Log.Information(String.Format(
                    "[REPO] ERROR GET DETAIL ROLE : \n REQUEST : {0} \n EXCEPTION : {1} \n STACK TRACE : {2}",
                    id,
                    ex.Message, ex.StackTrace));

                if (ex is TimeoutException)
                {
                    throw ex;
                }

                roles = null;
            }

            return roles;
        }

        public async Task<RoleData> CreateRole(CreateRoleRequest request)
        {
            RoleData role = new RoleData();
            var parameters = new DynamicParameters();
            parameters.Add("@Name", request.Name);
            string query = "INSERT INTO roles (name) VALUES (@Name)";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using var trans = connection.BeginTransaction();
                    var roleData = await connection.ExecuteAsync(query, parameters, trans);
                    var insertedData =
                        await connection.QueryFirstOrDefaultAsync<int>("SELECT LAST_INSERT_ID()", parameters, trans);
                    var insertedRole = await connection.QueryFirstOrDefaultAsync<RoleData>(
                        "SELECT * FROM roles WHERE id = @Id", new
                        {
                            Id = insertedData
                        }, trans);
                    trans.Commit();
                    connection.Close();
                    role = insertedRole;

                    Log.Information(String.Format("[REPO] SUCCESS CREATE ROLE : \n REQUEST : {0} \n RESPONSE : {1}",
                        JsonConvert.SerializeObject(request),
                        JsonConvert.SerializeObject(roleData)));
                }
            }
            catch (Exception ex)
            {
                Log.Information(String.Format(
                    "[REPO] ERROR CREATE ROLE : \n REQUEST : {0} \n EXCEPTION : {1} \n STACK TRACE : {2}",
                    JsonConvert.SerializeObject(request),
                    ex.Message, ex.StackTrace));

                if (ex is TimeoutException)
                {
                    throw ex;
                }

                role = null;
            }

            return role;
        }

        public async Task<RoleData> UpdateRole(UpdateRoleRequest request)
        {
            RoleData role = new RoleData();
            var parameters = new DynamicParameters();
            parameters.Add("@Name", request.Name);
            parameters.Add("@Id", request.Id);
            parameters.Add("@LastUpdate", DateTime.Now);
            string query = "UPDATE roles SET name = @Name, last_update = @LastUpdate WHERE id = @Id";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using var trans = connection.BeginTransaction();
                    var roleData = await connection.ExecuteAsync(query, parameters, trans);
                    var updatedRole = await connection.QueryFirstOrDefaultAsync<RoleData>(
                        "SELECT * FROM roles WHERE id = @Id", new
                        {
                            Id = request.Id
                        }, trans);
                    trans.Commit();
                    connection.Close();
                    role = updatedRole;

                    Log.Information(String.Format("[REPO] SUCCESS UPDATE ROLE : \n REQUEST : {0} \n RESPONSE : {1}",
                        JsonConvert.SerializeObject(request),
                        JsonConvert.SerializeObject(roleData)));
                }
            }
            catch (Exception ex)
            {
                Log.Information(String.Format(
                    "[REPO] ERROR UPDATE ROLE : \n REQUEST : {0} \n EXCEPTION : {1} \n STACK TRACE : {2}",
                    JsonConvert.SerializeObject(request),
                    ex.Message, ex.StackTrace));

                if (ex is TimeoutException)
                {
                    throw ex;
                }

                role = null;
            }

            return role;
        }

        public async Task<int> DeleteRole(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            string query = "DELETE FROM roles WHERE id = @Id";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using var trans = connection.BeginTransaction();
                    var userData = await connection.ExecuteAsync(query, parameters, trans);
                    trans.Commit();
                    connection.Close();

                    Log.Information(String.Format("[REPO] SUCCESS DELETE ROLE : \n REQUEST : {0} \n RESPONSE : {1}",
                        id,
                        userData));
                }
            }
            catch (Exception ex)
            {
                Log.Information(String.Format(
                    "[REPO] ERROR DELETE ROLE : \n REQUEST : {0} \n EXCEPTION : {1} \n STACK TRACE : {2}",
                    id,
                    ex.Message, ex.StackTrace));

                if (ex is TimeoutException)
                {
                    throw ex;
                }

                return 0;
            }

            return id;
        }
        
        public async Task<int> CreateUserRole(CreateUserRoleRequest request)
        {
            var ParameterInsert = new List<object>();
            foreach (var roleId in request.RoleId)
            {
                ParameterInsert.Add(new
                {
                    UserId = request.UserId,
                    RoleId = roleId
                });
            }
            string query = "INSERT INTO user_role (user_id, role_id) VALUES (@UserId, @RoleId)";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using var trans = connection.BeginTransaction();
                    var userRoleData = await connection.ExecuteAsync(query, ParameterInsert, trans);
                    // var insertedUserRole = await connection.QueryFirstOrDefaultAsync<UserRoleData>(
                    //     "SELECT * FROM user_role WHERE user_id = @UserId AND role_id IN @Id", new
                    //     {
                    //         UserId = request.UserId,
                    //         RoleId = String.Format("{0}", request.RoleId)
                    //     }, trans);
                    trans.Commit();
                    connection.Close();

                    Log.Information(String.Format("[REPO] SUCCESS CREATE USER ROLE : \n REQUEST : {0} \n RESPONSE : {1}",
                        JsonConvert.SerializeObject(request),
                        JsonConvert.SerializeObject(userRoleData)));
                    return userRoleData;
                }
            }
            catch (Exception ex)
            {
                Log.Information(String.Format(
                    "[REPO] ERROR CREATE USER ROLE : \n REQUEST : {0} \n EXCEPTION : {1} \n STACK TRACE : {2}",
                    JsonConvert.SerializeObject(request),
                    ex.Message, ex.StackTrace));

                if (ex is TimeoutException)
                {
                    throw ex;
                }

                return 0;
            }
        }
        
        public async Task<IEnumerable<UserRoleData>> GetAllUserRole(GetUserRoleRequest request)
        {
            IEnumerable<UserRoleData> userRoles = new List<UserRoleData>();
            var parameters = new DynamicParameters();
            var condition = "";
            if (!String.IsNullOrEmpty(request.UserId))
            {
                condition += " AND user_id = @UserId";
                parameters.Add("@UserId", request.UserId);
            }
            if (request.RoleId.Count() > 0)
            {
                condition += " AND role_id IN @RoleId";
                parameters.Add("@RoleId", request.RoleId);
            }

            string query = String.Format("SELECT user_id AS UserId, role_id AS RoleId FROM user_role WHERE 1=1 {0}", condition);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using var trans = connection.BeginTransaction();
                    var userRoleDatas = await connection.QueryAsync<UserRoleData>(query, parameters, trans);
                    trans.Commit();
                    connection.Close();
                    userRoles = userRoleDatas;

                    Log.Information(String.Format("[REPO] SUCCESS GET LIST USER ROLE : \n REQUEST : {0} \n RESPONSE : {1}",
                        JsonConvert.SerializeObject(request),
                        JsonConvert.SerializeObject(userRoles)));
                }
            }
            catch (Exception ex)
            {
                Log.Information(String.Format(
                    "[REPO] ERROR GET LIST USER ROLE : \n REQUEST : {0} \n EXCEPTION : {1} \n STACK TRACE : {2}",
                    JsonConvert.SerializeObject(request),
                    ex.Message, ex.StackTrace));

                if (ex is TimeoutException)
                {
                    throw ex;
                }

                userRoles = null;
            }

            return userRoles;
        }
    }
}
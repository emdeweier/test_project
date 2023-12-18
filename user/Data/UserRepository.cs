using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using user_api.Contexts;
using user_api.Data.Models;
using user_api.Data.Interfaces;
using Serilog;

namespace user_api.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContext _context;

        public UserRepository(DbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserData>> GetAllUser(GetUserRequest request)
        {
            IEnumerable<UserData> users = new List<UserData>();
            var parameters = new DynamicParameters();
            var condition = "";
            if (!String.IsNullOrEmpty(request.Name))
            {
                condition += " AND name = @Name";
                parameters.Add("@Name", request.Name);
            }
            if (!String.IsNullOrEmpty(request.Phone))
            {
                condition += " AND phone = @Phone";
                parameters.Add("@Phone", request.Phone);
            }
            if (!String.IsNullOrEmpty(request.Address))
            {
                condition += " AND address = @Address";
                parameters.Add("@Address", request.Address);
            }
                
            Log.Information(String.Format("[REPO] GET LIST USER CONDITION : {0}", condition));
            
            string query = String.Format("SELECT * FROM users WHERE 1=1 {0}", condition);
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using var trans = connection.BeginTransaction();
                    var history = await connection.QueryAsync<UserData>(query, parameters, trans);
                    trans.Commit();
                    connection.Close();
                    users = history;
                
                    Log.Information(String.Format("[REPO] SUCCESS GET LIST USER : \n REQUEST : {0} \n RESPONSE : {1}", JsonConvert.SerializeObject(request),
                        JsonConvert.SerializeObject(users)));
                }
            }
            catch (Exception ex)
            {
                Log.Information(String.Format("[REPO] ERROR GET LIST USER : \n REQUEST : {0} \n EXCEPTION : {1} \n STACK TRACE : {2}", JsonConvert.SerializeObject(request),
                    ex.Message, ex.StackTrace));
                
                if (ex is TimeoutException)
                {
                    throw ex;
                }

                users = null;
            }
                
            return users;
        }
        
        public async Task<UserData> GetDetailUser(string id)
        {
            UserData user = new UserData();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            string query = "SELECT * FROM users WHERE id = @Id";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using var trans = connection.BeginTransaction();
                    var userData = await connection.QueryFirstOrDefaultAsync<UserData>(query, parameters, trans);
                    trans.Commit();
                    connection.Close();
                    user = userData;

                    Log.Information(String.Format("[REPO] SUCCESS GET DETAIL USER : \n REQUEST : {0} \n RESPONSE : {1}",
                        id,
                        JsonConvert.SerializeObject(user)));
                }
            }
            catch (Exception ex)
            {
                Log.Information(String.Format(
                    "[REPO] ERROR GET DETAIL USER : \n REQUEST : {0} \n EXCEPTION : {1} \n STACK TRACE : {2}",
                    id,
                    ex.Message, ex.StackTrace));

                if (ex is TimeoutException)
                {
                    throw ex;
                }

                user = null;
            }

            return user;
        }
        
        public async Task<UserData> CreateUser(CreateUserRequest request)
        {
            UserData user = new UserData();
            string id = new Helper.GenerateStringHelper().RandomId();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);
            parameters.Add("@Name", request.Name);
            parameters.Add("@Phone", request.Phone);
            parameters.Add("@Address", request.Address);
            string query = "INSERT INTO users (id, name, phone, address) VALUES (@Id, @Name, @Phone, @Address)";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using var trans = connection.BeginTransaction();
                    var userData = await connection.ExecuteAsync(query, parameters, trans);
                    var insertedUser = await connection.QueryFirstOrDefaultAsync<UserData>(
                        "SELECT * FROM users WHERE id = @Id", new
                        {
                            Id = id
                        }, trans);
                    trans.Commit();
                    connection.Close();
                    user = insertedUser;

                    Log.Information(String.Format("[REPO] SUCCESS CREATE USER : \n REQUEST : {0} \n RESPONSE : {1}",
                        JsonConvert.SerializeObject(request),
                        JsonConvert.SerializeObject(userData)));
                }
            }
            catch (Exception ex)
            {
                Log.Information(String.Format(
                    "[REPO] ERROR CREATE USER : \n REQUEST : {0} \n EXCEPTION : {1} \n STACK TRACE : {2}",
                    JsonConvert.SerializeObject(request),
                    ex.Message, ex.StackTrace));

                if (ex is TimeoutException)
                {
                    throw ex;
                }

                user = null;
            }

            return user;
        }

        public async Task<UserData> UpdateUser(UpdateUserRequest request)
        {
            UserData user = new UserData();
            var parameters = new DynamicParameters();
            parameters.Add("@Name", request.Name);
            parameters.Add("@Id", request.Id);
            parameters.Add("@Address", request.Address);
            parameters.Add("@Phone", request.Phone);
            parameters.Add("@LastUpdate", DateTime.Now);
            string query = "UPDATE users SET name = @Name, phone = @Phone, address = @Address, last_update = @LastUpdate WHERE id = @Id";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using var trans = connection.BeginTransaction();
                    var userData = await connection.ExecuteAsync(query, parameters, trans);
                    var updatedUser = await connection.QueryFirstOrDefaultAsync<UserData>(
                        "SELECT * FROM roles WHERE id = @Id", new
                        {
                            Id = request.Id
                        }, trans);
                    trans.Commit();
                    connection.Close();
                    user = updatedUser;

                    Log.Information(String.Format("[REPO] SUCCESS UPDATE USER : \n REQUEST : {0} \n RESPONSE : {1}",
                        JsonConvert.SerializeObject(request),
                        JsonConvert.SerializeObject(userData)));
                }
            }
            catch (Exception ex)
            {
                Log.Information(String.Format(
                    "[REPO] ERROR UPDATE USER : \n REQUEST : {0} \n EXCEPTION : {1} \n STACK TRACE : {2}",
                    JsonConvert.SerializeObject(request),
                    ex.Message, ex.StackTrace));

                if (ex is TimeoutException)
                {
                    throw ex;
                }

                user = null;
            }

            return user;
        }
        
        public async Task<string> DeleteUser(string id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            string query = "DELETE FROM users WHERE id = @Id";
            try
            {
                using (var connection = _context.CreateConnection())
                {
                    connection.Open();
                    using var trans = connection.BeginTransaction();
                    var userData = await connection.ExecuteAsync(query, parameters, trans);
                    trans.Commit();
                    connection.Close();

                    Log.Information(String.Format("[REPO] SUCCESS DELETE USER : \n REQUEST : {0} \n RESPONSE : {1}",
                        id,
                        userData));
                }
            }
            catch (Exception ex)
            {
                Log.Information(String.Format(
                    "[REPO] ERROR DELETE USER : \n REQUEST : {0} \n EXCEPTION : {1} \n STACK TRACE : {2}",
                    id,
                    ex.Message, ex.StackTrace));

                if (ex is TimeoutException)
                {
                    throw ex;
                }

                return "";
            }

            return id;
        }
    }
}
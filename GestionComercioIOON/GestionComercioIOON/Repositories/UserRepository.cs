using GestionComercioIOON.IServices;
using GestionComercioIOON.Model;
using GestionComercioIOON.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestionComercioIOON.Repositories
{
    public class UserRepository : IAuxRepository<User>
    {
        private readonly DatabaseHelper _databaseHelper = new DatabaseHelper(AppSettings.Configuration.GetConnectionString("DefaultConnection"));

        public List<User> GetAllObjects(int offSet, int pageSize)
        {
            List<User> users = new List<User>();

            using (var command = _databaseHelper.CreateCommand("SpGetAllUsers"))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@OffSet", offSet));
                command.Parameters.Add(new SqlParameter("@PageSize", pageSize));

                try
                {
                    DataTable dataUsers = new DataTable();
                    _databaseHelper.OpenConnection();

                    using (var adapter = new SqlDataAdapter((SqlCommand)command))
                    {
                        adapter.Fill(dataUsers);
                    }

                    foreach (DataRow row in dataUsers.Rows)
                    {
                        User user = new User
                        {
                            UserId = row.Field<string>("UserId"),
                            Username = row.Field<string>("Username"),
                            Password = row.Field<string>("Password"),
                            Role = row.Field<string>("Role"),
                            CommerceId = row.IsNull("CommerceId") ? (string?)null : row.Field<string>("CommerceId"),
                            State = row.IsNull("State") ? (string?)null : row.Field<string>("State")
                        };

                        users.Add(user);
                    }

                    return users;
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener los usuarios", ex);
                }
                finally
                {
                    _databaseHelper.CloseConnection();
                }
            }
        }


        public User GetObjectById(object ID)
        {
            using (var command = _databaseHelper.CreateCommand("SpGetUserById"))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@UserId", ID));

                try
                {
                    DataTable dataUser = new DataTable();
                    _databaseHelper.OpenConnection();

                    using (var adapter = new SqlDataAdapter((SqlCommand)command))
                    {
                        adapter.Fill(dataUser);
                    }

                    _databaseHelper.CloseConnection();

                    if (dataUser.Rows.Count > 0)
                    {
                        DataRow row = dataUser.Rows[0];

                        User user = new User
                        {
                            UserId = row.Field<string>("UserId"),
                            Username = row.Field<string>("Username"),
                            Password = row.Field<string>("Password"),
                            Role = row.Field<string>("Role"),
                            CommerceId = row.IsNull("CommerceId") ? (String?)null : row.Field<String>("CommerceId"),
                            State = row.IsNull("State") ? (string?)null : row.Field<string>("State")
                        };

                        return user;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al obtener el usuario", ex);
                }
                finally
                {
                    _databaseHelper.CloseConnection();
                }
            }
        }


        public string UpdateCreateObject(User obj)
        {
            using (var command = _databaseHelper.CreateCommand("SpCreateOrUpdateUser"))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;

                // Agregar los parámetros del objeto User
                command.Parameters.Add(new SqlParameter("@UserId", obj.UserId));
                command.Parameters.Add(new SqlParameter("@Username", obj.Username));
                command.Parameters.Add(new SqlParameter("@Password", obj.Password));
                command.Parameters.Add(new SqlParameter("@Role", obj.Role));
                command.Parameters.Add(new SqlParameter("@CommerceId", obj.CommerceId ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@State", obj.State ?? (object)DBNull.Value));

                try
                {
                    _databaseHelper.OpenConnection();

                    object result = command.ExecuteScalar();

                    _databaseHelper.CloseConnection();

                    return result.ToString();
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al crear o actualizar el usuario", ex);
                }
                finally
                {
                    _databaseHelper.CloseConnection();
                }
            }
        }


        //Funciones Propias
        public User Login(string username, string password)
        {
            using (var command = _databaseHelper.CreateCommand("SpUserLogin"))
            {
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Username", username));
                command.Parameters.Add(new SqlParameter("@Password", password));

                try
                {
                    DataTable dataUser = new DataTable();
                    _databaseHelper.OpenConnection();

                    using (var adapter = new SqlDataAdapter((SqlCommand)command))
                    {
                        adapter.Fill(dataUser);
                    }

                    _databaseHelper.CloseConnection();

                    if (dataUser.Rows.Count > 0)
                    {
                        DataRow row = dataUser.Rows[0];

                        User user = new User
                        {
                            UserId = row.Field<string>("UserId"),
                            Username = row.Field<string>("Username"),
                            Password = row.Field<string>("Password"),
                            Role = row.Field<string>("Role"),
                            CommerceId = row.IsNull("CommerceId") ? (string?)null : row.Field<string>("CommerceId"),
                            State = row.IsNull("State") ? (string?)null : row.Field<string>("State")
                        };

                        return user;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (SqlException ex)
                {
                    throw new Exception("Error al iniciar sesión", ex);
                }
                finally
                {
                    _databaseHelper.CloseConnection();
                }
            }
        }

        public String AuthenticateAsync(User dtoUser)
        {
            try
            {
                var jwt = AppSettings.Configuration.GetSection("Jwt").Get<Jwt>();

                var claims = new[]
                {
                        new Claim(JwtRegisteredClaimNames.Sub, jwt.Subject),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", dtoUser.UserId)
                    };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key));
                var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    jwt.Issuer,
                    jwt.Audience,
                    claims,
                    expires: DateTime.UtcNow.AddDays(2),
                    signingCredentials: signingCredentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}

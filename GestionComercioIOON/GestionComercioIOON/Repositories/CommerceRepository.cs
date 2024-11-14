using GestionComercioIOON.IServices;
using GestionComercioIOON.Services;
using System.Data.SqlClient;
using System.Data;

namespace GestionComercioIOON.Repositories
{
    public class CommerceRepository:ICommerceService
    {
            private readonly DatabaseHelper _databaseHelper = new DatabaseHelper(AppSettings.Configuration.GetConnectionString("DefaultConnection"));

            // Método para crear comercio y propietario
            public string CreateCommerceAndOwner(string commerceName, string address, string ruc, string username, string password, string fullName, string email, string phone, string role)
            {
                try
                {
                    using (var command = _databaseHelper.CreateCommand("SpCreateCommerceAndOwner"))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@CommerceName", commerceName));
                        command.Parameters.Add(new SqlParameter("@Address", address));
                        command.Parameters.Add(new SqlParameter("@RUC", ruc));
                        command.Parameters.Add(new SqlParameter("@Username", username));
                        command.Parameters.Add(new SqlParameter("@Password", password));
                        command.Parameters.Add(new SqlParameter("@FullName", fullName));
                        command.Parameters.Add(new SqlParameter("@Email", email));
                        command.Parameters.Add(new SqlParameter("@Phone", phone));
                        command.Parameters.Add(new SqlParameter("@Role", role));

                        _databaseHelper.OpenConnection();
                         command.ExecuteNonQuery();
                        return "Comercio y propietario creados exitosamente";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al crear el comercio y propietario", ex);
                }
                finally
                {
                    _databaseHelper.CloseConnection();
                }
            }

            // Método para eliminar usuario y comercio
            public string DeleteUserAndCommerce(string userId)
            {
                try
                {
                    using (var command = _databaseHelper.CreateCommand("SpDeleteUserAndCommerce"))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@UserId", userId));

                        _databaseHelper.OpenConnection();
                        command.ExecuteNonQuery();
                        return "Usuario propietario y comercio eliminados exitosamente";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al eliminar el usuario y comercio", ex);
                }
                finally
                {
                    _databaseHelper.CloseConnection();
                }
            }

            // Método para agregar un usuario a un comercio
            public string AddUserToCommerce(string username, string password, string role, string commerceId)
            {
                try
                {
                    using (var command = _databaseHelper.CreateCommand("SpAddUserToCommerce"))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add(new SqlParameter("@Username", username));
                        command.Parameters.Add(new SqlParameter("@Password", password));
                        command.Parameters.Add(new SqlParameter("@Role", role));
                        command.Parameters.Add(new SqlParameter("@CommerceId", commerceId));

                        _databaseHelper.OpenConnection();
                        command.ExecuteNonQuery();
                        return "Usuario creado exitosamente";
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al agregar usuario al comercio", ex);
                }
                finally
                {
                    _databaseHelper.CloseConnection();
                }
            }

    }
}

using GestionComercioIOON.Repositories;
using System.Security.Claims;

namespace GestionComercioIOON.Services
{
    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Subject { get; set; }

        private static UserRepository _repository = new UserRepository();

        public static bool ValidateToken(ClaimsIdentity identity)
        {
            try
            {
                if (identity.Claims.Count() == 0)
                {
                    return false;
                }

                var id = identity.Claims.FirstOrDefault(x => x.Type == "Id").Value;

                return _repository.GetObjectById(id) != null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

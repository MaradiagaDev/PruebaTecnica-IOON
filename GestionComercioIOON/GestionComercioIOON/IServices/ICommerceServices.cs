namespace GestionComercioIOON.IServices
{
    public interface ICommerceService
    {
        string CreateCommerceAndOwner(string commerceName, string address, string ruc, string username, string password, string fullName, string email, string phone, string role);
        string DeleteUserAndCommerce(string userId);
        string AddUserToCommerce(string username, string password, string role, string commerceId);
    }

}

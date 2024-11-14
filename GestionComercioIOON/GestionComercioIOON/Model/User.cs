namespace GestionComercioIOON.Model
{
    public class User
    {
        public string UserId { get; set; }             
        public string Username { get; set; }        
        public string Password { get; set; }         
        public string Role { get; set; }            
        public string? CommerceId { get; set; }        
        public string? State { get; set; }            

        public User()
        {
            UserId = Guid.NewGuid().ToString();
        }

        public bool IsValidRole()
        {
            return Role == "Owner" || Role == "Employee";
        }
    }
}

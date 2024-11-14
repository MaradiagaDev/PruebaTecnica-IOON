namespace GestionComercioIOON.Model
{
    public class Sale
    {
        public string SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public string UserId { get; set; }
        public string CommerceId { get; set; }
        public string State { get; set; }
    }
}

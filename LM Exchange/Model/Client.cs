namespace LM_Exchange.Model
{
    public class Client
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string OwnerName { get; set; }
       
        public string Email { get; set; }

        public string MobileNumber { get; set; }

        public string aadharNumber { get; set; }

        public WalletBalance WalletBalance { get; set; }

    }
}

namespace LM_Exchange.Model
{
    public class WalletBalance
    {
        public int Id { get; set; }

       
        public decimal Balance { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;



        public int ClientId { get; set; } // EF will assume this is a FK to client.Id

        public Client client { get; set; }

    }
}

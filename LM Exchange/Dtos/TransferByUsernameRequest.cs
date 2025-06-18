namespace LM_Exchange.Dtos
{
    public class TransferByUsernameRequest
    {
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }
        public decimal Amount { get; set; }
    }
}

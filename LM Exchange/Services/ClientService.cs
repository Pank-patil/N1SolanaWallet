// Services/ClientService.cs
using LM_Exchange.Data;
using LM_Exchange.Model;
using LM_Exchange.Custom_Exception;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using LM_Exchange.Dtos;

namespace LM_Exchange.Services
{
    public class ClientService : IClientService
    {
        private readonly AppDbContext _context;

        public ClientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Client> CreateClientAsync(Client client)
        {
            if (await _context.Client.AnyAsync(c => c.Email == client.Email))
                throw new InvalidEmail("Email is already registered.");

            if (await _context.Client.AnyAsync(c => c.Username == client.Username))
                throw new Exception("Username is already taken.");

            if (await _context.Client.AnyAsync(c => c.aadharNumber == client.aadharNumber))
                throw new InvalidAadar("Aadhar number already exists.");

            if (await _context.Client.AnyAsync(c => c.MobileNumber == client.MobileNumber))
                throw new InvalidMobile("Mobile number already exists.");

            ValidateClient(client);

            _context.Client.Add(client);
            await _context.SaveChangesAsync();

            return client;
        }

        public async Task<IEnumerable<Client>> GetClientsAsync()
        {
            return await _context.Client.ToListAsync();
        }

        public async Task<Client> GetClientByIdAsync(int id)
        {
            var client = await _context.Client.FindAsync(id);
            if (client == null)
                throw new NotFoundException($"Client with ID {id} not found.");

            return client;
        }

        public async Task UpdateClientAsync(ClientUpdateDto clientUpdateDto)
        {
            var client = await _context.Client.FindAsync(clientUpdateDto.Id);
            if (client == null)
                throw new NotFoundException($"Client with ID {clientUpdateDto.Id} not found.");

            // Validate updated values
            ValidateClient(clientUpdateDto);

            // Update the entity fields with values from the DTO
            client.OwnerName = clientUpdateDto.OwnerName;
            client.Email = clientUpdateDto.Email;
            client.MobileNumber = clientUpdateDto.MobileNumber;
            client.aadharNumber = clientUpdateDto.aadharNumber;


            _context.Client.Update(client);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteClientAsync(int id)
        {
            var client = await _context.Client.FindAsync(id);
            if (client == null)
                throw new NotFoundException($"Client with ID {id} not found.");

            _context.Client.Remove(client);
            await _context.SaveChangesAsync();
        }

        private void ValidateClient(Client client)
        {
            var nameRegex = new Regex(@"^[A-Za-z\s]{2,}$");
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            var aadharRegex = new Regex(@"^\d{12}$");
            var mobileRegex = new Regex(@"^[6-9]\d{9}$");

            if (string.IsNullOrWhiteSpace(client.OwnerName) || !nameRegex.IsMatch(client.OwnerName))
            {
                throw new InvalidName("Owner name must contain only letters and spaces (minimum 2 characters).");
            }

            if (string.IsNullOrWhiteSpace(client.Email) || !emailRegex.IsMatch(client.Email))
            {
                throw new InvalidEmail("Invalid email format.");

            }
            if (string.IsNullOrWhiteSpace(client.aadharNumber) || !aadharRegex.IsMatch(client.aadharNumber))
            {
                throw new InvalidAadar("Aadhar must be a 12-digit numeric string.");

            }
            if (string.IsNullOrWhiteSpace(client.MobileNumber) || !mobileRegex.IsMatch(client.MobileNumber))
            {
                throw new InvalidMobile("Mobile number must be a 10-digit number starting with 6-9.");

            }
        }

        private void ValidateClient(ClientUpdateDto clientUpdateDto)
        {
            var nameRegex = new Regex(@"^[A-Za-z\s]{2,}$");
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            var aadharRegex = new Regex(@"^\d{12}$");
            var mobileRegex = new Regex(@"^[6-9]\d{9}$");

            if (string.IsNullOrWhiteSpace(clientUpdateDto.OwnerName) || !nameRegex.IsMatch(clientUpdateDto.OwnerName))
            {
                throw new InvalidName("Owner name must contain only letters and spaces (minimum 2 characters).");
            }

            if (string.IsNullOrWhiteSpace(clientUpdateDto.Email) || !emailRegex.IsMatch(clientUpdateDto.Email))
            {
                throw new InvalidEmail("Invalid email format.");

            }
            if (string.IsNullOrWhiteSpace(clientUpdateDto.aadharNumber) || !aadharRegex.IsMatch(clientUpdateDto.aadharNumber))
            {
                throw new InvalidAadar("Aadhar must be a 12-digit numeric string.");

            }
            if (string.IsNullOrWhiteSpace(clientUpdateDto.MobileNumber) || !mobileRegex.IsMatch(clientUpdateDto.MobileNumber))
            {
                throw new InvalidMobile("Mobile number must be a 10-digit number starting with 6-9.");

            }
           
        }

        public async Task AddMoneyAsync(int clientId, decimal amount)
        {

            if (amount <= 0)
            {
                throw new InvalidAmountExpection("Amount must be greater than zero.");
            }
            var wallet = await _context.WalletBalances
                  .FirstOrDefaultAsync(w => w.ClientId == clientId);

            if (wallet == null)
            {
                var exists = await _context.Client.AnyAsync(c => c.Id == clientId);
                if (!exists)
                {
                    throw new NotFoundException("Client not found.");
                }

                wallet = new WalletBalance
                {
                    ClientId = clientId,
                    Balance = amount
                };
                _context.WalletBalances.Add(wallet);
            }
            else
            {
                wallet.Balance += amount;
                wallet.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
        public async Task WithdrawMoneyAsync(int clientId, decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidAmountExpection("Amount must be greater than zero.");
            }
            var wallet = await _context.WalletBalances
                .FirstOrDefaultAsync(w => w.ClientId == clientId);

            if (wallet == null)
            {
                throw new NotFoundException("wallet not found.");
            }

            if (wallet.Balance < amount)
            {
                throw new InsufficientbalanceException("Insufficient balance.");
            }
                

            wallet.Balance -= amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task SendMoneyByUsernameAsync(string senderUsername, string receiverUsername, decimal amount)
        {
            if (senderUsername == receiverUsername)
            {
                throw new SelfTransationException("Cannot send money to yourself.");
            }
               

            if (amount <= 0)
            {
                throw new InvalidAmountExpection("Amount must be greater than zero.");
            }
             

            var sender = await _context.Client.FirstOrDefaultAsync(c => c.Username == senderUsername);
            var receiver = await _context.Client.FirstOrDefaultAsync(c => c.Username == receiverUsername);

            if (sender == null || receiver == null)
                throw new NotFoundException("Sender or receiver not found.");

            var senderWallet = await _context.WalletBalances.FirstOrDefaultAsync(w => w.ClientId == sender.Id);
            var receiverWallet = await _context.WalletBalances.FirstOrDefaultAsync(w => w.ClientId == receiver.Id);

            if (senderWallet == null || receiverWallet == null)
                throw new NotFoundException("Sender or receiver wallet not found.");

            if (senderWallet.Balance < amount)
            {
                throw new InsufficientbalanceException("Insufficient balance.");
            }
                

            // Update balances
            senderWallet.Balance -= amount;
            receiverWallet.Balance += amount;

           
            await _context.SaveChangesAsync();
        }



    }
}

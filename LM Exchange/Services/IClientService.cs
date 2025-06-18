using LM_Exchange.Dtos;
using LM_Exchange.Model;

namespace LM_Exchange.Services
{
        public interface IClientService
        {
            Task<Client> CreateClientAsync(Client client);


            Task<IEnumerable<Client>> GetClientsAsync();


            Task<Client> GetClientByIdAsync(int id);

            Task UpdateClientAsync(ClientUpdateDto clientUpdateDto);

            Task DeleteClientAsync(int id);
        Task AddMoneyAsync(int clientId, decimal amount);
        Task WithdrawMoneyAsync(int clientId, decimal amount);
        Task SendMoneyByUsernameAsync(string senderUsername, string receiverUsername, decimal amount);
    }
    }



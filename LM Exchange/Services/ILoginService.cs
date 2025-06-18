using System.Security.Claims;
using LM_Exchange.Dtos;
using LM_Exchange.Model;

namespace LM_Exchange.Services
{
    public interface ILoginService 
    {
    
        LoginResponseDto CreateToken(User user);             
        string GenerateRefreshToken();                        
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        void ValidateUser(User user);
    }
}

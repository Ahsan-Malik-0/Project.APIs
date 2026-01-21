using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Services
{
    public interface IAuthService
    {
        Task<Member?> RegisterAsync(MemberDto request);
        Task<string?> LoginAsync(MemberLoginDto request); 
    }
}

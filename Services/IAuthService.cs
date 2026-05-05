using Project.APIs.Model;
using Project.APIs.Model.DTOs;
using static Project.APIs.Model.DTOs.AdministrationDto;

namespace Project.APIs.Services
{
    public interface IAuthService
    {
        Task<Member?> RegisterAsync(MemberDto request);
        Task<string?> LoginAsync(string username, string hashPassword); 
        Task<string?> LoginAdminAsync(string username, string hashPassword);
        Task<Administration?> RegisterAdminAsync(CreateAdministrationDto request);
    }
}

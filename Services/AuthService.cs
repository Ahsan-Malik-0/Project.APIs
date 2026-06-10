using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Project.APIs.Model.DTOs.AdministrationDto;

namespace Project.APIs.Services
{
    public class AuthService(DB _dB, IPasswordHasher<Member> _passwordHasher, IConfiguration configuration) : IAuthService
    {
        // Handle Member Service --------------------------------------------------------
        public async Task<String?> LoginAsync(string username, string hashPassword)
        {
            var member = await _dB.Members.FirstOrDefaultAsync(m => m.Username == username);

            if (member == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(hashPassword))
            {
                throw new BusinessRuleException("Password must required");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                null!,
                member.HashPassword!,
                hashPassword
            );

            if (result == PasswordVerificationResult.Failed)
            {
                throw new BusinessRuleException("Incorrect Password");
            }

            string token = CreateToken(member);

            return token;
        }

        public async Task<Member?> RegisterAsync(MemberDto memberDto)
        {
            // Check for duplicate username FIRST
            var usernameExists = await _dB.Members
                .AnyAsync(m => m.Username == memberDto.Username);

            if (usernameExists)
            {
                throw new BusinessRuleException("Username already exist");
            }

            if (string.IsNullOrEmpty(memberDto.HashPassword))
            {
                throw new BusinessRuleException("Password must not be null");
            }

            // Validate society ONLY if SocietyId has a value AND it's not empty
            Guid? societyIdToUse = null;

            if (memberDto.SocietyId != Guid.Empty)
            {
                var society = await _dB.Societies.FirstOrDefaultAsync(s => s.Id == memberDto.SocietyId);
                if (society == null)
                {
                    throw new NotFoundException("Society Not Found");
                }
                societyIdToUse = memberDto.SocietyId;
            }
            // else keep societyIdToUse as NULL

            Member member = new Member
            {
                Id = Guid.NewGuid(),  // Explicitly create new GUID
                Name = memberDto.Name,
                Username = memberDto.Username,
                HashPassword = _passwordHasher.HashPassword(null!, memberDto.HashPassword!),
                Role = memberDto.Role,
                ProfileImage = memberDto.ProfileImage,
                SocietyId = societyIdToUse,  // This will be NULL when no society provided
            };

            await _dB.Members.AddAsync(member);
            await _dB.SaveChangesAsync();
            return member;
        }

        private string CreateToken(Member member)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSetting:Token")!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, member.Id.ToString()),
                new Claim(ClaimTypes.Name, member.Username),
                new Claim(ClaimTypes.Role, member.Role)
            };

            var tokenDescriptor = new JwtSecurityToken(
                signingCredentials: creds,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                issuer: configuration.GetValue<string>("AppSetting:Issuer"),
                audience: configuration.GetValue<string>("AppSetting:Audience")
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

       

    }
}

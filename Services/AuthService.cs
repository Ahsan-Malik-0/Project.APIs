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

namespace Project.APIs.Services
{
    public class AuthService(DB _dB, IPasswordHasher<Member> _passwordHasher, IConfiguration configuration) : IAuthService
    {
        public async Task<String?> LoginAsync(MemberLoginDto request)
        {
            var member = await _dB.Members.FirstOrDefaultAsync(m => m.Username == request.Username);

            if (member == null)
            {
                // Always return same error for security
                throw new NotFoundException("Member Not Found");
            }

            if (string.IsNullOrEmpty(request.HashPassword))
            {
                throw new BusinessRuleException("Password must required");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                null!,
                member.HashPassword!,
                request.HashPassword
            );

            if (result == PasswordVerificationResult.Failed)
            {
                throw new BusinessRuleException("Incorrent Password");
            }

            string token = CreateToken(member);

            return token;
        }

        public async Task<Member?> RegisterAsync(MemberDto memberDto)
        {
            var society = await _dB.Societies.FirstOrDefaultAsync(s => s.Id == memberDto.SocietyId);
            if (society == null)
            {
                throw new NotFoundException("Society Not Found");
            }

            // Check for duplicate username
            var usernameExists = await _dB.Members
                //.AnyAsync(m => m.SocietyId == memberDto.SocietyId &&
                //              m.Username == memberDto.Username);
                .AnyAsync(m => m.Username == memberDto.Username);

            if (usernameExists)
            {
                throw new BusinessRuleException("Username already exist");
            }

            if (string.IsNullOrEmpty(memberDto.HashPassword))
            {
                throw new BusinessRuleException("Password must not be null");
            }

            Member member = new Member
            {
                Name = memberDto.Name,
                Username = memberDto.Username,
                HashPassword = _passwordHasher.HashPassword(null!, memberDto.HashPassword!),
                Role = memberDto.Role,
                Picture = memberDto.Picture,
                SocietyId = memberDto.SocietyId, // Set SocietyId
                Society = society // Set Society object
            };

            await _dB.Members.AddAsync(member);
            await _dB.SaveChangesAsync();
            //return Created();
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

using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;
using System.Globalization;

namespace Project.APIs.Services
{
    public class MemberService(DB _dB, IPasswordHasher<Member> _passwordHasher, IWebHostEnvironment _env)
    {
        // View Profile
        public async Task<MemberProfileDto> ViewProfile(Guid id)
        {
            var member = await _dB.Members.FindAsync(id);

            if (member == null)
                throw new NotFoundException("Member not found");

            // Image handling (only send base64)
            string trimImage = member.Picture.TrimStart('/');
            string delimiters = "/";
            string[] splitImage = trimImage.Split(delimiters);
            string base64Image = splitImage[1];

            MemberProfileDto memberProfileDto = new MemberProfileDto()
            {
                Name = member.Name,
                Username = member.Username,
                Picture = base64Image,
                SocietyId = member.SocietyId
            };

            return memberProfileDto;
        }

        //Edit Profile by President
        public async Task EditProfile(UpdateMemberProfileDto updatedMember)
        {
            var oldMember = await _dB.Members.FindAsync(updatedMember.Id);

            if (oldMember == null)
                throw new NotFoundException("Member not found");

            if (string.IsNullOrEmpty(updatedMember.OldHashPassword))
                throw new BusinessRuleException("Old password must be provided.");

            // Verify password by comparing 
            var passVerify = _passwordHasher.VerifyHashedPassword(
                oldMember,
                oldMember.HashPassword!,
                updatedMember.OldHashPassword
            );

            if (passVerify == PasswordVerificationResult.Failed)
            {
                throw new BusinessRuleException("Password did not matched.");
            }


            // image handling

            if (!string.IsNullOrEmpty(updatedMember.Picture))
            {
                // Delete old image (if exists)
                if (!string.IsNullOrEmpty(oldMember.Picture))
                {
                    var oldImagePath = Path.Combine(_env.WebRootPath,oldMember.Picture.TrimStart('/'));

                    if (File.Exists(oldImagePath))
                        File.Delete(oldImagePath);
                }

                // Create folder if not exists
                var folderPath = Path.Combine(_env.WebRootPath, "profiles");
                Directory.CreateDirectory(folderPath);

                // Generate unique file name
                var fileName = $"{Guid.NewGuid()}.png";
                var fullPath = Path.Combine(folderPath, fileName);

                // Convert Base64 → byte[]
                byte[] imageBytes = Convert.FromBase64String(updatedMember.Picture);

                // Save file
                await File.WriteAllBytesAsync(fullPath, imageBytes);

                // Save relative path in DB
                oldMember.Picture = $"/profiles/{fileName}";
            }


            oldMember.Name = updatedMember.Name;
            oldMember.Username = updatedMember.Username;
            oldMember.HashPassword = _passwordHasher.HashPassword(oldMember, updatedMember.NewHashPassword!);
            
            _dB.Members.Update(oldMember);
            await _dB.SaveChangesAsync();
        }

        public async Task<Guid?> GetSocietyIdAsync(Guid memberId)
        {
            return await _dB.Members
                .Where(m => m.Id == memberId)
                .Select(m => m.SocietyId)
                .FirstOrDefaultAsync();
        }


    }
}

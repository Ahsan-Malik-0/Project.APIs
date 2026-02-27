using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;
using System.Diagnostics.Metrics;
using System.Dynamic;
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

            string? base64Image = null;

            if (!string.IsNullOrEmpty(member.Picture))
            {
                // Remove starting '/'
                var relativePath = member.Picture.TrimStart('/');

                // Get full physical path
                var fullPath = Path.Combine(_env.WebRootPath, relativePath);

                if (File.Exists(fullPath))
                {
                    // Read image as byte[]
                    byte[] imageBytes = await File.ReadAllBytesAsync(fullPath);

                    // Convert to Base64
                    base64Image = Convert.ToBase64String(imageBytes);
                }
            }

            MemberProfileDto memberProfileDto = new MemberProfileDto()
            {
                Name = member.Name,
                Username = member.Username,
                Picture = base64Image!,
                SocietyId = member.SocietyId
            };

            return memberProfileDto;
        }

        //Edit Profile by President
        //public async Task EditProfile(UpdateMemberProfileDto updatedMember)
        //{
        //    var oldMember = await _dB.Members.FindAsync(updatedMember.Id);

        //    if (oldMember == null)
        //        throw new NotFoundException("Member not found");

        //    if (string.IsNullOrEmpty(updatedMember.OldHashPassword))
        //        throw new BusinessRuleException("Old password must be provided.");

        //    // Verify password by comparing 
        //    var passVerify = _passwordHasher.VerifyHashedPassword(
        //        oldMember,
        //        oldMember.HashPassword!,
        //        updatedMember.OldHashPassword
        //    );

        //    if (passVerify == PasswordVerificationResult.Failed)
        //    {
        //        throw new BusinessRuleException("Password did not matched.");
        //    }


        //    // image handling

        //    if (!string.IsNullOrEmpty(updatedMember.Picture))
        //    {
        //        // Delete old image (if exists)
        //        if (!string.IsNullOrEmpty(oldMember.Picture))
        //        {
        //            var oldImagePath = Path.Combine(_env.WebRootPath,oldMember.Picture.TrimStart('/'));

        //            if (File.Exists(oldImagePath))
        //                File.Delete(oldImagePath);
        //        }

        //        // Create folder if not exists
        //        var folderPath = Path.Combine(_env.WebRootPath, "profiles");
        //        Directory.CreateDirectory(folderPath);

        //        // Generate unique file name
        //        var fileName = $"{Guid.NewGuid()}.png";
        //        var fullPath = Path.Combine(folderPath, fileName);

        //        // Convert Base64 → byte[]
        //        byte[] imageBytes = Convert.FromBase64String(updatedMember.Picture);

        //        // Save file
        //        await File.WriteAllBytesAsync(fullPath, imageBytes);

        //        // Save relative path in DB
        //        oldMember.Picture = $"/profiles/{fileName}";
        //    }


        //    oldMember.Name = updatedMember.Name;
        //    oldMember.Username = updatedMember.Username;
        //    oldMember.HashPassword = _passwordHasher.HashPassword(oldMember, updatedMember.NewHashPassword!);

        //    _dB.Members.Update(oldMember);
        //    await _dB.SaveChangesAsync();
        //}

        public async Task EditProfile(UpdateMemberProfileDto updatedMember)
        {
            var oldMember = await _dB.Members.FindAsync(updatedMember.Id);

            // ==============================
            // 🔐 PASSWORD CHANGE (OPTIONAL)
            // ==============================

            if (!string.IsNullOrEmpty(updatedMember.NewHashPassword))
            {
                if (string.IsNullOrEmpty(updatedMember.OldHashPassword))
                    throw new BusinessRuleException("Old password must be provided.");

                var passVerify = _passwordHasher.VerifyHashedPassword(
                    oldMember,
                    oldMember.HashPassword!,
                    updatedMember.OldHashPassword
                );

                if (passVerify == PasswordVerificationResult.Failed)
                    throw new BusinessRuleException("Old password is incorrect.");

                // Hash and update new password
                oldMember.HashPassword = _passwordHasher.HashPassword(
                    oldMember,
                    updatedMember.NewHashPassword
                );
            }

            // image handling

            if (!string.IsNullOrEmpty(updatedMember.Picture))
            {
                // Delete old image (if exists)
                if (!string.IsNullOrEmpty(oldMember.Picture))
                {
                    var oldImagePath = Path.Combine(_env.WebRootPath, oldMember.Picture.TrimStart('/'));

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

            await _dB.SaveChangesAsync();


              // 👉 We removed _dB.Members.Update(oldMember);
            // Because EF Core is already tracking oldMember after FindAsync.
            // Calling Update is unnecessary and can sometimes cause issues.
        }

        public async Task<Guid?> GetSocietyIdAsync(Guid memberId)
        {
            return await _dB.Members
                .Where(m => m.Id == memberId)
                .Select(m => m.SocietyId)
                .FirstOrDefaultAsync();
        }


        public async Task<ChairpersonDetailForRequisitionDto> GetChairpersonDetailsForRequisitionForm(Guid memberId)
        {
            var member = await _dB.Members
                .Include(m => m.Society)
                .FirstOrDefaultAsync(m => m.Id == memberId);

            if (member == null)
                throw new NotFoundException("Member not found");

            ChairpersonDetailForRequisitionDto memberDetail = new ChairpersonDetailForRequisitionDto()
            {
                Name = member.Name,
                Role = member.Role,
                SocietyName = member.Society!.Name,
            };

            return memberDetail;
        }

    }
}

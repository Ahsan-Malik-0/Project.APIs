using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;
using static Project.APIs.Model.DTOs.AdministrationDto;

namespace Project.APIs.Services
{
    public class AdministrationService(DB _dB, IPasswordHasher<Member> _passwordHasher, IWebHostEnvironment _env)
    {
        
        // View Admin Profile
        public async Task<AdminProfileDto> ViewProfile(Guid id)
        {
            var admin = await _dB.Administrations.FindAsync(id);

            if (admin == null)
                throw new NotFoundException("Admin not found");

            string? base64Image = null;

            if (!string.IsNullOrEmpty(admin.ProfileImage))
            {
                // Remove starting '/'
                var relativePath = admin.ProfileImage.TrimStart('/');

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

            AdminProfileDto adminProfileDto = new AdminProfileDto()
            {
                Name = admin.Name,
                Username = admin.Username,
                Picture = base64Image ?? admin.ProfileImage,
            };

            return adminProfileDto;
        }

        public async Task EditProfile(Guid memberId, UpdateAdminProfileDto updatedAdmin)
        {
            var oldAdmin = await _dB.Administrations.FindAsync(memberId);

            // ==============================
            // 🔐 PASSWORD CHANGE (OPTIONAL)
            // ==============================

            if (!string.IsNullOrEmpty(updatedAdmin.NewHashPassword))
            {
                if (string.IsNullOrEmpty(updatedAdmin.OldHashPassword))
                    throw new BusinessRuleException("Old password must be provided.");

                var passVerify = _passwordHasher.VerifyHashedPassword(
                    null!,
                    oldAdmin!.HashPassword!,
                    updatedAdmin.OldHashPassword
                );

                if (passVerify == PasswordVerificationResult.Failed)
                    throw new BusinessRuleException("Old password is incorrect.");

                // Hash and update new password
                oldAdmin.HashPassword = _passwordHasher.HashPassword(
                    null!,
                    updatedAdmin.NewHashPassword
                );
            }

            // image handling

            if (!string.IsNullOrEmpty(updatedAdmin.Picture))
            {
                // Delete old image (if exists)
                if (!string.IsNullOrEmpty(oldAdmin!.ProfileImage))
                {
                    var oldImagePath = Path.Combine(_env.WebRootPath, oldAdmin.ProfileImage.TrimStart('/'));

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
                byte[] imageBytes = Convert.FromBase64String(updatedAdmin.Picture);

                // Save file
                await File.WriteAllBytesAsync(fullPath, imageBytes);

                // Save relative path in DB
                oldAdmin.ProfileImage = $"/profiles/{fileName}";
            }

                
            oldAdmin!.Name = updatedAdmin.Name;
            oldAdmin.Username = updatedAdmin.Username;

            await _dB.SaveChangesAsync();
        }

    }
}
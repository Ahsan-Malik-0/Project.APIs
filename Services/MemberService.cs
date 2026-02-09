using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Services
{
    public class MemberService(DB _dB, IPasswordHasher<Member> _passwordHasher)
    {
        // View Profile
        public async Task<Member> ViewProfile(Guid id)
        {
            var member = await _dB.Members.FindAsync(id);

            if (member == null)
                throw new NotFoundException("Member not found");

            return member;
        }

        //Edit Profile by President
        public async Task EditProfile(UpdateMemberProfileDto updatedMember)
        {
            var oldMember = await _dB.Members.FindAsync(updatedMember.Id);

            if (oldMember == null)
                throw new NotFoundException("Member not found");

            //if (string.IsNullOrEmpty(updatedMember.OldHashPassword))
            //    throw new BusinessRuleException("Old password must be provided.");

            //var passVerify = _passwordHasher.VerifyHashedPassword(
            //    oldMember,
            //    oldMember.HashPassword!,
            //    updatedMember.OldHashPassword
            //);

            //if (passVerify == PasswordVerificationResult.Failed)
            //{
            //    throw new BusinessRuleException("Password did not matched.");
            //}


            oldMember.Name = updatedMember.Name;
            oldMember.Username = updatedMember.Username;
            //oldMember.HashPassword = _passwordHasher.HashPassword(oldMember, updatedMember.NewHashPassword!);
            oldMember.Picture = updatedMember.Picture;
            
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

using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using SkeletonDatingProject.DTO;
using SkeletonDatingProject.Entities;
using SkeletonDatingProject.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkeletonDatingProject.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser appUser, MemberUpdateDto memberUpdateDto);
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUserNameAsync(string userName);
        Task<MemberDto> GetMemberAsync(string userName);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        void DeleteAppUserAsync(AppUser user);
        bool IsUserAvailable(int id);
        Task<ImageUploadResult> AddPhoto(IFormFile file);
        Task<string> GetUserGender(string userName);

    }
}

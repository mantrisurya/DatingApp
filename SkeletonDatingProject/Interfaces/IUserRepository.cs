using SkeletonDatingProject.DTO;
using SkeletonDatingProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkeletonDatingProject.Interfaces
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByUserNameAsync(string userName);
        Task<MemberDto> GetMemberAsync(string userName);
        Task<IEnumerable<MemberDto>> GetMembersAsync();
        void DeleteAppUserAsync(AppUser user);
        bool IsUserAvailable(int id);
    }
}

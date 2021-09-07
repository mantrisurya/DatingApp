using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SkeletonDatingProject.DTO;
using SkeletonDatingProject.Entities;
using SkeletonDatingProject.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkeletonDatingProject.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext dataContext, IMapper mapper)
        {
            _context = dataContext;
            _mapper = mapper;
        }
        public async Task<AppUser> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        public bool IsUserAvailable(int id)
        {
            return _context.Users.Count(user => user.Id == id) > 0;
        }

        public async Task<AppUser> GetUserByUserNameAsync(string userName)
        {
            return await _context.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(user => user.UserName == userName.ToLower());
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users
                .Include(p => p.Photos)
                .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public void DeleteAppUserAsync(AppUser appUser)
        {
            _context.Users.Remove(appUser);
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<MemberDto> GetMemberAsync(string userName)
        {
            return await _context.Users
                            .Where(x => x.UserName == userName.ToLower())
                            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                            .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetMembersAsync()
        {
            return await _context.Users
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
    }
}

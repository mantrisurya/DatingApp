using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkeletonDatingProject.Data;
using SkeletonDatingProject.DTO;
using SkeletonDatingProject.Entities;
using SkeletonDatingProject.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonDatingProject.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _dataContext;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountController(DataContext dataContext, ITokenService tokenService, IMapper mapper)
        {
            _dataContext = dataContext;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<AppUserDto>> Register(RegisterUserDto registerUserDto)
        {
            if (await CheckUserExists(registerUserDto)) return BadRequest("User name already exists.");
            var user = _mapper.Map<AppUser>(registerUserDto); 
            using var hmac = new HMACSHA512();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerUserDto.Password));
            user.PasswordSalt = hmac.Key;

            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();

            return new AppUserDto {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                KnownAs = user.KnownAs
            };
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AppUserDto>> Login(LoginUserDto loginUserDto)
        {
            var user = await _dataContext.Users.Include(x => x.Photos).SingleOrDefaultAsync(user => user.UserName == loginUserDto.UserName.ToLower());
            if (user == null) return BadRequest("Invalid username");
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginUserDto.Password));
            for(int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Incorrect password.");
            }

            return new AppUserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs
            };
        }

        private async Task<bool> CheckUserExists(RegisterUserDto registerUserDto)
        {
            return await _dataContext.Users.AnyAsync(user => user.UserName == registerUserDto.UserName.ToLower());
        }
    }
}

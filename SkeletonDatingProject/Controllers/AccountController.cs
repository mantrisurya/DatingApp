using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkeletonDatingProject.DTO;
using SkeletonDatingProject.Entities;
using SkeletonDatingProject.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SkeletonDatingProject.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<AppUserDto>> Register(RegisterUserDto registerUserDto)
        {
            if (await CheckUserExists(registerUserDto)) return BadRequest("User name already exists.");
            var user = _mapper.Map<AppUser>(registerUserDto);
            //using var hmac = new HMACSHA512();
            //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerUserDto.Password));
            //user.PasswordSalt = hmac.Key;

            user.UserName = registerUserDto.UserName.ToLower();

            var result = await _userManager.CreateAsync(user, registerUserDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) return BadRequest(result.Errors);
            return new AppUserDto {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AppUserDto>> Login(LoginUserDto loginUserDto)
        {
            var user = await _userManager.Users.Include(x => x.Photos).SingleOrDefaultAsync(user => user.UserName == loginUserDto.UserName.ToLower());
            if (user == null) return BadRequest("Invalid username");
            //using var hmac = new HMACSHA512(user.PasswordSalt);
            //var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginUserDto.Password));
            //for(int i = 0; i < computedHash.Length; i++)
            //{
            //    if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Incorrect password.");
            //}
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginUserDto.Password, false);
            if (!result.Succeeded) return Unauthorized();

            return new AppUserDto
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }

        private async Task<bool> CheckUserExists(RegisterUserDto registerUserDto)
        {
            return await _userManager.Users.AnyAsync(user => user.UserName == registerUserDto.UserName.ToLower());
        }
    }
}

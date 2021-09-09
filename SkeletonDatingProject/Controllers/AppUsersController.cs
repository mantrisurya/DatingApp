﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkeletonDatingProject.Data;
using SkeletonDatingProject.DTO;
using SkeletonDatingProject.Entities;
using SkeletonDatingProject.Interfaces;

namespace SkeletonDatingProject.Controllers
{
    [Authorize]
    public class AppUsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AppUsersController(IUserRepository repository, IMapper mapper)
        {
            _userRepository = repository;
            _mapper = mapper;
        }

        // GET: api/AppUsers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetAppUsers()
        {
            return Ok(await _userRepository.GetMembersAsync());
        }

        // GET: api/AppUsers/5
        [HttpGet("{userName}")]
        public async Task<ActionResult<MemberDto>> GetAppUser(string userName)
        {
            return await _userRepository.GetMemberAsync(userName);
        }

        // PUT: api/AppUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> UpdateAppUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUserNameAsync(username);
            if (user == null)
            {
                return BadRequest();
            }

            _userRepository.Update(user, memberUpdateDto);
            if(await _userRepository.SaveAllAsync())
            return NoContent();

            return BadRequest("Failed to update user");
        }

        // DELETE: api/AppUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppUser(int id)
        {
            if (_userRepository.IsUserAvailable(id))
            {
                return NotFound();
            }
            var user = await _userRepository.GetUserByIdAsync(id);
            _userRepository.DeleteAppUserAsync(user);
            await _userRepository.SaveAllAsync();

            return NoContent();
        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkeletonDatingProject.DTO;
using SkeletonDatingProject.Entities;
using SkeletonDatingProject.Extensions;
using SkeletonDatingProject.Helpers;
using SkeletonDatingProject.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkeletonDatingProject.Controllers
{
    [Authorize]
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikesRepository _likesReppository;
        public LikesController(IUserRepository userRepository, ILikesRepository likesRepository)
        {
            _userRepository = userRepository;
            _likesReppository = likesRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string userName)
        {
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUserNameAsync(userName);
            var sourceUser = await _likesReppository.GetUserWithLikes(sourceUserId);
            if (likedUser == null) return NotFound();
            if (sourceUser.UserName == userName) return BadRequest("You cannot like yourself");
            var userLike = await _likesReppository.GetUserLike(sourceUserId, likedUser.Id);
            if (userLike != null) return BadRequest("Already liked this user.");

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);
            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("Failed to like user");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await _likesReppository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(users);
        }
    }
}

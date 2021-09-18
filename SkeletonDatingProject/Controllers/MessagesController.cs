using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepo;
        private readonly IMessageRepository _messageRepo;
        private readonly IMapper _mapper;
        public MessagesController(IUserRepository userRepo, IMessageRepository messageRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _messageRepo = messageRepo;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var userName = User.GetUserName();
            if (userName == createMessageDto.RecipientUserName.ToLower())
                return BadRequest("You cannot send messages to yourself");
            var sender = await _userRepo.GetUserByUserNameAsync(userName);
            var recipient = await _userRepo.GetUserByUserNameAsync(createMessageDto.RecipientUserName.ToLower());
            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepo.AddMessage(message);
            if (await _messageRepo.SaveAllAsync()) 
                return Ok(_mapper.Map<MessageDto>(message));
            return BadRequest("Failed to send the message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.UserName = User.GetUserName();
            var messages = await _messageRepo.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return messages;

        }

        [HttpGet("thread/{userName}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string userName)
        {
            var currentUserName = User.GetUserName();
            return Ok(await _messageRepo.GetMessageThread(currentUserName, userName));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var userName = User.GetUserName();
            var message = await _messageRepo.GetMessage(id);
            if (message.Sender.UserName != userName && message.Recipient.UserName != userName) return Unauthorized();
            if (message.Sender.UserName == userName) message.SenderDeleted = true;
            if (message.Recipient.UserName == userName) message.RecipientDeleted = true;
            if (message.SenderDeleted && message.RecipientDeleted) _messageRepo.DeleteMessage(message);
            if (await _messageRepo.SaveAllAsync()) return Ok();
            return BadRequest("Problem deleting the message");
        }
    }
}

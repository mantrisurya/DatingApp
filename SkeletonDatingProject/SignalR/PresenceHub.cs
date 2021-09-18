using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using SkeletonDatingProject.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkeletonDatingProject.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        public PresenceTracker _tracker { get; set; }
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }
        public override async Task OnConnectedAsync()
        {
            var isOnline = await _tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
            if(isOnline)
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserName());

            var currentUsers = await _tracker.GetonlineUsersDict();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception e)
        {
            var isOffline = await _tracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
            if(isOffline)
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());

            await base.OnDisconnectedAsync(e);
        }

    }
}

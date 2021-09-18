using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkeletonDatingProject.SignalR
{
    public class PresenceTracker
    {
        private static readonly Dictionary<string, List<string>> onlineUsersDict = new Dictionary<string, List<string>>();
        public Task<bool> UserConnected(string userName, string connectionId)
        {
            bool isOnline = false;
            lock (onlineUsersDict)
            {
                if (onlineUsersDict.ContainsKey(userName))
                {
                    onlineUsersDict[userName].Add(connectionId);
                }
                else
                {
                    onlineUsersDict.Add(userName, new List<string> { connectionId });
                    isOnline = true;
                }
            }
            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string userName, string connectionId)
        {
            bool isOffline = false;
            lock (onlineUsersDict)
            {
                if (!onlineUsersDict.ContainsKey(userName)) return Task.FromResult(isOffline);
                onlineUsersDict[userName].Remove(connectionId);
                if (onlineUsersDict[userName].Count == 0)
                {
                    onlineUsersDict.Remove(userName);
                    isOffline = true;
                }
            }

            return Task.FromResult(isOffline);
        }

        public Task<string[]> GetonlineUsersDict()
        {
            string[] onlineUsers;
            lock (onlineUsersDict)
            {
                onlineUsers = onlineUsersDict.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers);
        }

        public Task<List<string>> GetConnectionsForUser(string userName)
        {
            List<string> connectionIds;
            lock (onlineUsersDict)
            {
                connectionIds = onlineUsersDict.GetValueOrDefault(userName);
            }

            return Task.FromResult(connectionIds);
        }
    }
}

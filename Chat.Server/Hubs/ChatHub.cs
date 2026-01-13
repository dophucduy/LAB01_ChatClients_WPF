using Microsoft.AspNetCore.SignalR;
using Chat.Server.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chat.Server.Hubs
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, UserInfo> _connectedUsers = new();
        private static List<Message> _messageHistory = new();
        private static List<Reaction> _reactions = new();

        public async Task SendMessage(string user, string message)
        {
            var chatMessage = new Message
            {
                User = user,
                Content = message,
                Timestamp = DateTime.Now,
                Is_System = false,
                Is_Private = false
            };

            // Store in memory (for simplicity - production should use DB)
            _messageHistory.Add(chatMessage);

            // Keep only last 100 messages
            if (_messageHistory.Count > 100)
                _messageHistory.RemoveAt(0);

            await Clients.All.SendAsync("ReceiveMessage", chatMessage);
        }

        public async Task ReactToMessage(int messageId, string username, string reactionType)
        {
            // Simple toggle logic
            var existingReaction = _reactions
                .FirstOrDefault(r => r.Message_Id == messageId &&
                                   r.Username == username &&
                                   r.Reaction_Type == reactionType);

            if (existingReaction != null)
            {
                // Remove reaction
                _reactions.Remove(existingReaction);
            }
            else
            {
                // Add new reaction
                var reaction = new Reaction
                {
                    Message_Id = messageId,
                    Username = username,
                    Reaction_Type = reactionType,
                    Created_At = DateTime.Now
                };
                _reactions.Add(reaction);
            }

            // Get reaction counts for this message
            var reactionCounts = _reactions
                .Where(r => r.Message_Id == messageId)
                .GroupBy(r => r.Reaction_Type)
                .ToDictionary(g => g.Key, g => g.Count());

            // Broadcast updated reactions
            await Clients.All.SendAsync("MessageReactionsUpdated", messageId, reactionCounts);
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_connectedUsers.TryRemove(Context.ConnectionId, out var userInfo))
            {
                var systemMessage = new Message
                {
                    User = "System",
                    Content = $"{userInfo.Username} left the chat",
                    Timestamp = DateTime.Now,
                    Is_System = true,
                    Is_Private = false
                };

                await Clients.All.SendAsync("ReceiveMessage", systemMessage);
                await UpdateUserList();
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChat(string username)
        {
            // Check if username already exists
            if (_connectedUsers.Any(x => x.Value.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                await Clients.Caller.SendAsync("ErrorMessage", "Username already taken!");
                return;
            }

            var userInfo = new UserInfo
            {
                ConnectionId = Context.ConnectionId,
                Username = username,
                ConnectedAt = DateTime.Now
            };

            _connectedUsers[Context.ConnectionId] = userInfo;

            // Send recent message history
            foreach (var msg in _messageHistory)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", msg);
            }

            // Send current reactions for all messages
            var allReactionCounts = _reactions
                .GroupBy(r => r.Message_Id)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x => x.Reaction_Type)
                          .ToDictionary(x => x.Key, x => x.Count())
                );

            await Clients.Caller.SendAsync("LoadAllReactions", allReactionCounts);

            // Notify everyone about new user
            var systemMessage = new Message
            {
                User = "System",
                Content = $"{username} joined the chat",
                Timestamp = DateTime.Now,
                Is_System = true,
                Is_Private = false
            };

            await Clients.All.SendAsync("ReceiveMessage", systemMessage);
            await UpdateUserList();
        }

        private async Task UpdateUserList()
        {
            var usernames = _connectedUsers.Values.Select(u => u.Username).ToList();
            await Clients.All.SendAsync("UpdateUserList", usernames);
        }
    }
}

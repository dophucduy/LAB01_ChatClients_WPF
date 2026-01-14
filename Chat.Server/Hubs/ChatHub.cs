using Microsoft.AspNetCore.SignalR;
using Chat.Server.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase;

namespace Chat.Server.Hubs
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<string, UserInfo> _connectedUsers = new();
        private static Dictionary<string, int> _userIdMap = new(); // username -> user_id mapping
        private readonly Supabase.Client _supabase;

        public ChatHub(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public async Task SendMessage(string user, string message)
        {
            try
            {
                var userId = await GetOrCreateUserId(user);
                
                var chatMessage = new Message
                {
                    Sender_Id = userId,
                    Content = message,
                    Timestamp = DateTime.Now,
                    Is_Private = false
                };

                var result = await _supabase.From<Message>().Insert(chatMessage);
                var insertedMessage = result.Models.First();
                
                var messageDto = new MessageDto
                {
                    Id = insertedMessage.Id,
                    User = user,
                    Content = insertedMessage.Content,
                    Timestamp = insertedMessage.Timestamp,
                    Is_System = false,
                    Is_Private = false
                };

                await Clients.All.SendAsync("ReceiveMessage", messageDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        public async Task SendPrivateMessage(string sender, string recipient, string message)
        {
            try
            {
                var senderId = await GetOrCreateUserId(sender);
                var recipientId = await GetOrCreateUserId(recipient);
                
                var chatMessage = new Message
                {
                    Sender_Id = senderId,
                    Receiver_Id = recipientId,
                    Content = message,
                    Timestamp = DateTime.Now,
                    Is_Private = true
                };

                var result = await _supabase.From<Message>().Insert(chatMessage);
                var insertedMessage = result.Models.First();
                
                var messageDto = new MessageDto
                {
                    Id = insertedMessage.Id,
                    User = sender,
                    Content = insertedMessage.Content,
                    Timestamp = insertedMessage.Timestamp,
                    Is_System = false,
                    Is_Private = true,
                    Recipient = recipient
                };

                // Send to both sender and recipient
                var senderConnection = _connectedUsers.FirstOrDefault(x => x.Value.Username == sender).Key;
                var recipientConnection = _connectedUsers.FirstOrDefault(x => x.Value.Username == recipient).Key;
                
                if (senderConnection != null)
                    await Clients.Client(senderConnection).SendAsync("ReceiveMessage", messageDto);
                if (recipientConnection != null)
                    await Clients.Client(recipientConnection).SendAsync("ReceiveMessage", messageDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending private message: {ex.Message}");
            }
        }

        public async Task ReactToMessage(int messageId, string username, string reactionType)
        {
            try
            {
                var userId = await GetOrCreateUserId(username);
                
                var existingReactions = await _supabase.From<Reaction>()
                    .Where(r => r.Message_Id == messageId)
                    .Where(r => r.User_Id == userId)
                    .Where(r => r.Reaction_Type == reactionType)
                    .Get();

                if (existingReactions.Models.Any())
                {
                    var reactionToDelete = existingReactions.Models.First();
                    await _supabase.From<Reaction>().Delete(reactionToDelete);
                }
                else
                {
                    var reaction = new Reaction
                    {
                        Message_Id = messageId,
                        User_Id = userId,
                        Reaction_Type = reactionType,
                        Created_At = DateTime.Now
                    };
                    
                    await _supabase.From<Reaction>().Insert(reaction);
                }

                var allReactions = await _supabase.From<Reaction>()
                    .Where(r => r.Message_Id == messageId)
                    .Get();
                    
                var reactionCounts = allReactions.Models
                    .GroupBy(r => r.Reaction_Type)
                    .ToDictionary(g => g.Key, g => g.Count());

                await Clients.All.SendAsync("MessageReactionsUpdated", messageId, reactionCounts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reacting to message: {ex.Message}");
            }
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_connectedUsers.TryRemove(Context.ConnectionId, out var userInfo))
            {
                await UpdateUserList();
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChat(string username)
        {
            try
            {
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
                var userId = await GetOrCreateUserId(username);

                var messages = await _supabase.From<Message>()
                    .Where(m => m.Sender_Id != null)
                    .Order("timestamp", Supabase.Postgrest.Constants.Ordering.Descending)
                    .Limit(50)
                    .Get();

                var messageList = messages.Models
                    .Where(m => !m.Is_Private || m.Sender_Id == userId || m.Receiver_Id == userId)
                    .ToList();
                messageList.Reverse();
                
                foreach (var msg in messageList)
                {
                    var sender = await _supabase.From<User>()
                        .Where(u => u.Id == msg.Sender_Id)
                        .Single();
                    
                    var messageDto = new MessageDto
                    {
                        Id = msg.Id,
                        User = sender?.Username ?? "Unknown",
                        Content = msg.Content,
                        Timestamp = msg.Timestamp,
                        Is_System = false,
                        Is_Private = msg.Is_Private
                    };
                    
                    if (msg.Is_Private && msg.Receiver_Id.HasValue)
                    {
                        var receiver = await _supabase.From<User>()
                            .Where(u => u.Id == msg.Receiver_Id.Value)
                            .Single();
                        messageDto.Recipient = receiver?.Username;
                    }
                    
                    await Clients.Caller.SendAsync("ReceiveMessage", messageDto);
                }

                var allReactions = await _supabase.From<Reaction>().Get();
                var reactionCounts = allReactions.Models
                    .GroupBy(r => r.Message_Id)
                    .ToDictionary(
                        g => g.Key,
                        g => g.GroupBy(x => x.Reaction_Type)
                              .ToDictionary(x => x.Key, x => x.Count())
                    );

                await Clients.Caller.SendAsync("LoadAllReactions", reactionCounts);
                await UpdateUserList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error joining chat: {ex.Message}");
            }
        }

        private async Task<int> GetOrCreateUserId(string username)
        {
            try
            {
                if (_userIdMap.ContainsKey(username))
                    return _userIdMap[username];
                    
                var existingUser = await _supabase.From<User>()
                    .Where(u => u.Username == username)
                    .Single();
                    
                if (existingUser != null)
                {
                    _userIdMap[username] = existingUser.Id;
                    return existingUser.Id;
                }
                
                var newUser = new User
                {
                    Username = username,
                    Created_At = DateTime.Now,
                    Is_Online = true
                };
                
                var result = await _supabase.From<User>().Insert(newUser);
                var insertedUser = result.Models.First();
                
                _userIdMap[username] = insertedUser.Id;
                return insertedUser.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting/creating user: {ex.Message}");
                return 1; // fallback
            }
        }

        private async Task UpdateUserList()
        {
            var usernames = _connectedUsers.Values.Select(u => u.Username).ToList();
            await Clients.All.SendAsync("UpdateUserList", usernames);
        }
    }
}

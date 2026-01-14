using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chat.Client.Services
{
    public class ChatService
    {
        public event Action<JsonElement> OnMessageReceived;
        public event Action<List<string>> OnUserListUpdated;
        public event Action<string> OnError;
        public event Action<int, Dictionary<string, int>> OnReactionsUpdated;
        public event Action<Dictionary<int, Dictionary<string, int>>> OnLoadAllReactions;

        private HubConnection connection;
        public string CurrentUsername { get; private set; }
        private Dictionary<int, Dictionary<string, int>> _messageReactions = new();

        public async Task<bool> ConnectAsync(string username)
        {
            try
            {
                connection = new HubConnectionBuilder()
                    .WithUrl("http://172.31.176.1:5000/chathub")
                    .Build();

                connection.On<JsonElement>("ReceiveMessage", (message) =>
                {
                    OnMessageReceived?.Invoke(message);
                });

                connection.On<List<string>>("UpdateUserList", (users) =>
                {
                    OnUserListUpdated?.Invoke(users);
                });

                connection.On<string>("ErrorMessage", (error) =>
                {
                    OnError?.Invoke(error);
                });

                connection.On<int, Dictionary<string, int>>("MessageReactionsUpdated", (messageId, reactions) =>
                {
                    _messageReactions[messageId] = reactions;
                    OnReactionsUpdated?.Invoke(messageId, reactions);
                });

                connection.On<Dictionary<int, Dictionary<string, int>>>("LoadAllReactions", (allReactions) =>
                {
                    _messageReactions = allReactions;
                    OnLoadAllReactions?.Invoke(allReactions);
                });

                await connection.StartAsync();
                CurrentUsername = username;

                await connection.SendAsync("JoinChat", username);
                return true;
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"Connection failed: {ex.Message}");
                return false;
            }
        }

        public async Task SendMessage(string message)
        {
            if (connection != null && connection.State == HubConnectionState.Connected)
            {
                await connection.SendAsync("SendMessage", CurrentUsername, message);
            }
        }

        public async Task SendPrivateMessage(string recipient, string message)
        {
            if (connection != null && connection.State == HubConnectionState.Connected)
            {
                await connection.SendAsync("SendPrivateMessage", CurrentUsername, recipient, message);
            }
        }

        public async Task ReactToMessage(int messageId, string reactionType)
        {
            if (connection != null && connection.State == HubConnectionState.Connected)
            {
                await connection.SendAsync("ReactToMessage", messageId, CurrentUsername, reactionType);
            }
        }

        public async Task DisconnectAsync()
        {
            if (connection != null)
                await connection.StopAsync();
        }

        public bool IsConnected => connection?.State == HubConnectionState.Connected;
    }
}

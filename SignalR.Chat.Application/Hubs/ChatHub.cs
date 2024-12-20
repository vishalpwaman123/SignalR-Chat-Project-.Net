
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalR.Chat.Application.Model;
using SignalR.Chat.Application.Services;

namespace SignalR.Chat.Application.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatService _chatService;
        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Come2Chat");
            await Clients.Caller.SendAsync("UserConnected");
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Come2Chat");
            var user = _chatService.GetUserByConnectionId(Context.ConnectionId);
            _chatService.RemoveUserFromList(user);
            await DisplayOnlineUsers();
            await base.OnDisconnectedAsync(ex);
        }

        public async Task AddUserConnectionId(string userName)
        {
            _chatService.AddUserConnectionId(userName, Context.ConnectionId);
            var onlineUsers = _chatService.GetOnlineUsers();
            await Clients.Groups("Come2Chat").SendAsync("OnlineUsers", onlineUsers);
        }

        private async Task DisplayOnlineUsers()
        {
            var onlineUsers = _chatService.GetOnlineUsers();
            await Clients.Groups("Come2Chat").SendAsync("OnlineUsers", onlineUsers);
        }

        public async Task ReceiveMessage(MessageDto message)
        {
            await Clients.Group("Come2Chat").SendAsync("NewMessage", message);
        }

        public async Task CreatePrivateChat(MessageDto message)
        {
            string privateGroupName = GetPrivateGroupName(message.From, message.To);
            await Groups.AddToGroupAsync(Context.ConnectionId, privateGroupName);
            string? toConnectionId = _chatService.GetConnectionIdByUser(message.To);
            await Groups.AddToGroupAsync(toConnectionId, privateGroupName);
            // opening private chatbox for the other end user
            await Clients.Client(toConnectionId).SendAsync("OpenPrivateChat", message);
        }

        public async Task ReceivePrivateChat(MessageDto message)
        {
            string privateGroupName = GetPrivateGroupName(message.From, message.To);
            await Clients.Group(privateGroupName).SendAsync("NewPrivateMessage", message);
        }

        public async Task RemovePrivateChat(string from, string to)
        {
            string privateGroupName = GetPrivateGroupName(from, to);
            await Clients.Group(privateGroupName).SendAsync("ClosePrivateChat");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, privateGroupName);

            var toConnectionId = _chatService.GetConnectionIdByUser(to);
            await Groups.RemoveFromGroupAsync(toConnectionId, privateGroupName);
        }

        private string GetPrivateGroupName(string from, string to)
        {
            bool stringComapare = string.CompareOrdinal(from, to) < 0;
            return stringComapare ? $"{from}-{to}" : $"{to}-{from}";
        }
    }
}

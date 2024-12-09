
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
    }
}


using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
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
            await base.OnDisconnectedAsync(ex);
        }

        /*public async Task AddUserConnectionId(string userName)
        {
            _chatService.AddUserConnectionId(userName, Context.ConnectionId);
        }*/
    }
}

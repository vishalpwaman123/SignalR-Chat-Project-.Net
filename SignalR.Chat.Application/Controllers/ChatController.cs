
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SignalR.Chat.Application.Model;
using SignalR.Chat.Application.Services;

namespace SignalR.Chat.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;
        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("register-user")]
        public IActionResult Registration(RegistrationRequest request)
        {
            if (_chatService.AddUserToList(request.Name))
                return NoContent();

            return BadRequest(new { message = "User Already Exist" });
        }
    }
}

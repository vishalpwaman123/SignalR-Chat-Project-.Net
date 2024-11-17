using System.ComponentModel.DataAnnotations;

namespace SignalR.Chat.Application.Model
{
    public class RegistrationRequest
    {
        [Required(ErrorMessage ="Name is required")]
        public string Name { get; set; } = string.Empty;
    }
}

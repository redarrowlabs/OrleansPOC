using System;

namespace ChatClient.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Token { get; set; }
    }
}
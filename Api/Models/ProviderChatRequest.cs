namespace Api.Models
{
    public class ProviderChatRequest
    {
        public long PatientId { get; set; }

        public string Text { get; set; }
    }
}
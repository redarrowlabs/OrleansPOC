namespace Api.Models
{
    public class ProviderChatMessage
    {
        public long Id { get; set; }

        public long PatientId { get; set; }

        public string Text { get; set; }
    }
}
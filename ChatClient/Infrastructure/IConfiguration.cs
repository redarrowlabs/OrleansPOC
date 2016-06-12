namespace ChatClient.Infrastructure
{
    public interface IConfiguration
    {
        string RedirectUri { get; }

        string Authority { get; }

        string ApiBaseUrl { get; }
    }
}
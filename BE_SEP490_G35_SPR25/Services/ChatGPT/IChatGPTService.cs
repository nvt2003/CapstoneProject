namespace Planify_BackEnd.Services.ChatGPT
{
    public interface IChatGPTService
    {
        Task<string> GetSuggestion(string prompt);
    }
}

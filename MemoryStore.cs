// Stores user information that the chatbot can remember during the conversation.
namespace CyberSecurityBotPart2
{
    public class MemoryStore
    {
        public string UserName { get; set; } = "";
        public string FavouriteTopic { get; set; } = "";
        public string CurrentTopic { get; set; } = "";
    }
}
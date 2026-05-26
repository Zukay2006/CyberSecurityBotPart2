// Detects simple user emotions such as worried, curious or frustrated.s
namespace CyberSecurityBotPart2
{
    public class SentimentDetector
    {
        public string DetectSentiment(string input)
        {
            string lower = input.ToLower();

            if (lower.Contains("worried") ||
                lower.Contains("afraid") ||
                lower.Contains("scared"))
            {
                return "worried";
            }

            else if (lower.Contains("curious"))
            {
                return "curious";
            }

            else if (lower.Contains("frustrated") ||
                     lower.Contains("confused") ||
                     lower.Contains("overwhelmed"))
            {
                return "frustrated";
            }

            return "normal";
        }
    }
}
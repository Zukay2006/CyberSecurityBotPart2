using System;

namespace CyberSecurityBotPart2
{
    public class KeywordResponder
    {
        private Random random = new Random();

        private string[] phishingTips =
        {
            "Be careful of emails asking for personal information. Scammers often pretend to be trusted organisations.",
            "Never click suspicious links from unknown senders.",
            "Always verify email addresses before opening attachments.",
            "Phishing emails often create urgency.",
            "Never share OTPs through email or messages."
        };

        private string[] passwordTips =
        {
            "Use strong passwords with letters, numbers and symbols.",
            "Avoid birthdays in passwords.",
            "Use different passwords for every account.",
            "Enable two-factor authentication.",
            "Change passwords regularly."
        };

        private string[] privacyTips =
        {
            "Review privacy settings regularly.",
            "Avoid oversharing personal information.",
            "Check app permissions carefully.",
            "Limit who sees your posts.",
            "Protect location and contact details."
        };

        private string[] cybersecurityTips =
        {
            "Keep your software updated.",
            "Use antivirus software.",
            "Enable two-factor authentication.",
            "Avoid suspicious links.",
            "Protect personal information online."
        };

        private string[] scamTips =
        {
            "Always verify information before sending money.",
            "Avoid messages creating urgency.",
            "Never share OTPs.",
            "Be careful of offers that sound too good.",
            "Verify organisations before responding."
        };

        private string[] linkTips =
        {
            "Check the website address before clicking.",
            "Avoid links from unknown senders.",
            "Do not click links that create panic or urgency.",
            "Hover over links to preview where they go.",
            "Use official websites instead of links in messages."
        };

        private string[] sarcasticResponses =
        {
            "Of course I'm okay, I was built for this.",
            "ChatBuddy is running at 100%, unlike weak passwords.",
            "I'm fabulous, thanks for asking.",
            "Still alive and avoiding cyber criminals.",
            "I am doing great, no phishing scam can catch me.",
            "Better than a computer without antivirus."
        };

        public string GetRandomTip(string topic)
        {
            if (topic == "phishing")
                return phishingTips[random.Next(phishingTips.Length)];

            if (topic == "passwords")
                return passwordTips[random.Next(passwordTips.Length)];

            if (topic == "privacy")
                return privacyTips[random.Next(privacyTips.Length)];

            if (topic == "cybersecurity")
                return cybersecurityTips[random.Next(cybersecurityTips.Length)];

            if (topic == "scams")
                return scamTips[random.Next(scamTips.Length)];

            if (topic == "links")
                return linkTips[random.Next(linkTips.Length)];

            return "Stay safe online.";
        }

        public string GetSarcasticResponse()
        {
            return sarcasticResponses[random.Next(sarcasticResponses.Length)];
        }
    }
}
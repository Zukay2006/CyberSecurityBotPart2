// Handles the main chatbot conversation flow and responses.
namespace CyberSecurityBotPart2
{
    public class ChatBot
    {
        private MemoryStore memory;
        private KeywordResponder responder;
        private SentimentDetector sentiment;

        private bool waitingForTips = false;
        private bool waitingForDetails = false;
        private bool waitingForQuestion = false;
        private bool waitingForWorryReason = false;

        public ChatBot(
            MemoryStore memoryStore,
            KeywordResponder keywordResponder,
            SentimentDetector sentimentDetector)
        {
            memory = memoryStore;
            responder = keywordResponder;
            sentiment = sentimentDetector;
        }

        public string HandleFeeling(string input)
        {
            string lower = input.ToLower();

            if (lower.Contains("worried") ||
                lower.Contains("afraid") ||
                lower.Contains("scared"))
            {
                waitingForWorryReason = true;

                return responder.GetSarcasticResponse() +
                       "\n\nIt is okay to feel worried. Is there something that triggered you to feel worried?";
            }

            if (lower.Contains("curious"))
            {
                return responder.GetSarcasticResponse() +
                       "\n\nCuriosity is good. Is there a cybersecurity topic you are curious about?";
            }

            return responder.GetSarcasticResponse() +
                   "\n\nWhat would you like to learn about today?\n" +
                   "You can ask about cybersecurity, phishing, passwords, suspicious links, scams or privacy.";
        }

        public string GetResponse(string input)
        {
            string lower = input.ToLower();

            if (waitingForWorryReason)
            {
                waitingForWorryReason = false;

                if (lower.Contains("scam"))
                {
                    memory.CurrentTopic = "scams";
                    return "Oh no, I am sorry that happened. Scams can be stressful.\n\nHere is a tip so next time attackers find you prepared:\n" +
                           responder.GetRandomTip("scams");
                }

                if (lower.Contains("phish"))
                {
                    memory.CurrentTopic = "phishing";
                    return "I am sorry you experienced that. Phishing attacks can happen to anyone.\n\nHere is a tip so next time you are ready:\n" +
                           responder.GetRandomTip("phishing");
                }

                if (lower.Contains("link"))
                {
                    memory.CurrentTopic = "suspicious links";
                    return "Oh no, I am sorry that almost happened. Suspicious links can be dangerous.\n\nHere is a tip so next time attackers find you prepared:\n" +
                           responder.GetRandomTip("links");
                }

                memory.CurrentTopic = "cybersecurity";
                return "Thank you for sharing that with me. Here is a cybersecurity tip to help you stay safe:\n" +
                       responder.GetRandomTip("cybersecurity");
            }

            if (lower.Contains("thank you") ||
                lower.Contains("thanks") ||
                lower.Contains("thank u"))
            {
                return "Always a pleasure " +
                       memory.UserName +
                       "! If you need anything to help you stay safe online, hola ChatBuddy.";
            }

            if (lower.Contains("interested in"))
            {
                if (lower.Contains("cybersecurity"))
                    memory.FavouriteTopic = "cybersecurity";
                else if (lower.Contains("phishing"))
                    memory.FavouriteTopic = "phishing";
                else if (lower.Contains("password"))
                    memory.FavouriteTopic = "password safety";
                else if (lower.Contains("privacy"))
                    memory.FavouriteTopic = "privacy";
                else if (lower.Contains("scam"))
                    memory.FavouriteTopic = "scams";
                else if (lower.Contains("link"))
                    memory.FavouriteTopic = "suspicious links";

                if (memory.FavouriteTopic != "")
                {
                    return "Great! I'll remember that you're interested in " +
                           memory.FavouriteTopic +
                           ". It is an important part of staying safe online.";
                }
            }

            if (lower.Contains("remember") ||
                lower.Contains("what do you remember"))
            {
                if (memory.FavouriteTopic != "")
                {
                    return "I remember that your name is " +
                           memory.UserName +
                           " and your favourite cybersecurity topic is " +
                           memory.FavouriteTopic +
                           ".";
                }

                return "I remember that your name is " +
                       memory.UserName +
                       ", but you have not shared your favourite cybersecurity topic yet.";
            }

            string mood = sentiment.DetectSentiment(input);

            if (mood == "worried")
            {
                return "It's understandable to feel worried about online safety.\n\nTip: Use strong passwords, avoid suspicious links, and verify messages before responding.";
            }

            if (mood == "curious")
            {
                return "I like that you're curious! Curiosity helps you notice cyber risks before they become problems.";
            }

            if (mood == "frustrated")
            {
                return "I understand that cybersecurity can feel confusing. We can take it step by step.";
            }

            if (IsDefinitionQuestion(lower))
            {
                return GetDefinition(lower);
            }

            if (IsRandomTipRequest(lower))
            {
                return GetRequestedTip(lower);
            }

            if (lower.Contains("explain more") ||
                lower.Contains("tell me more"))
            {
                if (memory.CurrentTopic != "")
                    return GetMoreDetails();

                return "Tell me more about which topic: phishing, passwords, suspicious links, scams, privacy, or cybersecurity?";
            }

            if (lower.Contains("another tip") ||
                lower.Contains("more tips") ||
                lower.Contains("another one"))
            {
                return GetTipForCurrentTopic();
            }

            if (lower.Contains("no") ||
                lower.Contains("not really") ||
                lower.Contains("nope"))
            {
                waitingForTips = false;
                waitingForDetails = false;

                return "No problem. Do you have any other questions about cybersecurity, phishing, passwords, suspicious links, scams, or privacy?";
            }

            if (waitingForQuestion)
            {
                waitingForQuestion = false;

                if (lower.Contains("guess") && lower.Contains("password"))
                    return "If someone guesses your password, they may access your account, read private information, change your details, or even lock you out.";

                if (lower.Contains("click") && lower.Contains("link"))
                    return "Clicking suspicious links may redirect you to fake websites, install malware, or steal personal information.";

                return "That is a good cybersecurity question. Always protect personal information, avoid suspicious links, and use strong passwords.";
            }

            if (lower.Contains("i have a question") ||
                lower.Contains("can i ask") ||
                lower.Contains("question"))
            {
                waitingForQuestion = true;
                return "Yes, what is your question?";
            }

            if (IsTopic(lower))
            {
                return StartTopic(lower);
            }

            if (lower.Contains("yes") ||
                lower.Contains("sure") ||
                lower.Contains("okay"))
            {
                if (waitingForDetails)
                {
                    waitingForDetails = false;
                    return GetMoreDetails();
                }

                if (waitingForTips)
                {
                    waitingForTips = false;
                    return GetSafetyTips();
                }

                return "Yes to what exactly? Please ask for a topic such as phishing, passwords, suspicious links, scams, privacy, or cybersecurity.";
            }

            return "I'm not sure I understand that topic yet. Please ask about cybersecurity, phishing, passwords, suspicious links, scams or privacy.";
        }

        private bool IsDefinitionQuestion(string input)
        {
            return input.Contains("what") ||
                   input.Contains("define") ||
                   input.Contains("meaning") ||
                   input.Contains("how") ||
                   input.Contains("why");
        }

        private bool IsTopic(string lower)
        {
            return lower.Contains("phishing") ||
                   lower.Contains("phish") ||
                   lower.Contains("password") ||
                   lower.Contains("privacy") ||
                   lower.Contains("scam") ||
                   lower.Contains("cybersecurity") ||
                   lower.Contains("link");
        }

        private bool IsRandomTipRequest(string lower)
        {
            return lower.Contains("tip") ||
                   lower.Contains("tips");
        }

        private string GetRequestedTip(string lower)
        {
            if (lower.Contains("phishing") || lower.Contains("phish"))
            {
                memory.CurrentTopic = "phishing";
                return responder.GetRandomTip("phishing");
            }

            if (lower.Contains("password"))
            {
                memory.CurrentTopic = "passwords";
                return responder.GetRandomTip("passwords");
            }

            if (lower.Contains("privacy"))
            {
                memory.CurrentTopic = "privacy";
                return responder.GetRandomTip("privacy");
            }

            if (lower.Contains("scam"))
            {
                memory.CurrentTopic = "scams";
                return responder.GetRandomTip("scams");
            }

            if (lower.Contains("cybersecurity"))
            {
                memory.CurrentTopic = "cybersecurity";
                return responder.GetRandomTip("cybersecurity");
            }

            if (lower.Contains("link"))
            {
                memory.CurrentTopic = "suspicious links";
                return responder.GetRandomTip("links");
            }

            return GetTipForCurrentTopic();
        }

        private string GetTipForCurrentTopic()
        {
            if (memory.CurrentTopic == "phishing")
                return responder.GetRandomTip("phishing");

            if (memory.CurrentTopic == "passwords")
                return responder.GetRandomTip("passwords");

            if (memory.CurrentTopic == "privacy")
                return responder.GetRandomTip("privacy");

            if (memory.CurrentTopic == "scams")
                return responder.GetRandomTip("scams");

            if (memory.CurrentTopic == "cybersecurity")
                return responder.GetRandomTip("cybersecurity");

            if (memory.CurrentTopic == "suspicious links")
                return responder.GetRandomTip("links");

            return "More tips about which topic: phishing, passwords, suspicious links, scams, privacy, or cybersecurity?";
        }

        private string StartTopic(string lower)
        {
            if (lower.Contains("phishing") || lower.Contains("phish"))
            {
                memory.CurrentTopic = "phishing";
                waitingForDetails = true;
                waitingForTips = false;

                return "Phishing is a cyberattack where scammers pretend to be trustworthy people or companies.\n\nWould you like a detailed explanation about phishing?";
            }

            if (lower.Contains("password"))
            {
                memory.CurrentTopic = "passwords";
                waitingForDetails = true;
                waitingForTips = false;

                return "Passwords protect accounts from hackers.\n\nWould you like a detailed explanation about password safety?";
            }

            if (lower.Contains("link"))
            {
                memory.CurrentTopic = "suspicious links";
                waitingForDetails = true;
                waitingForTips = false;

                return "Suspicious links may lead to fake websites.\n\nWould you like a detailed explanation about suspicious links?";
            }

            if (lower.Contains("cybersecurity"))
            {
                memory.CurrentTopic = "cybersecurity";
                waitingForDetails = true;
                waitingForTips = false;

                return "Cybersecurity protects information and systems from online threats.\n\nWould you like a detailed explanation about cybersecurity?";
            }

            if (lower.Contains("scam"))
            {
                memory.CurrentTopic = "scams";
                waitingForDetails = true;
                waitingForTips = false;

                return "Scams trick people into giving information or money.\n\nWould you like a detailed explanation about scams?";
            }

            if (lower.Contains("privacy"))
            {
                memory.CurrentTopic = "privacy";
                waitingForDetails = true;
                waitingForTips = false;

                return "Privacy protects personal information online.\n\nWould you like a detailed explanation about privacy?";
            }

            return "Please ask about cybersecurity topics.";
        }

        private string GetDefinition(string lower)
        {
            if (lower.Contains("phishing") || lower.Contains("phish"))
            {
                memory.CurrentTopic = "phishing";
                return "Phishing is a cyberattack where scammers pretend to be trusted people or organisations to trick users into sharing private information such as passwords, banking details, or OTPs.";
            }

            if (lower.Contains("cybersecurity") || lower.Contains("cybersecuirty"))
            {
                memory.CurrentTopic = "cybersecurity";
                return "Cybersecurity is the practice of protecting devices, accounts, networks, and personal information from online threats such as hackers, scams, malware, and phishing attacks.";
            }

            if (lower.Contains("password"))
            {
                memory.CurrentTopic = "passwords";
                return "A password is a secret code used to protect an account. Strong passwords help prevent hackers from accessing private information.";
            }

            if (lower.Contains("privacy"))
            {
                memory.CurrentTopic = "privacy";
                return "Privacy means controlling your personal information online, including what you share, who can see it, and how it is used.";
            }

            if (lower.Contains("scam"))
            {
                memory.CurrentTopic = "scams";
                return "A scam is a dishonest trick used to steal money, personal information, or access to accounts.";
            }

            if (lower.Contains("link"))
            {
                memory.CurrentTopic = "suspicious links";
                return "Suspicious links are links that may lead users to fake websites, malware downloads, or pages designed to steal personal information.";
            }

            return "Please ask about a cybersecurity topic.";
        }

        private string GetMoreDetails()
        {
            waitingForTips = true;

            if (memory.CurrentTopic == "cybersecurity")
                return "Cybersecurity is the practice of protecting computers, phones, networks, systems, accounts, and personal information from digital threats. These threats include hacking, malware, phishing attacks, scams, identity theft, data leaks, and unauthorised access.\n\nGood cybersecurity habits include creating strong passwords, enabling two-factor authentication, updating devices regularly, avoiding suspicious links, using antivirus software, and thinking carefully before sharing information online.\n\nWould you like more cybersecurity tips?";

            if (memory.CurrentTopic == "phishing")
                return "Phishing is a cyberattack where scammers pretend to be trusted people or organisations so they can trick users into sharing private information. It often happens through fake emails, SMS messages, social media messages, phone calls, or websites that look real.\n\nPhishing messages usually create fear, pressure, or urgency. The goal is to make you panic and click a link before thinking properly.\n\nWould you like more phishing safety tips?";

            if (memory.CurrentTopic == "passwords")
                return "Passwords are one of the first lines of defence in cybersecurity because they protect your accounts from unauthorised access. Weak passwords can easily be guessed by hackers.\n\nA strong password should be unique, long, and difficult to guess. It should include uppercase letters, lowercase letters, numbers, and special characters.\n\nWould you like more password safety tips?";

            if (memory.CurrentTopic == "suspicious links")
                return "Suspicious links may lead users to unsafe websites, fake login pages, scams, or malware downloads. Cybercriminals often send these links through emails, SMS messages, social media, or messaging apps.\n\nBefore clicking a link, check the website address carefully, avoid unknown senders, and be careful of messages that create panic or urgency.\n\nWould you like more suspicious link safety tips?";

            if (memory.CurrentTopic == "scams")
                return "Online scams are dishonest tricks used by criminals to steal money, personal information, or access to accounts. Scammers may pretend to be banks, delivery companies, employers, or even family members.\n\nScams work because they pressure people into acting before thinking. Always verify information using official contact details and never share OTPs or passwords.\n\nWould you like more scam prevention tips?";

            if (memory.CurrentTopic == "privacy")
                return "Privacy means protecting your personal information and controlling who can see it, use it, or share it. Personal information includes your name, location, photos, phone number, email address, passwords, and private messages.\n\nTo protect privacy, review social media settings, limit who can view your posts, avoid sharing sensitive information publicly, and check app permissions regularly.\n\nWould you like more privacy tips?";

            return "Please choose a topic first.";
        }

        private string GetSafetyTips()
        {
            if (memory.CurrentTopic == "phishing")
                return "- Never click suspicious links\n- Verify sender addresses\n- Never share OTPs\n- Report suspicious emails";

            if (memory.CurrentTopic == "passwords")
                return "- Use unique passwords\n- Enable 2FA\n- Avoid birthdays\n- Change passwords";

            if (memory.CurrentTopic == "suspicious links")
                return "- Check HTTPS\n- Avoid urgent links\n- Verify websites";

            if (memory.CurrentTopic == "cybersecurity")
                return "- Update software\n- Use antivirus\n- Protect accounts";

            if (memory.CurrentTopic == "scams")
                return "- Verify information\n- Avoid urgency traps\n- Never share OTPs";

            if (memory.CurrentTopic == "privacy")
                return "- Review privacy settings\n- Avoid oversharing\n- Protect information";

            return "Stay safe online!";
        }
    }
}
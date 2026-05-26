
using System.Media;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace CyberSecurityBotPart2
{
    public partial class MainWindow : Window
    {
        private string userName = "";
        private string conversationStage = "name";

        private MemoryStore memory = new MemoryStore();
        private KeywordResponder keywordResponder = new KeywordResponder();
        private SentimentDetector sentimentDetector = new SentimentDetector();
        private ChatBot chatBot;

        public MainWindow()
        {
            InitializeComponent();
            ChatArea.Document.Blocks.Clear();

            chatBot = new ChatBot(
                memory,
                keywordResponder,
                sentimentDetector);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartIntroAnimation();
        }

        private void StartIntroAnimation()
        {
            DoubleAnimation slideIn = new DoubleAnimation();
            slideIn.From = -250;
            slideIn.To = 350;
            slideIn.Duration = TimeSpan.FromSeconds(1.5);

            slideIn.Completed += (s, e) =>
            {
                PlayVoiceGreeting();

                DoubleAnimation slideOut = new DoubleAnimation();
                slideOut.From = 350;
                slideOut.To = 900;
                slideOut.Duration = TimeSpan.FromSeconds(1.5);

                slideOut.Completed += (s2, e2) =>
                {
                    MovingBot.Visibility = Visibility.Collapsed;
                    DropTitle();
                    ShowWelcomeText();
                };

                MovingBotTransform.BeginAnimation(
                    TranslateTransform.XProperty,
                    slideOut);
            };

            MovingBotTransform.BeginAnimation(
                TranslateTransform.XProperty,
                slideIn);
        }

        private void DropTitle()
        {
            DoubleAnimation drop = new DoubleAnimation();
            drop.From = -40;
            drop.To = 0;
            drop.Duration = TimeSpan.FromSeconds(1);

            DoubleAnimation fade = new DoubleAnimation();
            fade.From = 0;
            fade.To = 1;
            fade.Duration = TimeSpan.FromSeconds(1);

            TitleDropTransform.BeginAnimation(
                TranslateTransform.YProperty,
                drop);

            TitlePanel.BeginAnimation(
                OpacityProperty,
                fade);
        }

        private void ShowWelcomeText()
        {
            AddBotMessage(
                "Welcome to the Cybersecurity Awareness Assistant!\n\n" +
                "My purpose is to enhance your knowledge of cybersecurity and help you stay safe online. " +
                "I will guide you in identifying threats such as phishing attempts and suspicious links, " +
                "and I will also help you create strong and secure passwords to protect your accounts from hackers. " +
                "With that being said, BUCKLE UP!");

            AddBotMessage(
                "Please enter your name to start.");
        }

        private void PlayVoiceGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("welcome.wav");
                player.PlaySync();
            }
            catch
            {
                MessageBox.Show("Voice greeting could not be played.");
            }
        }

        private void SendButton_Click(
            object sender,
            RoutedEventArgs e)
        {
            string input = UserInput.Text.Trim();

            if (IsExitCommand(input))
            {
                MessageBox.Show(
                    "Goodbye " + userName +
                    "! Stay safe online.");

                Application.Current.Shutdown();
                return;
            }

            if (string.IsNullOrWhiteSpace(input))
            {
                if (conversationStage == "name")
                {
                    AddBotMessage(
                        "Oops! Please enter your name to start.",
                        true);
                }
                else
                {
                    AddBotMessage(
                        "Oops! Please type a cybersecurity topic.",
                        true);
                }

                return;
            }

            AddUserMessage(input);

            if (conversationStage == "name")
            {
                if (input.ToLower().Contains("cybersecurity") ||
                    input.ToLower().Contains("phishing") ||
                    input.ToLower().Contains("password") ||
                    input.ToLower().Contains("privacy") ||
                    input.ToLower().Contains("scam") ||
                    input.ToLower().Contains("tip"))
                {
                    AddBotMessage(
                        "Oops! Please enter your name first before asking cybersecurity questions.",
                        true);

                    UserInput.Clear();
                    return;
                }

                string cleanedName =
                    CleanName(input);

                if (!IsValidName(cleanedName))
                {
                    AddBotMessage(
                        "Oops! Please enter a valid name between 2 and 60 letters.",
                        true);

                    UserInput.Clear();
                    return;
                }

                userName = cleanedName;
                memory.UserName = userName;

                AddBotMessage(
                    "Nice meeting you " +
                    userName +
                    "! How are you today?");

                conversationStage =
                    "feeling";
            }

            else if (conversationStage == "feeling")
            {
                AddBotMessage(
                    chatBot.HandleFeeling(input));

                conversationStage =
                    "topic";
            }

            else
            {
                string response =
                    chatBot.GetResponse(input);

                AddBotMessage(response);
            }

            UserInput.Clear();
        }

        private void AddUserMessage(string message)
        {
            Paragraph p = new Paragraph();

            Run userRun =
                new Run(
                    "You: " +
                    message +
                    "\n");

            userRun.Foreground =
                Brushes.Blue;

            p.Inlines.Add(userRun);

            ChatArea.Document.Blocks.Add(p);
            ChatArea.ScrollToEnd();
        }

        private void AddBotMessage(
            string message,
            bool isError = false)
        {
            Paragraph p = new Paragraph();

            Run r =
                new Run(
                    "ChatBuddy: " +
                    message +
                    "\n");

            r.Foreground =
                isError ?
                Brushes.Red :
                Brushes.Black;

            p.Inlines.Add(r);

            ChatArea.Document.Blocks.Add(p);
            ChatArea.ScrollToEnd();
        }

        private bool IsExitCommand(string input)
        {
            string lower = input.ToLower();

            return lower.Contains("exit") ||
                   lower.Contains("quit") ||
                   lower.Contains("bye") ||
                   lower.Contains("done");
        }

        private bool IsValidName(string name)
        {
            if (name.Length < 2 || name.Length > 60)
                return false;

            string lower = name.ToLower();

            if (lower == "hi" ||
                lower == "hello" ||
                lower == "hey")
            {
                return false;
            }

            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != ' ' && c != '-')
                    return false;
            }

            return true;
        }

        private string CleanName(string input)
        {
            string cleaned =
                input.ToLower();

            cleaned = cleaned
                .Replace("my name is", "")
                .Replace("name is", "")
                .Replace("i am", "")
                .Replace("i'm", "")
                .Replace("im", "")
                .Trim();

            return cleaned;
        }
    }
}
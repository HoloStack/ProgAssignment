using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using System.IO;
using chat.ChatBot;

namespace CybersecurityChatbot
{
    class Program
    {
        // Keyword dictionary storing predefined topic-based responses
        static Dictionary<string, List<string>> keywordResponses = ResponcesList.keywordResponses;

        // Memory dictionary for storing user-specific info like name and favorite topic
        static Dictionary<string, string> memory = new Dictionary<string, string>();

        // Sentiment word lists for basic emotion detection
        static List<string> positiveSentiments = new List<string>{"interested", "curious", "excited", "keen"};
        static List<string> worriedSentiments = new List<string>{"worried", "scared", "nervous", "concerned", "anxious"};
        static List<string> frustratedSentiments = new List<string>{"frustrated", "tired", "angry", "fed up"};

        static void Main(string[] args)
        {
            PlayGreeting();  // Play intro sound
            ShowAsciiArt();  // Display intro ASCII art

            // Ask for and store the user's name
            Console.Write("Enter your name: ");
            string name = Console.ReadLine() ?? "User";
            memory["name"] = name;

            // Ask for and validate user's favorite cybersecurity topic
            string favorite;
            while (true)
            {
                Console.Write("What's your favorite cybersecurity topic? ");
                favorite = Console.ReadLine()?.ToLower() ?? "";
                if (keywordResponses.ContainsKey(favorite))
                {
                    memory["favorite"] = favorite;
                    break;
                }
                else
                {
                    Console.WriteLine("Sorry, I don't recognize that topic. Please try a different one.");
                }
            }

            // Greet and explain how to interact with the bot
            Respond($"Welcome, {name}! I'm here to help you learn how to stay safe online. Feel free to ask me about {favorite} or anything else cybersecurity-related. Type 'exit' to quit.\n");

            // Main chatbot loop
            while (true)
            {
                Console.Write("You: ");
                string input = Console.ReadLine().ToLower();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Respond("I'm not sure what you mean. Could you rephrase it?");
                    continue;
                }

                if (input.Contains("exit"))
                {
                    Respond($"Goodbye {name}, stay cyber safe out there!");
                    break;
                }

                // Check for sentiment and respond
                if (DetectSentiment(input, out string mood))
                {
                    RespondWithSentiment(mood);
                }
                // Handle basic small-talk or guidance
                else if (input.Contains("how are you"))
                {
                    Respond("I'm fully patched and secure! Thanks for asking.");
                }
                else if (input.Contains("your purpose"))
                {
                    Respond("My goal is to help you understand and defend against cyber threats.");
                }
                else if (input.Contains("what can i ask"))
                {
                    Respond("You can ask me about phishing, scams, password safety, privacy, or general online security.");
                }
                else
                {
                    // Try to match user input to a known keyword
                    bool matched = false;
                    foreach (var keyword in keywordResponses.Keys)
                    {
                        if (input.Contains(keyword))
                        {
                            ShowRandomTip(keyword);
                            matched = true;
                            break;
                        }
                    }
                    // If no keyword match found, provide fallback response
                    if (!matched)
                    {
                        Respond($"I'm not sure about that, {name}, but I'm learning more every day! Try asking about passwords, scams, or privacy.");
                    }
                }
            }
        }

        // Attempts to play a greeting audio file at startup
        static void PlayGreeting()
        {
            try
            {
                using (SoundPlayer player = new SoundPlayer("greeting.wav"))
                {
                    player.Load();
                    player.PlaySync();
                }
            }
            catch
            {
                Console.WriteLine("[Audio] Greeting file missing or cannot be played.");
            }
        }

        // Displays startup ASCII art from a file
        static void ShowAsciiArt()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                string art = File.ReadAllText("ascii_art.txt");
                Console.WriteLine(art);
                Console.ResetColor();
            }
            catch
            {
                Console.WriteLine("[Error] ASCII art file not found. Please ensure ascii_art.txt is in the correct directory.");
            }
        }

        // Formats and displays bot responses in yellow
        static void Respond(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Bot: " + message);
            Console.ResetColor();
        }

        // Displays special responses (e.g., for favorite topic) in cyan
        static void RespondSpecial(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Bot: " + message);
            Console.ResetColor();
        // Displays a random tip for a given keyword topic
        static void ShowRandomTip(string topic)
        {
            var rand = new Random();
            var tips = keywordResponses[topic];
            string response = tips[rand.Next(tips.Count)];

            // Special message if topic is the user's favorite
            if (memory.ContainsKey("favorite") && topic == memory["favorite"])
            {
                RespondSpecial("Since it's your favorite topic, I'm sure you know a lot about it. Here's some extra info:");
            }

            Respond(response);
        }

        // Detects basic sentiment from input text
        static bool DetectSentiment(string input, out string sentiment)
        {
            foreach (var word in worriedSentiments)
                if (input.Contains(word)) { sentiment = "worried"; return true; }
            foreach (var word in frustratedSentiments)
                if (input.Contains(word)) { sentiment = "frustrated"; return true; }
            foreach (var word in positiveSentiments)
                if (input.Contains(word)) { sentiment = "positive"; return true; }
            sentiment = "";
            return false;
        }

        // Responds to detected sentiment with empathetic messages
        static void RespondWithSentiment(string sentiment)
        {
            string name = memory.ContainsKey("name") ? memory["name"] : "friend";
            switch (sentiment)
            {
                case "worried":
                    Respond($"I understand you're feeling worried, {name}. Let's take it step-by-step to make online safety easier.");
                    break;
                case "frustrated":
                    Respond($"I'm sorry you're feeling frustrated, {name}. Cybersecurity can be tricky, but I'm here to guide you.");
                    break;
                case "positive":
                    Respond($"That's awesome, {name}! I'm glad you're excited to learn more. Let's dive into a topic you're curious about.");
                    break;
                default:
                    Respond("Tell me more about how you're feeling or what you're thinking about online safety.");
                    break;
            }
        }
    }
}

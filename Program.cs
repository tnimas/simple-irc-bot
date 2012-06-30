using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace SimpleBot
{
    class Program
    {
        // Irc server to connect 
        public const string Server = "irc.run.net";
        // Irc server's port (6667 is default irc port)
        private const int Port = 6667;
        // User information defined in RFC 2812 (Internet Relay Chat: Client Protocol) is sent to irc server 
        private const string User = "USER CSharpBot 8 * :I'm a C# irc bot";
        // Channel to join
        private const string Channel = "#some_channel";
        // Bot's nickname
        private static readonly string Nick = "BotNick" + (new Random().Next(9999));

        private static void Main()
        {
            try
            {
                #region Init
                var irc = new TcpClient(Server, Port);
                NetworkStream stream = irc.GetStream();
                var reader = new StreamReader(stream);
                var writer = new StreamWriter(stream);
                #endregion
                Action<string> write = message =>
                {
                    writer.WriteLine(message);
                    writer.Flush();
                };

                write(User);
                write("NICK " + Nick);
                write("JOIN " + Channel);

                while (true)
                {
                    string inputLine;
                    while ((inputLine = reader.ReadLine()) != null)
                    {
                        string[] splitLine = inputLine.Split();

                        if (splitLine[0] == "PING")
                        {
                            write("PING :" + Server);
                        }

                        if (splitLine.Length > 1 && splitLine[1] == "JOIN")
                        {
                            // Parse nickname of person who joined the channel
                            string nickname = inputLine.Substring(1,
                                                                  inputLine.IndexOf("!", StringComparison.Ordinal) - 1);
                            // Welcome the nickname to channel by sending a notice
                            write("NOTICE " + nickname + " :Hi " + nickname +
                                  " and welcome to " + Channel + " channel!");
                        }
                    }
                    throw new IOException("end of the stream");
                }
            }
            catch (Exception e)
            {
                // Show the exception, sleep for a while and try to establish a new connection to irc server
                Console.WriteLine(e.ToString());
                Thread.Sleep(5000);
                Main();
            }
        }
    }
}

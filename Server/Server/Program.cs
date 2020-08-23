using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static List<string> text = new List<string>();
        static void Main(string[] args)
        {
            StreamWriter stream = new StreamWriter("Log.txt");
            socket.Bind(new IPEndPoint(IPAddress.Any, 1039));
            socket.Listen(1);

            ReadText();
            ChangeLines();
          
            string message;
            while (true)
            {
                Socket clientSocket = socket.Accept();
                Console.WriteLine("Device Connected");

                do
                {
                    try
                    {
                        ReceiveMessage(out message, clientSocket);
                    }
                    catch (Exception)
                    {
                        break;
                    }
                    Log(stream, message);
                    if (message.Contains("exit"))
                        break;
                    DoCommand(message, clientSocket);
                } while (true);
                
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
                stream.Flush();
                stream.Close();
            }
            
        }
        static void ReceiveMessage(out string message,Socket client)
        {
            byte[] buffer = new byte[10];
            int size = client.Receive(buffer);
            message = Encoding.ASCII.GetString(buffer,0,size);
        }
        static void SendAnswer(string answer,Socket client)
        {
            byte[] buffer;
            buffer = Encoding.ASCII.GetBytes(answer);
            client.Send(buffer,answer.Length,SocketFlags.None);
        }

        static string WhoInfo()
        {
            string answer = "Shymkiv Dmytro K-26\nFile Correction";
            return answer;
        }
        static void DoCommand(string command, Socket client)
        {
            int lineIndex;
            if (command == "Who")
                SendAnswer(WhoInfo(), client);
            else if (command == "Correction")
                ReadText();  
            else if (int.TryParse(command,out lineIndex))
                SendNewLine(client, lineIndex);
        }
        static void ReadText()
        {
            StreamReader streamRead = new StreamReader("file.txt");
            text.Clear();
            for (int i = 0; !streamRead.EndOfStream; i++)
                text.Add(streamRead.ReadLine());

            streamRead.Close();
        }
        static void ChangeLines()
        {
            Console.WriteLine("Text changes : ");
            
            Random random = new Random();
            string[] newLines = new string[] {"New Server text","Random Server text","Server answer text","Server string","Server line"};

            StreamWriter streamWrite = new StreamWriter("file.txt");
            
            for (int i = 0; i < text.Count; i++)
            {
                if (random.Next(2) == 1)
                {
                    string line = newLines[random.Next(newLines.Length)];
                    streamWrite.WriteLine(line);
                    Console.WriteLine(i.ToString() + " " + line);
                }
                else
                    streamWrite.WriteLine(text[i]);
                
            }
            streamWrite.Flush();
            streamWrite.Close();

        }
        static void SendNewLine(Socket client,int i)
        {
            StreamReader streamRead = new StreamReader("file.txt");
            string s = streamRead.ReadLine();
            for (int j = 0; j < i; j++)
                s = streamRead.ReadLine();
            if (i < text.Count)
            {
                if (!(text[i] == s))
                    SendAnswer(i.ToString() + " " + s, client);
                else
                    SendAnswer("No changes", client);
            }

            streamRead.Close();
        }
        static void Log(StreamWriter stream, string command)
        {
            DateTime localTime = DateTime.Now;
            stream.WriteLine(localTime.ToString() + " Command : " + command);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static List<string> text = new List<string>();
        static void Main(string[] args)
        {
            StreamWriter stream = new StreamWriter("Log.txt");
            string[] command = { "Correction", "Who","exit"};
            
            socket.Connect("127.0.0.1", 1039);
            string message = "";
            while (message != "exit")
            {
                message = Console.ReadLine();
                if (command.Contains(message))
                {
                    if (message != "Correction")
                    {
                        SendMessage(message);
                        Console.WriteLine(ReceiveAnswer());
                    }
                    else
                        FileCorrection();
                    Log(stream, message);
                }
                else
                {
                    Console.WriteLine("Incorrect Command");
                    Log(stream, "Incorrect Command");
                }
                
            }
            stream.Flush();
            stream.Close();
            Console.ReadLine();
        }
        static void SendMessage(string message)
        {
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            socket.Send(buffer,message.Length,SocketFlags.None);
        }
        static string ReceiveAnswer()
        {
            byte[] buffer = new byte[40];
            int size = socket.Receive(buffer);
            string s = Encoding.ASCII.GetString(buffer,0,size);
            return s;

        } 
        static void ReadText()
        {
            text.Clear();
            StreamReader stream = new StreamReader("file.txt");
            for (int i = 0; !stream.EndOfStream; i++)
                text.Add(stream.ReadLine());
            stream.Close();
        }
        static void FileCorrection()
        {
            ReadText();
            bool changhed = false;
            StreamWriter stream = new StreamWriter("file.txt");
            for (int i = 0; i < text.Count; ++i)
            {
                SendMessage(i.ToString());
                string str = ReceiveAnswer();
                if (str.Contains("No changes"))
                    stream.WriteLine(text[i]);
                else
                {
                    stream.WriteLine(str.Substring(2));
                    changhed = true;
                }
                    

            }
            stream.Flush();
            stream.Close();
            SendMessage("Correction");
            if (changhed)
                Console.WriteLine("File Changhed");
            else
                Console.WriteLine("No Changes");

        }
        static void Log(StreamWriter stream,string command)
        {
            DateTime localTime = DateTime.Now;
            stream.WriteLine(localTime.ToString() + " Command : " + command);
        }
    }
}

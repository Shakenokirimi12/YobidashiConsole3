using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Media;
using System.Xml;
using System.Runtime.InteropServices;


namespace DormitoryConsole
{
    class Program
    {
        static string filepath;

        private StreamReader OpenReadFile(string filePathName)
        {
            // 第4引数のFileShareは"ReadWrite"にします。
            var fileStream = new FileStream(filePathName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var reader = new StreamReader(fileStream);
            return reader;
        }

        private StreamWriter OpenWriteFile(string filePathName)
        {
            var fileStream = new FileStream(filePathName, FileMode.Create, FileAccess.Write, FileShare.Read);
            var writer = new StreamWriter(fileStream);
            return writer;
        }

        static int num;
        static HttpClient httpClient;
        static string readString;
        static string EngineType;
        static string SpeakerInfo;
        static void Main(string[] args)
        {
            Console.WriteLine("起動完了。");
            Console.WriteLine("各種アプリで呼び出し文を生成してください。");
            using (httpClient = new HttpClient())
            {
                while (true)
                {
                    readtextfile();
                    Thread.Sleep(10);
                }
            }
        }


        static async void VoiceSynthesis(string text, string speakerName)//VOICEVOXに指示を送る。
        {
            Console.WriteLine(text);
            Console.WriteLine("実行までしばらくお待ちください。");
            await httpClient.GetAsync(@"http://localhost:1000/?name=" + speakerName + "&text=" + text);   
        }



        static void readtextfile()
        {
            var program = new Program();
            using (var reader = program.OpenReadFile("Settings/Yobidashi_Temp.txt"))
            {
                readString = reader.ReadLine();
                var program2 = new Program();
                using (var fileStream = new FileStream("Settings/Yobidashi_Temp.txt", FileMode.Open))
                {
                    fileStream.SetLength(0);
                }
                if (readString != "")
                {
                    if (readString != null)
                    {
                        SetSpeaker();
                        string Name = SpeakerInfo;
                        VoiceSynthesis(readString, Name);

                    }
                }
            }
        }


        static SoundPlayer player = new SoundPlayer("Files/Chaim.wav");

        
        static void SetSpeaker()
        {
            var program = new Program();
            using (var reader = program.OpenReadFile("Settings/SpeakerName.txt"))
            {
                SpeakerInfo = reader.ReadLine();
            }
        }

    }
}
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


        static async void SendToVoiceVox(string text, int speakerNum)//VOICEVOXに指示を送る。
        {
            string speakerString = speakerNum.ToString();
            var parameters = new Dictionary<string, string>()
            {
                { "text", text },
                { "speaker", speakerString },
            };
            Console.WriteLine(text);
            Console.WriteLine("キャラクター番号は" + num.ToString() + "番です。");
            Console.WriteLine("実行までしばらくお待ちください。");
            string encodedParamaters = await new FormUrlEncodedContent(parameters).ReadAsStringAsync();
            using (var resultAudioQuery = await httpClient.PostAsync(@"http://localhost:50021/audio_query?" + encodedParamaters, null))
            {
                string resBodyStr = await resultAudioQuery.Content.ReadAsStringAsync();
                var content = new StringContent(resBodyStr, Encoding.UTF8, @"application/json");
                using (var resultSynthesis = await httpClient.PostAsync(@"http://localhost:50021/synthesis?speaker=" + speakerString, content))
                {
                    PlaySound();
                    StopSound();
                    Stream stream = await resultSynthesis.Content.ReadAsStreamAsync();
                    SoundPlayer soundPlayer = new SoundPlayer(stream);
                    soundPlayer.PlaySync();
                    PlaySound();
                    StopSound();
                }
            }
        }

        static async void SendToCoeiroInk(string text, int speakerNum)//COEIROINKに指示を送る。
        {
            string speakerString = speakerNum.ToString();
            var parameters = new Dictionary<string, string>()
            {
                { "text", text },
                { "speaker", speakerString },
            };
            Console.WriteLine(text);
            Console.WriteLine("キャラクター番号は" + num.ToString() + "番です。");
            Console.WriteLine("実行までしばらくお待ちください。");
            string encodedParamaters = await new FormUrlEncodedContent(parameters).ReadAsStringAsync();
            using (var resultAudioQuery = await httpClient.PostAsync(@"http://localhost:50031/audio_query?" + encodedParamaters, null))
            {
                string resBodyStr = await resultAudioQuery.Content.ReadAsStringAsync();
                var content = new StringContent(resBodyStr, Encoding.UTF8, @"application/json");
                using (var resultSynthesis = await httpClient.PostAsync(@"http://localhost:50031/synthesis?speaker=" + speakerString, content))
                {
                    PlaySound();
                    StopSound();
                    Stream stream = await resultSynthesis.Content.ReadAsStreamAsync();
                    SoundPlayer soundPlayer = new SoundPlayer(stream);
                    soundPlayer.PlaySync();
                    PlaySound();
                    StopSound();
                }
            }
        }



        static void readtextfile()
        {
            var program = new Program();
            using (var reader = program.OpenReadFile("C:\\DormitorySupporter\\Files\\Settings\\YobidashiConsole\\Yobidashi_Temp.txt"))
            {
                readString = reader.ReadLine();
                var program2 = new Program();
                using (var fileStream = new FileStream("C:\\DormitorySupporter\\Files\\Settings\\YobidashiConsole\\Yobidashi_Temp.txt", FileMode.Open))
                {
                    fileStream.SetLength(0);
                }
                if (readString != "")
                {
                    if (readString != null)
                    {
                        SetSpeaker();
                        if(EngineType == "VOICEVOX")
                        {
                            num = int.Parse(SpeakerInfo);
                            Console.WriteLine("VOICEVOXで音声合成を行います。");
                            SendToVoiceVox(readString, num);
                        }
                        else if(EngineType == "COEIROINK")
                        {
                            num = int.Parse(SpeakerInfo);
                            Console.WriteLine("COEIROINKで音声合成を行います。");
                            SendToCoeiroInk(readString, num);
                        }

                    }
                }
            }
        }


        static SoundPlayer player = new SoundPlayer("C:\\DormitorySupporter\\Apps\\YobidashiConsole\\Chaim.wav");

        static void PlaySound()
        {
            // 再生
            //player.Play();              // 別スレッドで再生
            //player.PlayLooping();     // 別スレッドでループ再生
            player.PlaySync();        // 当該スレッドで再生（再生中は他の操作ができません）
        }

        static void StopSound()
        {
            // 停止
            player.Stop();
        }
        
        static void SetSpeaker()
        {
            var program = new Program();
            using (var reader = program.OpenReadFile("C:\\DormitorySupporter\\Files\\Settings\\YobidashiConsole\\SpeakerNumSetting.txt"))
            {
                SpeakerInfo = reader.ReadLine();
            }

            var program2 = new Program();
            using (var reader = program2.OpenReadFile("C:\\DormitorySupporter\\Files\\Settings\\YobidashiConsole\\Yobidashi_Engine.txt"))
            {
                EngineType = reader.ReadLine();
            }
        }

    }
}
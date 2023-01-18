using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfVoiceAssistent.OpenSomething
{
    internal class OpenApplication
    {
        public static string StartProgramm(string[] _namePrograms)
        {
            string _NameForVoice = "";
            foreach (string _nameProgram in _namePrograms)
            {
                switch (_nameProgram)
                {
                    case "стим":
                        Process.Start(@"E:\Program Files (x86)\Steam\steam.exe");//путь к Стиму
                        break;
                    case "гугл":
                        Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe");//путь к гуглу
                        break;
                    case "вконтакте":
                        Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "https://vk.com/");//ссылка на вк
                        break;
                    case string _wordInCase when (_wordInCase == "телеграм" || _wordInCase == "телеграмм"):
                        Process.Start(@"E:\Доп.проги\Telega\Telegram Desktop\Telegram.exe");//путь к телеграму
                        break;
                    case "рабочую таблицу":
                        Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", "https://docs.google.com/spreadsheets/d/1c_YCoF7NfEPbIzega-rFxkveMq8ubC8F/edit#gid=1216431847");//путь к гуглу
                        break;
                    case "дискорд":
                        Process.Start(@"C:\Users\PC\AppData\Local\Discord\app-1.0.9008\Discord.exe");//путь к дискорду
                        break;
                    case " ":
                        break;
                    case "":
                        break;
                }
                _NameForVoice += _nameProgram + ", ";
            }
            return _NameForVoice;
        }


        private static List<string> _nameProgremsForStart = new List<string>();
        private static string[] myArr = new string[CreateGrammar._ListsForGrammar.NameProgram.ToArray().Length];

        /// <summary>
        /// Запускает протокол с заданным названием (стандартный/рабочий/игровой/полный)
        /// </summary>
        /// <param name="_nameProt"> Название протокола</param>
        /// <returns> Возвращает название всех запущенных программ</returns>
        public static string StartProtocol(string _nameProt)
        {
            switch (_nameProt)
            {
                case "стандартный":
                    _nameProgremsForStart.Add("гугл");
                    _nameProgremsForStart.Add("вконтакте");
                    _nameProgremsForStart.Add("телеграм");
                    _nameProgremsForStart.CopyTo(myArr);
                    return StartProgramm(myArr);

                case "рабочий":
                    _nameProgremsForStart.Add("телеграм");
                    _nameProgremsForStart.Add("гугл");
                    _nameProgremsForStart.Add("рабочую таблицу");
                    _nameProgremsForStart.CopyTo(myArr);
                    return StartProgramm(myArr);

                case "игровой":
                    _nameProgremsForStart.Add("дискорд");
                    _nameProgremsForStart.Add("стим");
                    _nameProgremsForStart.Add("гугл");
                    _nameProgremsForStart.Add("вконтакте");
                    _nameProgremsForStart.CopyTo(myArr);
                    return StartProgramm(myArr);

                case "полный":
                    _nameProgremsForStart.AddRange(CreateGrammar._ListsForGrammar.NameProgram.ToArray());
                    _nameProgremsForStart.CopyTo(myArr);
                    return StartProgramm(myArr);

            }
            return "";
        }
    }
}

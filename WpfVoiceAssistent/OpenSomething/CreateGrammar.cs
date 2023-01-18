using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Windows.Controls;
using System.Windows;

namespace WpfVoiceAssistent.OpenSomething
{
    public class CreateGrammar
    {
        public static CultureInfo _language = new CultureInfo("ru-RU");
        public static DirectoryInfo directoryInfoFolder;
        public static Item _ListsForGrammar;
        //названия всех файлов
        public static List<string[]> FileList = new List<string[]>();
        public static List<string[]> PlaylistList = new List<string[]>();
        public static List<string[]> MusicList = new List<string[]>();

        public static List<string> Playlist_ID_List = new List<string>();
        public static List<string> Playlist_Name_List = new List<string>();

        public static List<string> MusicFilePath = new List<string>();
        public static List<string> MusicName = new List<string>();

        public static string[][] _StartGameWithstartWord;
        static CreateGrammar() 
        {
            using (StreamReader r = new StreamReader(@"F:\КурсоваяТРПО\WpfVoiceAssistent\WpfVoiceAssistent\DataForDrammar.json"))
            {
                string json = r.ReadToEnd();

                _ListsForGrammar = JsonConvert.DeserializeObject<Item>(json);
            }
            _StartGameWithstartWord = new string[][] { _ListsForGrammar.StartAll.ToArray(), _ListsForGrammar.NameProgram.ToArray() };
        }

        /// <summary>
        /// Этот метод создаёт грамматики для локального использования.
        /// Т.е. Передаёт названия файлов, (аудио, фото, видео) по взятым из БД путям в листы и создаёт из них грамматики для работы с голосом
        /// </summary>
        public static Grammar LocalPlaylistGrammar()
        {
            try
            {
                //Все названия всех плейлистов
                PlaylistList = ControlDB.Class.SQL_Select($"select [ID Плейлиста],[Название] from [Плейлисты]");

                foreach (string[] both in PlaylistList)
                {
                    Playlist_ID_List.Add(both[0]);
                    Playlist_Name_List.Add(both[1]);
                }

                GrammarBuilder grammarBuilderPlaylist = new GrammarBuilder();
                grammarBuilderPlaylist.Culture = _language;
                grammarBuilderPlaylist.Append(new Choices(_ListsForGrammar.StartAll.ToArray()));
                grammarBuilderPlaylist.Append(new Choices("плейлист"));
                grammarBuilderPlaylist.Append(new Choices(Playlist_Name_List.ToArray()));
                
                return new Grammar(grammarBuilderPlaylist);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
                return null;
            }
        }

        public static Grammar LocalAudioGrammar()
        {
            try
            {
                MusicList = ControlDB.Class.SQL_Select($"select [ID Музыки],[Название],[Относительный путь] from [Музыка]");
                foreach (string[] music in MusicList)
                {
                    MusicName.Add(music[1]);
                    MusicFilePath.Add(music[2]);
                }

                GrammarBuilder grammarBuilderMusic = new GrammarBuilder();
                grammarBuilderMusic.Culture = _language;
                grammarBuilderMusic.Append(new Choices("открыть", "запустить", "включить", "включи"));
                grammarBuilderMusic.Append(new Choices("песню"));
                grammarBuilderMusic.Append(new Choices(MusicName.ToArray()));

                return new Grammar(grammarBuilderMusic);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
                return null;
            }
        }

        public static Grammar LocalMediaGrammar(out List<string> NameVideoFileList, out List<string> NameImageFileList)
        {
            try
            {
                List<string> LocalNameFile = new List<string>();

                List<string[]> list = ControlDB.Class.SQL_Select($"select [Путь] from Директории where [ID Категории] = (select [ID Категории] from Категории where Название = 'видео')");
                List<string> nameVideo = new List<string>();
                directoryInfoFolder = new DirectoryInfo(list[0][0]);

                //Создаём грамматику со всеми названиями папок в выбранной папке
                foreach (FileInfo _file in directoryInfoFolder.GetFiles())
                {
                    FileList.Add(new string[] { _file.Name, _file.ToString().Split('.')[0] });
                    nameVideo.Add(_file.ToString().Split('.')[0]);
                }
                NameVideoFileList = nameVideo;

                List<string[]> list1 = ControlDB.Class.SQL_Select($"select [Путь] from Директории where [ID Категории] = (select [ID Категории] from Категории where Название = 'фото')");
                List<string> nameImage = new List<string>();
                directoryInfoFolder = new DirectoryInfo(list1[0][0]);

                foreach (FileInfo _file in directoryInfoFolder.GetFiles())
                {
                    FileList.Add(new string[] { _file.Name, _file.ToString().Split('.')[0] });
                    nameImage.Add(_file.ToString().Split('.')[0]);
                }
                NameImageFileList = nameImage;

                
                foreach (string[] _file in FileList)
                {
                    LocalNameFile.Add(_file[1]);
                }

                GrammarBuilder grammarBuilder1 = new GrammarBuilder();
                grammarBuilder1.Culture = _language;
                grammarBuilder1.Append(new Choices(_ListsForGrammar.StartAll.ToArray()));
                grammarBuilder1.Append(new Choices("видео", "фото"));
                grammarBuilder1.Append(new Choices(LocalNameFile.ToArray()));

                return new Grammar(grammarBuilder1);
            }
            catch (Exception ex)
            {
                NameVideoFileList = null;
                NameImageFileList = null;
                MessageBox.Show("Ошибка: " + ex.Message);
                return null;
            }
        }


        #region Основные функции
        public static Grammar StartStopActiveLaunchGrammar()
        {
            Choices ch_StartStopActiveLaunch = new Choices(); //Создание Выборки

            //Создаём массив из всех значений, которые будем использовать во время приветствия
            ch_StartStopActiveLaunch.Add(_ListsForGrammar.StartStopActiveLaunch.ToArray());//Записываем массив в "Выборы"

            GrammarBuilder gb_SSAL = new GrammarBuilder();
            gb_SSAL.Culture = _language;//подключение русского языка

            //Заполняем шаблон GrammarBuilder «What is <x> plus <y>?»
            gb_SSAL.Append("переход в");
            gb_SSAL.Append(ch_StartStopActiveLaunch);
            gb_SSAL.Append("режим");

            Grammar g_StartStop = new Grammar(gb_SSAL); //управляющий Grammar

            return g_StartStop;
        }

        public static Grammar ProgrammGrammar()
        {
            Choices ch_StartSMTH = new Choices(_ListsForGrammar.StartAll.ToArray());
            Choices ch_StartGame = new Choices(_ListsForGrammar.NameProgram.ToArray());

            GrammarBuilder gb_PlayStart = new GrammarBuilder();
            gb_PlayStart.Culture = _language;
            //Заполняем шаблон GrammarBuilder «"Запуск" + "Название игры"»
            gb_PlayStart.Append(ch_StartSMTH);
            gb_PlayStart.Append(ch_StartGame);


            Grammar g_StartStop = new Grammar(gb_PlayStart);

            return g_StartStop;
        }

        public static Grammar ProtocolGrammar()
        {
            Choices ch_StartSMTH = new Choices(_ListsForGrammar.StartAll.ToArray());
            Choices ch_Protocol = new Choices(_ListsForGrammar.NameProtocol.ToArray());

            GrammarBuilder gb_P1 = new GrammarBuilder();
            GrammarBuilder gb_P2 = new GrammarBuilder();
            gb_P1.Culture = _language; gb_P2.Culture = _language;

            //Первый шаблон построения фразы
            gb_P1.Append(ch_StartSMTH);
            gb_P1.Append(ch_Protocol);
            gb_P1.Append("протокол");

            //второй шаблон построения фразы уже в другом GrammarBuilder
            gb_P2.Append(ch_StartSMTH);
            gb_P2.Append("протокола");
            gb_P2.Append(ch_Protocol);


            // Create a Choices for the two alternative phrases, convert the Choices  
            // to a GrammarBuilder, and construct the grammar from the result.  
            Choices bothChoices = new Choices(new GrammarBuilder[] { gb_P1, gb_P2 });

            Grammar g_V = new Grammar(bothChoices); //управляющий Grammar
            return g_V;
        }

        public static Grammar OnOffPCGrammar()
        {
            Choices ch_DRSS = new Choices("выключи компьютер", "перезагрузка");
            Grammar g_V = new Grammar(ch_DRSS);
            return g_V;
        }

        public static Grammar StartAndStopInterfaceGrammar()
        {
            List<string> StartStop = new List<string>();

            //Добавляю команды запуска и выключения в один лист
            StartStop.AddRange(_ListsForGrammar.StopAll);
            StartStop.AddRange(_ListsForGrammar.StartAll);

            Choices ch_StopSMTH = new Choices(StartStop.ToArray());
            GrammarBuilder gb_P2 = new GrammarBuilder();
            gb_P2.Culture = _language;

            gb_P2.Append(ch_StopSMTH);
            gb_P2.Append("интерфейс");

            Grammar g_V = new Grammar(gb_P2); //управляющий Grammar
            return g_V;
        }

        public static Grammar StopWorkTern()
        {
            Choices ch_StopSMTH = new Choices(_ListsForGrammar.StopAll.ToArray());
            Choices ch_Name = new Choices("себя");
            GrammarBuilder gb_P2 = new GrammarBuilder();
            gb_P2.Culture = _language;

            gb_P2.Append(ch_StopSMTH);
            gb_P2.Append(ch_Name);

            Grammar g_V = new Grammar(gb_P2); //управляющий Grammar
            return g_V;
        }
        #endregion

        #region Звуку и музыке
        public static Grammar AudioGrammar()
        {
            Choices ch_Protocol1 = new Choices(_ListsForGrammar.StartAll.ToArray());
            Choices ch_Protocol3 = new Choices(_ListsForGrammar.AudioName.ToArray());

            GrammarBuilder gb_P1 = new GrammarBuilder();
            GrammarBuilder gb_P2 = new GrammarBuilder();
            gb_P1.Culture = _language;
            gb_P2.Culture = _language;

            //Первый шаблон для открытия случайной песни
            gb_P1.Append(ch_Protocol1);
            gb_P1.Append("случайную");
            gb_P1.Append(ch_Protocol3);

            gb_P2.Append(ch_Protocol1);
            gb_P2.Append(ch_Protocol3);
            Choices bothChoices = new Choices(new GrammarBuilder[] { gb_P1, gb_P2 });

            Grammar g_V = new Grammar(bothChoices); //управляющий Grammar
            return g_V;
        }
        public static Grammar SetValueSoundGrammar()
        {
            Choices ch_value = new Choices();//Создание Выборки
            //Создаём массив из всех значений, которые будем использовать во время приветствия
            ch_value.Add(_ListsForGrammar.ValueSound.ToArray()); //Записываем массив в "Выборы"

            GrammarBuilder gb_SetValue = new GrammarBuilder(); //Создаём GrammarBuilder
            gb_SetValue.Culture = _language;//подключение русского языка
            gb_SetValue.Append("громкость");
            gb_SetValue.Append(ch_value); //Заполняем шаблон GrammarBuilder «What is <x> plus <y>?»

            Grammar g_v = new Grammar(gb_SetValue); //управляющий Grammar

            return g_v;
        }
        public static Grammar AudioStopGrammar()
        {
            Choices ch_Protocol = new Choices("останови", "продолжи", "продолжай", "выключи");
            Choices ch_P = new Choices(_ListsForGrammar.AudioName.ToArray());
            GrammarBuilder gb_P1 = new GrammarBuilder();
            gb_P1.Culture = _language;

            //Первый шаблон для открытия случайной песни
            gb_P1.Append(ch_Protocol);
            gb_P1.Append(ch_P);

            Choices bothChoices = new Choices(new GrammarBuilder[] { gb_P1 });

            Grammar g_V = new Grammar(bothChoices); //управляющий Grammar
            return g_V;
        }

        public static Grammar AudioNextGrammar()
        {
            Choices ch_Protocol = new Choices("следующая", "некст", "другая", "следующую");
            Choices ch_P = new Choices(_ListsForGrammar.AudioName.ToArray());
            GrammarBuilder gb_P1 = new GrammarBuilder();
            gb_P1.Culture = _language;

            //Первый шаблон для открытия случайной песни
            gb_P1.Append(ch_Protocol);
            gb_P1.Append(ch_P);

            Choices bothChoices = new Choices(new GrammarBuilder[] { gb_P1 });

            Grammar g_V = new Grammar(bothChoices); //управляющий Grammar
            return g_V;
        }
        #endregion

        #region Погода и необязательные функции
        public static Grammar WeatherGrammar()
        {
            Choices ch_StartSMTH = new Choices();
            Choices ch_Weather = new Choices();
            ch_StartSMTH.Add(_ListsForGrammar.StartAll.ToArray());
            ch_Weather.Add(_ListsForGrammar.Weather.ToArray());
            GrammarBuilder gb_W = new GrammarBuilder();
            gb_W.Culture = _language;

            gb_W.Append(ch_StartSMTH);
            gb_W.Append(ch_Weather);

            Grammar g_Weather = new Grammar(gb_W); //управляющий Grammar
            return g_Weather;
        }
        public static Grammar ComplimentsGrammar()
        {
            Choices ch_StartSMTH = new Choices(_ListsForGrammar.StartAll.ToArray());
            Choices ch_DRSS = new Choices("комплимент", "приятно");

            GrammarBuilder gb_Compl1 = new GrammarBuilder();
            gb_Compl1.Culture = _language;

            gb_Compl1.Append(ch_StartSMTH);
            gb_Compl1.Append(ch_DRSS);

            Grammar g_V = new Grammar(gb_Compl1); //управляющий Grammar
            return g_V;
        }
        public static Grammar VoiceJokeGrammar()
        {
            Choices ch_StartSMTH = new Choices(_ListsForGrammar.StartAll.ToArray());
            Choices ch_DRSS = new Choices("анекдот", "шутку");
            GrammarBuilder gb_P1 = new GrammarBuilder();
            gb_P1.Culture = _language;

            gb_P1.Append(ch_StartSMTH);
            gb_P1.Append(ch_DRSS);

            Grammar g_V = new Grammar(gb_P1); //управляющий Grammar
            return g_V;
        }
        #endregion

    }
    public class Item
    {
        public List<string> StartStopActiveLaunch;
        public List<string> StartAll;
        public List<string> StopAll;

        public List<string> NameProgram;
        public List<string> NameProtocol;

        public List<string> Weather;
        public List<string> AudioName;
        public List<string> ValueSound;

        public List<string> Compliments;
        public List<string> JustJoke;
        public List<string> Ternlaugh;
    }
}

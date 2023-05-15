using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using WpfVoiceAssistent.OpenSomething;
using WpfVoiceAssistent.TheWeather;
using System.Threading;

//using MahApps.Metro.IconPacks;

namespace WpfVoiceAssistent
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Audio.AudioPlayer aud;
        SpeechSynthesizer ss = new SpeechSynthesizer();
        static readonly CultureInfo _language = new CultureInfo("ru-RU");
        SpeechRecognitionEngine sre = new SpeechRecognitionEngine(_language);

        private static bool _firstStart;

        public void VoicуThisText(string txtForVoice)
        {
            ss.SpeakAsync(txtForVoice);
        }

        public MainWindow(bool firstStart)
        {
            InitializeComponent();
            _firstStart = firstStart;
            if (_firstStart)
                this.Visibility = Visibility.Hidden;

            ss.SetOutputToDefaultAudioDevice();
            ss.Volume = 100;// от 0 до 100 громкость голоса
            ss.Rate = 3; //от -10 до 10 скорость голоса

            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(Sre_SpeechRecognized);
            VoicуThisText("Программа запущена");

            List<string> NameVideoFileLists = new List<string>(); List<string> NameImageFileLists = new List<string>();

            sre.LoadGrammar(CreateGrammar.LocalMediaGrammar(out NameVideoFileLists, out NameImageFileLists));
            NameVideoFileList.ItemsSource = NameVideoFileLists;
            NameImageFileList.ItemsSource = NameImageFileLists;

            sre.LoadGrammar(CreateGrammar.LocalPlaylistGrammar());
            sre.LoadGrammar(CreateGrammar.LocalAudioGrammar());
            sre.LoadGrammar(CreateGrammar.AudioGrammar());
            sre.LoadGrammar(CreateGrammar.AudioStopGrammar());
            sre.LoadGrammar(CreateGrammar.AudioNextGrammar());
            sre.LoadGrammar(CreateGrammar.SetValueSoundGrammar());
            sre.LoadGrammar(CreateGrammar.ProtocolGrammar());
            sre.LoadGrammar(CreateGrammar.StartStopActiveLaunchGrammar());
            sre.LoadGrammar(CreateGrammar.WeatherGrammar());
            sre.LoadGrammar(CreateGrammar.ProgrammGrammar());
            sre.LoadGrammar(CreateGrammar.VoiceJokeGrammar());
            sre.LoadGrammar(CreateGrammar.OnOffPCGrammar());
            sre.LoadGrammar(CreateGrammar.ComplimentsGrammar());
            sre.LoadGrammar(CreateGrammar.StartAndStopInterfaceGrammar());
            sre.LoadGrammar(CreateGrammar.StopWorkTern());
            sre.LoadGrammar(CreateGrammar.LoopMusicGrammar());
            sre.LoadGrammar(CreateGrammar.PersonAnswerGrammar());
            sre.RecognizeAsync(RecognizeMode.Multiple);

            SelectItemsListBox();
            _firstStart = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string _SpokenText = e.Result.Text; //сказанный текст
            float confidence = e.Result.Confidence; //Точность сказаного текста(процент совпадения)

            List<string> LocalNameFile = new List<string>();
            foreach (string[] _file in CreateGrammar.FileList)
            {
                LocalNameFile.Add(_file[1]);
            }


            if (confidence >= 0.60)
            {
                PersonAnswer(_SpokenText);


                try
                {
                    foreach (string _wordForStart in CreateGrammar._ListsForGrammar.StartAll)
                    {
                        if (_SpokenText.Substring(0, _SpokenText.IndexOf(" ")).IndexOf($"{_wordForStart}") >= 0)
                        {
                            CallStartProtocol(_wordForStart, _SpokenText);

                            CallStartProgram(_wordForStart, _SpokenText);

                            CallTheWeather(_wordForStart, _SpokenText);

                            CallTheJoke(_wordForStart, _SpokenText);

                            CallPlayMusic(_wordForStart, _SpokenText);

                            CallPlayPlaylist(_wordForStart, _SpokenText);

                            CallCompliments(_wordForStart, _SpokenText);

                            CallShowIntarface(_wordForStart, _SpokenText);

                            CallShowVideo(_wordForStart, LocalNameFile, _SpokenText);

                            CallShowPhoto(_wordForStart, LocalNameFile, _SpokenText);

                            
                        }
                    }
                }
                catch 
                {
                    
                }
                foreach (string _wordForStop in CreateGrammar._ListsForGrammar.StopAll)
                {
                    //CallStopMusic(_wordForStop, _SpokenText);

                    CallHideIntarface(_wordForStop, _SpokenText);

                    CallStopTern(_wordForStop, _SpokenText);
                }
                

                CallPausePlayNextMusic(_SpokenText);

                CallSettingsValue(_SpokenText);

                CallLoopMusic(_SpokenText);
                
                WorkWithSystem.CoiseActionWithSystem(_SpokenText);

            }
        }

        private void CallLoopMusic(string _SpokenText)
        {//Зацикливание музыки
            foreach (string _valueS in CreateGrammar._ListsForGrammar.AudioName.ToArray())
            {
                if (_SpokenText.IndexOf($"повторяй {_valueS}") >= 0)
                {
                    if (aud != null)
                    {
                        aud.StartLoop();

                        VoicуThisText("Установлено зацикливание музыки");

                    }


                    return;
                }
                else if (_SpokenText.IndexOf($"не повторяй {_valueS}") >= 0) 
                {
                    if (aud != null)
                    {
                        aud.StopLoop();

                        VoicуThisText("Прекращено зацикливание музыки");

                    }
                    return;
                }
            }

        }

        private void PersonAnswer(string _SpokenText)
        {
            foreach (string _value in CreateGrammar.DictAnswerJSON.Keys)
            {
                if (_SpokenText.IndexOf($"{_value}") >= 0)
                {
                    VoicуThisText(CreateGrammar.DictAnswerJSON[_value].ToString());
                }
            }

        }

        private void CallPausePlayNextMusic(string _SpokenText)
        {
            foreach (string _music in CreateGrammar._ListsForGrammar.AudioName.ToArray())
            {
                if (_SpokenText.IndexOf($"останови {_music}") >= 0 || _SpokenText.IndexOf($"останови прейлист") >= 0)
                {
                    if (aud != null)
                        aud.PauseSong();
                    SliderNoise.IsEnabled = false;
                }
                else if (_SpokenText.IndexOf($"продолжи {_music}") >= 0 || _SpokenText.IndexOf($"продолжай {_music}") >= 0)
                {
                    if (aud == null)
                        aud = new Audio.AudioPlayer(@"F:\КурсоваяТРПО\FolderForAudio\remember_-_Kozhura.mp3");
                    else
                        aud.StartSong();
                    
                    SliderNoise.IsEnabled = true;
                }
                else if(_SpokenText.IndexOf($"следующая {_music}") >= 0 || _SpokenText.IndexOf($"cледующую {_music}") >= 0
                    )//|| _SpokenText.IndexOf($"некст {_music}") >= 0 || _SpokenText.IndexOf($"другая {_music}") >= 0
                {
                    if (aud != null)
                        aud.StopSong();
                    StartRandomSong();
                    SliderNoise.IsEnabled = true;
                }
                if (_SpokenText.IndexOf($"выключи {_music}") >= 0 || _SpokenText.IndexOf($"убери {_music}") >= 0 
                    || _SpokenText.IndexOf($"выключи прейлист") >= 0)
                {
                    if (aud != null)
                        aud.StopSong();
                    SliderNoise.IsEnabled = false;
                }
            }

        }
        private void CallSettingsValue(string _SpokenText)
        {//Изменение громкости
            foreach (string _valueS in CreateGrammar._ListsForGrammar.ValueSound.ToArray())
            {
                if (_SpokenText.IndexOf($"громкость {_valueS}") >= 0)
                {
                    if(aud != null)
                        aud.ChangingTheVolume(_valueS);
                    return;
                }
            }
           
        }
        private void CallStopTern(string _wordForStop, string _SpokenText)
        {
            if (_SpokenText.IndexOf($"выключи себя") >= 0 || _SpokenText.IndexOf($"закрой себя") >= 0 || _SpokenText.IndexOf($"останови себя") >= 0)
            {
                System.Windows.Application.Current.Shutdown();
            }

        }
        private void CallHideIntarface(string _wordForStop, string _SpokenText)
        {
            if (_SpokenText.IndexOf($"{_wordForStop} интерфейс") >= 0)
            {
                this.Hide();
                this.Visibility = Visibility.Hidden;

                VoicуThisText("Скрываю интерфейс");
                return;
            }
        }
        private void CallStopMusic(string _wordForStop, string _SpokenText)
        {
            foreach (string _music in CreateGrammar._ListsForGrammar.AudioName) { 
                if (_SpokenText.IndexOf($"{_wordForStop} {_music}") >= 0)
                {
                    if (aud != null)
                        aud.StopSong();
                    SliderNoise.IsEnabled = false;
                    txtBoxNameMusicForPlaying.Text = "Название";
                }
            }
        }
        private void CallShowVideo(string _wordForStart, List<string> LocalNameFile, string _SpokenText)
        {
            foreach (string _nameVideo in LocalNameFile)
            {
                if (_SpokenText.IndexOf($"{_wordForStart} видео {_nameVideo}") >= 0)
                {
                    OpenVideo(_SpokenText);
                    VoicуThisText($"Видео с названием {_nameVideo} открыто");
                }
            }
        }
        private void CallShowPhoto(string _wordForStart, List<string> LocalNameFile, string _SpokenText)
        {
            foreach (string _namePhoto in LocalNameFile)
            {
                if (_SpokenText.IndexOf($"{_wordForStart} фото {_namePhoto}") >= 0)
                {
                    OpenPhoto(_SpokenText);
                    VoicуThisText($"Фото с названием {_namePhoto} открыто");
                }
            }

        }
        private void CallPlayPlaylist(string _wordForStart, string _SpokenText)
        {
            foreach (string namePlaylist in CreateGrammar.Playlist_Name_List)
            {
                if (_SpokenText.IndexOf($"{_wordForStart} плейлист {namePlaylist}") >= 0)
                {
                    VoicуThisText($"Начинаю проигрыватние плейлиста {namePlaylist}");
                    Thread.Sleep(2000);
                    StartPlaylist(namePlaylist);
                }
            }
        }
        private void CallStartProtocol(string _wordForStart, string _SpokenText)
        {
            foreach (string[] _nameProt in CreateGrammar.Protocol_List)
            {
                if (_SpokenText.IndexOf(_wordForStart + " " + _nameProt[1] + " протокол") >= 0 || _SpokenText.IndexOf(_wordForStart + " протокол " + _nameProt[1]) >= 0)
                {
                    VoicуThisText("Запускаю " + OpenApplication.StartProtocol(_nameProt[1]));
                    return;
                }
            }
        }
        private void CallStartProgram(string _wordForStart, string _SpokenText)
        {
            foreach (string _wordGame in CreateGrammar._StartGameWithstartWord[1])
            {
                if (_SpokenText.IndexOf(_wordForStart + " " + _wordGame) >= 0)
                {
                     OpenApplication.StartProgramm(_wordGame);
                    VoicуThisText("Запускаю " + _wordGame);
                    return;
                }
            } //Запуск Программ
        }
        private void CallTheWeather(string _wordForStart, string _SpokenText)
        {
            foreach (string _wordWeather in CreateGrammar._ListsForGrammar.Weather)
            {
                if (_SpokenText.IndexOf($"{_wordForStart} {_wordWeather}") >= 0)
                {
                    DateTime localDate = DateTime.Now;
                    VoicуThisText(OpenWeather.FullWeatherAnswer(_wordForStart, _SpokenText, localDate));
                    return;
                }
            }//Погода
        }
        private void CallTheJoke(string _wordForStart, string _SpokenText)
        {
            if (_SpokenText.IndexOf($"{_wordForStart} анекдот") >= 0 || _SpokenText.IndexOf($"{_wordForStart} шутку") >= 0)
            {
                Random _randomJoke = new Random();
                int _numberJoke = _randomJoke.Next(0, CreateGrammar._ListsForGrammar.JustJoke.ToArray().Length);
                VoicуThisText(CreateGrammar._ListsForGrammar.JustJoke[_numberJoke]);

                return;
            }//шутки
        }
        private void CallCompliments(string _wordForStart, string _SpokenText)
        {
            
            if (_SpokenText.IndexOf($"{_wordForStart} комплимент") >= 0 || _SpokenText.IndexOf($"{_wordForStart} тепло") >= 0 )
            {
                Random _random = new Random();
                int _numbercompl = _random.Next(0, CreateGrammar._ListsForGrammar.Compliments.ToArray().Length);
                VoicуThisText(CreateGrammar._ListsForGrammar.Compliments[_numbercompl].ToString());
                return;
            }
           
        }
        private void CallPlayMusic(string _wordForStart, string _SpokenText)
        {
            foreach (string _music in CreateGrammar._ListsForGrammar.AudioName)
            {
                foreach (string _nameAudio in CreateGrammar.MusicName)
                {
                    if (_SpokenText.IndexOf($"{_wordForStart} {_music} {_nameAudio}") >= 0 || _SpokenText.IndexOf($"{_wordForStart} {_music} {_nameAudio}") >= 0)
                    {
                        if (aud != null)
                            aud.StopSong();
                        VoicуThisText($"Включаю песню {_nameAudio}");
                        Thread.Sleep(2000);
                        StartMusic(_nameAudio);
                        txtBoxNameMusicForPlaying.Text = "Название песни: " + _nameAudio;
                        return;
                    }
                }

                if (_SpokenText.IndexOf($"{_wordForStart} случайную {_music}") >= 0)
                {
                    
                    if (aud != null)
                        aud.StopSong();

                    VoicуThisText("включаю случайную песню");
                    Thread.Sleep(2000);
                    StartRandomSong();
                    txtBoxNameMusicForPlaying.Text = "Включена случайная песня";
                    return;
                }

                
            }
        }
        private void CallShowIntarface(string _wordForStart, string _SpokenText)
        {
            if (_SpokenText.IndexOf($"{_wordForStart} интерфейс") >= 0)
            {
                this.Show();
                this.Visibility = Visibility.Visible;

                VoicуThisText("Показываю интерфейс");
                return;
            }

        }


        #region Методы обработки команд
        public void StartPlaylist(string namePlaylist)
        {
            List<string> ID_Music = new List<string>();
            List<string> Path_Music = new List<string>();

            foreach (string[] both in CreateGrammar.PlaylistList)
            {
                if (both[1] == namePlaylist)
                {
                    ID_Music = ControlDB.Class.SQL_SelectList($"select [ID Музыки] from [МузыкаСвязьПлейлисты] where  [ID Плейлиста] = {both[0]}"); // 1? 2? 3
                }
            }

            foreach (string music in ID_Music)
            {
                Path_Music.Add(ControlDB.Class.SQLSelectOneItem($"select [Путь] from [Директории] where [ID Категории] = (select [ID Категории] from [Категории] where [Название] = 'аудио')")
                    + "\\" + ControlDB.Class.SQLSelectOneItem($"select [Относительный путь] from [Музыка] where [ID Музыки] = {music}"));
            } // путь 1, путь 2, путь 3

            aud = new Audio.AudioPlayer();
            aud.openPlaylist(Path_Music);
            SliderNoise.IsEnabled = true;
            VoicуThisText($"Плейлист " + namePlaylist + " запущен");
        }
        public void StartRandomSong()
        {
            try
            {
                string audioPath = ControlDB.Class.SQLSelectOneItem($"select [Путь] from Директории where [ID Категории] = (select [ID Категории] from Категории where Название = 'аудио')");
               

                string file = null;
                if (!string.IsNullOrEmpty(audioPath))
                {
                    var extensions = new string[] { ".mp3" };
                    try
                    {
                        var di = new DirectoryInfo(audioPath);
                        var rgFiles = di.GetFiles("*.*").Where(f => extensions.Contains(f.Extension.ToLower()));
                        Random R = new Random();
                        file = rgFiles.ElementAt(R.Next(0, rgFiles.Count())).FullName;
                        aud = new Audio.AudioPlayer(file);
                        SliderNoise.IsEnabled = true;
                        txtBoxNameMusicForPlaying.Text = "Проигрывается случайная песня";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + " - ошибка при работе с файлом:" + audioPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Песни с таким названием не существует: " + ex.Message);
            }
        }
        public void StartMusic(string _nameSong)
        {
            try
            {
                string listAudioPath = ControlDB.Class.SQLSelectOneItem($"select [Путь] from Директории where [ID Категории] = (select [ID Категории] from Категории where Название = 'аудио')");

                int fileIndex = CreateGrammar.MusicName.IndexOf(_nameSong);
                string name = CreateGrammar.MusicFilePath[fileIndex];

                aud = new Audio.AudioPlayer(listAudioPath + @"\" + name);
                aud.StartSong();
                SliderNoise.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Песни с таким названием не существует: " + ex.Message);
            }
        }
        public void OpenVideo(string _requestText)
        {
            string[] arrayText = _requestText.Split(' ');// {str, str, strNameFile1Word, strNameFile2Word...}
            string nameFile = "";
            try
            {

                for (int i = 2; i < arrayText.Length; i++)
                {
                    nameFile += arrayText[i] + " ";
                }
                //nameFile.Substring(0, nameFile.Length - 1) - полное название нужного файла
                Process process = new Process();

                List<string[]> list = ControlDB.Class.SQL_Select($"select [Путь] from Директории where [ID Категории] = (select [ID Категории] from Категории where Название = '{arrayText[1]}')");

                process.StartInfo.FileName = list[0][0] + $"\\{nameFile.Substring(0, nameFile.Length - 1)}.mp4";
                process.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{arrayText[1]} с таким названием не существует: " + ex.Message);
            }
        }
        public void OpenPhoto(string _requestText)
        {
            string[] arrayText = _requestText.Split(' ');// {str, str, strNameFile1Word, strNameFile2Word...}
            string nameFile = "";
            try
            {

                for (int i = 2; i < arrayText.Length; i++)
                {
                    nameFile += arrayText[i] + " ";
                }

                Process process = new Process();
                List<string[]> list = ControlDB.Class.SQL_Select($"select [Путь] from Директории where [ID Категории] = (select [ID Категории] from Категории where Название = '{arrayText[1]}')");

                process.StartInfo.FileName = list[0][0] + $"\\{nameFile.Substring(0, nameFile.Length - 1)}.jpg";
                process.Start();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"{arrayText[1]} с таким названием не существует: " + ex.Message);
            }
        }
   

        #endregion

        #region Кнопки бокового меню и музыки
        private void Border_MouseDown(object sender, MouseButtonEventArgs e) { }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string idSong = ControlDB.Class.SQLSelectOneItem($"select [ID Музыки] from [Музыка] where  [Название] = '{AllMusicList.SelectedItem}'");
                string idPlaylist = ControlDB.Class.SQLSelectOneItem($"select [ID Плейлиста] from [Плейлисты] where  [Название] = '{AllPlaylistList.SelectedItem}'");


                ControlDB.Class.SQL_Update_Insert($"insert into [МузыкаСвязьПлейлисты] ([ID Плейлиста] ,[ID Музыки])  values ('{idPlaylist}','{idSong}')");
                VoicуThisText("Песня успешно добавлена в плейлист");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SetPlaylist();
        }

        private void CreateNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControlDB.Class.SQL_Update_Insert($"insert into [Плейлисты] ([Название] ,[Дата добавления])  values ('{txtNameForNewPlaylist.Text}', GETDATE())");
                VoicуThisText("Новый плейлист успешно создан");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void SetPlaylist()
        {
            AllPlaylistList.ItemsSource = ControlDB.Class.SQL_SelectList($"select [Название] from [Плейлисты]");
            AllMusicList.ItemsSource = ControlDB.Class.SQL_SelectList($"select [Название] from [Музыка]");
        }


        private void SelectItemsListBox()
        {
            List<string[]> list = ControlDB.Class.SQL_Select($"select [Путь] from Директории where [ID Категории] = (select [ID Категории] from Категории where Название = 'аудио')");
            CreateGrammar.directoryInfoFolder = new DirectoryInfo(list[0][0]);

            List<string> pathListBox = new List<string>();

            //Создаём грамматику со всеми названиями файлов в выбранной папке
            foreach (FileInfo _file in CreateGrammar.directoryInfoFolder.GetFiles())
            {
                pathListBox.Add(_file.Name); //Тут все треки что есть в файловой системе
            }
            string[] array1 = CreateGrammar.MusicFilePath.ToArray();
            string[] array2 = pathListBox.ToArray();
            var arr = array2.Except(array1);

            listNewMusic.Items.Clear();

            foreach (string item in arr)
            {
                listNewMusic.Items.Add(item);
            }
        }
        private void ButtonPlaylist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GridPlaylist.Visibility = Visibility.Visible;
            GridMusic.Visibility = Visibility.Hidden;
            GridImage.Visibility = Visibility.Hidden;
            GridAlert.Visibility = Visibility.Hidden;
            GridSetting.Visibility = Visibility.Hidden;
            GridLink.Visibility = Visibility.Hidden;
            SetPlaylist();
        }
        private void ButtonConnection_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GridPlaylist.Visibility = Visibility.Hidden;
            GridMusic.Visibility = Visibility.Hidden;
            GridImage.Visibility = Visibility.Hidden;
            GridAlert.Visibility = Visibility.Hidden;
            GridSetting.Visibility = Visibility.Hidden;
            GridLink.Visibility = Visibility.Visible;

        }
        private void ButtonMusic_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectItemsListBox();
            List<string> list = ControlDB.Class.SQL_SelectList($"select [Название] from [Плейлисты]");
            listFullPlaylist.ItemsSource = list;
            GridPlaylist.Visibility = Visibility.Hidden;
            GridMusic.Visibility = Visibility.Visible;
            GridImage.Visibility = Visibility.Hidden;
            GridAlert.Visibility = Visibility.Hidden;
            GridSetting.Visibility = Visibility.Hidden;
            GridLink.Visibility = Visibility.Hidden;
        }
        private void ButtonImage_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GridPlaylist.Visibility = Visibility.Hidden;
            GridMusic.Visibility = Visibility.Hidden;
            GridImage.Visibility = Visibility.Visible;
            GridAlert.Visibility = Visibility.Hidden;
            GridSetting.Visibility = Visibility.Hidden;
            GridLink.Visibility = Visibility.Hidden;
        }
        private void ButtonSupport_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GridPlaylist.Visibility = Visibility.Hidden;
            GridMusic.Visibility = Visibility.Hidden;
            GridImage.Visibility = Visibility.Hidden;
            GridAlert.Visibility = Visibility.Visible;
            GridSetting.Visibility = Visibility.Hidden;
            GridLink.Visibility = Visibility.Hidden;
        }
        private void ButtonSetting_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            GridPlaylist.Visibility = Visibility.Hidden;
            GridMusic.Visibility = Visibility.Hidden;
            GridImage.Visibility = Visibility.Hidden;
            GridAlert.Visibility = Visibility.Hidden;
            GridSetting.Visibility = Visibility.Visible;
            GridLink.Visibility = Visibility.Hidden;

            ListPathToChange.ItemsSource = ControlDB.Class.SQL_SelectList($"select [Путь] from Директории");
        }


        private void buttonСhangePath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog() { Description = $"Выберете новый путь вместо: \"{ListPathToChange.SelectedItem}\"." })
                {
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        CreateGrammar.directoryInfoFolder = new DirectoryInfo(fbd.SelectedPath);
                        ControlDB.Class.SQL_Update_Insert($"UPDATE Директории SET Директории.Путь" +
                            $" = '{CreateGrammar.directoryInfoFolder.FullName}' where  Директории.Путь = '{ListPathToChange.SelectedItem}'");
                        VoicуThisText("Путь усмешно изменён");
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            ListPathToChange.ItemsSource = ControlDB.Class.SQL_SelectList($"select [Путь] from Директории");
        }

        private void SetNewPath(string type)
        {
            try
            {
                using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog() { Description = $"Выберете новый для: \"{type}\"." })
                {
                    if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        //insert into [Музыка] ([Название],[Автор],[Относительный путь], [Дата добавления]) values ('Название','Автор','Путь', GETDATE())
                        //directoryInfoFolder = new DirectoryInfo(fbd.SelectedPath);

                        switch (type)
                        {
                            case "видео":
                                ControlDB.Class.SQL_Update_Insert($"insert into Директории ([ID Категории] ,[Путь])  values ('1','{fbd.SelectedPath}')");
                                break;
                            case "аудио":
                                ControlDB.Class.SQL_Update_Insert($"insert into Директории ([ID Категории] ,[Путь])  values ('2','{fbd.SelectedPath}')");
                                break;
                            case "фото":
                                ControlDB.Class.SQL_Update_Insert($"insert into Директории ([ID Категории] ,[Путь])  values ('3','{fbd.SelectedPath}')");
                                break;
                            default: break;
                        }
                        VoicуThisText("Вы добавили новый путь для " + type);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }

            ListPathToChange.ItemsSource = ControlDB.Class.SQL_SelectList($"select [Путь] from Директории");
        }

        private void buttonNewPathMusic_Click(object sender, RoutedEventArgs e)
        {
            SetNewPath("аудио");
        }

        private void buttonNewPathPhoto_Click(object sender, RoutedEventArgs e)
        {
            SetNewPath("фото");
        }

        private void buttonNewPathVideo_Click(object sender, RoutedEventArgs e)
        {
            SetNewPath("видео");
        }



        private void ButtonExit_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Работа с музыкой через интерфейс
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(aud.Player != null)
                aud.ChangingTheVolume(Convert.ToInt32(Math.Round(SliderNoise.Value)));
        }

        private void ButtomSetMusicInDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ControlDB.Class.SQL_Update_Insert($"insert into [Музыка] ([Название],[Автор],[Относительный путь],[Дата добавления]) values ('{txtBoxNameMusic.Text}','{txtBoxAuthorMusic.Text}','{txtBoxPathMusic.Text}', GETDATE())");

                int index = Convert.ToInt32(ControlDB.Class.SQLSelectOneItem($"select [ID Музыки] from Музыка where [Относительный путь] = '{txtBoxPathMusic.Text}'"));

                foreach (string[] var in CreateGrammar.PlaylistList)
                {
                    if (var[1] == listFullPlaylist.SelectedItem.ToString()) 
                    {
                        ControlDB.Class.SQL_Update_Insert($"insert into [МузыкаСвязьПлейлисты] ([ID Плейлиста],[ID Музыки]) values ({var[0]},{index})");
                    }
                }           
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка добавления, проверьте корректность вводимых данных \n | Ошибка: " + ex.Message );
            }
        }

        private void ButtomStopMusic_Click(object sender, RoutedEventArgs e)
        {
            if (aud != null)
                aud.StopSong();
            aud = null;
            SliderNoise.IsEnabled = false;
            txtBoxNameMusicForPlaying.Text = "Название";
        }

        private void ButtomPlayMusic_Click(object sender, RoutedEventArgs e)
        {
            if (aud == null)
                aud = new Audio.AudioPlayer(@"F:\КурсоваяТРПО\FolderForAudio\remember_-_Kozhura.mp3");
            else
                aud.StartSong();

            txtBoxNameMusicForPlaying.Text = "Название песни: Кожура";
            SliderNoise.IsEnabled = true;
        }

        private void ButtomPauseMusic_Click(object sender, RoutedEventArgs e)
        {
            if(aud != null)
                aud.PauseSong();
            SliderNoise.IsEnabled = false;
        }

        private void listNewMusic_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            txtBoxPathMusic.Text = listNewMusic.SelectedItem.ToString();
        }

        #endregion

        #region Тех поддержка

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // for .NET Core you need to add UseShellExecute = true
            // see https://learn.microsoft.com/dotnet/api/system.diagnostics.processstartinfo.useshellexecute#property-value
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void buttonSendMessage_Click(object sender, RoutedEventArgs e)
        {
            SmtpClient client = new SmtpClient("smtp.mail.ru"); //465
            // Credentials are necessary if the server requires the client 
            // to authenticate before it will send e-mail on the client's behalf.
            client.EnableSsl = true;

            // set smtp-client with basicAuthentication
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential basicAuthenticationInfo = new
               System.Net.NetworkCredential("voice.assistent.clients@mail.ru", "eTYs8tsAhDyvUfYprwyB"); // пароль для входа в почту oRYrTCa2u(p3
            client.Credentials = basicAuthenticationInfo;

            // add from,to mailaddresses
            MailAddress from = new MailAddress("voice.assistent.clients@mail.ru", "VoiceAssistent Clients");
            MailAddress to = new MailAddress("voice.assistent.clients@mail.ru", "VoiceAssistent Helps");
            MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

            // add ReplyTo
            MailAddress replyTo = new MailAddress(txtFromEmail.Text);
            myMail.ReplyToList.Add(replyTo);

            // set subject and encoding
            myMail.Subject = txtTopicEmail.Text;
            myMail.SubjectEncoding = System.Text.Encoding.UTF8;

            // set body-message and encoding
            myMail.Body = txtMessageEmail.Text;
            myMail.BodyEncoding = System.Text.Encoding.UTF8;
            // text or html | false or true
            myMail.IsBodyHtml = false;

            try
            {
                client.Send(myMail);
                VoicуThisText("Всё окей");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTimeoutTestMessage(): {0}",
                      ex.ToString());
            }
        }



        private void ButtonMap_Click(object sender, RoutedEventArgs e)
        {

            MapAdress.Center = new Location(59.707624, 30.787825);
        }



        #endregion

        private void buttonNewComand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(@"F:\КурсоваяТРПО\WpfVoiceAssistent\WpfVoiceAssistent\Answer.txt");
            }
            catch
            {
                MessageBox.Show("Файл был перемещён, обратитесь к Администратору");
            }
        }

        private void openSettingFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(@"F:\КурсоваяТРПО\WpfVoiceAssistent\WpfVoiceAssistent\DataForDrammar.json");
            }
            catch 
            {
                MessageBox.Show("Файл был перемещён, обратитесь к Администратору");
            }
        }
        private void buttonAllComands_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(@"F:\КурсоваяТРПО\WpfVoiceAssistent\WpfVoiceAssistent\AllComands.docx");
            }
            catch
            {
                MessageBox.Show("Файл был перемещён, обратитесь к Администратору");
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (SettingCheck.IsChecked == true)
            {
                StackPannelSettingLink.Visibility = Visibility.Visible;
            }
            else
            {
                StackPannelSettingLink.Visibility = Visibility.Hidden;
            }
        }

        private void SettingCheck_Click(object sender, RoutedEventArgs e)
        {
            if (SettingCheck.IsChecked == true)
            {
                StackPannelSettingLink.Visibility = Visibility.Visible;
            }
            else
            {
                StackPannelSettingLink.Visibility = Visibility.Hidden;
            }
        }

        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы хотите перейти в режим разработчика. Компания вынуждена вас уведомить, что после нажатия кнопки согласия перехода в этот режим," +
                " мы перестаём нести ответственность за работоспособность приложения. Все действия выполняемые в этом режиме вы делаете на свой страх и риск." +
                "После нажатия клавиши согласия, гарантийный срок работы приложения анулируется." +
                "С уважением, администрация Голосового Помощника  ");
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}

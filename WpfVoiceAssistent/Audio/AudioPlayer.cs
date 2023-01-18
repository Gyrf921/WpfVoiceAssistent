using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;

namespace WpfVoiceAssistent.Audio
{
    class AudioPlayer
    {
        public WMPLib.WindowsMediaPlayer Player;

        public AudioPlayer(string url)
        {
            Player = new WMPLib.WindowsMediaPlayer();
            Player.URL = url;
        }
        public AudioPlayer()
        {
            Player = new WMPLib.WindowsMediaPlayer();
        }

        public void openPlaylist(List<string> _musicPath)
        {
            WMPLib.IWMPPlaylist tempPlaylist = Player.newPlaylist("Новое название", null);
            
            foreach (string path in _musicPath) { 
                string tempMediaUrl = path;
                WMPLib.IWMPMedia tempMedia = Player.newMedia(tempMediaUrl);
                tempPlaylist.appendItem(tempMedia);
            }
            Player.currentPlaylist = tempPlaylist;
            Player.controls.play();
        }

        public void StartSong()
        {
            Player.controls.play();
        }

        public void NextSong()
        {
            Player.controls.next();
        }

        public void StopSong()
        {
            Player.controls.stop();

        }

        public void PauseSong()
        {
            Player.controls.pause();
        }

        /// <summary>
        /// Метод для изменения громкости звука
        /// </summary>
        /// <param name="_valueS">число от 0 до 100 измеряющее громкость (inclusive)</param>
        public void ChangingTheVolume(int _valueS)
        {
            Player.settings.volume = _valueS;
        }

        public void ChangingTheVolume(string _valueS)
        {
            switch (_valueS)
            {
                case "максимум":
                    Player.settings.volume = 90;
                    break;

                case "минимум":
                    Player.settings.volume = 10;
                    break;

                case "середина":
                    Player.settings.volume = 50;
                    break;

                case "тише":
                    if(Player.settings.volume >= 20)
                        Player.settings.volume -= 20;
                    break;

                case "больше":
                    if (Player.settings.volume <= 80)
                        Player.settings.volume += 20;
                    break;
            }
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WpfVoiceAssistent.TheWeather
{
    public class OpenWeather
    {
        private static OpenWeather WeathersForMethod()
        {
            WebRequest request = WebRequest.Create("https://api.openweathermap.org/data/2.5/weather?lat=59.9386&lon=30.3141&appid=464f1770e9e7bf8ccc5894f0de94a113");
            request.Method = "POST";
            request.ContentType = "application/x-www-urlencoded";

            WebResponse response = request.GetResponse();
            string answer = string.Empty;

            using (Stream s = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(s))
                {
                    answer = reader.ReadToEnd();
                }
            }
            OpenWeather _OpenWeath = JsonConvert.DeserializeObject<OpenWeather>(answer);
            return _OpenWeath;
        }

        /// <summary>
        /// Выдаёт строку для озвучивания с информацие о текущей погоде в СПб
        /// </summary>
        /// <param name="_DoWeather">Слово включения (включи, скажи, открой)</param>
        /// <param name="_voiceRequest">Голосовая команда полностью</param>
        /// <param name="localDate">Текущее время</param>
        /// <returns>Строка с информацие о погоде</returns>
        public static string FullWeatherAnswer(string _DoWeather, string _voiceRequest, DateTime localDate)
        {
            OpenWeather _OpenWeath = WeathersForMethod();

            string _forWeather = "погода не определена";
            string _stringForVoice = "Ошибка определения погоды";

            var culture = new CultureInfo("ru-RU");
            localDate.ToShortTimeString();

            DateTime night = DateTime.Parse("22:00");
            DateTime morning = DateTime.Parse("08:00");

            switch (_OpenWeath.weather[0].main.ToString())
            {
                case "Clear":
                    _forWeather = "ясно";
                    break;
                case "Clouds":
                    _forWeather = "облочно";
                    break;
                case "Snow":
                    _forWeather = "идёт снег";
                    break;
                case "Rain":
                    _forWeather = "идёт дождь";
                    break;
                case "Drizzle":
                    _forWeather = "слегка моросит";
                    break;
                case "Thunderstorm":
                    _forWeather = "идёт гроза";
                    break;
                case "Atmosphere":
                    _forWeather = "идёт всё странно";
                    break;
            }
            if (DateTime.Parse(localDate.ToShortTimeString()) > night || DateTime.Parse(localDate.ToShortTimeString()) < morning)
            {
                if (_voiceRequest == $"{_DoWeather} погоду")
                    _stringForVoice = "Сейчас ночь, миссир, лучше отдыхайте";
                else if (_voiceRequest == $"{_DoWeather} температуру")
                    _stringForVoice = "Температура: " + Math.Round(_OpenWeath.main.temp, 0).ToString();
                else if (_voiceRequest == $"{_DoWeather} ветер")
                    _stringForVoice = "Скорость ветра:" + Math.Round(_OpenWeath.wind.speed, 0).ToString() + " метров в секунду";
                else if (_voiceRequest == $"{_DoWeather} полный прогноз")
                    _stringForVoice = "За окном ночь. Температура: " + Math.Round(_OpenWeath.main.temp, 0).ToString() + " .Скорость ветра: " + Math.Round(_OpenWeath.wind.speed, 0).ToString() + " метров в секунду.";
                else
                    _stringForVoice = "Обнаружено новое ключевое слово для запроса в классе PersonGrammar массиве _Weather, обратитесь к разработчику";
            }
            else
            {
                if (_voiceRequest == $"{_DoWeather} погоду")
                    _stringForVoice = "Сейчас " + _forWeather;
                else if (_voiceRequest == $"{_DoWeather} температуру")
                    _stringForVoice = "Температура: " + Math.Round(_OpenWeath.main.temp, 0).ToString();
                else if (_voiceRequest == $"{_DoWeather} ветер")
                    _stringForVoice = "Скорость ветра:" + Math.Round(_OpenWeath.wind.speed, 0).ToString() + " метров в секунду";
                else if (_voiceRequest == $"{_DoWeather} полный прогноз")
                    _stringForVoice = "За окном " + _forWeather + ". Температура: " + Math.Round(_OpenWeath.main.temp, 0).ToString() + " .Скорость ветра: " + Math.Round(_OpenWeath.wind.speed, 0).ToString() + " метров в секунду.";
                else
                    _stringForVoice = "Обнаружено новое ключевое слово для запроса в классе PersonGrammar массиве _Weather, обратитесь к разработчику";
            }

            return _stringForVoice;
        }



        
        public Weather[] weather;
        public Main main;
        public Wind wind;

        [JsonProperty("base")]
        public string _base;

        public Coord coord;
        public int visibility;
        public Clouds clouds;
        public int dt;
        public Sys sys;
        public int timezone;
        public int id;
        public string name;
        public int cod;


        public class Coord
        {
            public double lon { get; set; }
            public double lat { get; set; }
        }
        public class Weather
        {
            public int id { get; set; }
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }
        public class Main
        {
            public double _temp;
            public float _temp_min;
            public float _temp_max;

            public double temp
            {
                get { return _temp; }
                set { _temp = value - 273.15; }
            }//цельсий
            public float feels_like { get; set; }
            public float temp_min
            {
                get { return _temp_min; }
                set { _temp_min = value - 273.15f; }
            }
            public float temp_max
            {
                get { return _temp_max; }
                set { _temp_max = value - 273.15f; }
            }

            public double _pressure;
            public double pressure
            {
                get { return _pressure; }
                set { _pressure = value / 1.3332239; }
            }
            public int humidity { get; set; }


        }
        public class Wind
        {
            public float speed { get; set; }
            public int deg { get; set; }
        }
        public class Clouds
        {
            public double all { get; set; }
        }
        public class Sys
        {
            public double type { get; set; }
            public int id { get; set; }
            public double message { get; set; }
            public string country { get; set; }
            public double sunrise { get; set; }
            public double sunset { get; set; }
        }
    }
}

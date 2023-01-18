//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using Microsoft.Speech.Recognition;
//using System.Text;
//using System.Threading.Tasks;

//namespace WpfVoiceAssistent.Audio
//{
//    class StaticAudioGrammar
//    {
//        static readonly CultureInfo _language = new CultureInfo("ru-RU");

//        public static string[] _ValueSound = new string[] { "максимум", "минимум", "середина", "тише", "больше" };


//        public static Grammar AudioGrammar()
//        {
//            Choices ch_Protocol = new Choices("случайную", "любимую");

//            GrammarBuilder gb_P1 = new GrammarBuilder();
//            gb_P1.Culture = _language;

//            //Первый шаблон для открытия случайной песни
//            gb_P1.Append("включи");
//            gb_P1.Append(ch_Protocol);
//            gb_P1.Append("песню");

//            Choices bothChoices = new Choices(new GrammarBuilder[] { gb_P1 });

//            Grammar g_V = new Grammar(bothChoices); //управляющий Grammar
//            return g_V;
//        }
//        public static Grammar SetValueSoundGrammar()
//        {
//            Choices ch_value = new Choices();//Создание Выборки
//            //Создаём массив из всех значений, которые будем использовать во время приветствия
//            ch_value.Add(_ValueSound); //Записываем массив в "Выборы"

//            GrammarBuilder gb_SetValue = new GrammarBuilder(); //Создаём GrammarBuilder
//            gb_SetValue.Culture = _language;//подключение русского языка
//            gb_SetValue.Append("громкость");
//            gb_SetValue.Append(ch_value); //Заполняем шаблон GrammarBuilder «What is <x> plus <y>?»

//            Grammar g_v = new Grammar(gb_SetValue); //управляющий Grammar

//            return g_v;
//        }
//        public static Grammar AudioStopGrammar()
//        {
//            Choices ch_Protocol = new Choices("Останови", "Продолжи", "Продолжай", "Выключи");
//            Choices ch_P = new Choices("музыку", "играть", "плейлист");
//            GrammarBuilder gb_P1 = new GrammarBuilder();
//            gb_P1.Culture = _language;

//            //Первый шаблон для открытия случайной песни
//            gb_P1.Append(ch_Protocol);
//            gb_P1.Append(ch_P);

//            Choices bothChoices = new Choices(new GrammarBuilder[] { gb_P1 });

//            Grammar g_V = new Grammar(bothChoices); //управляющий Grammar
//            return g_V;
//        }

//        public static Grammar AudioNextGrammar()
//        {
//            Choices ch_Protocol = new Choices("следующая", "некст", "другая", "следующую");
//            Choices ch_P = new Choices("музыку", "песня", "трек");
//            GrammarBuilder gb_P1 = new GrammarBuilder();
//            gb_P1.Culture = _language;

//            //Первый шаблон для открытия случайной песни
//            gb_P1.Append(ch_Protocol);
//            gb_P1.Append(ch_P);

//            Choices bothChoices = new Choices(new GrammarBuilder[] { gb_P1 });

//            Grammar g_V = new Grammar(bothChoices); //управляющий Grammar
//            return g_V;
//        }
//    }
//}

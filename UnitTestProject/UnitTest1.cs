using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WpfVoiceAssistent;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestWeather()
        {
            // arrange
            DateTime timeForTast = DateTime.Parse("03:00");
            string expected = "Сейчас ночь, миссир, лучше отдыхайте";

            var account = WpfVoiceAssistent.TheWeather.OpenWeather.FullWeatherAnswer("Открой", "Открой погоду", timeForTast);

            // assert
            Assert.AreEqual(expected, account);
        }

        [TestMethod]
        public void TestProtocol()
        {
            // arrange
            string nameProtocol = "стандартный";
            string expected = "гугл вконтакте телеграм ";
            var grammar = WpfVoiceAssistent.OpenSomething.CreateGrammar.ProtocolGrammar();

            var account = WpfVoiceAssistent.OpenSomething.OpenApplication.StartProtocol(nameProtocol);

            // assert
            Assert.AreEqual(expected, account);
        }
    }
}



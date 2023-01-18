using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfVoiceAssistent.OpenSomething
{
    internal class WorkWithSystem
    {
        public static void CoiseActionWithSystem(string _act)
        {
            switch (_act)
            {
                case "выключи компьютер":
                    TurnOffPC();
                    break;
                case "перезагрузка":
                    WorkWithSystem.RestartPC();
                    break;
            }
        }

        public static void TurnOffPC()
        {
            Process.Start("shutdown", "/s /t /f 00");
        }
        public static void RestartPC()
        {
            ProcessStartInfo proc = new ProcessStartInfo();
            proc.WindowStyle = ProcessWindowStyle.Hidden;
            proc.FileName = "cmd";
            proc.Arguments = "/C shutdown -f -r";
            Process.Start(proc);
        }
    }
}

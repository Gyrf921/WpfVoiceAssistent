using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfVoiceAssistent.OpenSomething
{
    public class OpenApplication
    {
        public static void StartProgramm(string _namePrograms)
        {
            foreach (string[] _aboutProgram in CreateGrammar.Programm_List)
            {
                if (_aboutProgram[1] == _namePrograms) 
                {
                    if (_aboutProgram[3] != "" || _aboutProgram[3] != " " || _aboutProgram[3] != null)
                        Process.Start(_aboutProgram[2], _aboutProgram[3]);
                    else if (_aboutProgram[3] == "" || _aboutProgram[3] == " " || _aboutProgram[3] == null)
                        Process.Start(_aboutProgram[2]);
                }
            }
        }


        /// <summary>
        /// Запускает протокол с заданным названием (стандартный/рабочий/игровой/полный)
        /// </summary>
        /// <param name="_nameProt"> Название протокола</param>
        /// <returns> Возвращает название всех запущенных программ</returns>
        public static string StartProtocol(string _nameProt)
        {
            string numProtStr = "";
            foreach (string[] _aboutProgram in CreateGrammar.Protocol_List)
            {
                if (_aboutProgram[1] == _nameProt)
                {
                    numProtStr = _aboutProgram[0];
                }
            }

            List<string[]> ProtProgram = ControlDB.Class.SQL_Select($"SELECT [Название],[Путь],[Дополнительная ссылка] FROM [Программы] where [ID Программы] in (select[ID Программы] from [ПрограммаСвязьПротоколы] where [ID Протокола] = '{numProtStr}')") ;
            string name = "";

            foreach (string[] _Programs in ProtProgram)
            {
                name += _Programs[0] + " ";

                if (_Programs[2] != "" || _Programs[2] != " " || _Programs[2] != null)
                    Process.Start(_Programs[1], _Programs[2]);
                else if (_Programs[2] == "" || _Programs[2] == " " || _Programs[2] == null)
                    Process.Start(_Programs[1]);
            }
            return name;

        }
    }
}

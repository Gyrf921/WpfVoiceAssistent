using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfVoiceAssistent
{
    class ControlDB
    {
        private static volatile ControlDB myClass;
        private static object syncObj = new object();

        private static readonly string connectionString = @"Data Source= DESKTOP-PMCGNVB\MSSQLSERVERLAST2; Initial catalog=429192-28 Щин Курс; Integrated Security=True";
        private SqlConnection MyConnection = new SqlConnection(connectionString);

        public static ControlDB Class
        {
            get
            {
                if (myClass is null)
                {
                    lock (syncObj)
                    {
                        if (myClass is null)
                            myClass = new ControlDB();
                    }
                }
                return myClass;
            }
        }


        /// <summary>
        /// Метод поиска данных из БД
        /// </summary>
        /// <param name="text">текст запроса, параметры вводить через @p1 и цифра номер</param>
        /// <param name="list">параметры через запятую</param>
        /// <returns>Возвращает список List string[]</returns>
        public List<string[]> SQL_Select(string text, params object[] list)
        {
            //SqlConnection MyConnection = new SqlConnection(connectionString);
            SqlCommand cmd1 = new SqlCommand(text, MyConnection);

            for (int i = 0; i < list.Length; i++)
            {
                cmd1.Parameters.Add(new SqlParameter($"@p{i}", list[i]));
            }
            SqlDataReader Reader1;

            MyConnection.Open();

            Reader1 = cmd1.ExecuteReader();

            List<string[]> arr = new List<string[]>();

            while (Reader1.Read())
            {
                string[] sw = new string[Reader1.FieldCount];

                for (int i = 0; i < sw.Length; i++)
                {
                    sw[i] = Reader1[i].ToString();
                }

                arr.Add(sw);
            }
            Reader1.Close();
            MyConnection.Close();

            return arr;
        }

        public List<string> SQL_SelectList(string text, params object[] list)
        {
            //SqlConnection MyConnection = new SqlConnection(connectionString);
            SqlCommand cmd1 = new SqlCommand(text, MyConnection);

            for (int i = 0; i < list.Length; i++)
            {
                cmd1.Parameters.Add(new SqlParameter($"@p{i}", list[i]));
            }
            SqlDataReader Reader1;

            MyConnection.Open();

            Reader1 = cmd1.ExecuteReader();

            List<string> arr = new List<string>();

            while (Reader1.Read())
            {
                string[] sw = new string[Reader1.FieldCount];

                for (int i = 0; i < sw.Length; i++)
                {
                    sw[i] = Reader1[i].ToString();
                    arr.Add(sw[i]);
                }


            }
            Reader1.Close();
            MyConnection.Close();

            return arr;
        }

        public string SQLSelectOneItem(string text, params object[] list)
        {
            SqlCommand cmd1 = new SqlCommand(text, MyConnection);

            for (int i = 0; i < list.Length; i++)
            {
                cmd1.Parameters.Add(new SqlParameter($"@p{i}", list[i]));
            }
            SqlDataReader Reader1;

            MyConnection.Open();

            Reader1 = cmd1.ExecuteReader();

            List<string[]> arr = new List<string[]>();

            while (Reader1.Read())
            {
                string[] sw = new string[Reader1.FieldCount];

                for (int i = 0; i < sw.Length; i++)
                {
                    sw[i] = Reader1[i].ToString();
                }

                arr.Add(sw);
            }

            Reader1.Close();
            MyConnection.Close();

            return arr[0][0];
        }


        /// <summary>
        /// Метод обновление и вставки данных в БД
        /// </summary>
        /// <param name="text">текст запроса, параметры вводить через @p1 и цифра номер</param>
        /// <param name="list">параметры через запитую</param>
        public void SQL_Update_Insert(string text, params object[] list)
        {

            //insert into [Музыка] ([Название],[Автор],[Относительный путь], [Дата добавления]) values ('Название','Автор','Путь', GETDATE())

            //delete[Музыка] where[ID Музыки] = 1002

            SqlCommand cmd1 = new SqlCommand(text, MyConnection);

            for (int i = 0; i < list.Length; i++)
            {
                cmd1.Parameters.Add(new SqlParameter($"@p{i}", list[i]));
            }

            MyConnection.Open();

            cmd1.ExecuteNonQuery();

            MyConnection.Close();

        }

        public void FillList(ListBox listBox, string selectText)
        {
            try
            {
                listBox.ItemsSource = Class.SQL_SelectList(selectText);   
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}

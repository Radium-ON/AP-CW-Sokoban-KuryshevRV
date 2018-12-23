using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;
using static AP_CW_Sokoban_KuryshevRV.App;

namespace AP_CW_Sokoban_KuryshevRV
{
    /// <summary>
    /// Логика взаимодействия для StartMenuWindow.xaml
    /// </summary>
    [DataContract]
    public partial class StartMenuWindow : Window
    {
        [DataMember]
        List<User> users = new List<User>();
        DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<User>));

        public StartMenuWindow()
        {
            InitializeComponent();
            try
            {
                using (FileStream fs = new FileStream("users_list.json", FileMode.OpenOrCreate))
                {
                    users = (List<User>)jsonFormatter.ReadObject(fs);
                }
            }
            catch { }
        }

        private void buttonStartGame_Click(object sender, RoutedEventArgs e)
        {
            LabirintWindow labirint = new LabirintWindow();
            labirint.Owner = this;
            labirint.OpenLevel(1, mStartFilename);
            Visibility = Visibility.Collapsed;
            if (labirint.ShowDialog() == true)
            {
                users.Add(labirint.user);
            };
            Visibility = Visibility.Visible;
        }

        private void btLoad_Click(object sender, RoutedEventArgs e)
        {
            if (users.Count != 0)
            {//десериализация коллекции обращение к игроку загрузка его уровня
                LabirintWindow labirint = new LabirintWindow(users);
                labirint.Owner = this;
                var level = users.Last().datas.Last().CompletedLVL;
                if (level != 0)
                {
                    //открытие сохранённого уровня
                    labirint.OpenSavedLevel(level, mSaveFileName);

                }
                else
                {
                    labirint.OpenSavedLevel(1, mSaveFileName);
                }
                Visibility = Visibility.Collapsed;
                if (labirint.ShowDialog() == true)
                {
                    users.Add(labirint.user);
                };
                Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Нет сохранённых уровней!");
            }
        }

        private void btHighscore_Click(object sender, RoutedEventArgs e)
        {
            ScoresWindow scores = new ScoresWindow(users);
            scores.ShowDialog();
        }

        private void startWindow_Closing(object sender, CancelEventArgs e)
        {
            JsonSerialize("users_list.json", users, jsonFormatter);
        }

        private void btHelp_Click(object sender, RoutedEventArgs e)
        {
            var help = Properties.Resources.HelpString;
            MessageBox.Show(help, "Правила игры", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}

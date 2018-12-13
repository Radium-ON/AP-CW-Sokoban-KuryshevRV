using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AP_CW_Sokoban_KuryshevRV
{
    /// <summary>
    /// Логика взаимодействия для StartMenuWindow.xaml
    /// </summary>
    public partial class StartMenuWindow : Window
    {
        public StartMenuWindow()
        {
            InitializeComponent();
        }

        private void buttonStartGame_Click(object sender, RoutedEventArgs e)
        {
            LabirintWindow labirint = new LabirintWindow();
            labirint.OpenLevel(1);
            labirint.ShowDialog();
        }
    }
}

﻿using System;
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
    /// Логика взаимодействия для ScoresWindow.xaml
    /// </summary>
    public partial class ScoresWindow : Window
    {
        public ScoresWindow(List<User> users)
        {
            InitializeComponent();
            listRecords.ItemsSource = users;
        }        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

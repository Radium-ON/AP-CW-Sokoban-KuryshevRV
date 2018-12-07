using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AP_CW_Sokoban_KuryshevRV
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public struct MapPlace
        {
            public int X;
            public int Y;

            public MapPlace(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }
        public enum Cell
        {
            none,//пусто
            wall, //стена
            box,//ящик
            done,//ящик на месте
            here,//сюда поставить ящик
            user//грузчик
        };
    }
}

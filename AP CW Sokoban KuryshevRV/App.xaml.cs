using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace AP_CW_Sokoban_KuryshevRV
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string mStartFilename = "LevelList.txt";
        public const string mSaveFileName = "SaveList.txt";

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

        public static void JsonSerialize(string fileName, object obj, DataContractJsonSerializer jsonFormatter)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                jsonFormatter.WriteObject(fs, obj);
            }
        }
    }
}

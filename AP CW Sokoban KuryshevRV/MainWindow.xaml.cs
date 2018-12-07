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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static AP_CW_Sokoban_KuryshevRV.App;

namespace AP_CW_Sokoban_KuryshevRV
{
    //делегат для обращения игрового метода к ячейкам
    public delegate void delegateShowItem(MapPlace place, Cell item);
    //делегат доступа игры к полям статистики
    public delegate void delegateShowStats(int placed, int totals);

    public partial class MainWindow : Window
    {
        int currentLevelNumber;
        int mLastLevel;//последний непройденный уровень
        int labirintWidth, labirintHeight;
        PictureBox[,] pictureBoxes;

        Game game;

        string path_target = "";//куда пойдет чел по тикам таймера

        public MainWindow()
        {
            InitializeComponent();
            mLastLevel = 1;//доступный 1 уровень в начале игры
            game = new Game(ShowItem, ShowStats);

            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        public void OpenLevel(int levelN)
        {
            if (levelN > mLastLevel)
                return;//нельзя загрузить, если уровень не пройден
            currentLevelNumber = levelN;

            if (!game.InitializeLevel(levelN, out labirintWidth, out labirintHeight))
            {
                MessageBox.Show("Вы прошли все уровни! Так держать!");
                DialogResult = DialogResult.OK;
                return;
            }
            ;
            InitPictureBoxes(labirintWidth, labirintHeight);
            game.ShowLevel();
        }

        //делегированные методы
        public void ShowItem(MapPlace place, Cell item)
        {
            pictureBoxes[place.X, place.Y].Image = CellToPicture(item);
        }

        private Image CellToPicture(Cell item)
        {
            switch (item)
            {
                case Cell.none: return Properties.Resources.none;
                case Cell.wall:
 return Properties.Resources.wall;
                case Cell.box: return Properties.Resources.dropbox;
                case Cell.here: return Properties.Resources.checkbox_blank_outline;
                case Cell.done: return Properties.Resources.check_circle_outline;
                case Cell.user: return Properties.Resources.account_black;
                default: return Properties.Resources.none;
            }
        }

        public void ShowStats(int placed, int totals)
        {
            toolStat.Text = placed.ToString() + " из " + totals.ToString();
            levelNumberLabel.Text = currentLevelNumber.ToString();//показать номер уровня
            if (placed == totals)
            {
                //если прошли уровень:вернуться назад и вперед до непройденного
                if (currentLevelNumber == mLastLevel)
                    mLastLevel = currentLevelNumber + 1;

                MessageBox.Show("Уровень пройден!");
            }
        }

        public void InitPictureBoxes(int width, int height)
        {
            pictureBoxes = new PictureBox[width, height];
            //размеры бокса пропорциональны размерам панели
            int pictureBoxHeight = panel1.Height / height;
            int pictureBoxWidth = panel1.Width / width;
            //боксы квадратные = должны быть равны ширина и длина
            if (pictureBoxWidth < pictureBoxHeight)
            {
                pictureBoxHeight = pictureBoxWidth;
            }
            else
            {
                pictureBoxWidth = pictureBoxHeight;
            }
            panel1.Children.Clear();
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    PictureBox pic = new PictureBox();
                    pic.BackColor = Color.White;
                    pic.BorderStyle = BorderStyle.FixedSingle;
                    pic.Location = new Point(x * (pictureBoxWidth - 1), y * (pictureBoxHeight - 1));
                    pic.Size = new Size(pictureBoxWidth, pictureBoxHeight);
                    pic.SizeMode = PictureBoxSizeMode.StretchImage;
                    pic.MouseClick += new MouseEventHandler(pictureBox_MouseClick);
                    pic.MouseDoubleClick += new MouseEventHandler(picturebox_MouseDoubleClick);
                    //запись координат в тэг ячейки
                    pic.Tag = new MapPlace(x, y);
                    panel1.Children.Add(pic);
                    pictureBoxes[x, y] = pic;
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (path_target == "")
                return;
            switch (path_target[0])
            {
                case '4':
                    game.UserStep(-1, 0);
                    break;
                case '6':
                    game.UserStep(1, 0);
                    break;
                case '2':
                    game.UserStep(0, 1);
                    break;
                case '8':
                    game.UserStep(0, -1);
                    break;
            }
            if (path_target != "")
                path_target = path_target.Substring(1);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (pictureBoxes != null)
            {
                int pictureBoxHeight = panel1.Height / labirintHeight;
                int pictureBoxWidth = panel1.Width / labirintWidth;
                if (pictureBoxWidth < pictureBoxHeight)
                {
                    pictureBoxHeight = pictureBoxWidth;
                }
                else
                {
                    pictureBoxWidth = pictureBoxHeight;
                }
                for (int x = 0; x < labirintWidth; x++)
                {
                    for (int y = 0; y < labirintHeight; y++)
                    {
                        pictureBoxes[x, y].Location = new Point(x * (pictureBoxWidth - 1), y * (pictureBoxHeight - 1));
                        pictureBoxes[x, y].Size = new Size(pictureBoxWidth, pictureBoxHeight);
                    }
                }
            }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MapPlace place = (MapPlace)((PictureBox)sender).Tag;
            string my_path = "";
            if (box.X == -1)
            {//если не выбран ящик - просто идём
                my_path = game.SolveMouse(place);
            }
            else //решение для установки ящика на место
            {
                my_path = game.SolveBox(box, place);
                box.X = -1;
                box.Y = -1;
            }
            path_target = my_path;//передали путь куда двигаться
        }

        MapPlace box = new MapPlace(-1, -1);//не указан ящик

        private void Border_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            box = (MapPlace)((PictureBox)sender).Tag;
        }

        private void buttonPreviousLevel_Click(object sender, RoutedEventArgs e)
        {
            if (currentLevelNumber > 1)
            {
                OpenLevel(currentLevelNumber - 1);
            }
        }

        private void buttonNextLevel_Click(object sender, RoutedEventArgs e)
        {
            OpenLevel(currentLevelNumber + 1);
        }

        private void buttonRestartLevel_Click(object sender, RoutedEventArgs e)
        {
            RestartLevel();
        }

        private static void RestartLevel()
        {
            game.InitializeLevel(currentLevelNumber, out labirintWidth, out labirintHeight);
            game.ShowLevel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            labirintWidth = pictureBoxes.GetLength(0);
            labirintHeight = pictureBoxes.GetLength(1);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (path_target != "")//сброс пути если нажали клавишу. остановка нпс
            {
                path_target = "";
                return;
            }
            switch (e.Key)
            {
                case Key.Escape:
                    RestartLevel();
                    break;
                case Key.Left:
                    game.UserStep(-1, 0);
                    break;
                case Key.Up:
                    game.UserStep(0, -1);
                    break;
                case Key.Right:
                    game.UserStep(1, 0);
                    break;
                case Key.Down:
                    game.UserStep(0, 1);
                    break;
            }
        }
    }
}

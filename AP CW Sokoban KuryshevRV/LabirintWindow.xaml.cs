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

    public partial class LabirintWindow : Window
    {
        int currentLevelNumber;
        int mLastLevel;//последний непройденный уровень
        int labirintWidth, labirintHeight;
        Image[,] pictureBoxes;//PictureBox->image

        Game game;
        string path_target = "";//куда пойдет чел по тикам таймера

        public LabirintWindow()
        {
            InitializeComponent();
            mLastLevel = 12;//доступный 1 уровень в начале игры
            game = new Game(ShowItem, ShowStats);

            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(100);
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
                DialogResult = true;
                return;
            }
            ;
            InitPictureBoxes(labirintWidth, labirintHeight);
            game.ShowLevel();
        }

        //делегированные методы
        public void ShowItem(MapPlace place, Cell item)
        {
            pictureBoxes[place.X, place.Y].Source = new BitmapImage(CellToPicture(item));
        }

        private Uri CellToPicture(Cell item)//Image->URI ??
        {
            switch (item)
            {
                case Cell.none: return new Uri(@"pack://siteoforigin:,,,/Resources/none.png");
                case Cell.wall: return new Uri(@"pack://siteoforigin:,,,/Resources/wall.png");
                case Cell.box: return new Uri(@"pack://siteoforigin:,,,/Resources/dropbox.png");
                case Cell.here: return new Uri(@"pack://siteoforigin:,,,/Resources/checkbox-blank-outline.png");
                case Cell.done: return new Uri(@"pack://siteoforigin:,,,/Resources/check-circle-outline.png");
                case Cell.user: return new Uri(@"pack://siteoforigin:,,,/Resources/user-black.png");
                default: return new Uri(@"pack://siteoforigin:,,,/Resources/none.png");
            }
        }

        public void ShowStats(int placed, int totals)
        {
            labelStat.Content = placed.ToString() + " из " + totals.ToString();
            levelNumberLevel.Content = currentLevelNumber.ToString();//показать номер уровня
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
            pictureBoxes = new Image[width, height];
            //размеры бокса пропорциональны размерам панели
            //int canvasHeight = Convert.ToInt32(gridGame.Height);
            //int canvasWidth = Convert.ToInt32(gridGame.Width);
            //int pictureBoxHeight = (int)gridGame.ActualHeight / height;//Canvas.Height (double)->int
            //int pictureBoxWidth = (int) gridGame.ActualWidth/ width;
            //боксы квадратные = должны быть равны ширина и длина
            //if (pictureBoxWidth < pictureBoxHeight)
            //{
            //    pictureBoxHeight = pictureBoxWidth;
            //}
            //else
            //{
            //    pictureBoxWidth = pictureBoxHeight;
            //}
            gridGame.Children.Clear();
            gridGame.RowDefinitions.Clear();
            gridGame.ColumnDefinitions.Clear();
            for (int x = 0; x < width; x++)
            {
                gridGame.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                for (int y = 0; y < height; y++)
                {
                    gridGame.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    Image pic = new Image()
                    {
                        Stretch = Stretch.UniformToFill,
                        //запись координат в тэг ячейки
                        Tag = new MapPlace(x, y),
                    };
                    pic.MouseLeftButtonDown += new MouseButtonEventHandler(pictureBox_MouseLeftButtonDown);
                    pic.MouseRightButtonDown += new MouseButtonEventHandler(pictureBox_MouseRightButtonDown);
                    Grid.SetRow(pic, y);
                    Grid.SetColumn(pic, x);
                    gridGame.Children.Add(pic);
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

        private void pictureBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MapPlace place = (MapPlace)((Image)sender).Tag;
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

        private void pictureBox_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            box = (MapPlace)((Image)sender).Tag;
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

        private void RestartLevel()
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

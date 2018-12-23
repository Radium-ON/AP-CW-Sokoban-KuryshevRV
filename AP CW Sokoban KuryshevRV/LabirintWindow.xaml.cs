using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
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
using System.ComponentModel;
using System.Windows.Threading;
using System.Xml.Serialization;
using static AP_CW_Sokoban_KuryshevRV.App;
using System.Runtime.CompilerServices;

namespace AP_CW_Sokoban_KuryshevRV
{
    //делегат для обращения игрового метода к ячейкам
    public delegate void delegateShowItem(MapPlace place, Cell item);
    //делегат доступа игры к полям статистики
    public delegate void delegateShowStats(int placed, int totals, int steps);

    public partial class LabirintWindow : Window
    {
        int currentLevelNumber;
        int mLastLevel;//последний непройденный уровень
        int labirintWidth, labirintHeight;
        Image[,] pictureBoxes;

        public User user;
        //string mUserName;
        int mSteps;

        Game game;
        string path_target = "";//куда пойдет чел по тикам таймера

        //SharpSerializer formatter;

        public LabirintWindow()
        {
            InitializeComponent();
            mLastLevel = 1;//доступный 1 уровень в начале игры
            game = new Game(ShowItem, ShowStats);
            labirintWindow.DataContext = game;
            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(100),
            };
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
            user = new User();
        }

        public LabirintWindow(List<User> users):this()
        {
            game.StepsNum = users.Last().datas.Last().StepsNum;
            game.AttemptNum = users.Last().datas.Last().AttemptedCount;
        }

        public void OpenLevel(int levelN, string origFile)
        {
            if (levelN > mLastLevel)
                return;//нельзя загрузить, если уровень не пройден
            currentLevelNumber = levelN;
            OpenGame(levelN,mStartFilename);
        }

        public void OpenSavedLevel(int levelN, string save)
        {
            currentLevelNumber = mLastLevel = levelN;
            OpenGame(levelN,mSaveFileName);
        }

        private void OpenGame(int levelN, string file)
        {
            if (!game.InitializeLevel(levelN, out labirintWidth, out labirintHeight, file))
            {
                MessageBox.Show("Вы прошли все уровни! Так держать!");
                DialogResult = true;
                return;
            };
            InitPictureBoxes(labirintWidth, labirintHeight);
            game.ShowLevel();
        }

        //делегированные методы: назначение ресурса на ячейку ImageBoxes
        public void ShowItem(MapPlace place, Cell item)
        {
            pictureBoxes[place.X, place.Y].Source = new BitmapImage(CellToPicture(item));
        }

        private Uri CellToPicture(Cell item)
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

        public void ShowStats(int placed, int totals, int steps)
        {
            labelStat.Content = placed + " из " + totals;
            levelNumberLevel.Content = currentLevelNumber;//показать номер уровня
            lbStepsNum.Content = mSteps = steps;
            if (placed == totals)
            {
                //если прошли уровень:вернуться назад и вперед до непройденного
                if (currentLevelNumber == mLastLevel)
                    mLastLevel = currentLevelNumber + 1;

                var msg = "Уровень пройден!\nПродолжить?";
                MessageBoxResult result = MessageBox.Show(msg, "Следующий уровень", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    buttonNextLevel_Click(this, null);
                }
            }
        }

        public void InitPictureBoxes(int width, int height)
        {
            pictureBoxes = new Image[width, height];
            //int pictureBoxHeight = (int)gridGame.ActualHeight / height;         
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
                    pic.MouseLeftButtonDown += new MouseButtonEventHandler(ImageCell_MouseLeftButtonDown);
                    pic.MouseRightButtonDown += new MouseButtonEventHandler(ImageCell_MouseRightButtonDown);
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

        private void ImageCell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void ImageCell_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            box = (MapPlace)((Image)sender).Tag;
        }

        private void buttonPreviousLevel_Click(object sender, RoutedEventArgs e)
        {
            if (currentLevelNumber > 1)
            {
                OpenLevel(currentLevelNumber - 1, mStartFilename);
            }
        }

        private void buttonNextLevel_Click(object sender, RoutedEventArgs e)
        {
            user.datas.Add(new User.UserData(game.AttemptNum, mSteps, currentLevelNumber));
            game.AttemptNum = 1;
            game.StepsNum = 0;
            OpenLevel(currentLevelNumber + 1, mStartFilename);
        }

        private void buttonRestartLevel_Click(object sender, RoutedEventArgs e)
        {
            RestartLevel();
        }

        private void RestartLevel()
        {
            game.AttemptNum++;
            game.StepsNum = 0;
            game.InitializeLevel(currentLevelNumber, out labirintWidth, out labirintHeight, mStartFilename);
            game.ShowLevel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            labirintWidth = pictureBoxes.GetLength(0);
            labirintHeight = pictureBoxes.GetLength(1);

        }

        private void labirintWindow_Closing(object sender, CancelEventArgs e)
        {
            string msg = "Несохранённые данные. Сохранить и закрыть?";
            MessageBoxResult result = MessageBox.Show(msg, "Завершение игры", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Cancel)
            {
                // Если пользователь не желает закрыть - отмена
                e.Cancel = true;
            }
            if (result == MessageBoxResult.Yes)
            {
                // Закрыть игру и сохраниться
                UserNameDialog nameDialog = new UserNameDialog();
                nameDialog.Owner = this;
                if (nameDialog.ShowDialog() == true)
                {
                    if (user.datas.Count==0)
                    {
                        user.datas.Add(new User.UserData(game.AttemptNum, mSteps, currentLevelNumber));
                    }
                    user.UserName = nameDialog.tbNameInput.Text;
                    game.TopToMapReverse();
                    LevelSetup.SaveLevel(currentLevelNumber, game.TopReverse, mSaveFileName);
                    DialogResult = true;
                }
            }
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

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using static AP_CW_Sokoban_KuryshevRV.App;

namespace AP_CW_Sokoban_KuryshevRV
{
    [Serializable]
    public class Game:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        MapPlace userPlace;//координата игрока на поле

        int mapWidth, mapHeight;

        Cell[,] ItemsOnTop;
        Cell[,] Map;
        public Cell[,] TopReverse;

        //строка статистики
        private int _placed;
        private int _totals;

        delegateShowItem ShowItem;
        delegateShowStats ShowStats;
        private int _attemptNum = 1;

        public int StepsNum { get; set; } = 0;
        public int AttemptNum
        {
            get{ return _attemptNum; }
            set
            {
                _attemptNum = value;
                OnPropertyChanged("AttemptNum");
            }
        }
        public Game() { }

        public Game(delegateShowItem showItem, delegateShowStats showStats)
        {
            ShowItem = showItem;
            ShowStats = showStats;
        }

        public bool InitializeLevel(int levelN, out int width, out int height, string file)
        {
            Map = LevelSetup.LoadLevel(levelN, file);
            if (Map == null)
            {
                width = 0;
                height = 0;
                return false;
            }
            width = mapWidth = Map.GetLength(0);
            height = mapHeight = Map.GetLength(1);
            ItemsOnTop = new Cell[mapWidth, mapHeight];//изменяемое поле поверх
            TopReverse = new Cell[mapWidth, mapHeight];
            for (int i = 0; i < mapWidth; i++)
                for (int j = 0; j < mapHeight; j++)
                {//удаляем ящики и нпс из матрицы уровня - добавляем объекты в активную матрицу
                    switch (Map[i, j])
                    {
                        case Cell.none:
                            TopReverse[i, j] = Cell.none;
                            break;

                        case Cell.wall:
                            TopReverse[i, j] = Cell.wall;
                            break;

                        case Cell.box:
                            ItemsOnTop[i, j] = Cell.box;
                            Map[i, j] = Cell.none;
                            break;

                        case Cell.here:
                            TopReverse[i, j] = Cell.here;
                            ItemsOnTop[i, j] = Cell.none;
                            break;

                        case Cell.done:
                            ItemsOnTop[i, j] = Cell.box;
                            Map[i, j] = Cell.here;
                            TopReverse[i, j] = Cell.here;
                            break;

                        case Cell.user:
                            userPlace = new MapPlace(i, j);
                            Map[i, j] = Cell.none;
                            TopReverse[i, j] = Cell.none;
                            ItemsOnTop[i, j] = Cell.user;
                            break;
                    }
                }
            return true;
        }

        public void ShowLevel()
        {
            _placed = _totals = 0;//чистая статистика перед загрузкой уровня
            for (int i = 0; i < mapWidth; i++)
                for (int j = 0; j < mapHeight; j++)
                {
                    ShowItemMapWithTop(i, j);
                    if (Map[i, j] == Cell.here)
                    {
                        _totals++;
                        if (ItemsOnTop[i, j] == Cell.box)
                        {
                            _placed++;
                        }
                    }
                }
            ShowStats(_placed, _totals, StepsNum);
        }

        private void ShowItemMapWithTop(int i, int j)
        {
            if (ItemsOnTop[i, j] == Cell.none)
            {//назначает ресурс на ячейку
                ShowItem(new MapPlace(i, j), Map[i, j]);
            }
            else
            {
                if (ItemsOnTop[i, j] == Cell.box && Map[i, j] == Cell.here)
                {
                    ShowItem(new MapPlace(i, j), Cell.done);
                }
                else
                {
                    ShowItem(new MapPlace(i, j), ItemsOnTop[i, j]);
                }
            }
        }

        public void TopToMapReverse()
        {
            for (int i = 0; i < mapWidth; i++)
                for (int j = 0; j < mapHeight; j++)
                {
                    switch (ItemsOnTop[i, j])
                    {
                        case Cell.none:
                            break;
                        case Cell.box:
                            if (TopReverse[i, j] == Cell.here)
                            {
                                TopReverse[i, j] = Cell.done;
                            }
                            else
                            {
                                TopReverse[i, j] = Cell.box;
                            }
                            break;
                        case Cell.user:
                            if (TopReverse[i, j] == Cell.here)
                                userPlace = new MapPlace(i, j);
                            TopReverse[i, j] = Cell.user;
                            break;
                    }
                }
        }

        //делаем шаги юзера
        internal void UserStep(int sx, int sy)
        {
            MapPlace place = new MapPlace(userPlace.X + sx, userPlace.Y + sy);
            if (!UserInRange(place))
                return;
            if (ItemsOnTop[place.X, place.Y] == Cell.none)
            {
                ItemsOnTop[userPlace.X, userPlace.Y] = Cell.none; ShowItemMapWithTop(userPlace.X, userPlace.Y);
                ItemsOnTop[place.X, place.Y] = Cell.user; ShowItemMapWithTop(place.X, place.Y);
                userPlace = place;
                StepsNum++;//счёт шагов
            }
            //шагаем на ящик
            if (ItemsOnTop[place.X, place.Y] == Cell.box)
            {
                //координаты объекта за ящиком
                MapPlace placeAhead = new MapPlace(place.X + sx, place.Y + sy);
                if (!UserInRange(placeAhead))
                    return;//наступить на ящик нельзя
                if (ItemsOnTop[placeAhead.X, placeAhead.Y] != Cell.none)
                    return;//ящик можно двигать только на пустую клетку

                //двигаем ящик на цель:счётчик+ смещаем с цели:счётчик-
                if (Map[place.X, place.Y] == Cell.here) _placed--;
                if (Map[placeAhead.X, placeAhead.Y] == Cell.here) _placed++;

                ItemsOnTop[userPlace.X, userPlace.Y] = Cell.none; ShowItemMapWithTop(userPlace.X, userPlace.Y);
                ItemsOnTop[place.X, place.Y] = Cell.user; ShowItemMapWithTop(place.X, place.Y);
                ItemsOnTop[placeAhead.X, placeAhead.Y] = Cell.box; ShowItemMapWithTop(placeAhead.X, placeAhead.Y);
                userPlace = place;
                StepsNum++;
            }
            ShowStats(_placed, _totals, StepsNum);//обновили статус ящиков
        }

        //перемещаем ящик на нужное место
        internal string SolveBox(MapPlace box, MapPlace target)
        {
            if (ItemsOnTop[box.X, box.Y] != Cell.box)
                return "";
            if (!UserInRange(target))
                return "";
            BoxSolver solver = new BoxSolver(Map, ItemsOnTop);
            return solver.MoveBox(userPlace, box, target);
        }

        public string SolveMouse(MapPlace target)
        {
            if (!UserInRange(target))
                return "";
            MouseSolver solver = new MouseSolver(Map, ItemsOnTop);
            return solver.MoveMouse(userPlace, target);
        }

        //проверяем, что юзер вышел за лабиринт
        internal bool UserInRange(MapPlace userPlace)
        {
            if (userPlace.X < 0 || userPlace.X >= mapWidth)
            {
                return false;
            }
            if (userPlace.Y < 0 || userPlace.Y >= mapHeight)
            {
                return false;
            }
            if (Map[userPlace.X, userPlace.Y] == Cell.none)
            {
                return true;
            }
            if (Map[userPlace.X, userPlace.Y] == Cell.here)
            {
                return true;
            }
            return false;
        }
    }

}

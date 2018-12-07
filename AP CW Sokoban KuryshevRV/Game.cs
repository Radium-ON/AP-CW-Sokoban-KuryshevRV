using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AP_CW_Sokoban_KuryshevRV.App;

namespace AP_CW_Sokoban_KuryshevRV
{
    class Game
    {
        Cell[,] map;
        Cell[,] itemsOnTop;//хранит ящики и персонажа поверх исходного поля
        MapPlace userMouse;//координата клика пользователя
        int mapWidth, mapHeight;
        //строка статистики
        int placed, totals;

        delegateShowItem ShowItem;
        delegateShowStats ShowStats;

        public Game(delegateShowItem showItem, delegateShowStats showStats)
        {
            ShowItem = showItem;
            ShowStats = showStats;
        }

        public bool InitializeLevel(int levelN, out int width, out int height)
        {
            LevelSetup level = new LevelSetup();
            map = level.LoadLevel(levelN);
            if (map == null)
            {
                width = 0;
                height = 0;
                return false;
            }
            width = mapWidth = map.GetLength(0);
            height = mapHeight = map.GetLength(1);
            itemsOnTop = new Cell[mapWidth, mapHeight];//изменяемое поле поверх
            for (int i = 0; i < mapWidth; i++)
                for (int j = 0; j < mapHeight; j++)
                {//заменяем матрицу отображения пустыми клетками, если в матрице топ есть объект
                    switch (map[i, j])
                    {
                        case Cell.none:
                            break;

                        case Cell.wall:
                            break;

                        case Cell.box:
                            itemsOnTop[i, j] = Cell.box;
                            map[i, j] = Cell.none;
                            break;

                        case Cell.here:
                            itemsOnTop[i, j] = Cell.none;
                            break;

                        case Cell.done:
                            itemsOnTop[i, j] = Cell.box;
                            map[i, j] = Cell.here;
                            break;

                        case Cell.user:
                            userMouse = new MapPlace(i, j);
                            map[i, j] = Cell.none;
                            itemsOnTop[i, j] = Cell.user;
                            break;
                    }
                }
            return true;
        }

        public void ShowLevel()
        {
            placed = totals = 0;//чистая статистика перед загрузкой уровня
            for (int i = 0; i < mapWidth; i++)
                for (int j = 0; j < mapHeight; j++)
                {
                    ShowItemMapWithTop(i, j);
                    if (map[i, j] == Cell.here)
                    {
                        totals++;
                        if (itemsOnTop[i, j] == Cell.box)
                        {
                            placed++;
                        }
                    }
                }
            ShowStats(placed, totals);
        }

        private void ShowItemMapWithTop(int i, int j)
        {
            if (itemsOnTop[i, j] == Cell.none)
            {
                ShowItem(new MapPlace(i, j), map[i, j]);
            }
            else
            {
                if (itemsOnTop[i, j] == Cell.box && map[i, j] == Cell.here)
                {
                    ShowItem(new MapPlace(i, j), Cell.done);
                }
                else
                {
                    ShowItem(new MapPlace(i, j), itemsOnTop[i, j]);
                }
            }
        }

        //делаем шаги юзера
        internal void UserStep(int sx, int sy)
        {
            MapPlace place = new MapPlace(userMouse.X + sx, userMouse.Y + sy);
            if (!UserInRange(place))
                return;
            if (itemsOnTop[place.X, place.Y] == Cell.none)
            {
                itemsOnTop[userMouse.X, userMouse.Y] = Cell.none; ShowItemMapWithTop(userMouse.X, userMouse.Y);
                itemsOnTop[place.X, place.Y] = Cell.user; ShowItemMapWithTop(place.X, place.Y);
                userMouse = place;
            }
            //шагаем на ящик
            if (itemsOnTop[place.X, place.Y] == Cell.box)
            {
                //координаты объекта за ящиком
                MapPlace placeAhead = new MapPlace(place.X + sx, place.Y + sy);
                if (!UserInRange(placeAhead))
                    return;//наступить на ящик нельзя
                if (itemsOnTop[placeAhead.X, placeAhead.Y] != Cell.none)
                    return;//ящик можно двигать только на пустую клетку

                //двигаем ящик на цель:счётчик+ смещаем с цели:счётчик-
                if (map[place.X, place.Y] == Cell.here) placed--;
                if (map[placeAhead.X, placeAhead.Y] == Cell.here) placed++;
                ShowStats(placed, totals);//обновили статус ящиков

                itemsOnTop[userMouse.X, userMouse.Y] = Cell.none; ShowItemMapWithTop(userMouse.X, userMouse.Y);
                itemsOnTop[place.X, place.Y] = Cell.user; ShowItemMapWithTop(place.X, place.Y);
                itemsOnTop[placeAhead.X, placeAhead.Y] = Cell.box; ShowItemMapWithTop(placeAhead.X, placeAhead.Y);
                userMouse = place;
            }
        }

        //перемещаем ящик на нужное место
        internal string SolveBox(MapPlace box, MapPlace target)
        {
            if (itemsOnTop[box.X, box.Y] != Cell.box)
                return "";
            if (!UserInRange(target))
                return "";
            BoxSolver solver = new BoxSolver(map, itemsOnTop);
            return solver.MoveBox(userMouse, box, target);
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
            if (map[userPlace.X, userPlace.Y] == Cell.none)
            {
                return true;
            }
            if (map[userPlace.X, userPlace.Y] == Cell.here)
            {
                return true;
            }
            return false;
        }

        public string SolveMouse(MapPlace target)
        {
            if (!UserInRange(target))
                return "";
            MouseSolver solver = new MouseSolver(map, itemsOnTop);
            return solver.MoveMouse(userMouse, target);
        }
    }

}

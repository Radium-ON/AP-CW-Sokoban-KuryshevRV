using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AP_CW_Sokoban_KuryshevRV.App;

namespace AP_CW_Sokoban_KuryshevRV
{
    class MouseSolver
    {
        int width, height;

        List<Chain> directionChains;//направления движения в координатах

        Cell[,] map;
        Cell[,] top;

        struct Chain //путь до ячейки (ребро графа)
        {
            public int X;
            public int Y;
            public string Path;

            public Chain(int x, int y, string path)
            {
                X = x;
                Y = y;
                Path = path;
            }

            public Chain(MapPlace place, string path)
            {
                X = place.X;
                Y = place.Y;
                Path = path;
            }
        }
        public MouseSolver(Cell[,] map, Cell[,] top)//обход направлений в MoveAlpha
        {
            this.map = map;
            this.top = top;
            width = map.GetLength(0);
            height = map.GetLength(1);
            directionChains = new List<Chain>();
            directionChains.Add(new Chain(-1, 0, "4"));
            directionChains.Add(new Chain(1, 0, "6"));
            directionChains.Add(new Chain(0, -1, "8"));
            directionChains.Add(new Chain(0, 1, "2"));
        }
        public string MoveMouse(MapPlace source, MapPlace destination)
        {
            if (source.X == destination.X && source.Y == destination.Y)
            {
                return "";
            }

            Queue<Chain> queueChains = new Queue<Chain>();//очередь для поиска в ширину
            List<MapPlace> visitedPlaces = new List<MapPlace>();//уже посещённые вершины (ячейки)

            queueChains.Clear();
            visitedPlaces.Clear();

            //начальная позиция игрока
            Chain chain;
            chain.X = source.X;
            chain.Y = source.Y;
            chain.Path = "";

            MapPlace place;//новые координаты
            queueChains.Enqueue(chain);

            while (queueChains.Count > 0)
            {
                chain = queueChains.Dequeue();
                foreach (var side in directionChains)
                {
                    place.X = chain.X + side.X;
                    place.Y = chain.Y + side.Y;
                    if (!InRange(place))
                        continue;
                    if (visitedPlaces.Contains(place))
                        continue;
                    visitedPlaces.Add(place);//добавили координату, куда переместились
                    //отобразили, куда сдвинулись
                    Chain stepUser = new Chain(place, chain.Path + side.Path);
                    if (place.Equals(destination))
                        return stepUser.Path;
                    queueChains.Enqueue(stepUser);
                }

            }
            return "-";
        }

        private bool InRange(MapPlace place)//проверка позиций
        {
            if (place.X < 0 || place.X >= width)
                return false;
            if (place.Y < 0 || place.Y >= height)
                return false;
            if (map[place.X, place.Y] == Cell.none &&
                top[place.X, place.Y] == Cell.none)
                return true;
            if (map[place.X, place.Y] == Cell.here &&
                top[place.X, place.Y] == Cell.none)
                return true;
            return false;
        }
    }
    {
    }
}

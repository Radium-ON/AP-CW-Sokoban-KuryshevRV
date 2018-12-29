using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static AP_CW_Sokoban_KuryshevRV.App;

namespace AP_CW_Sokoban_KuryshevRV
{
    public abstract class GameSolver
    {
        public List<Direction> directionChains;

        protected int height;
        protected int width;

        protected Cell[,] map;
        protected Cell[,] top;

        public class Direction
        {
            public int X;
            public int Y;
            public string Path;

            public Direction(int x, int y, string path)
            {
                X = x;
                Y = y;
                Path = path;
            }
            public Direction(MapPlace place, string path)
            {
                X = place.X;
                Y = place.Y;
                Path = path;
            }
        }

        public GameSolver(Cell[,] map, Cell[,] top)
        {
            this.map = map;
            this.top = top;
            width = map.GetLength(0);
            height = map.GetLength(1);
            directionChains = new List<Direction>();
            directionChains.Add(new Direction(-1, 0, "4"));
            directionChains.Add(new Direction(1, 0, "6"));
            directionChains.Add(new Direction(0, -1, "8"));
            directionChains.Add(new Direction(0, 1, "2"));
        }

        protected bool InRange(MapPlace place)//проверка позиций
        {
            if (place.X < 0 || place.X >= width)
                return false;
            if (place.Y < 0 || place.Y >= height)
                return false;
            if (map[place.X, place.Y] == Cell.none &&
                (top[place.X, place.Y] == Cell.none ||
                top[place.X, place.Y] == Cell.user))
                return true;
            if (map[place.X, place.Y] == Cell.here &&
                (top[place.X, place.Y] == Cell.none ||
                top[place.X, place.Y] == Cell.user))
                return true;
            return false;
        }

        public abstract string MoveItem(MapPlace mouse, MapPlace source, MapPlace destination);

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AP_CW_Sokoban_KuryshevRV.App;

namespace AP_CW_Sokoban_KuryshevRV
{
    class BoxSolver : GameSolver
    {
        struct Chain //путь до ячейки (ребро графа)
        {
            public MapPlace Mouse;
            public MapPlace Apple;
            public string Path;

            public Chain(MapPlace mouse, MapPlace apple, string path)
            {
                Mouse = mouse;
                Apple = apple;
                Path = path;
            }
        }
        public BoxSolver(Cell[,] map, Cell[,] top) : base(map,top) //обход направлений в MoveUser
        {
            
        }

        public override string MoveItem(MapPlace mouse, MapPlace source, MapPlace destination)
        {
            //mouse - точка мыши, сурс - исходная точка ящика, дест - назначение ящика
            if (source.X == destination.X && source.Y == destination.Y)
            {
                return "";

            }

            top[source.X, source.Y] = Cell.none;//убрали яблоко с поля

            bool[,,,] visitedCells = new bool[width, height, width, height];

            Queue<Chain> queueChains = new Queue<Chain>();//очередь для поиска в ширину
            //List<PlaceMouseApple> visitedPlaces = new List<PlaceMouseApple>();//уже посещённые вершины (ячейки) яблока и мыши

            queueChains.Clear();
            //visitedPlaces.Clear();

            //начальная позиция игрока
            Chain chain;
            chain.Mouse = mouse;
            chain.Apple = source;
            chain.Path = "";

            MapPlace newMouse;//новые координаты мыши
            MapPlace newApple;
            queueChains.Enqueue(chain);
            while (queueChains.Count > 0)
            {
                chain = queueChains.Dequeue();
                foreach (Direction side in directionChains)
                {
                    newMouse.X = chain.Mouse.X + side.X;
                    newMouse.Y = chain.Mouse.Y + side.Y;
                    if (!InRange(newMouse))
                        continue;
                    if (newMouse.X == chain.Apple.X && newMouse.Y == chain.Apple.Y)
                    {//запись координат яблока и мыши
                        newApple.X = newMouse.X + side.X;
                        newApple.Y = newMouse.Y + side.Y;
                        if (!InRange(newApple))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        newApple = chain.Apple;
                    }

                    if (visitedCells[newMouse.X, newMouse.Y, newApple.X, newApple.Y])
                        continue;
                    visitedCells[newMouse.X, newMouse.Y, newApple.X, newApple.Y] = true;//добавили координату, куда переместились
                    //отобразили, куда сдвинулись
                    Chain stepUser = new Chain(newMouse, newApple, chain.Path + side.Path);
                    if (newApple.X == destination.X && newApple.Y == destination.Y)
                    {
                        top[source.X, source.Y] = Cell.box;
                        return stepUser.Path;
                    }
                    queueChains.Enqueue(stepUser);
                }

            }
            top[source.X, source.Y] = Cell.box;
            return "NO";//решения не существует
        }
    }
}

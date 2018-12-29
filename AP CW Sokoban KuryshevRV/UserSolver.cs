using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AP_CW_Sokoban_KuryshevRV.App;

namespace AP_CW_Sokoban_KuryshevRV
{
    class UserSolver : GameSolver
    {
        public UserSolver(Cell[,] map, Cell[,] top):base(map,top)//обход направлений в MoveAlpha
        {
            
        }
        public override string MoveItem(MapPlace mouse, MapPlace source, MapPlace destination)
        {
            if (mouse.X == destination.X && mouse.Y == destination.Y)
            {
                return "";
            }

            Queue<Direction> queueChains = new Queue<Direction>();//очередь для поиска в ширину
            List<MapPlace> visitedPlaces = new List<MapPlace>();//уже посещённые вершины (ячейки)

            queueChains.Clear();
            visitedPlaces.Clear();

            //начальная позиция игрока
            Direction chain = new Direction(mouse.X, mouse.Y, "");

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
                    Direction stepUser = new Direction(place, chain.Path + side.Path);
                    if (place.Equals(destination))
                        return stepUser.Path;
                    queueChains.Enqueue(stepUser);
                }

            }
            return "-";
        }

    }    
}

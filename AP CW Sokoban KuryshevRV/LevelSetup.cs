using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AP_CW_Sokoban_KuryshevRV.App;

namespace AP_CW_Sokoban_KuryshevRV
{
    public class LevelSetup
    {
        static string mFilename = "LevelList.txt";
        int mLevelSizeNew = 8;
        public LevelSetup()
        {

        }
        public Cell[,] LoadLevel(int levelNumber)
        {
            Cell[,] cell = null;
            string[] lines;
            //считывание всех строк из файла уровня
            try
            {
                lines = File.ReadAllLines(mFilename);
            }
            catch
            {
                return cell;
            }

            int currentLine = 0;//текущая считанная строка
            int width;
            int height;
            int currentlevelNum = 0;
            while (currentLine < lines.Length)
            {
                readLeverHeader(lines[currentLine], out currentlevelNum, out width, out height);
                if (levelNumber == currentlevelNum)
                {
                    cell = new Cell[width, height];
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            cell[x, y] = CharToCell(lines[currentLine + 1 + y][x]);

                        };
                    };
                    break;
                }
                else
                {
                    //новая текущая строка = +1 и высота пропущенного уровня
                    currentLine = currentLine + 1 + height;
                }
            }
            if (cell == null)
            {
                /*Array.Resize(ref lines, lines.Length+ mLevelSizeNew+ 1);
                lines[currentLine] = (currentlevelNum + 1).ToString() + " "+mLevelSizeNew.ToString()+" "+mLevelSizeNew.ToString();
                for (int i = 0; i < mLevelSizeNew; i++)
                {
                    lines[currentLine + i + 1] = new string(' ',mLevelSizeNew);
                }
                File.WriteAllLines(mFilename, lines);
                return LoadLevel(levelNumber);*/
            }
            return cell;
        }
        public void SaveLevel(int levelNumber, Cell[,] map)
        {
            string[] lines;
            //считывание всех строк из файла уровня
            try
            {
                lines = File.ReadAllLines(mFilename);
            }
            catch
            {
                return;
            }

            int currentLine = 0;//текущая считанная строка
            int width = 0;
            int height = 0;
            int currentlevelNum;
            while (currentLine < lines.Length)
            {
                readLeverHeader(lines[currentLine], out currentlevelNum, out width, out height);
                if (levelNumber == currentlevelNum)
                {
                    break;
                }
                else
                {
                    //новая текущая строка = +1 и высота пропущенного уровня
                    currentLine = currentLine + 1 + height;
                }
            }
            int old_length = lines.Length;
            int delta = map.GetLength(1) - height;
            int new_length = old_length + delta;
            if (new_length > old_length)
            {
                Array.Resize(ref lines, new_length);
                for (int i = new_length - 1; i > currentLine; i--)
                {
                    lines[i] = lines[i - delta];
                }
            }
            if (new_length < old_length)
            {
                for (int i = currentLine; i < new_length; i++)
                {
                    lines[i] = lines[i - delta];
                }
                Array.Resize(ref lines, new_length);
            }
            lines[currentLine] = String.Format("{0} {1} {2}", levelNumber, map.GetLength(0), map.GetLength(1));
            int newLevelHeight = lines.Length - height + map.GetLength(1);
            for (int y = 0; y < map.GetLength(1); y++)
            {
                lines[currentLine + 1 + y] = "";//переход на новую строку
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    lines[currentLine + 1 + y] += CellToChar(map[x, y]);

                };
            };
            try
            {
                File.WriteAllLines(mFilename, lines);
            }
            catch
            {
                return;
            }
        }
        public void readLeverHeader(string line, out int levelNumber, out int width, out int height)
        {
            string[] parts = line.Split();
            levelNumber = 0;
            width = 0;
            height = 0;//обнуляем
            if (parts.Length != 3)//если массив длиннее 3 - выход
                return;
            int.TryParse(parts[0], out levelNumber);
            int.TryParse(parts[1], out width);
            int.TryParse(parts[2], out height);


        }
        public Cell CharToCell(char x)
        {
            //возвращает ячейку в зависимости от считанного символа
            switch (x)
            {
                case ' ': return Cell.none;
                case '#': return Cell.wall;
                case '0': return Cell.box;
                case '.': return Cell.here;
                case 'C': return Cell.done;
                case '1': return Cell.user;
                default:
                    return Cell.none;
            }
        }
        public char CellToChar(Cell c)
        {
            //возвращает символ в зависимости от ячейки
            switch (c)
            {
                case Cell.none: return ' ';
                case Cell.wall: return '#';
                case Cell.box: return '0';
                case Cell.here: return '.';
                case Cell.done: return 'C';
                case Cell.user: return '1';
                default: return ' ';


            }
        }
    }
}

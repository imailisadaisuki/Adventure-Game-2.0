using System;

namespace Adventure_Game_2._0
{
    public class Item
    {
        protected int xcoor, ycoor;
        public Item() => (xcoor, ycoor) = (0, 0);
        public void update(int x, int y) => (xcoor, ycoor) = (x, y);
        protected bool inRange(int x, int y, int horSize, int verSize) => x >= 0 && x < horSize && y >= 0 && y < verSize;
        public void removeItem(char[,] grid)
        {
            grid[xcoor, ycoor] = ' ';
            Console.CursorLeft = ycoor;
            Console.CursorTop = xcoor + 4;
            Console.Write(' ');
        }
        public void display(char[,] grid, char c)
        {
            grid[xcoor, ycoor] = c;
            Console.CursorLeft = ycoor;
            Console.CursorTop = xcoor + 4;
            Console.Write(c);
        }
        public virtual char get() => ' ';
    }

    public class Character : Item
    {
        public Character() : base() { }
        private char chatacter = 'C';
        public override char get() => chatacter;
        public char move(char[,] maze, ref string exp)
        {
            ConsoleKeyInfo key = Console.ReadKey(true);
            int newX = xcoor, newY = ycoor;
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    newX--;
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    newX++;
                    break;
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    newY--;
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    newY++;
                    break;
                case ConsoleKey.C:
                    exp = "";
                    return 'N';
                default:
                    return 'N';
            }
            if (!inRange(newX, newY, maze.GetLength(0), maze.GetLength(1)) || maze[newX, newY] == '#') return 'N';
            char item = maze[newX, newY];
            removeItem(maze);
            maze[xcoor, ycoor] = ' ';
            xcoor = newX;
            ycoor = newY;
            Console.ForegroundColor = ConsoleColor.Green;
            display(maze, get());
            Console.ForegroundColor = ConsoleColor.White;
            return item;
        }
    }

    public class Number : Item
    {
        private int num;
        public Number(Random rand) => num = rand.Next(0, 10);
        public override char get() => num.ToString()[0];
    }
    public class Operator : Item
    {
        private char op;
        public Operator(string[] ops, Random rand) => op = ops[rand.Next(0, ops.Length)][0];
        public override char get() => op;
    }
}


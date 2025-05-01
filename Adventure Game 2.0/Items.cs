using System;

namespace Adventure_Game_2._0
{
    public class Item
    {
        protected int xcoor, ycoor;
        public Item() => (xcoor, ycoor) = (0, 0);
        public void update(int x, int y) => (xcoor, ycoor) = (x, y);
        protected bool inRange(int x, int y, int horSize, int verSize) => x >= 0 && x < horSize && y >= 0 && y < verSize;
        public virtual void display(char[,] grid) => grid[xcoor, ycoor] = ' ';
    }

    public class Character : Item
    {
        public Character() : base() { }
        public override void display(char[,] grid) => grid[xcoor, ycoor] = 'C';
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
            maze[xcoor, ycoor] = ' ';
            xcoor = newX;
            ycoor = newY;
            display(maze);
            return item;
        }
    }

    public class Number : Item
    {
        private int num;
        public Number(Random rand) => num = rand.Next(0, 10);
        public override void display(char[,] grid) => grid[xcoor, ycoor] = num.ToString()[0];
    }
    public class Operator : Item
    {
        private char op;
        public Operator(string[] ops, Random rand) => op = ops[rand.Next(0, ops.Length)][0];
        public override void display(char[,] grid) => grid[xcoor, ycoor] = op;
    }
}


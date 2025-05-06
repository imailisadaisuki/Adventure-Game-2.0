using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Adventure_Game_2._0
{
    internal class Program
    {
        const int verSize = 60;
        const int horSize = 125;
        const int newNumberPerMove = 8;
        const int newOperatorsPerMove = 5;
        static string[] ops = { "+", "-", "*", "/", "B" };
        static Random rand = new Random();
        static List<int> readHighScore()
        {
            List<int> highScore = new List<int>();
            using (StreamReader sr = new StreamReader("highscore.txt"))
            {
                while (!sr.EndOfStream) highScore.Add(int.Parse(sr.ReadLine()));
            }
            return highScore;
        }
        static void writeHighScore(List<int> highScore)
        {
            using (StreamWriter sw = new StreamWriter("highscore.txt"))
            {
                foreach (var val in highScore) sw.WriteLine(val);
            }
        }
        static int menu(ref bool canFrac, ref bool canNega, List<int> highScore)
        {
            Console.WriteLine("Press Up/Down arrow or W/S key to choose difficulty of target number and press Enter key to confirm.\n");
            Console.WriteLine($"     Easy: posotive integers. High Score: {highScore[0]}");
            Console.WriteLine($"     Medium: posotive and negative integers. High Score: {highScore[1]}");
            Console.WriteLine($"     Hard: posotive integers and fractions. High Score: {highScore[2]}");
            Console.WriteLine($"     Expert: posotive and negative integers and fraction. High Score: {highScore[3]}");
            Console.CursorLeft = 0;
            Console.CursorTop = 2;
            Console.Write("->");
            int option = 1;
            do
            {
                ConsoleKeyInfo choice = Console.ReadKey(true);
                if ((choice.Key == ConsoleKey.DownArrow || choice.Key == ConsoleKey.S) && option < 4)
                {
                    Console.CursorTop = option + 1;
                    Console.CursorLeft = 0;
                    Console.Write("  ");
                    option++;
                    Console.CursorTop = option + 1;
                    Console.CursorLeft = 0;
                    Console.Write("->");
                }
                else if ((choice.Key == ConsoleKey.UpArrow || choice.Key == ConsoleKey.W) && option > 1)
                {
                    Console.CursorTop = option + 1;
                    Console.CursorLeft = 0;
                    Console.Write("  ");
                    option--;
                    Console.CursorTop = option + 1;
                    Console.CursorLeft = 0;
                    Console.Write("->");

                }
                else if (choice.Key == ConsoleKey.Enter)
                {
                    switch (option)
                    {
                        case 1:
                            canNega = false;
                            canFrac = false;
                            return option;
                        case 2:
                            canNega = true;
                            canFrac = false;
                            return option;
                        case 3:
                            canNega = false;
                            canFrac = true;
                            return option;
                        case 4:
                            canNega = true;
                            canFrac = true;
                            return option;
                    }
                }
            } while (true);
        }
        static bool inRange(int x, int y) => x >= 0 && x < verSize && y >= 0 && y < horSize;
        static void genMaze(int x, int y, char[,] maze, bool[,] visited)
        {
            visited[x, y] = true;
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { 1, 0, -1, 0 };
            for (int i = 0; i < dx.Length; i++)
            {
                int dir = rand.Next(4);
                int newX = x + dx[dir];
                int newY = y + dy[dir];
                if (inRange(newX, newY) && !visited[newX, newY])
                {
                    maze[x, y] = ' ';
                    maze[newX, newY] = '#';
                    genMaze(newX, newY, maze, visited);
                }
            }
        }
        static void initMaze(char[,] maze)
        {
            bool[,] visited = new bool[verSize, horSize];
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    maze[i, j] = ' ';
                    visited[i, j] = false;
                }
            }
            genMaze(0, 0, maze, visited);
        }
        static void printMaze(char[,] maze)
        {
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    switch (maze[i, j])
                    {
                        case '#':
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        case 'C':
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.White;
                            break;
                    }
                    Console.Write(maze[i, j]);
                }
                Console.WriteLine();
            }
            Console.ResetColor();
        }
        static void addItems(Item item, char[,] maze)
        {
            int x = rand.Next(0, verSize);
            int y = rand.Next(0, horSize);
            while (maze[x, y] != ' ')
            {
                x = rand.Next(0, verSize);
                y = rand.Next(0, horSize);
            }
            item.update(x, y);
            item.display(maze, item.get());
        }
        static void updateMaze(char[,] maze)
        {
            for (int i = 0; i < newNumberPerMove; i++)
            {
                addItems(new Number(rand), maze);
            }
            for (int i = 0; i < newOperatorsPerMove; i++)
            {
                addItems(new Operator(ops, rand), maze);
            }
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (rand.Next(1, 15) == 1 && maze[i, j] != '#' && maze[i, j] != 'C')
                    {
                        maze[i, j] = ' ';
                        Console.CursorLeft = j;
                        Console.CursorTop = i + 4;
                        Console.Write(' ');
                    }
                }
            }

        }
        static Fraction convert2Frac(string str)
        {
            if (str.Contains("/") && str.Length > 2)
            {
                string[] parts = str.Split('/');
                return new Fraction(int.Parse(parts[0]), int.Parse(parts[1]));
            }
            return new Fraction(int.Parse(str), 1);
        }
        static string evaluateRPN(string expression, ref int moveleft)
        {
            Stack<Fraction> RPN = new Stack<Fraction>();
            List<string> arr = expression.Split(' ').ToList();
            arr.RemoveAll(x => x == "");
            expression = "";
            foreach (var val in arr)
            {
                if (ops.Contains(val))
                {
                    if (RPN.Peek().denominator == 0 && val == "/")
                    {
                        moveleft = -2;
                        return "";
                    }
                    Fraction b = RPN.Pop();
                    Fraction a = RPN.Pop();
                    switch (val)
                    {
                        case "+": RPN.Push(a + b); break;
                        case "-": RPN.Push(a - b); break;
                        case "*": RPN.Push(a * b); break;
                        case "/": RPN.Push(a / b); break;
                    }
                }
                else RPN.Push(convert2Frac(val));
            }
            foreach (var val in RPN)
            {
                expression = val.outputStrFrac() + " " + expression;
            }
            return expression;
        }
        static void playGame(int mazeNum, ref int moveLeft, bool canFrac, bool canNega)
        {
            Console.Clear();
            string expression = "", targetNum;
            char[,] maze = new char[verSize, horSize];
            Character player = new Character();
            if (!canFrac && !canNega)
                targetNum = rand.Next(1, (int)Math.Pow(10, mazeNum) / 2).ToString() + " ";
            else if (canFrac && !canNega)
                targetNum = new Fraction((int)Math.Pow(10, mazeNum) / 2, rand.Next(1, 10 * mazeNum)).outputStrFrac() + " ";
            else if (!canFrac && canNega)
                targetNum = rand.Next(-(int)Math.Pow(10, mazeNum) / 2, (int)Math.Pow(10, mazeNum) / 2).ToString() + " ";
            else
                targetNum = new Fraction(rand.Next(-(int)Math.Pow(10, mazeNum) / 2, (int)Math.Pow(10, mazeNum)) / 2, rand.Next(1, 10 * mazeNum)).outputStrFrac() + " ";
            initMaze(maze);
            Console.Clear();
            player.display(maze, player.get());
            Console.CursorLeft = 0;
            Console.CursorTop = 4;
            printMaze(maze);
            while (moveLeft > 0)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                Console.WriteLine($"Your score: {mazeNum - 1}");
                Console.Write(new string(' ', Console.WindowWidth));
                Console.CursorLeft = 0;
                Console.WriteLine($"Move(s) left: {moveLeft}");
                Console.Write($"Target number: {targetNum}");
                Fraction f = convert2Frac(targetNum);
                Console.WriteLine(targetNum.Contains("/") && f.numerator > f.denominator ? $" = {f.outputStrMixFrac()}" : "");
                Console.WriteLine($"Current expression: {expression}\n");
                char item = player.move(maze, ref expression);
                if (item == 'B')
                {
                    moveLeft = -3;
                    return;
                }
                if (ops.Contains(item.ToString()) && expression.Split(' ').Length < 3)
                {
                    moveLeft = -1;
                    return;
                }
                if (item != ' ' && item != 'N') expression += item + " ";
                if (expression.Split(' ').Length > 3) expression = evaluateRPN(expression, ref moveLeft);
                if (expression == targetNum) return;
                if (item != 'N')
                {
                    updateMaze(maze);
                    moveLeft--;
                }
            }
            return;
        }
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Console.SetWindowSize(horSize + 10, verSize + 10);
            Console.WriteLine(@"Welcome to the Adventure Game 2.0 : RPN Maze
You are a character in the maze, try to form the target number using number and operators to form expressions.
You can move using the arrow keys or W/A/S/D keys.
You can use the C key to clear the expression whenever you need.
Everytime you reach the target number, a new maze will be generated and you will have to form a new target number.
You score increases by 1 for every maze you have completed.
The game will over if you either:
1. Run out of moves
2. Form an invalid expression (put less than 2 values in front of an operator)
3. Divide by zero
4. Step onto a bomb (B)
You have a maximum of 500 moves, try to get the highest score possible.
Press any key to continue.");
            Console.ReadKey();
            Console.Clear();
            int mazeNum = 0, moveLeft = 500;
            bool canFrac = false, canNega = false;
            List<int> highScore = readHighScore();
            int option = menu(ref canFrac, ref canNega, highScore);
            while (moveLeft > 0)
            {
                mazeNum++;
                playGame(mazeNum, ref moveLeft, canFrac, canNega);
            }
            Console.Clear();
            Console.WriteLine("Game Over");
            if (moveLeft == -1) Console.WriteLine("Invalid Expression : Require 2 values in front of an expression");
            else if (moveLeft == -2) Console.WriteLine("Invalid Expression: Division by Zero");
            else if (moveLeft == -3) Console.WriteLine("You have stepped on a bomb");
            else Console.WriteLine("You have run out of moves");
            Console.WriteLine($"Your score: {mazeNum - 1}");
            if (mazeNum > highScore[option - 1])
            {
                highScore[option - 1] = mazeNum - 1;
                writeHighScore(highScore);
                Console.WriteLine("High score gets updated!");
            }
            Console.WriteLine("Press any key to exit");
            Console.WriteLine(@"%%#%%%%@@@@%=..#@@@%####++*%%@@=...#@@@@@@@@@@@@@+           -%@@@#:=%#::#%#%==##:..=+###%%@@*:.-%@@
.@@@@@@@@@@@#=@@@@@@@@@@@@@@@@%=--@@@%%%%*@@@+     .....          @@@@=+@@-@@%@@+=+@@@@%@@@@*-=@@@@@
 ......:+@@@@@@@@@@@@@@@@@@@@@=--@@@%%@%*@@  .:+***+============-.. @@@@%+@%#@#-:+@@@@@@@@%+-%@@@@* 
  ........::...-#@@@@@@@@@@@@=-=@@@@@@@*#@ ++=:------::--::--:::---:.-@@%@%@@+--%@@@%@@@@%#@@@%.....
@@@#:     ...:--:.:....+@@@@@@@@@@@@@@@@@@===---::-::::-::::-:::::-==*@@@@@@=::@@@@%@@@@%%*-:...... 
@@@@@@@@@@@#:.     ..:-:.....-*@@@@@@@%#@@+++++=---:.         ..-:--=+#@@@@+=*@@@@@@@@::-=:...:%@@@:
*##%@@@@@@@@@@@@@@@@-.......:=--:::::=#@@@.     ...   .@@@@@.    ..:-*@@@%++@@@@@@--::::::=@@@@:    
*%%@@@@@==%@@@@@@@@@@@@@@@@@=......-==-. =@@@@@  ..:@@@@@@@@@@@@@@..:+@@%+%@@@-..--...+@@@@.  :@@@@@
#%@@@@@#=#@@@%%#*#%#@@@@@@@@@@@@@@@*=-.@@@@@@@@@@==@@@%@@@@@@@@@=...:+@@*%*-:::....@@@-::+@@@@@  +  
+@@@@@@#@@@@@@%#%%%%%%@@%-=#@@@@@@@@@@@@@@@@@@@@@@-+@@@@ -: .#. @@+..=%@@@+-::#@@@@@@@:--@@....@@@. 
=#+++*#%%@@@@@@@@@@@@@@@++%@@@%#*%@%@@=@*    ++@@@..%@@@  =    -@@@=.=##+*%@@@@@@@@@@@:::@#..*.=@@ %
.=%@#--**+++*####%%@@@@##@@@@@@#%@%%%@*..@@:   @@@ .*@@ @@@@@@  #@@@-=##+*++***++***@@:::@*.@@.. ..@
@@@@@@%+=#@#:=++===****#%@@%@@@@@@@@@@@ @@@@@  @@= .=#@:+@@@@@.:::+*--@@@@%#+++++***%@-::@-:@@@@@@@-
*@@@@@@@@@@@@@%%@@@====#*-+**##%%%%%@@@.@@@@%. @@  ==.:@*#%%*.%@@   .-@@@@@@@@@@@@%*%@:.:@@@#.@.  @ 
+@@@%%@%%@@@@@@@@@@@@@@@@@+=+**-=**#%@@: .::@@@@=  *+. -@@@@@@@    ..:@@@@##@@@@@%%@@@-.:@*.+@:=@-++
=%%%%%@%%%%%%%%%%@%@@@@@@@@@@@@@@##@@@.@@@@@@+ :# .*+-.          ...-+@@@@@@@@%*+%@@@@:.:@@. -.=@.. 
-%%%%@@%%%#%%%%%%@@%@@@@@@@@@@@@@@@@@*        :@=  #####+......:-*#%@@%@@@@@@@%%@@@@@@-.:@%-@@@   @%
:%%##@@@@@@@@@@@@###%@%@@@@@@@@@@@@@@ @#....+%@        @@@@@@@@@@@@@@##@##%%%%@@@@@@@@-.:%%+@@@@@@@ 
.###*##..+*...-+%@@@#%%%%%%%%%%%@@@@@ .@@@@@@@@@@@@@@@@%.@@@@@@@@@@@@**%%##*%@@@@@@@@@=.-#@@@#@@@@@-
.##*+#%%%.-@@@@%#=@@*#%%%%%%@@%%%@%@@@% @@@@@. @@@@@@@@    .@@@@@@@@@*@@#%@@::@:.##+%@=..#@-.=*.... 
.**++@%+%@-.#@%#%*@@+%%%%%%%%%%%%%%%@@@@  @@=  -@@@@     .%@@%@@@@@@@*%@@@@@@@@@@@@@@@=.:-+.-*%--@@@
.#*=:---*#%#..@@#+@@+###%%%%%%%%%%%@@@@@@@ @@@@@@@@@@@@@@@-*..*%@@@@@#%@@@@@@@@@@@@@@@+.:=@+-+%-.@@-
        :=*%%- %@+#@+####%%%%%%%%%%%%@@@@@+:@@%=%@+     *@%  :@+@@@@@#%@@@@@@@@@@@@@@@*.=-@=-+%-.#@+
 :.       .+*%# .@@@=*##%#%#%%%%%%%%%%@@@@@ @+@@@@@@@@@@@:  .#=#@@%@@#%@@@@@@@@@@@@@@@#.:=@+-+%=.*@=
 .......:-===+#%+ @@=*#########%%%%%%%%%%@@ @@+#@@@@@@.    :@@@@%#*@%#%@@@@@@@@@@@@@@@#.:-@*-+@=.#@*
 ..........:--=*@ .@-*#######%##%%%%%%%%%@@ :@%**+:.    ..#@@@@**#*@@%%@@@@@@@@@@@@@@@#.:=@*-+%=:%@.
 ::::::..::::--=@ .@:+****######%%%%%%%%%@@  @@@@%#*#%@@@@@@%=::+#*%@%%@@@@@@@@@@@@@@@#.+*@*-=@=:.. 
 --::::::::-::-=@ :@.++****#######%%%%%%%%@* @@@@@@@@@@@@@#...:+##%@@@@@@@@@@@@@@@@@@@#.++@*-=@=-@@=
 --------------=@ =@.++++****######%%%%%%%@@ @@@@@@@@@@@+:..=.=++*=  *@@@@@@@@@@@@@@@@%.:=@*-+@=.@@@
 ===---==------=% *@.==+++*****########%%%@@ @@@@@@@@*+:..::#-=+**=  .@@@@@@@@@@@@@@@@%.-=@#-=%=.-@#
 =======-=======@ %@.++++++*****#*#####%%%@* %@@@#*+=-+===--%++#%%@...%@@@@@@%@@@@@@@@@.-=@#=+@+.-@%
 +=============+@ @@.=+++*********#######%@:. @@@**=--*++===@####@@%..%@@@@@@@@@@%%@@@@.:=@*-=@=:=@%
 ====---=======+@ @@.----=+******########%@:@ @@@+++++#++++=@*##*#@@..=@@@@%@@@@@@@%@@@.:-@#-=@+:-@*
.###*#****++++=+@ @@:*#@@=:-=+*#*########%*#@ @@@##**+#**++*%+##*#@@-..@@@%%%%%%%%%%@@@.-+@%==@+:-@%
.******#####%@@@@ @@-@@@@@@@%+-==+**#####@.@@ @@@##**+#*+***#=##**@@%..+@@%@%@@%%%%%@@@.:=@%==%+--%%
.##**+++*#%@@@+:= @@+%#####%@@@@@#+++**##@ @@ @@@#%%%*%%+#+**+#*#*@@@@**@@%%%%%%%%%%@@@::=@#++#++:@%
.++#**#%@@*:. .*@ @@-#+*#*##*###%@@@@@#**@ @@ @@@*%##*%#*%+*+=*+++@@.  -@@%%%%%%%%%%@@@.=*@#+*@@@-%%
 :-=+**-.  .----+ @%.::--===++++**###%%@@@ @@ #@@%%%#+%*=+==+=+++-@@  .@@@%@%%%%@@@@@@@:-+@%+*=*@=@@
 ...    .--%@@@@@ @@.-:.:::::::.::::----=# @@:*@%**+++%*=*+##+***-@@  %@@@@%%@@@%**#%@@:-+@#+%+*%-=.
.%%%%%@@@@@@@@@@@ @@@@@@@@@@@@@@@@@@@@@@@@ @@=+@%###**%***=+%+==*+@@  @@@@@@@%%%@@@@#%%-=*@#+*=::...
-@#*#%@@@%@%###@@ @@-**%%####%*#@@@#*###@@ @@.+@@%%#**%+*#*+%-++**@: +@@@%@%@%@#%#%@%@#+*#@%###@@@@=
.%%@@%%%%%@*=**## @@:=+*=++**#*#%*%%#%%%@* @@-=@@%%%##%#**+=@+%#@@@  @@@%%@%@+@##..: # =@#*@@@%@@%#.
.@%%%%%%%%@*-++** @@.-=+=*++=*=*#-#@#%%%@. .@@:@@%%%##%#+#*+@+*#@@  *@@%%%%*@-@%*=#+.@-+%..+..--:.. 
.@%%%%%%%%@*-*#%+.@@:++*+*#%%%+*%=%@%@@@@.  *@#@@@%%#*#+*%#+@=%@.  .%@@%%%%*@#@@##@@@@@*% .*-=#%++#.
:@%%%%%%%%@*=#%#=.@@-#*#**+**%+*%=%@%@@@@-@- #@#@@%%###*##*=%*%  @- @@@@@@@@@@@@@@@@@@@=#.:@@@%..@@ 
-@%%%%%%%%@#*%%%+.@@-*#%#%%%##=#%=%@%@@@@-*@=+@*@@%%###%****@+ :@@ @@@@%%@@%@#%%%%%%%@@-:.:#@@+ =@@ 
-@@@@@@%%@@%#%#@+.@@-==+++===*+##-#@%@%@@@@  @@+@%#**##%*++=%@@@* @@@=..::-=*#*###**+#%@@@@#%@- %@@ 
=@@@@@@@@#++##%@%.@@*##%%%###@*#@*@@%@@@@@  -@@=+++===+**+-.-:= #@@%%@@@@@@@@@%@@@@@@@%.@@%*@@..@@@ 
=@@@@@@@@@@@@@@@@=@@@@@@@@@@@@@@@@@@@@@@@  *@@@@@@@@@@@@@@@@@@@@@@@@@@@@@-.+@@@@@@@@@@@=@@*#@@..@@= 
.##**+++++++++-..-@%..........::::::....  #@..=@@@@@@%:--.@@@+.#@@.-------+%*++++++++*#---%#@@.+@@. 
                @@@@                    .*@   .@@@@@@. .. @@*-:=@% .....+==*..::::-==+#+-%@@@#.@@@. 
 --:--=--=++++=:.  .+@@@@@@@%#**++*%@@:.=@:.+==@-*@@@-.+*.@@=-=*@#=%%###@*#@@@@@@@@@@@@@*@-.:=-=.--.
.#%@%#######%%+*@%##+#*==+#%@@@@@@@@=  -@*+%%%%@:-@@@%#@@#@#-.=+@##%@@@#=--+++==-::.....:%@@@@@@@@@:
.@%+*####%%###%@#%%%##***###**+*#@@+  =@@*%%@@@@+=%@@%#%%%@*..=+@@@@@@%%@@@@*+==:........         . 
.*%#%@@%##@@%**%#*###*%%%%#%%%%%@@. .-@@@@@@@@@=.:+%@@%%%@@@. =@@@@@@@%@@@@@@@@@@@@@@@@@@@@@@@@@@@@.
:%%#*++%@%##%@@%%%%@%%%%%####%@@@. .+@@%###%@@@#+*=-@@@@@@@*..##:@@%%%@*-==--::-:.......:::-=*#@@@@@
.***@%%%#*%%***#%%%%##%@#%@@%@@@...+@@#%%%%@#@@@@@@@@@%@@@@@@@%*=@@@@@@@**+*+#@@@@@@@@@%*%%#*+=-:::.
=@@+%@@@%%%%@@@@@%@*@@@@@%%@@@@. .*@@@@@%@%@%@@@@@@%@@@@@@@@@@@@=*@@@@@@**@@@@@@%@@@@@@#*@@@@@@@@@@#
=@%@@@##@@%%%####@@@@%#%@#@@@+  .+@@@%@@%@%%@%@%%@+-=@@%@@@@#@@:-+@@@@@@%*=@*=++*=--=*#@@@@@@@@%@@%*
:#%@+*%@@#=%@@@%*+*@@@@@@@@@.  :+@@%@*#@%%%@@#%#**=--%@@%@@@%--:+%@@@@@@@##%@@@@@@@@@@@@@@@@@@@@@@@*
-%*+#+*###@#+#@@@@@@%@#@@@* . .=%@@#@@@@%@@@@@@@@%+==@@@@@#+@=-=*@+ .:=*%%#@@@%*-::::::=@@@@@@@@@@@%
*@@@@@@@@@@@@@#+-..+%@#@@ .::=+@@@@@@@@@@%#-..  =@#*#@..-%@@@#*#@@@+#*-=@%%@..:::===--:+@@@@@@@@@@@@
-@#@@%#@%@@@@@@@@@@@@@@@ --:-#@@       .:+@@@@@@@@##@@-.:...#%+*@@..-+-+@@@@...:::..-+++%@@@@@@@%%%%
@@@@@@@@@:             :+:.-*@@%#@@@@@@%#+==---:+@%##@-.===--@++@@.-=-.=@@@@#@@@@@@@@@@@@%%@@@@@@@@@
         .*@@@@@@@@@@+#*---=@@@*%#*+=--:::-==*+:=@#*+@+=*===.@=+@@.:--:=@@@@+-.:+@@@@@@@@@@@**#@@@@*
=@@@@@@@@@%%+-=*%##++++.-+%@@#.-=+*+++*#####*****%++#@+-+++*-@+*@@.=++-:.   .:::..     ..:*@@@@@@@@%
 -*+--*##%%%@@@+-::++#-=**@@%.+*##*###*******+++#%=+%@%*%##%@%-+@@*%#***++====*+====+=:...      ....
.@@@@@%**++=:....=#%+..+#@@@.=***+***#####*##%%@@@.::*@@@@@@@+:*@@@@%%%%%%##*+==========++*#*=:.... 
 .........:---.:#%*=:=+@@@@.=*****#%%@%%%@@@@@@@@:--.:@@@@@@@-=@@@@@@@@@@@%%##**+=+++++++====+*#@%# 
              ..:....==#%.     .......:---==++     .+%-#@@@@    ..##+-:........    ");
            Console.ReadKey();
            System.Environment.Exit(0);
        }
    }
}

using System;

namespace HW
{

    enum TileType { Start, Goal, Empty, Food, Trap, WormholeStart, WormholeEnd } //WormholeStart teleports the player to WormholeEnd, WormholeEnd teleports the player back to the WormholeStart. 
    enum TileNames { S, G, E, F, T, W, WE }

    class Player
    {
        private int health;
        private int position;
        private bool hasWon;
        private bool hasLost;

        public Player()
        {
            health = 100;
            position = 0;
            hasWon = false;
            hasLost = false;
        }

        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        public int Position
        {
            get { return position; }
            set { position = value; }
        }
        public bool HasWon
        {
            get { return hasWon; }
            set { hasWon = value; }
        }
        public bool HasLost
        {
            get { return hasLost; }
            set { hasLost = value; }
        }

        public bool IsAlive()
        { return (health > 0); }

        public void ApplyDamage()
        {
            health -= 20;
        }

        public void Heal()
        {
            health += 30;
        }

        public void MovePlayerBy(int diceNumber, TileType[] gameMap)
        { position = (position + diceNumber) % gameMap.GetLength(0); }

        public void Teleport(int position, TileType[] gameMap)
        {
            if ((position >= 0) && (position < gameMap.GetLength(0)))
                this.position = position;
            else
                return;
        }

        public string ReturnStats()
        {
            if (IsAlive())
                return $"|PLAYER STATUS|Health: {health}|Alive and well.|";
            else
                return $"|PLAYER STATUS|Health: {health}|Dead and sad.|";
        }


    }
    class Kılınçarslan_1902406_HW3
    {
        static void Main(string[] args)
        {

            Random dice = new Random();
            Player player = new Player();
            bool afterFirst = false;

            //constructing the map
            TileType[] gameMapTiles = new TileType[15];
            TileNames[] tileNames = new TileNames[7];
            GenerateTiles(gameMapTiles);
            bool[,] mapBorders = GenerateBorders(gameMapTiles);

            //Console.WriteLine(TileType.Food); tested if the console outputs enums in string format.

            for (int i = 0; i < 7; i++)
                tileNames[i] = (TileNames)i;

            //first appearance of the board
            ColoredOutputter(ConsoleColor.Red, "______________AHOOY CAPTAIN, WELCOME TO THE BOARD!______________\n________________________________________________________________\n");
            PressToContinue(true);
            VisualizeMap(tileNames, gameMapTiles, mapBorders, player.Position);
            Console.WriteLine("\t\t(initial state of the board.)");
            ColoredOutputter(ConsoleColor.Green, "\n\t" + player.ReturnStats() + "\n");
            ColoredOutputter(ConsoleColor.Red, "\n" + GiveTileInfo());

            //main game loop.
            while (!player.HasLost && !player.HasWon)
            {

                if (afterFirst)
                {
                    VisualizeMap(tileNames, gameMapTiles, mapBorders, player.Position);
                    Console.WriteLine("\t(state of the board after the move)\n");
                    ColoredOutputter(ConsoleColor.Red, "\t(.. . ..tile gets activated... .. .)\n");
                    ActivateTile(gameMapTiles[player.Position], player, gameMapTiles);
                    ColoredOutputter(ConsoleColor.Green, "\n\t" + player.ReturnStats() + "\n\n");

                    if (player.HasLost || player.HasWon)
                    {
                        ColoredOutputter(ConsoleColor.Red, "______________AHOOY CAPTAIN, SOON TO BE ON BOARD!______________\n_______________________________________________________________\n");

                        if (player.HasLost)
                            ColoredOutputter(ConsoleColor.Red, "______________GAME IS OVER, AND YOU'VE LOST HAHA!______________\n_______________________________________________________________\n");
                        else
                            ColoredOutputter(ConsoleColor.Red, "______________GAME IS OVER, BUT YOU'VE WON HAHAA!______________\n_______________________________________________________________\n");

                        PressToContinue(true);
                        break;
                    }

                    if ((gameMapTiles[player.Position] == TileType.WormholeStart) || (gameMapTiles[player.Position] == TileType.WormholeEnd))
                    {
                        ColoredOutputter(ConsoleColor.Red, "Since you've landed on a W tile(wormhole tile), this. .. . ..means it's time.. . .for a...\n\n");
                        PressToContinue(false);
                        Console.WriteLine();

                        VisualizeMap(tileNames, gameMapTiles, mapBorders, player.Position);
                        Console.WriteLine("\t(state of the board after teleportation)\n");
                        PressToContinue(false);
                    }
                }

                bool flag = true;
                while (flag)
                {
                    Console.WriteLine("\n\n\tRoll the dice! Press \"D\".");
                    try
                    {
                        ConsoleKeyInfo keyPressed = Console.ReadKey();
                        if (keyPressed.Key != ConsoleKey.D)
                            throw new Exception("\n\tInvalid input!");
                        else
                            flag = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + "\n\tYou have to press D t roll the dice...\n");
                    }
                }

                afterFirst = true;
                int movesToDo = dice.Next(1, 5); //1D4 
                Console.WriteLine($"\n\tYou've rolled the dice, you have {movesToDo} available move(s) now!\n\n");
                player.MovePlayerBy(movesToDo, gameMapTiles);

                PressToContinue(true);
            }

        }


        static void GenerateTiles(TileType[] mapArray)
        {
            int mapLength = mapArray.GetLength(0);
            Random rand = new Random();

            for (int tile = 0; tile < mapLength; tile++)
            {
                if (tile == 0)
                    mapArray[tile] = TileType.Start;
                else if (tile == (mapLength - 1))
                    mapArray[tile] = TileType.Goal;
                else if (tile == 2) //fixed position for wormholes.
                    mapArray[tile] = TileType.WormholeStart;
                else if (tile == 11)
                    mapArray[tile] = TileType.WormholeEnd;
                else //random generation for empty, food, trap tiles.
                    mapArray[tile] = (TileType)rand.Next(2, 5);

            }
        }

        static bool[,] GenerateBorders(TileType[] mapArray)
        {
            bool[,] squares = new bool[3, (2 * mapArray.GetLength(0) + 1)]; //for borders and hollow parts for tiles.
            for (int row = 0; row < squares.GetLength(0); row++)
            {
                for (int column = 0; column < squares.GetLength(1); column++)
                {
                    if ((row == 1) && (column % 2 == 1))
                        squares[row, column] = true; //is a tile
                    else
                        squares[row, column] = false; //not a tile
                }
            }

            return squares;
        }

        static void VisualizeMap(TileNames[] tileNames, TileType[] tileArray, bool[,] borderArray, int position)
        {

            ConsoleColor borderColor = ConsoleColor.DarkYellow;
            ConsoleColor tileColor = ConsoleColor.Yellow;
            ConsoleColor playerColor = ConsoleColor.Magenta;

            for (int row = 0; row < borderArray.GetLength(0); row++)
            {
                int innerCount = 0;
                int offset = 1;

                for (int column = 0; column < borderArray.GetLength(1); column++)
                {
                    if (borderArray[row, column] == false)
                    {
                        if (row == 1)
                            ColoredOutputter(borderColor, "|");
                        else
                            ColoredOutputter(borderColor, "[]");
                    }
                    else if (borderArray[row, column] == true)
                    {
                        TileType tile = tileArray[innerCount];
                        int indexTileType = (int)tile;
                        TileNames tileName = (TileNames)indexTileType;

                        if (column == position + offset)
                            ColoredOutputter(playerColor, tileName);
                        else
                            ColoredOutputter(tileColor, tileName);
                        innerCount++;
                        offset++;

                    }

                }
                Console.WriteLine();
            }

        }

        static void ActivateTile(TileType tile, Player player, TileType[] tileMap)
        {
            if (player.IsAlive())
            {

                switch (tile)
                {
                    case TileType.Empty:
                        Console.WriteLine("Except for the wind blowing, nothing happened here.");
                        break;
                    case TileType.Food:
                        {

                            if (player.Health > 100)
                            {
                                player.Health = 100;
                                Console.WriteLine("Since you've already had a maxed out health, nothing happened.");
                            }
                            else
                            {
                                player.Heal();
                                Console.WriteLine("You've been granted with 30 HP. Hurray...");

                                if (player.Health >= 100)
                                {
                                    if (player.Health > 100)
                                        Console.WriteLine("You've maxed out your HP! Excess HP was given to a charity...");
                                    else if (player.Health == 100)
                                        Console.WriteLine("You've maxed out your HP! Yay!");

                                    player.Health = 100;
                                }

                            }
                        }
                        break;
                    case TileType.Goal:
                        {
                            player.HasWon = true;
                            Console.WriteLine("Congrats, you've reached your destination! Your path ends here...");
                        }
                        break;
                    case TileType.Start:
                        Console.WriteLine("So, you didn't make it- made it to the start of the board. Again.");
                        break;
                    case TileType.Trap:
                        {
                            player.ApplyDamage();

                            if (!player.IsAlive())
                            {
                                player.HasLost = true;
                                Console.WriteLine($"Your health has suffered too much damage from the ambush, game over...");
                            }
                            else
                                Console.WriteLine("Ouuch! Elves in the woods have been harsh on ya! You've lost 20HP...");
                        }
                        break;
                    case TileType.WormholeEnd:
                        {
                            player.Teleport(2, tileMap);
                            Console.WriteLine("You've stumbled across the end of a wormhole, you try to escape but you get teleported to its starting point anyways...");
                        }
                        break;
                    case TileType.WormholeStart:
                        {
                            player.Teleport(11, tileMap);
                            Console.WriteLine("There's a black hole- no, a wormhole! And it's its starting point, by going through you'll teleport to its end point! Yay!");
                        }
                        break;
                }
            }
            else
                player.HasLost = true;
        }

        static void PressToContinue(bool toClear)
        {
            bool flaggy = true;
            while (flaggy)
            {
                ColoredOutputter(ConsoleColor.Cyan, "\n\t\t.. ..Press E to continue.. .    .\n");

                try
                {
                    ConsoleKeyInfo keyyo = Console.ReadKey();
                    if (keyyo.Key == ConsoleKey.E)
                        flaggy = false;
                    else
                        throw new Exception();
                }
                catch
                {
                    ColoredOutputter(ConsoleColor.DarkCyan, "\n\t!.. .  ...Please press only the E button..  . ..!\n");
                }

            }
            if (toClear)
                Console.Clear();

        }
        static string GiveTileInfo()
        {
            return "\t|[Tile Information]|\n|[S]: Start(Your initial position.)|\n|[G]: Goal(You need to get here without dying!)|\n|[W]: Wormhole(Space travel!)|\n|[WE]: " +
                "End of the Wormhole(Oops! Going back...)|\n|[F]: Food(+30 yummines!)|\n|[T]: Trap(Ouch!-20 to yo health :<...)|\n|[E]: Empty(Wind's howling...Nothing here.)|" +
                "\n\n<When you occupy a tile, out of joy its color turns into a beautiful magenta...>";
        }

        static void ColoredOutputter(ConsoleColor color, string output)
        {
            ConsoleColor prev = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(output);
            Console.ForegroundColor = prev;
        }

        static void ColoredOutputter(ConsoleColor color, TileNames tile) //overloaded for printing TileType.
        {
            ConsoleColor prev = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write("[" + tile + "]");
            Console.ForegroundColor = prev;
        }

    }
}

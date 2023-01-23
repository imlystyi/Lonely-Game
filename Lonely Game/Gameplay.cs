using System.Runtime.Versioning;

namespace LonelyGame
{
    /// <summary>
    /// Provides gameplay process.
    /// </summary>
    [SupportedOSPlatform("windows")]

    internal class Gameplay
    {
        readonly Player player = new();
        readonly Enemy enemy = new();
        readonly Graphics graphics = new();

        int player_xPos = 0, player_yPos = 0,
            enemy_xPos = 0, enemy_yPos = 0,
            mPos = 0;

        // These functions provide system beep sound that depends by its meaning: error beep, beep with the 
        // successful attack, etc.
        static void ArrowsBeep() => Console.Beep(300, 50);
        static void BuildBeep() => Console.Beep(250, 100);
        static void DestroyBeep() => Console.Beep(220, 100);
        static void SuccessfulAttackBeep() => Console.Beep(200, 100);
        static void MishitBeep() => Console.Beep(180, 200);
        static void ErrorBeep() => Console.Beep(150, 100);

        /// <summary>
        /// Starts the game process of the factories construction.
        /// </summary>
        /// <remarks>
        /// Ends when the player has constructed the factories with the <see cref="Player.MAX_FACTORIES_COUNT"/> count, and then calls <see cref="StartBattleProcess()"/> function.
        /// </remarks>
        void StartConstructionProcess()
        {
            mPos = 0;

            graphics.DisplayGameScreen(player_xPos, player_yPos, player.SunkenShipsCount, enemy.DestroyedFactoriesCount);

            while (true && player.factories.Count < Player.MAX_FACTORIES_COUNT)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        ArrowsBeep();

                        player_yPos -= player_yPos > 0 ? 1 : 0;

                        break;

                    case ConsoleKey.DownArrow:
                        ArrowsBeep();

                        player_yPos += player_yPos < Player.FIELD_SIZE - 1 ? 1 : 0;

                        break;

                    case ConsoleKey.LeftArrow:
                        ArrowsBeep();

                        player_xPos -= player_xPos > 0 ? 1 : 0;

                        break;

                    case ConsoleKey.RightArrow:
                        ArrowsBeep();

                        player_xPos += player_xPos < Player.FIELD_SIZE - 1 ? 1 : 0;

                        break;

                    case ConsoleKey.B:
                        if (player.BuildFactory(player_xPos, player_yPos))
                        {
                            BuildBeep();

                            graphics.UpdatePlayerField(player_xPos, player_yPos, Graphics.CellTypes.Factory);
                            graphics.DisplayMessage("Factory was successfuly built!");
                        }

                        else
                        {
                            ErrorBeep();

                            graphics.DisplayMessage("Factory is already built in this zone!");
                        }

                        break;

                    case ConsoleKey.D:
                        if (player.DestroyFactory(player_xPos, player_yPos))
                        {
                            DestroyBeep();

                            graphics.UpdatePlayerField(player_xPos, player_yPos, Graphics.CellTypes.Sea);
                            graphics.DisplayMessage("Factory was successfuly destroyed!");
                        }

                        else
                        {
                            ErrorBeep();

                            graphics.DisplayMessage("There are no factories to destroy in this cell!");
                        }

                        break;

                    case ConsoleKey.M:
                        ShowMenu(true);

                        break;

                    case ConsoleKey.Enter:
                        graphics.DisplayMessage($"{player_xPos}, {player_yPos}");

                        break;
                }

                graphics.DisplayGameScreen(player_xPos, player_yPos, player.SunkenShipsCount, enemy.DestroyedFactoriesCount);
            }

            // Starts the next part of the game.
            StartBattleProcess();
        }

        /// <summary>
        /// Starts the game process of the battle.
        /// </summary>
        /// <remarks>
        /// Ends when the player has sunk all of the enemy's ships (with the <see cref="Enemy.MAX_SHIPS_COUNT"/> count), or when the enemy has destroyed all of the player's factories (with the <see cref="Player.MAX_FACTORIES_COUNT"/> count).
        /// </remarks>. 
        void StartBattleProcess()
        {
            player_xPos = 0; player_yPos = 0; mPos = 0;

            graphics.DisplayGameScreen(player_xPos + Player.FIELD_SIZE + 1, player_yPos, player.SunkenShipsCount, enemy.DestroyedFactoriesCount);

            while (true && player.SunkenShipsCount < Enemy.MAX_SHIPS_COUNT && enemy.DestroyedFactoriesCount < Player.MAX_FACTORIES_COUNT)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        ArrowsBeep();

                        player_yPos -= player_yPos > 0 ? 1 : 0;

                        break;

                    case ConsoleKey.DownArrow:
                        ArrowsBeep();

                        player_yPos += player_yPos < Enemy.FIELD_SIZE - 1 ? 1 : 0;

                        break;

                    case ConsoleKey.LeftArrow:
                        ArrowsBeep();

                        player_xPos -= player_xPos > 0 ? 1 : 0;

                        break;

                    case ConsoleKey.RightArrow:
                        ArrowsBeep();

                        player_xPos += player_xPos < Enemy.FIELD_SIZE - 1 ? 1 : 0;

                        break;

                    case ConsoleKey.M:
                        ShowMenu();

                        break;

                    case ConsoleKey.A:
                        if (player.AttackEnemy(enemy, player_xPos, player_yPos))
                        {
                            SuccessfulAttackBeep();

                            _ = graphics.UpdateEnemyField(player_xPos, player_yPos, Graphics.CellTypes.Destroyed);

                            graphics.DisplayMessage("You hit the ship! Press any key to\n" +
                                "see the enemy's attack!");
                        }

                        else
                        {
                            MishitBeep();

                            if (graphics.UpdateEnemyField(player_xPos, player_yPos, Graphics.CellTypes.Sea))
                            {
                                graphics.DisplayMessage("You missed! Press any key to see\n" +
                                    "the enemy's attack!");
                            }

                            else
                            {
                                graphics.DisplayMessage("You hitted the same cell again... \n" +
                                    "why?\n" +
                                    "However, press any key to see\n" +
                                    "the enemy's attack!");
                            }
                        }

                        graphics.DisplayGameScreen(player_xPos + Player.FIELD_SIZE + 1, player_yPos, player.SunkenShipsCount, enemy.DestroyedFactoriesCount);

                        Console.ReadKey(true);

                        if (enemy.AttackPlayer(player, out enemy_xPos, out enemy_yPos))
                        {
                            SuccessfulAttackBeep();

                            graphics.UpdatePlayerField(enemy_xPos, enemy_yPos, Graphics.CellTypes.Destroyed);

                            graphics.DisplayMessage($"Enemy shooted your factory in ({enemy_xPos}, {enemy_yPos}) cell!\n" +
                                $"Press any key.");
                        }

                        else
                        {
                            MishitBeep();

                            graphics.UpdatePlayerField(enemy_xPos, enemy_yPos, Graphics.CellTypes.OpenedByEnemy);

                            graphics.DisplayMessage($"Enemy missed you in ({enemy_xPos}, {enemy_yPos}) cell!\n" +
                                $"Press any key.");
                        }

                        graphics.DisplayGameScreen(player_xPos + Player.FIELD_SIZE + 1, player_yPos, player.SunkenShipsCount, enemy.DestroyedFactoriesCount);

                        break;

                    case ConsoleKey.Enter:
                        graphics.DisplayMessage($"{player_xPos}, {player_yPos}");
                        graphics.DisplayGameScreen(player_xPos + Player.FIELD_SIZE + 1, player_yPos, player.SunkenShipsCount, enemy.DestroyedFactoriesCount);
                        break;
                }

                graphics.DisplayGameScreen(player_xPos + Player.FIELD_SIZE + 1, player_yPos, player.SunkenShipsCount, enemy.DestroyedFactoriesCount);
            }

            // Shows win or loss messages and statistics.
            graphics.DisplayMessage((player.SunkenShipsCount == Enemy.MAX_SHIPS_COUNT ? "Victory!\n" : "Loss!\n") +
                $"Totally:\n" +
                $"* {player.MishitsCount} mishits by player\n" +
                $"* {enemy.MishitsCount} mishits by enemy\n" +
                $"* {player.SunkenShipsCount} sunken ships\n" +
                $"* {enemy.DestroyedFactoriesCount} destroyed factories\n" +
                $"(press R to retry, Q to quit!)");

            graphics.DisplayGameScreen(player_xPos, player_yPos, player.SunkenShipsCount, enemy.DestroyedFactoriesCount);

            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.R:
                        Main();

                        break;

                    case ConsoleKey.Q:
                        Environment.Exit(0);

                        break;
                }
            }
        }

        /// <summary>
        /// Shows a menu that can be controlled by user.
        /// </summary>
        /// <param name="isConstructionProcess">Determines whether this funtion was called from <see cref="StartConstructionProcess()"/> function.</param>
        void ShowMenu(bool isConstructionProcess = false)
        {
            mPos = 0;

            Graphics.DisplayMenuScreen(mPos);

            while (true)
            {
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow:
                        mPos -= mPos > 0 ? 1 : 0;
                        Graphics.DisplayMenuScreen(mPos);
                        break;

                    case ConsoleKey.DownArrow:
                        mPos += mPos < Graphics.MENU_OPTIONS_COUNT ? 1 : 0;
                        Graphics.DisplayMenuScreen(mPos);
                        break;

                    case ConsoleKey.Enter:
                        switch (mPos)
                        {
                            case 0:
                                // Checks which process to return to.
                                if (isConstructionProcess)
                                {
                                    StartConstructionProcess();
                                }

                                else
                                {
                                    StartBattleProcess();
                                }

                                break;

                            case 1:
                                Graphics.DisplayHelpScreen();

                                while (true)
                                {
                                    switch (Console.ReadKey(true).Key)
                                    {
                                        case ConsoleKey.M:
                                            ShowMenu(isConstructionProcess);

                                            break;
                                    }
                                }

                            case 2:
                                Graphics.DisplayAboutScreen();

                                while (true)
                                {
                                    switch (Console.ReadKey(true).Key)
                                    {
                                        case ConsoleKey.M:
                                            ShowMenu(isConstructionProcess);

                                            break;
                                    }
                                }

                            case 3:
                                // Starts a new game loop.
                                Main();

                                break;

                            case 4:
                                Environment.Exit(0);

                                break;
                        }
                        break;
                }
            }
        }

        static void Main()
        {
            // Sets the preferences of the console.
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;
            // Starts a game loop.
            Gameplay game = new();
            game.StartConstructionProcess();
        }
    }
}
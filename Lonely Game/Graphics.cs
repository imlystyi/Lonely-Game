namespace LonelyGame
{
    /// <summary>
    /// Provides graphics component of the game.
    /// </summary>
    public class Graphics
    {
        /// <summary>
        /// Specifies the possible types of the cell. 
        /// </summary>
        public enum CellTypes { Factory = '●', Destroyed = '✶', Sea = '~', OpenedByEnemy = 'x', Unknown = '#' };

        public const int MENU_OPTIONS_COUNT = 5;

        private readonly char[,] enemyField = new char[10, 10]
        {
            {'#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            {'#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            {'#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            {'#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            {'#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            {'#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            {'#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            {'#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            {'#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
            {'#', '#', '#', '#', '#', '#', '#', '#', '#', '#'},
        },

        playerField = new char[Player.FIELD_SIZE, Player.FIELD_SIZE]
        {
            { '~', '~', '~', '~', '~', '~', '~', '~', '~', '~'},
            { '~', '~', '~', '~', '~', '~', '~', '~', '~', '~'},
            { '~', '~', '~', '~', '~', '~', '~', '~', '~', '~'},
            { '~', '~', '~', '~', '~', '~', '~', '~', '~', '~'},
            { '~', '~', '~', '~', '~', '~', '~', '~', '~', '~'},
            { '~', '~', '~', '~', '~', '~', '~', '~', '~', '~'},
            { '~', '~', '~', '~', '~', '~', '~', '~', '~', '~'},
            { '~', '~', '~', '~', '~', '~', '~', '~', '~', '~'},
            { '~', '~', '~', '~', '~', '~', '~', '~', '~', '~'},
            { '~', '~', '~', '~', '~', '~', '~', '~', '~', '~'}
        };

        private string messageField = "";

        /// <summary>
        /// Updates the type of the cell on the player's field.
        /// </summary>
        /// <param name="xPos">The abscissa of the cell that type should be updated.</param>
        /// <param name="yPos">The ordinate of the cell that type should be updated.</param>
        /// <param name="cellStatus">The type to which the cell is updated. Must be defined in <see cref="CellTypes"/> enum.</param>
        public void UpdatePlayerField(int xPos, int yPos, CellTypes cellStatus) => playerField[xPos, yPos] = (char)cellStatus;

        /// <summary>
        /// Updates the type of the cell on the enemy's field.
        /// </summary>
        /// <remarks>
        /// The cell will be updated only if its type before the updating is equal to <see cref="CellTypes.Unknown"/>.
        /// </remarks>
        /// <param name="xPos">The abscissa of the cell that type should be updated.</param>
        /// <param name="yPos">The ordinate of the cell that type should be updated.</param>
        /// <param name="cellStatus">The type to which the cell is updated. Must be defined in <see cref="CellTypes"/> enum.</param>
        /// <returns><see langword="true"/> if the cell has been updated; otherwise, <see langword="false"/>.</returns>
        public bool UpdateEnemyField(int xPos, int yPos, CellTypes cellStatus)
        {
            if (enemyField[xPos, yPos].Equals((char)CellTypes.Unknown))
            {
                enemyField[xPos, yPos] = (char)cellStatus;

                return true;
            }

            else
            {
                return false;
            }
        }

        /// <summary>
        /// Displays the game screen with the cursor at the specified coordinates.
        /// </summary>
        /// <param name="xPos">The abscissa of the cursor.</param>
        /// <param name="yPos">The ordinate of the cursor.</param>
        /// <param name="sunkenShipsCount">The count of ships sunken by the player.</param>
        /// <param name="destroyedFactoriesCount">The count of factories destroyed by the enemy.</param>
        public void DisplayGameScreen(int xPos, int yPos, int sunkenShipsCount, int destroyedFactoriesCount)
        {
            Console.Clear();

            for (int y = 0; y < Enemy.FIELD_SIZE; y++)
            {
                for (int x = 0; x < Player.FIELD_SIZE + Enemy.FIELD_SIZE + 1; x++)
                {
                    if (x == xPos && y == yPos)
                    {
                        Console.Write("+ ");
                        continue;
                    }

                    if (x < Player.FIELD_SIZE && y < Player.FIELD_SIZE)
                    {
                        Console.Write(playerField[x, y].ToString() + ' ');

                        continue;
                    }

                    else if (x == Player.FIELD_SIZE)
                    {
                        Console.Write('║');

                        continue;
                    }

                    else
                    {
                        Console.Write(enemyField[x - Player.FIELD_SIZE - 1, y].ToString() + ' ');

                        continue;
                    }
                }

                Console.Write("\n");
            }

            Console.WriteLine(
                "════════════════════╩═══════════════════\n" +
                "(press M to enter the menu)\n" +
                "════════════════════════════════════════\n" +
                $"Ships, sunken by you: {sunkenShipsCount}\n" +
                $"Factories, destroyed by enemy: {destroyedFactoriesCount}\n" +
                "════════════════════════════════════════\n" +
                messageField);
        }

        /// <summary>
        /// Changes the <see cref="messageField"/> to the specified string.
        /// </summary>
        /// <param name="message">The string to which <see cref="messageField"/> should be changed.</param>
        public void DisplayMessage(string message) => messageField = message;

        /// <summary>
        /// Displays the menu screen with the cursor at the specified position.
        /// </summary>
        /// <param name="mPos">The position of the cursor.</param>
        public static void DisplayMenuScreen(int mPos)
        {
            Console.Clear();

            Console.WriteLine((mPos == 0 ? '>' : ' ') + " Continue\n" +
                (mPos == 1 ? '>' : ' ') + " Help\n" +
                (mPos == 2 ? '>' : ' ') + " About\n" +
                (mPos == 3 ? '>' : ' ') + " Retry\n" +
                (mPos == 4 ? '>' : ' ') + " Quit\n" +
                "(control by Up and Down arrows and Enter key)");
        }

        /// <summary>
        /// Displays the help screen.
        /// </summary>
        public static void DisplayHelpScreen()
        {
            Console.Clear();

            Console.WriteLine("For you, the main goal of the game is to destroy all enemy ships. \n" +
                "For the enemy - to destroy all your factories. It's very simple - first set up your factories, then hit enemy ships.\n" +
                "\"I started the game. What to do?\"\n" +
                "1. First part of the game - construction a factories. Move the cursor on your field (left) with the arrows key, build factories with\n" +
                "the 'B' key, destroy with the 'D' key. Keep in mind: you cannot build factories next to each other, the minimum distance is 1 cell\n" +
                "in a straight line.\n" +
                "2. After you've built 4 factories, it's battle time! First you attack - control the cursor with arrows key, attack with the 'B' key.\n" +
                "Keep in mind: enemy's ships cannot be located next to each other, the minimum distance, as for your factories, is one cell in a straight line.\n" +
                "3. Fight to win. Your goal is to sunk all enemy ships. The enemy's goal is to destroy out all your enterprises!\n" +
                "Good luck!\n" +
                "(back to menu - press M)");
        }

        /// <summary>
        /// Displays the about screen.
        /// </summary>
        public static void DisplayAboutScreen()
        {
            Console.Clear();

            Console.WriteLine("Lonely Game, v. 0.1,\n" +
                "by imlystyi.\n" +
                "(back to menu - press M)");
        }
    }
}

namespace LonelyGame
{
    /// <summary>
    /// Represents player that is controled by user.
    /// </summary>
    public class Player
    {
        public const int FIELD_SIZE = 10,
            MAX_FACTORIES_COUNT = 4;

        /// <summary>
        /// Gets or sets the count of ships sunken by the player.
        /// </summary>
        /// <remarks>
        /// Cannot be set outside the class or instance methods.
        /// </remarks>
        /// <returns>
        /// The count of ships sunken by the player.
        /// </returns>
        public int SunkenShipsCount
        {
            private set;
            get;
        } = 0;

        /// <summary>
        /// Gets or sets the count of player's mishits.
        /// </summary>
        /// <remarks>
        /// Cannot be set outside the class or instance methods.
        /// </remarks>
        /// <returns>
        /// The count of player's mishits.
        /// </returns>
        public int MishitsCount
        {
            private set;
            get;
        } = 0;

        /// <summary>
        /// Contains the coordinates of the factories of the player.
        /// </summary>
        public List<int[]> factories = new(MAX_FACTORIES_COUNT);

        /// <summary>
        /// Attacks the cell with the specified coordinates on the enemy's field.
        /// </summary>
        /// <param name="enemy">Attacked enemy</param>
        /// <param name="xPos">The abscissa of the cell that should be attacked.</param>
        /// <param name="yPos">The ordinate of the cell that should be attacked.</param>
        /// <returns><see langword="true"/> if the player hit the enemy's ship; otherwise, <see langword="false"/>.</returns>
        public bool AttackEnemy(Enemy enemy, int xPos, int yPos)
        {
            int[] target = new int[2] { xPos, yPos };

            int[][] bufferPosition;

            foreach (Ship ship in enemy.ships)
            {
                bufferPosition = ship.Position;

                ship.Position = ship.Position.Where((_, index) => index != Array.FindIndex(ship.Position, 
                    match:
                    coords => coords.SequenceEqual(target))).ToArray();

                if (!ship.Position.SequenceEqual(bufferPosition))
                {
                    ship.Health--;

                    SunkenShipsCount += (ship.Health == 0) ? 1 : 0;

                    return true;
                }

                else
                {
                    continue;
                }
            }

            MishitsCount++;

            return false;
        }

        /// <summary>
        /// Creates a factory on a cell with the specified coordinates on the player's field and adds it in <see cref="factories"/> list.
        /// </summary>
        /// <remarks>
        /// If there is already a factory in this cell or construction zone, a new one will not be created.
        /// </remarks>
        /// <param name = "xPos" > The abscissa of the cell in which the factory should be created.</param>
        /// <param name = "yPos" > The ordinate of the cell in which the factory should be created.</param>
        /// <returns><see langword="true"/> if the factory was created; otherwise, <see langword="false"/>.</returns>
        public bool BuildFactory(int xPos, int yPos)
        {
            // constructionZone is an array of the coordinates of the cells in which the factory cannot be created: 
            // it contains the coordinates of the cell of the factory itself, and the cells adjacent to it 
            // (left-right and top-bottom). The zero (0) element of constructionZone is the cell in which factory 
            // will be built. 
            int[][] constructionZone = new int[5][]
            {
                new int[2] { xPos, yPos },
                new int[2] { xPos + 1, yPos},
                new int[2] {xPos - 1, yPos},
                new int[2] {xPos, yPos + 1},
                new int[2] {xPos, yPos - 1}
            };

            for (int ii = 0; ii < constructionZone.Length; ii++)
            {
                if (factories.Find(match:
                    coords => coords.SequenceEqual(constructionZone[ii])) != default(int[]))
                {
                    return false;
                }
            }

            factories.Add(constructionZone[0]);

            return true;
        }

        /// <summary>
        /// Deletes a factory on a cell with the specified coordinates on the player's field and deletes it from the <see cref="factories"/> list.
        /// </summary>
        /// <remarks>
        /// If there is no factory in this cell, nothing will be deleted.
        /// </remarks>
        /// (<paramref name="xPos"/>,<paramref name="yPos"/>).
        /// <param name = "xPos" > The abscissa of the cell in which the factory should be deleted</param>
        /// <param name = "yPos" > The ordinate of the cell in which the factory should be deleted</param>
        /// <returns><see langword="true"/> if the factory was destroyed; otherwise, <see langword="false"/>.</returns>
        public bool DestroyFactory(int xPos, int yPos)
        {
            int[] cellToDestroyIn = new int[2] { xPos, yPos };

            if (factories.Find(match:
                coords => coords.SequenceEqual(cellToDestroyIn)) == default(int[]))
            {
                return false;
            }

            else
            {
                factories.RemoveAll(match:
                    coords => coords.SequenceEqual(cellToDestroyIn));

                return true;
            }
        }
    }
}

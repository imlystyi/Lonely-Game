namespace LonelyGame
{
    /// <summary>
    /// Represents opponent with the primitive AI.
    /// </summary>
    public class Enemy
    {
        public const int FIELD_SIZE = 10,
            MAX_SHIPS_COUNT = 7;

        /// <summary>
        /// Gets or sets the count of factories destroyed by the enemy.
        /// </summary>
        /// <remarks>
        /// Cannot be set outside the class or instance methods.
        /// </remarks>
        /// <returns>
        /// The count of factories destroyed by the enemy.
        /// </returns>
        public int DestroyedFactoriesCount
        {
            get;
            private set;
        } = 0;

        /// <summary>
        /// Gets or sets the count of enemy's mishits.
        /// </summary>
        /// <remarks>
        /// Cannot be set outside the class or instance methods.
        /// </remarks>
        /// <returns>
        /// The count of enemy's mishits.
        /// </returns>
        public int MishitsCount
        {            
            get;
            private set;
        } = 0;

        /// <summary>
        /// Contains ships that the enemy has.
        /// </summary>
        public List<Ship> ships = new()
        {
            new Ship(5),
            new Ship(4),
            new Ship(3),
            new Ship(2),
            new Ship(2),
            new Ship(1),
            new Ship(1),
        };

        private readonly Random random = new();

        /// <summary>
        /// Contains possible targets for attack.
        /// </summary>
        private readonly List<int[]> targets = new();

        /// <summary>
        /// Sets the random position for the all ships in <see cref="ships"/> list.
        /// </summary>
        private void InitializeShips()
        {
            List<int[]> goodPositions = new();

            foreach (Ship ship in ships)
            {
                ship.SetRandomPosition(ref goodPositions);
            }
        }

        /// <summary>
        /// Excepts the coordinates of the construction zone of the factory from the <see cref="targets"/> list.
        /// </summary>
        /// <param name="xPos">The abscissa of the factory cell that construction zone coordinates should be excepted from list.</param>
        /// <param name="yPos">The ordinate of the factory cell that construction zone coordinates should be excepted from list.</param>
        private void ExceptConstructionZoneCoords(int xPos, int yPos)
        {
            List<int[]> badTargets = new()
            {
                new int[] { xPos, yPos },
                new int[] { xPos - 1, yPos },
                new int[] { xPos, yPos - 1 },
                new int[] { xPos + 1, yPos },
                new int[] { xPos, yPos + 1 }
            };

            for (int ii = 0; ii < badTargets.Count; ii++)
            {
                _ = targets.RemoveAll(match:
                    coords => coords.SequenceEqual(badTargets[ii]));
            }
        }

        /// <summary>
        /// Attacks the cell with the specified coordinates on the player's field.
        /// </summary>
        /// <param name="player">Attacked player.</param>
        /// <param name="xPos">The abscissa of the cell that was attacked.</param>
        /// <param name="yPos">The ordinate of the cell that was attacked.</param>
        /// <returns><see langword="true"/> if the enemy hit the player's factory; otherwise, <see langword="false"/>.</returns>
        public bool AttackPlayer(Player player, out int xPos, out int yPos)
        {
            int randomTargetIndex = random.Next(0, targets.Count);

            xPos = targets[randomTargetIndex][0];
            yPos = targets[randomTargetIndex][1];

            int[] target = { xPos, yPos };

            // If the enemy hit the player's factory, the coordinates of its cell removes from the list of the targets
            // and the coordinates of its construction zone excepts from the list of the targets.
            if (player.factories.Find(match:
                coords => coords.SequenceEqual(target)) != default(int[]))
            {
                _ = player.factories.RemoveAll(element => element.SequenceEqual(target));

                ExceptConstructionZoneCoords(xPos, yPos);

                DestroyedFactoriesCount++;

                return true;
            }

            // Else, only the coordinates of the cell attacked by the enemy is removed from the list of the targets.
            else
            {
                _ = targets.RemoveAll(match:
                    coords => coords.SequenceEqual(target));

                return false;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Enemy"/> class. Calls <see cref="InitializeShips()"/> function and creates a new <see cref="targets"/> list.
        /// </summary>
        public Enemy()
        {
            InitializeShips();

            for (int x = 0; x < Player.FIELD_SIZE; x++)
                for (int y = 0; y < Player.FIELD_SIZE; y++)
                    targets.Add(new int[] { x, y });
        }
    }
}

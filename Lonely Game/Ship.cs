namespace LonelyGame
{
    /// <summary>
    /// Represents targets with unique characteristics that the player can hit.
    /// </summary>
    public class Ship
    {
        /// <summary>
        /// Specifies the possible directions of the ship. 
        /// </summary>
        public enum Directions
        {
            Vertical,
            Horizontal
        };

        /// <summary>
        /// Describes the length of the ship.
        /// </summary>
        private readonly int length;

        private object direction = Directions.Vertical;
        /// <summary>
        /// Gets the direction of the ship. Can not be set and can be only initialized in the constructor.
        /// </summary>
        /// <returns>
        /// The direction of the ship that is defined in <see cref="Directions"/> enum.
        /// </returns>
        public object Direction
        {
            get => direction;

            private init => direction = Enum.IsDefined((Directions)value)
                    ? direction = value
                    : throw new ArgumentException("Bad ship initialization: direction not defined in enum of directions", nameof(direction).ToString());
        }

        private int[][] position = Array.Empty<int[]>();
        /// <summary>
        /// Gets or sets the position of the ship on the enemy's field.
        /// </summary>
        /// <returns>
        /// The position of the ship on the enemy's field.
        /// </returns>
        public int[][] Position
        {
            get => position;

            set => position = value != null
                ? (int[][])value.Clone()
                : throw new ArgumentNullException(nameof(position).ToString(), "Bad ship initialization: position can not be null");
        }


        private int health;
        /// <summary>
        /// Gets or sets the health of the ship. 
        /// </summary>
        /// <remarks>
        /// The value must be less than or equal to <see cref="length"/> and greater than or equal to 0.
        /// </remarks>
        /// <returns>
        /// The health of the ship.
        /// </returns>
        public int Health
        {
            get => health;

            set => health = (value <= length && value >= 0)
                ? value
                : throw new ArgumentOutOfRangeException(nameof(position).ToString(), "Bad ship initialization: value of health was out of range.");
        }

        /// <summary>
        /// Gets the random direction from <see cref="Directions"/> enum.
        /// </summary>
        /// <returns>
        /// The random direction that is defined in <see cref="Directions"/> enum.
        /// </returns>
        static Directions GetRandomDirection()
        {
            Random random = new();

            return (random.NextDouble() < 0.5) ? Directions.Horizontal : Directions.Vertical;
        }

        /// <summary>
        /// Sets the random position for the ship from a certain list of free ones.
        /// </summary>
        /// <param name="spawnablePositions">The list of positions where the ship can be located. If empty, takes all positions from the enemy field. Ship positions are removed from it after their generation.</param>
        /// <exception cref="ArgumentException"></exception>
        public void SetRandomPosition(ref List<int[]> spawnablePositions)
        {
            if (!spawnablePositions.Any())
            {
                for (int x = 0; x < Enemy.FIELD_SIZE; x++)
                {
                    for (int y = 0; y < Enemy.FIELD_SIZE; y++)
                    {
                        spawnablePositions.Add(new int[] { x, y });
                    }
                }
            }

            Random random = new();

            int[] cell;

            // A list of position potentially possible for generation for other ships, but not possible for this one.
            List<int[]> sieve = new(spawnablePositions);

            while (sieve.Any() && Position.GetLength(0) != length)
            {
                cell = new int[] { sieve[random.Next(0, sieve.Count)][0], sieve[random.Next(0, sieve.Count)][1] };

                if (Direction.Equals(Directions.Horizontal))
                {
                    for (int ii = 0; ii < length; ii++)
                    {
                        if (spawnablePositions.Find(match:
                            coords => coords.SequenceEqual(new int[] { cell[0] + ii, cell[1] })) != default(int[]))
                        {
                            Position = Position.Append(new int[] { cell[0] + ii, cell[1] }).ToArray();

                            continue;
                        }

                        else
                        {
                            Position = Array.Empty<int[]>();

                            sieve.RemoveAll(match:
                                coords => coords.SequenceEqual(cell));

                            break;
                        }
                    }

                    if (!Position.Any())
                    {
                        for (int jj = 0; jj < length; jj++)
                        {
                            if (spawnablePositions.Find(match:
                                coords => coords.SequenceEqual(new int[] { cell[0] - jj, cell[1] })) != default(int[]))
                            {
                                Position = Position.Append(new int[] { cell[0] - jj, cell[1] }).ToArray();

                                continue;
                            }

                            else
                            {
                                Position = Array.Empty<int[]>();

                                sieve.RemoveAll(match:
                                    coords => coords.SequenceEqual(cell));

                                break;
                            }
                        }
                    }
                }

                else if (Direction.Equals(Directions.Vertical))
                {
                    for (int kk = 0; kk < length; kk++)
                    {
                        if (spawnablePositions.Find(match:
                            coords => coords.SequenceEqual(new int[] { cell[0], cell[1] + kk })) != default(int[]))
                        {
                            Position = Position.Append(new int[] { cell[0], cell[1] + kk }).ToArray();

                            continue;
                        }

                        else
                        {
                            Position = Array.Empty<int[]>();

                            sieve.RemoveAll(match:
                                coords => coords.SequenceEqual(cell));

                            break;
                        }
                    }

                    if (!Position.Any())
                    {
                        for (int ll = 0; ll < length; ll++)
                        {
                            if (spawnablePositions.Find(match:
                                coords => coords.SequenceEqual(new int[] { cell[0], cell[1] - ll })) != default(int[]))
                            {
                                Position = Position.Append(new int[] { cell[0], cell[1] - ll }).ToArray();

                                continue;
                            }

                            else
                            {
                                Position = Array.Empty<int[]>();

                                break;
                            }
                        }
                    }
                }

                else
                {
                    throw new ArgumentException("Bad ship position generation: invalid direction", nameof(Direction).ToString());
                }
            }

            List<int[]> coordsToExcept = new();

            for (int ii = 0; ii < length; ii++)
            {
                coordsToExcept.AddRange(new int[][]
                {
                    new int[] { Position[ii][0], Position[ii][1] },
                    new int[] { Position[ii][0] + 1, Position[ii][1] },
                    new int[] { Position[ii][0] - 1, Position[ii][1] },
                    new int[] { Position[ii][0], Position[ii][1] + 1 },
                    new int[] { Position[ii][0], Position[ii][1] - 1 }
                });
            }

            for (int ii = 0; ii < coordsToExcept.Count; ii++)
            {
                _ = spawnablePositions.RemoveAll(match:
                    coords => coords.SequenceEqual(coordsToExcept[ii]));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ship"/> class with the specified length, direction and position.
        /// </summary>
        /// <param name="length">The length of the new ship.</param>
        /// <param name="direction">The direction of the new ship. Must be defined in <see cref="Directions"/> enum. If <see langword="null"/>, gets value by <see cref="GetRandomDirection"/> function.</param>
        /// <param name="position">The position of the new ship. If <see langword="null"/>, gets the value of an empty array of <see cref="int"/>[].</param>
        /// <exception cref="ArgumentException"></exception>
        public Ship(int length, Directions? direction = null, int[][]? position = null)
        {
            this.length = length;

            Direction = direction ?? GetRandomDirection();

            if (position != null)
                if (position.GetLength(1) == this.length)
                    Position = position;

                else throw new ArgumentException("Bad ship initialization: invalid position (too few coordinates)", nameof(position));

            Health = this.length;
        }
    }
}

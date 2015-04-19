using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace GameOfLife
{
    [TestFixture]
    public class GameOfLifeTest
    {
        public class Location
        {
            public int X { get; private set; }
            public int Y { get; private set; }
            
            public Location(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            public IEnumerable<Cell> ReturnNeighboursOf(IEnumerable<Cell> cells)
            {
                return cells.Where(l => this.X >= l.Location.X-1 &&
                                   this.X <= l.Location.X+1 &&
                                   this.Y >= l.Location.Y-1 &&
                                   this.Y <= l.Location.Y+1 &&
                                   !this.Equals(l.Location));
            }

            public IEnumerable<Location> NeighbouringLocations()
            {
                return new List<Location>()
                {
                    new Location(X-1, Y),
                    new Location(X-1, Y-1),
                    new Location(X-1, Y+1),
                    new Location(X, Y-1),
                    new Location(X, Y+1),
                    new Location(X+1, Y-1),
                    new Location(X+1, Y),
                    new Location(X+1, Y+1)
                };
            }
            
            public override bool Equals(object obj)
            {
                var location = (Location)obj;
                return location.X == this.X && location.Y == this.Y;
            }

            public override int GetHashCode()
            {
                return X ^ Y;
            }
        }

        public abstract class Cell
        {
            public Location Location { get; private set; }
            
            public Cell(Location location)
            {
                this.Location = location;
            }
        }
        
        public class AliveCell : Cell
        {
            public AliveCell(Location location)
                : base(location)
            {
            }
        }

        public class DeadCell : Cell
        {
            public DeadCell(Location location)
                : base(location)
            {
            }
        }

        public class World
        {
            IEnumerable<Cell> livingCells = new List<Cell>();
            
            public void SetLiveCellAt(Location location)
            {
                var cells = new List<Cell>();
                cells.AddRange(livingCells);
                cells.Add(new AliveCell(location));
                livingCells = cells;
            }
            
            public Cell GetCellAt(Location location)
            {
                if(livingCells.Any(cell => cell.Location.Equals(location)))
                {
                    return new AliveCell(location);
                }

                return new DeadCell(location);
            }

            private IEnumerable<AliveCell> CellsToComeAlive()
            {
                var cellsToBeRessurected = new List<AliveCell>();
                var deadLocations = new HashSet<Location>();

                foreach(Cell livingCell in livingCells)
                {
                    foreach(Location neighbour in livingCell.Location.NeighbouringLocations())
                    {
                        deadLocations.Add(neighbour);
                    }
                }
                
                foreach (Location deadLocation in deadLocations)
                {
                    if (deadLocation.ReturnNeighboursOf(livingCells).Count() == 3)
                    {
                        cellsToBeRessurected.Add(new AliveCell(deadLocation));
                    }
                }
                return cellsToBeRessurected;
            }
            
            private List<Cell> CellsToDie()
            {
                var dyingCells = new List<Cell>();
                foreach (var livingCell in livingCells)
                {
                    int neighbours = livingCell.Location.ReturnNeighboursOf(this.livingCells).Count();
                    if (neighbours < 2 || neighbours > 3)
                    {
                        dyingCells.Add(livingCell);
                    }
                }
                return dyingCells;
            }
            
            public World Tick()
            {
                var nextWorldCells= new List<Cell>(this.livingCells);
                var dyingCells = CellsToDie();
                nextWorldCells.RemoveAll(cell => dyingCells.Contains(cell));
                nextWorldCells.AddRange(CellsToComeAlive());
                
                return new World()
                {
                    livingCells = nextWorldCells
                };
            }
        }

        [Test]
        public void LocationReturnsAllPossibleNeighbours()
        {
            var neighbouringLocations = new Location(1, 1).NeighbouringLocations();
            Assert.AreEqual(8, neighbouringLocations.Count(), "count");
        }
        
        [Test]
        public void DeadCellWithExactlyThreeLivingNeighboursBecomesALiveCellAfterATick()
        {
            var world = new World();
            world.SetLiveCellAt(new Location(1, 0));   //  -x-   
            world.SetLiveCellAt(new Location(0, 1));   //  xo-
            world.SetLiveCellAt(new Location(1, 2));   //  -x-
            World nextWorld = world.Tick();
            Assert.IsInstanceOfType(typeof(AliveCell), nextWorld.GetCellAt(new Location(1, 1)));
        }

        [Test]
        public void GetCellAtShouldOnlyReturnLivingCellsWhenSet()
        {
            var world = new World();
            world.SetLiveCellAt(new Location(99, 99));
            Assert.IsInstanceOfType(typeof(DeadCell), world.GetCellAt(new Location(1, 1)));
            Assert.IsInstanceOfType(typeof(AliveCell), world.GetCellAt(new Location(99, 99)));
        }

        [Test]
        public void CellWithThreeNeighboursShouldLiveOnAfterNextTick()
        {
            var world = new World();
            world.SetLiveCellAt(new Location(1, 0));   
            world.SetLiveCellAt(new Location(1, 1));   //  ---   
            world.SetLiveCellAt(new Location(1, 2));   //  xcx
            world.SetLiveCellAt(new Location(2, 1));   //  -x-
            World nextWorld = world.Tick();
            Assert.IsInstanceOfType(typeof(AliveCell), nextWorld.GetCellAt(new Location(1, 1)));
        }
        
        [Test]
        public void CellWithTwoNeighboursShouldLiveOnAfterNextTick()
        {
            var world = new World();
            world.SetLiveCellAt(new Location(1, 1));
            world.SetLiveCellAt(new Location(1, 2));
            world.SetLiveCellAt(new Location(2, 1));
            World nextWorld = world.Tick();
            Assert.IsInstanceOfType(typeof(AliveCell), nextWorld.GetCellAt(new Location(1, 1)));
        }

        [Test]
        public void CellWithOneNeighbourDiesAfterATick()
        {
            var world = new World();
            world.SetLiveCellAt(new Location(1, 1));
            world.SetLiveCellAt(new Location(1, 2));
            World nextWorld = world.Tick();
            Assert.IsInstanceOfType(typeof(DeadCell), nextWorld.GetCellAt(new Location(1, 2)));
        }
        
        [Test]
        public void OneAliveCellShouldDieAfterATick()
        {
            var world = new World();
            world.SetLiveCellAt(new Location(1,1));
            Assert.IsAssignableFrom(typeof(AliveCell), world.GetCellAt(new Location(1, 1)), "before tick");
            World nextWorld = world.Tick();
            Assert.IsInstanceOfType(typeof(DeadCell), nextWorld.GetCellAt(new Location(1, 1)), "after tick");
        }
    }
}



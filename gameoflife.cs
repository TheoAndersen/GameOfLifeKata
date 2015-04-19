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

            public IEnumerable<Cell> ReturnNeighbours(IEnumerable<Cell> cells)
            {
                return cells.Where(l => this.X >= l.Location.X-1 &&
                                   this.X <= l.Location.X+1 &&
                                   this.Y >= l.Location.Y-1 &&
                                   this.Y <= l.Location.Y+1 &&
                                   !(this.X == l.Location.X &&
                                     this.Y == l.Location.Y));
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
            List<Cell> livingCells = new List<Cell>();
            
            public void SetLiveCellAt(Location location)
            {
                livingCells.Add(new AliveCell(location));
            }
            
            public Cell GetCellAt(Location location)
            {
                if(livingCells.Count == 0)
                {
                    return new DeadCell(location);
                }

                return new AliveCell(location);
            }

            public void Tick()
            {
                List<Cell> dyingCells = new List<Cell>();

                foreach(var livingCell in livingCells)
                {
                    if(livingCell.Location.ReturnNeighbours(this.livingCells).Count() != 2)
                    {
                        dyingCells.Add(livingCell);
                    }
                }
                
                livingCells.RemoveAll(cell => dyingCells.Contains(cell));
            }
        }

        [Test]
        public void CellWithThreeNeighboursShouldLiveOnAfterNextTick()
        {
            var world = new World();
            world.SetLiveCellAt(new Location(1, 1));
            world.SetLiveCellAt(new Location(1, 2));
            world.SetLiveCellAt(new Location(2, 1));
            world.SetLiveCellAt(new Location(1, 0));
            world.Tick();
            Assert.IsInstanceOfType(typeof(AliveCell), world.GetCellAt(new Location(1, 1)));
        }
        
        [Test]
        public void CellWithTwoNeighboursShouldLiveOnAfterNextTick()
        {
            var world = new World();
            world.SetLiveCellAt(new Location(1, 1));
            world.SetLiveCellAt(new Location(1, 2));
            world.SetLiveCellAt(new Location(2, 1));
            world.Tick();
            Assert.IsInstanceOfType(typeof(AliveCell), world.GetCellAt(new Location(1, 1)));
        }

        [Test]
        public void CellWithOneNeighbourDiesAfterATick()
        {
            var world = new World();
            world.SetLiveCellAt(new Location(1, 1));
            world.SetLiveCellAt(new Location(1, 2));
            world.Tick();
            Assert.IsInstanceOfType(typeof(DeadCell), world.GetCellAt(new Location(1, 2)));
        }
        
        [Test]
        public void OneAliveCellShouldDieAfterATick()
        {
            var world = new World();
            world.SetLiveCellAt(new Location(1,1));
            Assert.IsAssignableFrom(typeof(AliveCell), world.GetCellAt(new Location(1, 1)), "before tick");
            world.Tick();
            Assert.IsInstanceOfType(typeof(DeadCell), world.GetCellAt(new Location(1, 1)), "after tick");
        }
    }
}
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
                livingCells.Clear();
            }

            public int NeighboursOfCellAt(Location location)
            {
                var neighbours = location.ReturnNeighbours(this.livingCells);

                return neighbours.Count();
            }
        }

        [Test]
        public void ShouldBeAbleToFindNumberOfNeighboursOfACell()
        {
            /*     0 1 2 3 4
             *     
             *  0  x x x x x
                1  x n n n x
             *  2  x n c n x
             *  3  x n n n x
             *  4  x x x x x
             */
              
            var location = new Location(2, 2);
            Assert.AreEqual(0, world.NeighboursOfCellAt(location));
            world.SetLiveCellAt(new Location(2, 2));
            Assert.AreEqual(0, world.NeighboursOfCellAt(location));
            world.SetLiveCellAt(new Location(0, 0));
            world.SetLiveCellAt(new Location(0, 1));
            world.SetLiveCellAt(new Location(0, 2));
            world.SetLiveCellAt(new Location(0, 3));
            world.SetLiveCellAt(new Location(0, 4));
            world.SetLiveCellAt(new Location(1, 0));
            world.SetLiveCellAt(new Location(1, 4));
            world.SetLiveCellAt(new Location(2, 0));
            world.SetLiveCellAt(new Location(2, 4));
            world.SetLiveCellAt(new Location(3, 0));
            world.SetLiveCellAt(new Location(3, 4));
            world.SetLiveCellAt(new Location(4, 0));
            world.SetLiveCellAt(new Location(4, 1));
            world.SetLiveCellAt(new Location(4, 2));
            world.SetLiveCellAt(new Location(4, 3));
            world.SetLiveCellAt(new Location(4, 4));
            Assert.AreEqual(0, world.NeighboursOfCellAt(location));
            world.SetLiveCellAt(new Location(1, 1));
            world.SetLiveCellAt(new Location(1, 2));
            world.SetLiveCellAt(new Location(1, 3));
            world.SetLiveCellAt(new Location(2, 1));
            world.SetLiveCellAt(new Location(2, 3));
            world.SetLiveCellAt(new Location(3, 1));
            world.SetLiveCellAt(new Location(3, 2));
            world.SetLiveCellAt(new Location(3, 3));
            Assert.AreEqual(8, world.NeighboursOfCellAt(location));
        }
        
        [Test]
        [Ignore]
        public void CellWithTwoNeighboursShouldLiveOnAfterNextTick()
        {
            world.SetLiveCellAt(new Location(1, 1));
            world.SetLiveCellAt(new Location(1, 2));
            world.SetLiveCellAt(new Location(2, 1));
            world.Tick();
            Assert.IsInstanceOfType(typeof(AliveCell), world.GetCellAt(new Location(1, 1)));
        }
        
        [Test]
        public void OneAliveCellShouldDieAfterATick()
        {
            world.SetLiveCellAt(new Location(1,1));
            Assert.IsAssignableFrom(typeof(AliveCell), world.GetCellAt(new Location(1, 1)), "before tick");
            world.Tick();
            Assert.IsInstanceOfType(typeof(DeadCell), world.GetCellAt(new Location(1, 1)), "after tick");
        }

        World world;

        [TestFixtureSetUp]
        public void TestInitialize()
        {
            world = new World();
        }


    }
}
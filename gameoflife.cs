using NUnit.Framework;

namespace GameOfLife
{
    [TestFixture]
    public class GameOfLifeTest
    {
        public class Location
        {
            int X;
            int Y;
            
            public Location(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        public class Cell
        {
        }

        public class DeadCell : Cell
        {
        }
        
        public class World
        {
            bool isEmpty = false;
            
            public void SetLiveCell(Location location)
            {
                isEmpty = true;
            }
            
            public Cell GetCellAt(Location location)
            {
                if(isEmpty)
                {
                    return new DeadCell();
                }

                return new Cell();
            }

            public void Tick()
            {
                isEmpty = true;
            }
        }

        
        
        [Test]
        public void OneAliveCellShouldDieAfterATick()
        {
            World world = new World();
            world.SetLiveCell(new Location(1,1));
            Assert.IsInstanceOfType(typeof(Cell), world.GetCellAt(new Location(1, 1)), "before tick");
            world.Tick();
            Assert.IsInstanceOfType(typeof(DeadCell), world.GetCellAt(new Location(1, 1)), "after tick");
        }
    }
}
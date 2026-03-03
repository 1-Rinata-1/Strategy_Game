namespace StrategyGame
{
    public interface ICellPrototype
    {
        Cell Clone(int row, int col, CellOwner owner);
    }

    public class DefaultCellPrototype : ICellPrototype
    {
        public Cell Clone(int row, int col, CellOwner owner)
        {
            return new Cell(row, col, owner);
        }
    }
}


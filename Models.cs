using System;
using System.Collections.Generic;

namespace StrategyGame
{
    public enum CellOwner { Player1, Neutral, Player2 }
    public enum BuildingType { None, Income, Main, Tower }
    public enum RaceType { Dwarf, Elf, Orc }
    public enum RoleType { Warrior, Builder, Healer }

    public class Cell
    {
        public int row;
        public int col;
        public CellOwner owner;
        public BuildingType building = BuildingType.None;
        public string building_owner = null;
        public int main_hp = 0;
        public Cell(int r, int c, CellOwner o) { row = r; col = c; owner = o; }
    }

    public class Player
    {
        public string name;
        public RaceType race;
        public int treasury;
        public int base_income = 5;
        public int main_hp;
        public int territory_rows;
        private List<Cell> _buildings = new List<Cell>();
        public int buildings_count => _buildings.Count;
        public CellOwner owner_type;
        public RaceTheme theme;
        public IRoleFamily role_family;

        public Player(string name_val, RaceType race_val, int treasury_val, int main_hp_val, int territory_rows_val, CellOwner owner_val)
        {
            name = name_val; race = race_val; treasury = treasury_val; main_hp = main_hp_val; territory_rows = territory_rows_val; owner_type = owner_val;
        }

        public void add_building(Cell c) => _buildings.Add(c);
        public void remove_building(Cell c) => _buildings.Remove(c);
        public int income_per_turn() => base_income + 5 * _buildings.Count;
    }

    public class GameState
    {
        public const int rows = 9;
        public const int cols = 9;
        public Cell[,] grid = new Cell[rows, cols];
        public Player player_one;
        public Player player_two;
        public int current_player_index = 0;
        public bool action_taken_this_turn = false;
        private readonly ICellPrototype _cellPrototype;

        public GameState(Player p1, Player p2, ICellPrototype prototype = null)
        {
            player_one = p1; player_two = p2;
            _cellPrototype = prototype ?? new DefaultCellPrototype();
            int top = p1.territory_rows;
            int bottom = p2.territory_rows;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    CellOwner owner_val = CellOwner.Neutral;
                    if (r < top) owner_val = CellOwner.Player1;
                    else if (r >= rows - bottom) owner_val = CellOwner.Player2;
                    grid[r, c] = _cellPrototype.Clone(r, c, owner_val);
                }
            }
            int center = cols / 2;
            grid[0, center].building = BuildingType.Main;
            grid[0, center].main_hp = p1.main_hp;
            grid[0, center].building_owner = p1.name;
            grid[0, center].owner = CellOwner.Player1;

            grid[rows - 1, center].building = BuildingType.Main;
            grid[rows - 1, center].main_hp = p2.main_hp;
            grid[rows - 1, center].building_owner = p2.name;
            grid[rows - 1, center].owner = CellOwner.Player2;
        }

        public Player current_player => current_player_index == 0 ? player_one : player_two;
        public Player other_player => current_player_index == 0 ? player_two : player_one;

        public void end_turn()
        {
            current_player.treasury += current_player.income_per_turn();
            action_taken_this_turn = false;
            current_player_index = 1 - current_player_index;
        }

        public bool is_adjacent_to_owner(Cell cell, CellOwner owner)
        {
            int r = cell.row, c = cell.col;
            if (r - 1 >= 0 && grid[r - 1, c].owner == owner) return true;
            if (r + 1 < rows && grid[r + 1, c].owner == owner) return true;
            if (c - 1 >= 0 && grid[r, c - 1].owner == owner) return true;
            if (c + 1 < cols && grid[r, c + 1].owner == owner) return true;
            return false;
        }
    }
}

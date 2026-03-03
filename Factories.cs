using System;

namespace StrategyGame
{
    // паттерн фабричный метод
    public interface IRole
    {
        string name { get; }
        void PerformAction(Player player, GameState state, Cell selected_cell);
    }

    public interface IRoleFactory { IRole CreateRole(); }

    public class WarriorRole : IRole
    {
        public string name => "Warrior";
        public void PerformAction(Player player, GameState state, Cell selected_cell)
        {
            if (player.treasury < 10) throw new InvalidOperationException("Недостаточно золота для атаки.");
            if (selected_cell == null) throw new InvalidOperationException("Не выбрана клетка для атаки.");
            if (!state.is_adjacent_to_owner(selected_cell, player.owner_type)) throw new InvalidOperationException("Можно атаковать только соседние клетки.");
            if (selected_cell.building == BuildingType.Tower)
            {
                if (player.treasury < 30) throw new InvalidOperationException("Нужно 30 монет, чтобы разрушить башню.");
                player.treasury -= 30;
                selected_cell.building = BuildingType.None;
                selected_cell.building_owner = null;
                selected_cell.owner = player.owner_type;
                return;
            }
            player.treasury -= 10;
            if (selected_cell.building == BuildingType.Main)
            {
                if (selected_cell.building_owner == state.player_one.name) { state.player_one.main_hp -= 10; selected_cell.main_hp = state.player_one.main_hp; }
                else { state.player_two.main_hp -= 10; selected_cell.main_hp = state.player_two.main_hp; }
            }
            else if (selected_cell.building == BuildingType.Income)
            {
                if (selected_cell.building_owner == state.player_one.name) state.player_one.remove_building(selected_cell);
                if (selected_cell.building_owner == state.player_two.name) state.player_two.remove_building(selected_cell);
                selected_cell.building = BuildingType.None;
                selected_cell.building_owner = null;
                selected_cell.owner = player.owner_type;
            }
            else
            {
                selected_cell.owner = player.owner_type;
            }
        }
    }

    public class BuilderRole : IRole
    {
        public string name => "Builder";
        public void PerformAction(Player player, GameState state, Cell selected_cell)
        {
            if (selected_cell == null) throw new InvalidOperationException("Не выбрана клетка для постройки.");
            if (selected_cell.owner != player.owner_type) throw new InvalidOperationException("Можно строить только на своей клетке.");
            if (!state.is_adjacent_to_owner(selected_cell, player.owner_type)) throw new InvalidOperationException("Строить можно только на клетке, соседней с вашей территорией.");
            if (player.treasury < 10) throw new InvalidOperationException("Недостаточно золота для постройки дома.");
            player.treasury -= 10;
            selected_cell.building = BuildingType.Income;
            selected_cell.building_owner = player.name;
            player.add_building(selected_cell);
        }
    }

    public class TowerBuilderRole : IRole
    {
        public string name => "TowerBuilder";
        public void PerformAction(Player player, GameState state, Cell selected_cell)
        {
            if (selected_cell == null) throw new InvalidOperationException("Не выбрана клетка для постройки.");
            if (selected_cell.owner != player.owner_type) throw new InvalidOperationException("Можно строить только на своей клетке.");
            if (!state.is_adjacent_to_owner(selected_cell, player.owner_type)) throw new InvalidOperationException("Строить можно только на клетке, соседней с вашей территорией.");
            if (player.treasury < 20) throw new InvalidOperationException("Недостаточно золота для постройки башни.");
            player.treasury -= 20;
            selected_cell.building = BuildingType.Tower;
            selected_cell.building_owner = player.name;
        }
    }
    public class HealerRole : IRole
    {
        public string name => "Healer";
        public void PerformAction(Player player, GameState state, Cell selected_cell)
        {
            if (player.treasury < 20) throw new InvalidOperationException("Недостаточно золота для исцеления.");
            player.treasury -= 20;
            if (player.owner_type == CellOwner.Player1)
            {
                state.player_one.main_hp += 20;
                int center = GameState.cols / 2;
                state.grid[0, center].main_hp = state.player_one.main_hp;
            }
            else
            {
                state.player_two.main_hp += 20;
                int center = GameState.cols / 2;
                state.grid[GameState.rows - 1, center].main_hp = state.player_two.main_hp;
            }
        }
    }

    public class WarriorFactory : IRoleFactory { public IRole CreateRole() => new WarriorRole(); }
    public class BuilderFactory : IRoleFactory { public IRole CreateRole() => new BuilderRole(); }
    public class TowerBuilderFactory : IRoleFactory { public IRole CreateRole() => new TowerBuilderRole(); }
    public class HealerFactory : IRoleFactory { public IRole CreateRole() => new HealerRole(); }

    public interface IPlayerFactory { Player CreatePlayer(string name, CellOwner owner); }

    public class DwarfFactory : IPlayerFactory { public Player CreatePlayer(string name, CellOwner owner) => new Player(name, RaceType.Dwarf, 40, 100, 3, owner); }
    public class ElfFactory : IPlayerFactory { public Player CreatePlayer(string name, CellOwner owner) => new Player(name, RaceType.Elf, 20, 100, 4, owner); }
    public class OrcFactory : IPlayerFactory { public Player CreatePlayer(string name, CellOwner owner) => new Player(name, RaceType.Orc, 20, 150, 3, owner); }
}

using System.Drawing;

namespace StrategyGame
{
    public interface IRoleFamily
    {
        IRoleFactory GetWarriorFactory();
        IRoleFactory GetBuilderFactory();
        IRoleFactory GetTowerBuilderFactory();
        IRoleFactory GetHealerFactory();
    }

    public class DefaultRoleFamily : IRoleFamily
    {
        public IRoleFactory GetWarriorFactory() => new WarriorFactory();
        public IRoleFactory GetBuilderFactory() => new BuilderFactory();

        // роль башни отключена
        public IRoleFactory GetTowerBuilderFactory() => null;

        public IRoleFactory GetHealerFactory() => new HealerFactory();
    }


    public class RaceTheme
    {
        public Color territoryColor { get; }
        public RaceTheme(Color territoryColorVal)
        {
            territoryColor = territoryColorVal;
        }
    }

    public interface IRaceFactory
    {
        Player CreatePlayer(string name, CellOwner owner);
        RaceTheme CreateTheme();
        IRoleFamily CreateRoleFamily();
    }

    public class DwarfRaceFactory : IRaceFactory
    {
        public Player CreatePlayer(string name, CellOwner owner)
        {
            var p = new Player(name, RaceType.Dwarf, 40, 100, 3, owner);
            p.theme = CreateTheme();
            p.role_family = CreateRoleFamily();
            return p;
        }
        public RaceTheme CreateTheme() => new RaceTheme(ColorTranslator.FromHtml("#fddde6"));
        public IRoleFamily CreateRoleFamily() => new DefaultRoleFamily();
    }

    public class ElfRaceFactory : IRaceFactory
    {
        public Player CreatePlayer(string name, CellOwner owner)
        {
            var p = new Player(name, RaceType.Elf, 20, 100, 4, owner);
            p.theme = CreateTheme();
            p.role_family = CreateRoleFamily();
            return p;
        }
        public RaceTheme CreateTheme() => new RaceTheme(ColorTranslator.FromHtml("#e3f7dd"));
        public IRoleFamily CreateRoleFamily() => new DefaultRoleFamily();
    }

    public class OrcRaceFactory : IRaceFactory
    {
        public Player CreatePlayer(string name, CellOwner owner)
        {
            var p = new Player(name, RaceType.Orc, 20, 150, 3, owner);
            p.theme = CreateTheme();
            p.role_family = CreateRoleFamily();
            return p;
        }
        public RaceTheme CreateTheme() => new RaceTheme(ColorTranslator.FromHtml("#ddeeff"));
        public IRoleFamily CreateRoleFamily() => new DefaultRoleFamily();
    }
}


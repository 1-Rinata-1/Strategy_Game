namespace StrategyGame
{
    public class GameSetupBuilder
    {
        private IRaceFactory _raceFactoryP1;
        private IRaceFactory _raceFactoryP2;
        private string _nameP1 = "Player 1";
        private string _nameP2 = "Player 2";
        private ICellPrototype _prototype = new DefaultCellPrototype();

        public GameSetupBuilder WithRaceFactories(IRaceFactory p1, IRaceFactory p2)
        {
            _raceFactoryP1 = p1; 
            _raceFactoryP2 = p2; 
            return this;
        }

        public GameSetupBuilder WithNames(string nameP1, string nameP2)
        {
            _nameP1 = string.IsNullOrWhiteSpace(nameP1) ? _nameP1 : nameP1;
            _nameP2 = string.IsNullOrWhiteSpace(nameP2) ? _nameP2 : nameP2;
            return this;
        }

        public GameSetupBuilder WithCellPrototype(ICellPrototype proto)
        {
            _prototype = proto ?? _prototype;
            return this;
        }

        public GameForm BuildGameForm()
        {
            var p1 = (_raceFactoryP1 ?? new DwarfRaceFactory()).CreatePlayer(_nameP1, CellOwner.Player1);
            var p2 = (_raceFactoryP2 ?? new OrcRaceFactory()).CreatePlayer(_nameP2, CellOwner.Player2);
            return new GameForm(p1, p2, _prototype);
        }
    }
}


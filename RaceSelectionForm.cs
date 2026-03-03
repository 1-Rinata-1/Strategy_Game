using System;
using System.Drawing;
using System.Windows.Forms;

namespace StrategyGame
{
    public class RaceSelectionForm : Form
    {
        private ComboBox cb_player1;
        private ComboBox cb_player2;
        private Button btn_start;
        public IRaceFactory player_one_factory;
        public IRaceFactory player_two_factory;

        public RaceSelectionForm()
        {
            Text = "Выбор рас";
            Width = 600; Height = 220; StartPosition = FormStartPosition.CenterScreen;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            Label l1 = new Label { Left = 20, Top = 20, Width = 250, Text = "Выберите расу для Игрока 1:" };
            cb_player1 = new ComboBox { Left = 20, Top = 45, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cb_player1.Items.AddRange(new string[] { "Гномы", "Эльфы", "Орки" }); cb_player1.SelectedIndex = 0;

            Label l2 = new Label { Left = 300, Top = 20, Width = 250, Text = "Выберите расу для Игрока 2:" };
            cb_player2 = new ComboBox { Left = 300, Top = 45, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
            cb_player2.Items.AddRange(new string[] { "Гномы", "Эльфы", "Орки" }); cb_player2.SelectedIndex = 1;

            btn_start = new Button { Left = 220, Top = 110, Width = 140, Height = 36, Text = "Далее" };
            btn_start.Click += Btn_start_Click;

            Controls.AddRange(new Control[] { l1, cb_player1, l2, cb_player2, btn_start });
        }

        private void Btn_start_Click(object sender, EventArgs e)
        {
            player_one_factory = CreateFactoryByName(cb_player1.SelectedItem.ToString());
            player_two_factory = CreateFactoryByName(cb_player2.SelectedItem.ToString());
            using (var rules = new RulesForm(player_one_factory, player_two_factory))
            {
                this.Hide();
                rules.ShowDialog();
                this.Close();
            }
        }

        private IRaceFactory CreateFactoryByName(string name)
        {
            return name switch
            {
                "Гномы" => new DwarfRaceFactory(),
                "Эльфы" => new ElfRaceFactory(),
                "Орки" => new OrcRaceFactory(),
                _ => new DwarfRaceFactory(),
            };
        }
    }
}

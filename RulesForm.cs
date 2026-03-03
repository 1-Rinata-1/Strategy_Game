using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace StrategyGame
{
    public class RulesForm : Form
    {
        private IRaceFactory player_one_factory;
        private IRaceFactory player_two_factory;
        private Button btn_continue;

        public RulesForm(IRaceFactory p1, IRaceFactory p2)
        {
            player_one_factory = p1; player_two_factory = p2;
            Text = "Правила"; Width = 800; Height = 650; StartPosition = FormStartPosition.CenterScreen;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            var lbl_rules = new Label { Left = 20, Top = 20, Width = 520, Height = 480, AutoSize = false,
                Text = "Правила игры:\n\n- Поле 9×9. Верхние 3 ряда - Игрок 1, нижние 3 - Игрок 2, середина 3 - нейтральные.\n- Цель: разрушить главное здание противника.\n- Роли: Воин (атака, 10), Строитель (постройка, дом 10 / башня 20), Целитель (лечение 20).\n- Доход: 5 монет +5 за дом.\n- Атака возможна только на соседних по стороне клетках со своим полем.\n- Постройка дома или башни возможна только на своем поле.\n- Башня блокирует захват; для разрушения башни требуется 30 монет.\n"
            };

            Panel pnl_icons = new Panel { Left = 560, Top = 20, Width = 200, Height = 400 };
            AddIconWithLabel(pnl_icons, "main.png", "Главное здание", 10);
            AddIconWithLabel(pnl_icons, "house.png", "Дом (+доход)", 110);
            AddIconWithLabel(pnl_icons, "tower.png", "Башня (блокирует)", 210);

            btn_continue = new Button { Left = 320, Top = 520, Width = 160, Height = 36, Text = "Продолжить" };
            btn_continue.Click += Btn_continue_Click;

            Controls.Add(lbl_rules); Controls.Add(pnl_icons); Controls.Add(btn_continue);
        }

        private void AddIconWithLabel(Panel pnl, string iconFileName, string labelText, int top)
        {
            PictureBox pb = new PictureBox { Left = 20, Top = top, Width = 48, Height = 48, SizeMode = PictureBoxSizeMode.StretchImage };
            var img = AssetsManager.Instance.GetImage(iconFileName);
            if (img != null) pb.Image = img;
            Label lbl = new Label { Left = 80, Top = top + 12, Width = 120, Height = 24, Text = labelText };
            pnl.Controls.Add(pb); pnl.Controls.Add(lbl);
        }

        private void Btn_continue_Click(object sender, EventArgs e)
        {
            var builder = new GameSetupBuilder()
                .WithRaceFactories(player_one_factory, player_two_factory)
                .WithNames("Игрок 1", "Игрок 2")
                .WithCellPrototype(new DefaultCellPrototype());
            this.Hide();
            using (var game = builder.BuildGameForm())
            {
                game.ShowDialog();
            }
            this.Close();
        }
    }
}

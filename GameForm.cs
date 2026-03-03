using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace StrategyGame
{
    public class GameForm : Form
    {
        private GameState state;
        private Panel board_panel;
        private Panel right_panel;
        private Label lbl_info;
        private Button btn_action;
        private Button btn_skip;
        private RadioButton rb_house, rb_tower;
        private const int cell_size = 56;
        private Cell selected_cell = null;
        private Image icon_house, icon_main, icon_tower;

        public GameForm(Player p1, Player p2)
        {
            Text = "Strategy Game";
            Width = GameState.cols * cell_size + 420;
            Height = GameState.rows * cell_size + 140;
            StartPosition = FormStartPosition.CenterScreen;

            state = new GameState(p1, p2);
            InitializeComponents();
            LoadAssets();
            PromptRoleForCurrentPlayer();
            UpdateLabels();
            board_panel.Invalidate();
        }

        public GameForm(Player p1, Player p2, ICellPrototype prototype)
        {
            Text = "Strategy Game";
            Width = GameState.cols * cell_size + 420;
            Height = GameState.rows * cell_size + 140;
            StartPosition = FormStartPosition.CenterScreen;

            state = new GameState(p1, p2, prototype);
            InitializeComponents();
            LoadAssets();
            PromptRoleForCurrentPlayer();
            UpdateLabels();
            board_panel.Invalidate();
        }

        private void InitializeComponents()
        {
            board_panel = new Panel { Left = 10, Top = 10, Width = GameState.cols * cell_size, Height = GameState.rows * cell_size };
            board_panel.Paint += Board_panel_Paint;
            board_panel.MouseClick += Board_panel_MouseClick;
            Controls.Add(board_panel);

            right_panel = new Panel { Left = board_panel.Right + 10, Top = 10, Width = 380, Height = board_panel.Height };
            lbl_info = new Label { Left = 10, Top = 10, Width = 360, Height = 160, BorderStyle = BorderStyle.FixedSingle };
            btn_action = new Button { Left = 10, Top = 180, Width = 160, Height = 36, Text = "Выполнить действие" };
            btn_skip = new Button { Left = 200, Top = 180, Width = 160, Height = 36, Text = "Пропустить ход" };
            btn_action.Click += Btn_action_Click;
            btn_skip.Click += Btn_skip_Click;

            rb_house = new RadioButton { Left = 10, Top = 230, Width = 120, Text = "Дом (10)" };
            rb_tower = new RadioButton { Left = 150, Top = 230, Width = 120, Text = "Башня (20)" };
            right_panel.Controls.AddRange(new Control[] { lbl_info, btn_action, btn_skip, rb_house, rb_tower });
            Controls.Add(right_panel);
        }

        private void LoadAssets()
        {
            icon_house = AssetsManager.Instance.GetImage("house.png");
            icon_main = AssetsManager.Instance.GetImage("main.png");
            icon_tower = AssetsManager.Instance.GetImage("tower.png");
        }

        private void Board_panel_MouseClick(object sender, MouseEventArgs e)
        {
            int col = e.X / cell_size;
            int row = e.Y / cell_size;
            if (row < 0 || row >= GameState.rows || col < 0 || col >= GameState.cols) return;
            selected_cell = state.grid[row, col];
            board_panel.Invalidate();
            UpdateLabels();
        }

        private void Board_panel_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.Black);
            for (int r = 0; r < GameState.rows; r++)
            {
                for (int c = 0; c < GameState.cols; c++)
                {
                    var cell = state.grid[r, c];
                    Rectangle rect = new Rectangle(c * cell_size, r * cell_size, cell_size, cell_size);
                    Color fill = ColorTranslator.FromHtml("#faeedd");
                    if (cell.owner == CellOwner.Player1 && state.player_one?.theme != null) fill = state.player_one.theme.territoryColor;
                    else if (cell.owner == CellOwner.Player2 && state.player_two?.theme != null) fill = state.player_two.theme.territoryColor;
                    using (Brush br = new SolidBrush(fill)) g.FillRectangle(br, rect);

                    if (cell.building == BuildingType.Income && icon_house != null)
                        g.DrawImage(icon_house, rect.Left + 6, rect.Top + 6, cell_size - 12, cell_size - 12);
                    else if (cell.building == BuildingType.Tower && icon_tower != null)
                        g.DrawImage(icon_tower, rect.Left + 6, rect.Top + 6, cell_size - 12, cell_size - 12);
                    else if (cell.building == BuildingType.Main && icon_main != null)
                        g.DrawImage(icon_main, rect.Left + 6, rect.Top + 6, cell_size - 12, cell_size - 12);

                    using (Pen p = new Pen(Color.Black, 1)) g.DrawRectangle(p, rect);

                    if (selected_cell == cell)
                    {
                        using (Pen p = new Pen(Color.Yellow, 3)) g.DrawRectangle(p, rect);
                    }
                    else if (state.is_adjacent_to_owner(cell, state.current_player.owner_type))
                    {
                        using (Pen p = new Pen(Color.FromArgb(0, 150, 0), 2)) g.DrawRectangle(p, rect);
                    }
                }
            }
        }

        private void PromptRoleForCurrentPlayer()
        {
            using (var roleForm = new RoleSelectionForm(state.current_player.role_family))
            {
                var res = roleForm.ShowDialog();
                if (res == DialogResult.OK && roleForm.selected_role_impl != null)
                {
                    this.Tag = roleForm.selected_role_impl;
                    UpdateActionUI();
                }
                else
                {
                    // skip selection -> end turn and prompt next
                    state.end_turn();
                    PromptRoleForCurrentPlayer();
                }
            }
        }

        private void UpdateActionUI()
        {
            var role = this.Tag as IRole;
            if (role == null) { lbl_info.Text = "Роль не выбрана."; btn_action.Enabled = false; return; }
            if (role.name == "Warrior")
            {
                lbl_info.Text = $"Текущий игрок: {state.current_player.name}\nРоль: Воин.\nВыберите соседнюю клетку для атаки.";
                btn_action.Text = "Атаковать (10)";
                rb_house.Visible = false; rb_tower.Visible = false;
            }
            else if (role.name == "Builder")
            {
                lbl_info.Text = $"Текущий игрок: {state.current_player.name}\nРоль: Строитель.\nВыберите свою клетку, затем выберите здание и нажмите Построить.";
                btn_action.Text = "Построить (10)";
                rb_house.Visible = true; rb_tower.Visible = true; rb_house.Checked = true;
            }
            else if (role.name == "TowerBuilder")
            {
                lbl_info.Text = $"Текущий игрок: {state.current_player.name}\nРоль: Строитель башни.\nВыберите свою клетку и нажмите Построить башню (20).";
                btn_action.Text = "Построить башню (20)";
                rb_house.Visible = false; rb_tower.Visible = false;
            }
            else if (role.name == "Healer")
            {
                lbl_info.Text = $"Текущий игрок: {state.current_player.name}\nРоль: Целитель.\nНажмите Исцелить, чтобы восстановить HP главного здания (20).";
                btn_action.Text = "Исцелить (20)";
                rb_house.Visible = false; rb_tower.Visible = false;
            }
            btn_action.Enabled = true;
        }

        private void Btn_action_Click(object sender, EventArgs e)
        {
            var role = this.Tag as IRole;
            if (role == null) return;
            try
            {
                if (role.name == "Builder" && rb_tower != null && rb_tower.Checked)
                {
                    var towerRole = new TowerBuilderFactory().CreateRole();
                    towerRole.PerformAction(state.current_player, state, selected_cell);
                }
                else
                {
                    role.PerformAction(state.current_player, state, selected_cell);
                }
                if (state.player_one.main_hp <= 0 || state.player_two.main_hp <= 0)
                {
                    string winner = state.player_one.main_hp <= 0 ? state.player_two.name : state.player_one.name;
                    MessageBox.Show($"{winner} победил!");
                    this.Close(); return;
                }
                state.end_turn();
                selected_cell = null;
                this.Tag = null;
                UpdateLabels();
                PromptRoleForCurrentPlayer();
                UpdateLabels();
                board_panel.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void Btn_skip_Click(object sender, EventArgs e)
        {
            state.end_turn();
            selected_cell = null;
            this.Tag = null;
            UpdateLabels();
            PromptRoleForCurrentPlayer();
            board_panel.Invalidate();
        }

        private void UpdateLabels()
        {
            lbl_info.Text = $"Текущий игрок: {state.current_player.name} (игрок {(state.current_player_index+1)})\n" +
                $"Золото: {state.current_player.treasury}\nДоход/ход: {state.current_player.income_per_turn()}\n" +
                $"HP: P1={state.player_one.main_hp}  P2={state.player_two.main_hp}\n\n" +
                "Выберите клетку кликом, затем выполните действие справа.";
        }
    }
}

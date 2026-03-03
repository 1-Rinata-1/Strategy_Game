using System;
using System.Windows.Forms;

namespace StrategyGame
{
    public class RoleSelectionForm : Form
    {
        public IRole selected_role_impl;
        private Button btn_warrior, btn_builder, btn_towerbuilder, btn_healer, btn_skip;
        private readonly IRoleFamily _roleFamily;

        public RoleSelectionForm() : this(null) { }

        public RoleSelectionForm(IRoleFamily roleFamily)
        {
            _roleFamily = roleFamily ?? new DefaultRoleFamily();
            Text = "Выберите роль";
            StartPosition = FormStartPosition.CenterParent;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            int left = 30;
            int top = 10;
            int width = 120;
            int height = 30;
            int spacing = 20;

            btn_warrior = new Button { Text = "Воин", Width = width, Height = height, Top = top, Left = left };
            left += width + spacing;
            btn_builder = new Button { Text = "Строитель", Width = width, Height = height, Top = top, Left = left };
            left += width + spacing;
            btn_healer = new Button { Text = "Целитель", Width = width, Height = height, Top = top };

            if (_roleFamily.GetTowerBuilderFactory() != null)
            {
                btn_towerbuilder = new Button { Text = "Башня", Width = width, Height = height, Top = top, Left = left };
                left += width + spacing;
                btn_towerbuilder.Click += (s, e) =>
                {
                    var factory = _roleFamily.GetTowerBuilderFactory();
                    if (factory != null) selected_role_impl = factory.CreateRole();
                    else selected_role_impl = null;
                    this.DialogResult = selected_role_impl != null ? DialogResult.OK : DialogResult.Cancel;
                    this.Close();
                };
                Controls.Add(btn_towerbuilder);
            }

            btn_healer.Left = left;

            // Обработчики кнопок
            btn_warrior.Click += (s, e) => { selected_role_impl = _roleFamily.GetWarriorFactory().CreateRole(); this.DialogResult = DialogResult.OK; this.Close(); };
            btn_builder.Click += (s, e) => { selected_role_impl = _roleFamily.GetBuilderFactory().CreateRole(); this.DialogResult = DialogResult.OK; this.Close(); };
            btn_healer.Click += (s, e) => { selected_role_impl = _roleFamily.GetHealerFactory().CreateRole(); this.DialogResult = DialogResult.OK; this.Close(); };

            Controls.Add(btn_warrior);
            Controls.Add(btn_builder);
            Controls.Add(btn_healer);

            btn_skip = new Button { Text = "Пропустить", Width = 100, Height = 30 };
            btn_skip.Click += (s, e) => { selected_role_impl = null; this.DialogResult = DialogResult.Cancel; this.Close(); };
            btn_skip.Top = top + height + 20; // снизу
            btn_skip.Left = (left + width + 30 - btn_skip.Width) / 2;

            Controls.Add(btn_skip);

            // Подгоняем размер формы под кнопки
            this.Width = btn_skip.Left + btn_skip.Width + 190;
            this.Height = btn_skip.Top + btn_skip.Height + 70;
        }
    }
}

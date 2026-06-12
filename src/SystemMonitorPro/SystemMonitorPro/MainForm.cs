using System.Runtime.InteropServices;
using SystemMonitorPro.Helpers;

namespace SystemMonitorPro
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        private Panel _titleBar = null!;
        private Panel _navPanel = null!;
        private Panel _contentPanel = null!;

        private Button _btnDashboard = null!;
        private Button _btnProcesses = null!;
        private Button _btnFiles = null!;
        private Button _btnDisk = null!;

        private Button _btnClose = null!;
        private Button _btnMaximize = null!;
        private Button _btnMinimize = null!;

        private Button? _activeTab;

        public MainForm()
        {
            InitializeComponent();

            AutoScaleMode = AutoScaleMode.Dpi;
            DoubleBuffered = true;

            BuildUI();

            this.Resize += MainForm_Resize;
            this.Shown += MainForm_Shown;
        }

        private void MainForm_Shown(object? sender, EventArgs e)
        {
            SelectTab(_btnDashboard);
        }

        private void MainForm_Resize(object? sender, EventArgs e)
        {
            if (_contentPanel != null && _contentPanel.Controls.Count > 0)
            {
                foreach (Control c in _contentPanel.Controls)
                {
                    if (c is UserControl uc)
                    {
                        uc.Invalidate();
                        uc.PerformLayout();
                    }
                }
            }
        }

        private void BuildUI()
        {
            this.Text = "System Monitor Pro";
            this.Size = new Size(1200, 750);
            this.MinimumSize = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = ThemeColors.Background;
            this.ForeColor = ThemeColors.TextPrimary;
            this.FormBorderStyle = FormBorderStyle.None;

            BuildTitleBar();
            BuildNavPanel();
            BuildContentPanel();
        }

        private void BuildTitleBar()
        {
            _titleBar = new Panel
            {
                Height = 40,
                Dock = DockStyle.Top,
                BackColor = ThemeColors.Surface,
            };

            var lblTitle = new Label
            {
                Text = "◈  System Monitor Pro",
                ForeColor = ThemeColors.TextPrimary,
                Font = new Font("Segoe UI", 10f),
                AutoSize = true,
                Location = new Point(12, 10),
            };

            _btnClose = MakeTitleButton("✕", Color.FromArgb(196, 43, 28));
            _btnMaximize = MakeTitleButton("□", ThemeColors.SurfaceLight);
            _btnMinimize = MakeTitleButton("─", ThemeColors.SurfaceLight);

            _btnClose.Click += (s, e) => Application.Exit();
            _btnMaximize.Click += (s, e) =>
                this.WindowState = this.WindowState == FormWindowState.Maximized
                    ? FormWindowState.Normal : FormWindowState.Maximized;
            _btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

            void PositionButtons()
            {
                _btnClose.Location = new Point(_titleBar.Width - 40, 0);
                _btnMaximize.Location = new Point(_titleBar.Width - 80, 0);
                _btnMinimize.Location = new Point(_titleBar.Width - 120, 0);
            }

            _titleBar.Resize += (s, e) => PositionButtons();
            this.Shown += (s, e) => PositionButtons();

            void StartDrag(object? s, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                { ReleaseCapture(); SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0); }
            }

            _titleBar.MouseDown += StartDrag;
            lblTitle.MouseDown += StartDrag;

            _titleBar.Controls.AddRange(new Control[]
                { lblTitle, _btnClose, _btnMaximize, _btnMinimize });

            this.Controls.Add(_titleBar);
        }

        private Button MakeTitleButton(string text, Color hoverColor)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(40, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = ThemeColors.Surface,
                ForeColor = ThemeColors.TextPrimary,
                Font = new Font("Segoe UI", 10f),
                Cursor = Cursors.Hand,
                TabStop = false,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = hoverColor;
            btn.FlatAppearance.MouseDownBackColor = hoverColor;
            return btn;
        }

        private void BuildNavPanel()
        {
            _navPanel = new Panel
            {
                Width = 180,
                Dock = DockStyle.Left,
                BackColor = ThemeColors.Surface,
            };

            _btnDisk = MakeNavButton("💾  Диск");
            _btnFiles = MakeNavButton("📁  Файлы");
            _btnProcesses = MakeNavButton("⚙️  Процессы");
            _btnDashboard = MakeNavButton("📊  Дашборд");

            _navPanel.Controls.AddRange(new Control[]
                { _btnDisk, _btnFiles, _btnProcesses, _btnDashboard });

            _btnDashboard.Click += (s, e) => SelectTab(_btnDashboard);
            _btnProcesses.Click += (s, e) => SelectTab(_btnProcesses);
            _btnFiles.Click += (s, e) => SelectTab(_btnFiles);
            _btnDisk.Click += (s, e) => SelectTab(_btnDisk);

            this.Controls.Add(_navPanel);
        }

        private Button MakeNavButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Height = 48,
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                BackColor = ThemeColors.Surface,
                ForeColor = ThemeColors.TextSecondary,
                Font = new Font("Segoe UI", 10f),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                Cursor = Cursors.Hand,
                TabStop = false,
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ThemeColors.SurfaceLight;
            return btn;
        }

        private void BuildContentPanel()
        {
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeColors.Background,
            };
            this.Controls.Add(_contentPanel);
        }

        private void SelectTab(Button tab)
        {
            if (_activeTab != null)
            {
                _activeTab.BackColor = ThemeColors.Surface;
                _activeTab.ForeColor = ThemeColors.TextSecondary;
            }

            tab.BackColor = ThemeColors.SurfaceLight;
            tab.ForeColor = ThemeColors.TextPrimary;
            _activeTab = tab;

            if (_contentPanel.Controls.Count > 0)
            {
                foreach (Control c in _contentPanel.Controls)
                {
                    c.Dispose();
                }
                _contentPanel.Controls.Clear();
            }

            Control view;
            if (tab == _btnDashboard)
            {
                view = new Views.DashboardView();
                view.Dock = DockStyle.Fill;
            }
            else
            {
                view = new Label
                {
                    Text = tab.Text.Trim() + " — в разработке",
                    ForeColor = ThemeColors.TextSecondary,
                    Font = new Font("Segoe UI", 14f),
                    AutoSize = true,
                    Location = new Point(20, 20),
                };
            }

            _contentPanel.Controls.Add(view);
            _contentPanel.Invalidate();
            _contentPanel.PerformLayout();
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int border = 6;

            if (m.Msg == WM_NCHITTEST && this.WindowState == FormWindowState.Normal)
            {
                var pos = PointToClient(new Point(m.LParam.ToInt32()));
                int x = pos.X, y = pos.Y, w = this.Width, h = this.Height;

                if (x < border && y < border) { m.Result = (IntPtr)13; return; }
                if (x > w - border && y < border) { m.Result = (IntPtr)14; return; }
                if (x < border && y > h - border) { m.Result = (IntPtr)16; return; }
                if (x > w - border && y > h - border) { m.Result = (IntPtr)17; return; }
                if (x < border) { m.Result = (IntPtr)10; return; }
                if (x > w - border) { m.Result = (IntPtr)11; return; }
                if (y < border) { m.Result = (IntPtr)12; return; }
                if (y > h - border) { m.Result = (IntPtr)15; return; }
            }

            base.WndProc(ref m);
        }
    }
}
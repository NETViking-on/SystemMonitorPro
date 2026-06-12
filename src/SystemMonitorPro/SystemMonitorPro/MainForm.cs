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

        public MainForm()
        {
            InitializeComponent();
            BuildUI();
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
            this.DoubleBuffered = true;

            BuildTitleBar();
            BuildNavPanel();
            BuildContentPanel();

            
            SelectTab(_btnDashboard);
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
                Text = "⬡  System Monitor Pro",
                ForeColor = ThemeColors.TextPrimary,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(12, 10),
            };

            
            _btnClose = MakeTitleButton("✕", Color.FromArgb(196, 43, 28));
            _btnClose.Click += (s, e) => Application.Exit();

            
            _btnMaximize = MakeTitleButton("□", ThemeColors.SurfaceLight);
            _btnMaximize.Click += (s, e) =>
                this.WindowState = this.WindowState == FormWindowState.Maximized
                    ? FormWindowState.Normal
                    : FormWindowState.Maximized;

            
            _btnMinimize = MakeTitleButton("─", ThemeColors.SurfaceLight);
            _btnMinimize.Click += (s, e) => this.WindowState = FormWindowState.Minimized;

            
            _titleBar.Resize += (s, e) =>
            {
                _btnClose.Location = new Point(_titleBar.Width - 40, 0);
                _btnMaximize.Location = new Point(_titleBar.Width - 80, 0);
                _btnMinimize.Location = new Point(_titleBar.Width - 120, 0);
            };

           
            _titleBar.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };
            lblTitle.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };

            _titleBar.Controls.AddRange(new Control[]
            {
                lblTitle, _btnClose, _btnMaximize, _btnMinimize
            });

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
                Width = 200,
                Dock = DockStyle.Left,
                BackColor = ThemeColors.Surface,
                Padding = new Padding(0, 8, 0, 0),
            };

            _btnDashboard = MakeNavButton("📊  Дашборд", 0);
            _btnProcesses = MakeNavButton("⚙️  Процессы", 1);
            _btnFiles = MakeNavButton("📁  Файлы", 2);
            _btnDisk = MakeNavButton("💾  Диск", 3);

            _btnDashboard.Click += (s, e) => SelectTab(_btnDashboard);
            _btnProcesses.Click += (s, e) => SelectTab(_btnProcesses);
            _btnFiles.Click += (s, e) => SelectTab(_btnFiles);
            _btnDisk.Click += (s, e) => SelectTab(_btnDisk);

            _navPanel.Controls.AddRange(new Control[]
            {
                _btnDashboard, _btnProcesses, _btnFiles, _btnDisk
            });

            this.Controls.Add(_navPanel);
        }

        private Button MakeNavButton(string text, int index)
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

            
            _navPanel?.Controls.Add(btn);

            return btn;
        }

       
        private void BuildContentPanel()
        {
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeColors.Background,
                Padding = new Padding(20),
            };

            this.Controls.Add(_contentPanel);
        }

        
        private Button? _activeTab;

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

            
            _contentPanel.Controls.Clear();

            
            var lbl = new Label
            {
                Text = tab.Text.Trim() + " — в разработке",
                ForeColor = ThemeColors.TextSecondary,
                Font = new Font("Segoe UI", 14f),
                AutoSize = true,
                Location = new Point(20, 20),
            };
            _contentPanel.Controls.Add(lbl);
        }

       
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;
            const int HTLEFT = 10;
            const int HTRIGHT = 11;
            const int HTTOP = 12;
            const int HTBOTTOM = 15;
            const int HTTOPLEFT = 13;
            const int HTTOPRIGHT = 14;
            const int HTBOTTOMLEFT = 16;
            const int HTBOTTOMRIGHT = 17;
            const int border = 6;

            if (m.Msg == WM_NCHITTEST && this.WindowState == FormWindowState.Normal)
            {
                var pos = PointToClient(new Point(m.LParam.ToInt32()));
                int x = pos.X, y = pos.Y, w = this.Width, h = this.Height;

                if (x < border && y < border) { m.Result = (IntPtr)HTTOPLEFT; return; }
                if (x > w - border && y < border) { m.Result = (IntPtr)HTTOPRIGHT; return; }
                if (x < border && y > h - border) { m.Result = (IntPtr)HTBOTTOMLEFT; return; }
                if (x > w - border && y > h - border) { m.Result = (IntPtr)HTBOTTOMRIGHT; return; }
                if (x < border) { m.Result = (IntPtr)HTLEFT; return; }
                if (x > w - border) { m.Result = (IntPtr)HTRIGHT; return; }
                if (y < border) { m.Result = (IntPtr)HTTOP; return; }
                if (y > h - border) { m.Result = (IntPtr)HTBOTTOM; return; }
            }

            base.WndProc(ref m);
        }
    }
}
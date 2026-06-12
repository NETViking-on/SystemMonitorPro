using SystemMonitorPro.Controls;
using SystemMonitorPro.Helpers;
using SystemMonitorPro.Models;

namespace SystemMonitorPro.Views
{
    public class DashboardView : UserControl
    {
        private LineChartControl _cpuChart = null!;
        private LineChartControl _ramChart = null!;
        private LineChartControl _diskChart = null!;

        private Label _lblTitle = null!;
        private Panel _contentPanel = null!;
        private Panel _chartsContainer = null!;

        private readonly PerformanceService _service;
        private readonly System.Windows.Forms.Timer _timer;

        private const int ChartWidth = 420;
        private const int ChartHeight = 300;
        private const int ChartMargin = 10;

        
        private const int OffsetX = 40;

        public DashboardView()
        {
            DoubleBuffered = true;
            Dock = DockStyle.Fill;
            BackColor = ThemeColors.Background;

            _service = new PerformanceService();

            BuildUI();
            CenterCharts();

            _timer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            _timer.Tick += OnTick;

            this.HandleCreated += (s, e) => _timer.Start();
            this.Disposed += (s, e) =>
            {
                _timer?.Stop();
                _timer?.Dispose();
                _service?.Dispose();
            };

            
            this.Resize += (s, e) => CenterCharts();
        }

        private void BuildUI()
        {
           
            _lblTitle = new Label
            {
                Text = "Производительность системы",
                ForeColor = ThemeColors.TextPrimary,
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(20, 12, 0, 0)
            };

            
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ThemeColors.Background
            };

            
            _chartsContainer = new Panel
            {
                BackColor = ThemeColors.Background,
                Width = ChartWidth * 2 + ChartMargin * 2 + 10,
                Height = ChartHeight * 2 + ChartMargin * 2 + 10
            };

            
            _cpuChart = new LineChartControl
            {
                ChartLabel = "CPU",
                LineColor = ThemeColors.CpuChart,
                MaxValue = 100f,
                Width = ChartWidth,
                Height = ChartHeight
            };

            _ramChart = new LineChartControl
            {
                ChartLabel = "RAM",
                LineColor = ThemeColors.RamChart,
                MaxValue = _service.TotalRamGb > 0 ? _service.TotalRamGb : 16f,
                Width = ChartWidth,
                Height = ChartHeight
            };

            _diskChart = new LineChartControl
            {
                ChartLabel = "Диск (MB/s)",
                LineColor = ThemeColors.DiskRead,
                MaxValue = 500f,
                Width = ChartWidth,
                Height = ChartHeight
            };

           
            _cpuChart.Location = new Point(ChartMargin, ChartMargin);
            _ramChart.Location = new Point(ChartWidth + ChartMargin + ChartMargin + 5, ChartMargin);
            _diskChart.Location = new Point(ChartMargin, ChartHeight + ChartMargin + ChartMargin + 5);

            _chartsContainer.Controls.Add(_cpuChart);
            _chartsContainer.Controls.Add(_ramChart);
            _chartsContainer.Controls.Add(_diskChart);

            _contentPanel.Controls.Add(_chartsContainer);
            Controls.Add(_contentPanel);
            Controls.Add(_lblTitle);
        }

        private void CenterCharts()
        {
            if (_contentPanel == null || _contentPanel.IsDisposed) return;
            if (_chartsContainer == null || _chartsContainer.IsDisposed) return;
            if (_contentPanel.Width <= 0 || _contentPanel.Height <= 0) return;

            
            float scale = Math.Min(
                (float)_contentPanel.Width / 900f,
                (float)_contentPanel.Height / 680f
            );
            scale = Math.Clamp(scale, 0.7f, 1.6f);

            int cardW = (int)(ChartWidth * scale);
            int cardH = (int)(ChartHeight * scale);
            int margin = (int)(ChartMargin * scale);

            
            _cpuChart.Size = new Size(cardW, cardH);
            _ramChart.Size = new Size(cardW, cardH);
            _diskChart.Size = new Size(cardW, cardH);

            
            _cpuChart.Location = new Point(margin, margin);
            _ramChart.Location = new Point(cardW + margin * 2 + 5, margin);
            _diskChart.Location = new Point(margin, cardH + margin * 2 + 5);

            
            _chartsContainer.Size = new Size(
                cardW * 2 + margin * 3 + 5,
                cardH * 2 + margin * 3 + 5
            );

            
            int x = (_contentPanel.Width - _chartsContainer.Width) / 2 + OffsetX;
            int y = (_contentPanel.Height - _chartsContainer.Height) / 2;

            if (x < 10) x = 10;
            if (y < 10) y = 10;

            _chartsContainer.Location = new Point(x, y);
        }

        private void OnTick(object? sender, EventArgs e)
        {
            if (this.IsDisposed || !this.IsHandleCreated) return;

            Task.Run(() =>
            {
                try
                {
                    PerformanceData data = _service.Collect();

                    if (this.IsDisposed || !this.IsHandleCreated) return;

                    this.BeginInvoke(new Action(() =>
                    {
                        if (this.IsDisposed) return;
                        _cpuChart.PushValue(data.CpuLoad);
                        _ramChart.PushValue(data.RamUsedGb);
                        _diskChart.PushValue(data.DiskReadMbs);
                    }));
                }
                catch { /* Игнорируем ошибки */ }
            });
        }
    }
}
using System.Drawing.Drawing2D;
using SystemMonitorPro.Helpers;

namespace SystemMonitorPro.Controls
{
    public class LineChartControl : Control
    {
        private const int MaxPoints = 60;

        private readonly List<float> _values = new();

        private Color _lineColor = ThemeColors.CpuChart;
        private string _label = "CPU";
        private float _maxValue = 100f;

        public Color LineColor
        {
            get => _lineColor;
            set
            {
                _lineColor = value;
                Invalidate();
            }
        }

        public string ChartLabel
        {
            get => _label;
            set
            {
                _label = value;
                Invalidate();
            }
        }

        public float MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                Invalidate();
            }
        }

        public LineChartControl()
        {
            DoubleBuffered = true;

            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.OptimizedDoubleBuffer,
                true);

            BackColor = ThemeColors.Surface;

            
            for (int i = 0; i < MaxPoints; i++)
                _values.Add(0);
        }

        public void PushValue(float value)
        {
            value = Math.Clamp(value, 0f, _maxValue);

            _values.RemoveAt(0);
            _values.Add(value);

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int w = Width;
            int h = Height;

            if (w <= 0 || h <= 0) return;

            g.Clear(ThemeColors.Surface);

            int topPadding = 45;
            int bottomPadding = 28;
            int chartHeight = h - topPadding - bottomPadding;

            
            using (Pen gridPen = new Pen(ThemeColors.Border))
            {
                gridPen.DashStyle = DashStyle.Dash;

                for (int i = 1; i <= 4; i++)
                {
                    int y = topPadding + chartHeight * i / 4;
                    g.DrawLine(gridPen, 0, y, w, y);
                }
            }

            
            float stepX = (float)(w - 1) / (MaxPoints - 1);

            List<PointF> linePoints = new();

            for (int i = 0; i < MaxPoints; i++)
            {
                float x = i * stepX;

                float y = topPadding + chartHeight - (_values[i] / _maxValue) * chartHeight;
                y = Math.Clamp(y, topPadding, topPadding + chartHeight);

                linePoints.Add(new PointF(x, y));
            }

            
            List<PointF> polygon = new();
            polygon.Add(new PointF(0, h - bottomPadding));
            polygon.AddRange(linePoints);
            polygon.Add(new PointF(w, h - bottomPadding));

            using (SolidBrush brush = new SolidBrush(Color.FromArgb(40, _lineColor)))
            {
                g.FillPolygon(brush, polygon.ToArray());
            }

           
            using (Pen linePen = new Pen(_lineColor, 2f))
            {
                g.DrawLines(linePen, linePoints.ToArray());
            }

            
            float current = _values[^1];

            string valueText;

            if (_label == "RAM")
                valueText = $"{current:F1} GB";
            else if (_label.Contains("Disk"))
                valueText = $"{current:F1}";
            else
                valueText = $"{current:F0}%";

            using (Font valueFont = new Font("Segoe UI", 18, FontStyle.Bold))
            using (Brush valueBrush = new SolidBrush(ThemeColors.TextPrimary))
            {
                g.DrawString(valueText, valueFont, valueBrush, 10, 8);
            }

            
            using (Font labelFont = new Font("Segoe UI", 9))
            using (Brush labelBrush = new SolidBrush(ThemeColors.TextSecondary))
            {
                g.DrawString(_label, labelFont, labelBrush, 10, h - 20);
            }

            
            using (Pen borderPen = new Pen(ThemeColors.Border))
            {
                g.DrawRectangle(borderPen, 0, 0, w - 1, h - 1);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}
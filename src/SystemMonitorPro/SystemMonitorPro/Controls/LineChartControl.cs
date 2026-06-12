using SystemMonitorPro.Helpers;

namespace SystemMonitorPro.Controls
{
    
    public class LineChartControl : Control
    {
        
        private readonly int _maxPoints = 60;    
        private readonly Queue<float> _values = new();
        private Color _lineColor = ThemeColors.CpuChart;
        private string _label = "CPU";
        private float _maxValue = 100f;

        public Color LineColor
        {
            get => _lineColor;
            set { _lineColor = value; Invalidate(); }
        }

        public string ChartLabel
        {
            get => _label;
            set { _label = value; Invalidate(); }
        }

        public float MaxValue
        {
            get => _maxValue;
            set { _maxValue = value; Invalidate(); }
        }

        public LineChartControl()
        {
            
            this.DoubleBuffered = true;
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);

            this.BackColor = ThemeColors.Surface;
        }

        
        public void PushValue(float value)
        {
            if (_values.Count >= _maxPoints)
                _values.Dequeue();

            _values.Enqueue(Math.Clamp(value, 0f, _maxValue));
            Invalidate(); 
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int w = this.Width;
            int h = this.Height;

            
            g.Clear(ThemeColors.Surface);

            
            using var gridPen = new Pen(ThemeColors.Border, 1f);
            gridPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            for (int i = 1; i <= 4; i++)
            {
                int y = (int)(h * i / 4f);
                g.DrawLine(gridPen, 0, y, w, y);
            }

            
            var points = _values.ToArray();
            if (points.Length >= 2)
            {
                float stepX = (float)w / (_maxPoints - 1);

                
                var fillPoints = new List<PointF>();
                fillPoints.Add(new PointF(0, h)); 

                for (int i = 0; i < points.Length; i++)
                {
                    float x = (i + (_maxPoints - points.Length)) * stepX;
                    float y = h - (points[i] / _maxValue * (h - 20));
                    fillPoints.Add(new PointF(x, y));
                }

                fillPoints.Add(new PointF(fillPoints[^1].X, h)); 

                using var fillBrush = new SolidBrush(
                    Color.FromArgb(40, _lineColor));
                g.FillPolygon(fillBrush, fillPoints.ToArray());

                
                var linePoints = fillPoints
                    .Skip(1).Take(points.Length).ToArray();

                using var linePen = new Pen(_lineColor, 2f);
                g.DrawLines(linePen, linePoints);
            }

            
            float current = points.Length > 0 ? points[^1] : 0f;
            string valueText = _label == "RAM"
                ? $"{current:F1} GB"
                : $"{current:F0}%";

            using var valueBrush = new SolidBrush(ThemeColors.TextPrimary);
            using var valueFont = new Font("Segoe UI", 18f, FontStyle.Bold);
            g.DrawString(valueText, valueFont, valueBrush, new PointF(8, 6));

            
            using var labelBrush = new SolidBrush(ThemeColors.TextSecondary);
            using var labelFont = new Font("Segoe UI", 9f);
            g.DrawString(_label, labelFont, labelBrush, new PointF(8, h - 20));

            
            using var borderPen = new Pen(ThemeColors.Border, 1f);
            g.DrawRectangle(borderPen, 0, 0, w - 1, h - 1);
        }
    }
}
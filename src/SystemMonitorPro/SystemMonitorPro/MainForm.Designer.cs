namespace SystemMonitorPro
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer? components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(884, 919);
            MinimumSize = new Size(900, 600);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "System Monitor Pro";
            ResumeLayout(false);
        }
    }
}
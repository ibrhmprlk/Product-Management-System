namespace ÜrünYönetimSistemi
{
    partial class frmverilerAktarılıyor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            timer1 = new System.Windows.Forms.Timer(components);
            panel1 = new Panel();
            progressBar1 = new ProgressBar();
            label1 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // timer1
            // 
            timer1.Tick += timer1_Tick;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(progressBar1);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(472, 50);
            panel1.TabIndex = 3;
            panel1.Paint += panel1_Paint;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(180, 10);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(260, 29);
            progressBar1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 13);
            label1.Name = "label1";
            label1.Size = new Size(123, 20);
            label1.TabIndex = 1;
            label1.Text = "Veriler Aktarılıyor";
            // 
            // frmverilerAktarılıyor
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(472, 50);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "frmverilerAktarılıyor";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmverilerAktarılıyor";
            Load += frmverilerAktarılıyor_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private Panel panel1;
        private ProgressBar progressBar1;
        private Label label1;
    }
}
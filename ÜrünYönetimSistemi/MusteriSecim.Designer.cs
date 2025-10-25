namespace ÜrünYönetimSistemi
{
    partial class MusteriSecim
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MusteriSecim));
            dataGridView1 = new DataGridView();
            ımageList1 = new ImageList(components);
            label1 = new Label();
            textBox2 = new TextBox();
            panel1 = new Panel();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(4, 62);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(532, 309);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellClick += dataGridView1_CellClick;
            // 
            // ımageList1
            // 
            ımageList1.ColorDepth = ColorDepth.Depth32Bit;
            ımageList1.ImageStream = (ImageListStreamer)resources.GetObject("ımageList1.ImageStream");
            ımageList1.TransparentColor = Color.Transparent;
            ımageList1.Images.SetKeyName(0, "Delete3.png");
            ımageList1.Images.SetKeyName(1, "Restart1.png");
            ımageList1.Images.SetKeyName(2, "Add Shopping Cart.png");
            ımageList1.Images.SetKeyName(3, "Edit Pencil.png");
            ımageList1.Images.SetKeyName(4, "Broom.png");
            ımageList1.Images.SetKeyName(5, "Cancel.png");
            ımageList1.Images.SetKeyName(6, "Product.png");
            ımageList1.Images.SetKeyName(7, "Left.png");
            ımageList1.Images.SetKeyName(8, "Right.png");
            ımageList1.Images.SetKeyName(9, "Add.png");
            ımageList1.Images.SetKeyName(10, "Go Back.png");
            ımageList1.Images.SetKeyName(11, "Cancel1.png");
            ımageList1.Images.SetKeyName(12, "Delete4.png");
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 33);
            label1.Name = "label1";
            label1.Size = new Size(30, 20);
            label1.TabIndex = 8;
            label1.Text = "Bul";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(39, 29);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(497, 27);
            textBox2.TabIndex = 7;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(label2);
            panel1.Controls.Add(dataGridView1);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(textBox2);
            panel1.Location = new Point(3, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(543, 378);
            panel1.TabIndex = 9;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(36, 6);
            label2.Name = "label2";
            label2.Size = new Size(266, 20);
            label2.TabIndex = 9;
            label2.Text = "Müşteri Adı veya GSM Telefon No Girin";
            // 
            // MusteriSecim
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(550, 394);
            Controls.Add(panel1);
            Cursor = Cursors.Hand;
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MusteriSecim";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Müşteri Seçim";
            Load += MusteriSecim_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridView1;
        private Label label1;
        private TextBox textBox2;
        private ImageList ımageList1;
        private Panel panel1;
        private Label label2;
    }
}
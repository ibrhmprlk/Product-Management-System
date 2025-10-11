namespace ÜrünYönetimSistemi
{
    partial class ToptanciBorcListesi
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToptanciBorcListesi));
            panel1 = new Panel();
            button1 = new Button();
            ımageList1 = new ImageList(components);
            dataGridView1 = new DataGridView();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            checkBox3 = new CheckBox();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            label1 = new Label();
            label2 = new Label();
            button2 = new Button();
            button3 = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(button1);
            panel1.Controls.Add(dataGridView1);
            panel1.Location = new Point(12, 120);
            panel1.Name = "panel1";
            panel1.Size = new Size(1065, 423);
            panel1.TabIndex = 0;
            // 
            // button1
            // 
            button1.ImageKey = "Export Excel.png";
            button1.ImageList = ımageList1;
            button1.Location = new Point(4, 363);
            button1.Name = "button1";
            button1.Size = new Size(1057, 57);
            button1.TabIndex = 1;
            button1.Text = "Tabloyu Excel'e Aktar";
            button1.TextAlign = ContentAlignment.MiddleRight;
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // ımageList1
            // 
            ımageList1.ColorDepth = ColorDepth.Depth32Bit;
            ımageList1.ImageStream = (ImageListStreamer)resources.GetObject("ımageList1.ImageStream");
            ımageList1.TransparentColor = Color.Transparent;
            ımageList1.Images.SetKeyName(0, "Cancel1.png");
            ımageList1.Images.SetKeyName(1, "Broom.png");
            ımageList1.Images.SetKeyName(2, "Go Back.png");
            ımageList1.Images.SetKeyName(3, "Add Shopping Cart.png");
            ımageList1.Images.SetKeyName(4, "Add.png");
            ımageList1.Images.SetKeyName(5, "Broom.png");
            ımageList1.Images.SetKeyName(6, "Cancel.png");
            ımageList1.Images.SetKeyName(7, "Delete4.png");
            ımageList1.Images.SetKeyName(8, "Lowest Price.png");
            ımageList1.Images.SetKeyName(9, "Cash in Hand.png");
            ımageList1.Images.SetKeyName(10, "6008154.png");
            ımageList1.Images.SetKeyName(11, "Card Payment.png");
            ımageList1.Images.SetKeyName(12, "nakit+kredi kartı.jpg");
            ımageList1.Images.SetKeyName(13, "Online Money Transfer.png");
            ımageList1.Images.SetKeyName(14, "9359564.png");
            ımageList1.Images.SetKeyName(15, "Printer.png");
            ımageList1.Images.SetKeyName(16, "Add Male User.png");
            ımageList1.Images.SetKeyName(17, "Customer.png");
            ımageList1.Images.SetKeyName(18, "Edit Pencil.png");
            ımageList1.Images.SetKeyName(19, "List View.png");
            ımageList1.Images.SetKeyName(20, "Add1.png");
            ımageList1.Images.SetKeyName(21, "View.png");
            ımageList1.Images.SetKeyName(22, "images (1).png");
            ımageList1.Images.SetKeyName(23, "TikTok Verified Account.png");
            ımageList1.Images.SetKeyName(24, "Export Excel.png");
            ımageList1.Images.SetKeyName(25, "Chart.png");
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(4, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(1057, 354);
            dataGridView1.TabIndex = 0;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(715, 90);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(135, 24);
            checkBox1.TabIndex = 1;
            checkBox1.Text = "Ada Göre Sırala";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(873, 90);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(204, 24);
            checkBox2.TabIndex = 2;
            checkBox2.Text = "Borç Miktarına Göre Sırala";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Location = new Point(16, 546);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(216, 24);
            checkBox3.TabIndex = 3;
            checkBox3.Text = "Borcu 0.00 TL Olanları Sırala";
            checkBox3.UseVisualStyleBackColor = true;
            checkBox3.CheckedChanged += checkBox3_CheckedChanged;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(948, 553);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(125, 27);
            textBox1.TabIndex = 4;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(50, 87);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Toptancı Adı veya GSM Telefon No Girin";
            textBox2.Size = new Size(431, 27);
            textBox2.TabIndex = 5;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 91);
            label1.Name = "label1";
            label1.Size = new Size(30, 20);
            label1.TabIndex = 6;
            label1.Text = "Bul";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(848, 559);
            label2.Name = "label2";
            label2.Size = new Size(93, 20);
            label2.TabIndex = 7;
            label2.Text = "Toplam Borç";
            // 
            // button2
            // 
            button2.ImageAlign = ContentAlignment.MiddleLeft;
            button2.ImageKey = "Go Back.png";
            button2.ImageList = ımageList1;
            button2.Location = new Point(1037, 12);
            button2.Name = "button2";
            button2.Size = new Size(121, 63);
            button2.TabIndex = 8;
            button2.Text = "Geri Dön";
            button2.TextAlign = ContentAlignment.MiddleRight;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.ImageAlign = ContentAlignment.MiddleLeft;
            button3.ImageKey = "Cancel1.png";
            button3.ImageList = ımageList1;
            button3.Location = new Point(1164, 12);
            button3.Name = "button3";
            button3.Size = new Size(121, 63);
            button3.TabIndex = 9;
            button3.Text = "Kapat";
            button3.TextImageRelation = TextImageRelation.ImageBeforeText;
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click_1;
            // 
            // ToptanciBorcListesi
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1297, 595);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(checkBox3);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "ToptanciBorcListesi";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Toptancı Borç Listesi";
            Load += ToptanciBorcListesi_Load;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private DataGridView dataGridView1;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label1;
        private Button button1;
        private Label label2;
        private Button button2;
        private Button button3;
        private ImageList ımageList1;
    }
}
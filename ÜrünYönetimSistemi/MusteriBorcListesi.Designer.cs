namespace ÜrünYönetimSistemi
{
    partial class MusteriBorcListesi
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MusteriBorcListesi));
            button3 = new Button();
            ımageList1 = new ImageList(components);
            button2 = new Button();
            label2 = new Label();
            label1 = new Label();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            checkBox3 = new CheckBox();
            checkBox2 = new CheckBox();
            checkBox1 = new CheckBox();
            panel1 = new Panel();
            button1 = new Button();
            dataGridView1 = new DataGridView();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // button3
            // 
            button3.ImageAlign = ContentAlignment.MiddleLeft;
            button3.ImageKey = "Cancel1.png";
            button3.ImageList = ımageList1;
            button3.Location = new Point(1158, 12);
            button3.Name = "button3";
            button3.Size = new Size(129, 63);
            button3.TabIndex = 19;
            button3.Text = "Kapat";
            button3.TextImageRelation = TextImageRelation.ImageBeforeText;
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
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
            // 
            // button2
            // 
            button2.ImageAlign = ContentAlignment.MiddleLeft;
            button2.ImageKey = "Go Back.png";
            button2.ImageList = ımageList1;
            button2.Location = new Point(1158, 81);
            button2.Name = "button2";
            button2.Size = new Size(129, 63);
            button2.TabIndex = 18;
            button2.Text = "Geri Dön";
            button2.TextImageRelation = TextImageRelation.ImageBeforeText;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(872, 564);
            label2.Name = "label2";
            label2.Size = new Size(93, 20);
            label2.TabIndex = 17;
            label2.Text = "Toplam Borç";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(38, 96);
            label1.Name = "label1";
            label1.Size = new Size(30, 20);
            label1.TabIndex = 16;
            label1.Text = "Bul";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(74, 92);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Toptancı Adı veya GSM Telefon No Girin";
            textBox2.Size = new Size(431, 27);
            textBox2.TabIndex = 15;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(972, 558);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(125, 27);
            textBox1.TabIndex = 14;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Location = new Point(40, 551);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(216, 24);
            checkBox3.TabIndex = 13;
            checkBox3.Text = "Borcu 0.00 TL Olanları Sırala";
            checkBox3.UseVisualStyleBackColor = true;
            checkBox3.CheckedChanged += checkBox3_CheckedChanged;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(897, 95);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(204, 24);
            checkBox2.TabIndex = 12;
            checkBox2.Text = "Borç Miktarına Göre Sırala";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(739, 95);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(135, 24);
            checkBox1.TabIndex = 11;
            checkBox1.Text = "Ada Göre Sırala";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(button1);
            panel1.Controls.Add(dataGridView1);
            panel1.Location = new Point(36, 125);
            panel1.Name = "panel1";
            panel1.Size = new Size(1065, 423);
            panel1.TabIndex = 10;
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
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(4, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(1057, 354);
            dataGridView1.TabIndex = 0;
            // 
            // MusteriBorcListesi
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1298, 644);
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
            Name = "MusteriBorcListesi";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Müşteri Borç Listesi";
            Load += MusteriBorcListesi_Load;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button3;
        private Button button2;
        private Label label2;
        private Label label1;
        private TextBox textBox2;
        private TextBox textBox1;
        private CheckBox checkBox3;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
        private Panel panel1;
        private Button button1;
        private DataGridView dataGridView1;
        private ImageList ımageList1;
    }
}
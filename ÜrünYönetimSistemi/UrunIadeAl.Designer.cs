namespace ÜrünYönetimSistemi
{
    partial class UrunIadeAl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UrunIadeAl));
            textBox5 = new TextBox();
            textBox4 = new TextBox();
            label3 = new Label();
            button5 = new Button();
            ımageList1 = new ImageList(components);
            button2 = new Button();
            textBox3 = new TextBox();
            label2 = new Label();
            button1 = new Button();
            label1 = new Label();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            panel1 = new Panel();
            dataGridView1 = new DataGridView();
            timer1 = new System.Windows.Forms.Timer(components);
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // textBox5
            // 
            textBox5.Location = new Point(6, 38);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(125, 27);
            textBox5.TabIndex = 41;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(5, 3);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(125, 27);
            textBox4.TabIndex = 40;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(725, 79);
            label3.Name = "label3";
            label3.Size = new Size(183, 20);
            label3.TabIndex = 39;
            label3.Text = "İade Alınacak Miktarı Girin";
            // 
            // button5
            // 
            button5.ImageAlign = ContentAlignment.MiddleLeft;
            button5.ImageKey = "Cancel1.png";
            button5.ImageList = ımageList1;
            button5.Location = new Point(1077, 3);
            button5.Name = "button5";
            button5.Size = new Size(129, 63);
            button5.TabIndex = 38;
            button5.Text = "Kapat";
            button5.TextImageRelation = TextImageRelation.ImageBeforeText;
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
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
            ımageList1.Images.SetKeyName(24, "Get Cash.png");
            ımageList1.Images.SetKeyName(25, "Exclamation Mark.png");
            // 
            // button2
            // 
            button2.ImageAlign = ContentAlignment.MiddleLeft;
            button2.ImageKey = "Cash in Hand.png";
            button2.ImageList = ımageList1;
            button2.Location = new Point(919, 515);
            button2.Name = "button2";
            button2.Size = new Size(146, 70);
            button2.TabIndex = 35;
            button2.Text = "Müşteriye Nakit Ödendi";
            button2.TextImageRelation = TextImageRelation.ImageBeforeText;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(74, 524);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(125, 27);
            textBox3.TabIndex = 34;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 527);
            label2.Name = "label2";
            label2.Size = new Size(59, 20);
            label2.TabIndex = 33;
            label2.Text = "Toplam";
            // 
            // button1
            // 
            button1.ImageAlign = ContentAlignment.TopLeft;
            button1.ImageKey = "Exclamation Mark.png";
            button1.ImageList = ımageList1;
            button1.Location = new Point(767, 515);
            button1.Name = "button1";
            button1.Size = new Size(146, 70);
            button1.TabIndex = 32;
            button1.Text = "İade Türünü Belirtmek İstemiyorum ";
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 82);
            label1.Name = "label1";
            label1.Size = new Size(30, 20);
            label1.TabIndex = 31;
            label1.Text = "Bul";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(49, 76);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Ürün Adı Veya Barkod No Girin";
            textBox2.Size = new Size(417, 27);
            textBox2.TabIndex = 30;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(910, 75);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(155, 27);
            textBox1.TabIndex = 29;
            // 
            // panel1
            // 
            panel1.Controls.Add(dataGridView1);
            panel1.Location = new Point(10, 111);
            panel1.Name = "panel1";
            panel1.Size = new Size(1055, 401);
            panel1.TabIndex = 28;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(3, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(1049, 395);
            dataGridView1.TabIndex = 0;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            // 
            // UrunIadeAl
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1208, 606);
            Controls.Add(textBox5);
            Controls.Add(textBox4);
            Controls.Add(label3);
            Controls.Add(button5);
            Controls.Add(button2);
            Controls.Add(textBox3);
            Controls.Add(label2);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "UrunIadeAl";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ürün İade Al";
            Load += UrunIadeAl_Load;
            panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox5;
        private TextBox textBox4;
        private Label label3;
        private Button button5;
        private Button button2;
        private TextBox textBox3;
        private Label label2;
        private Button button1;
        private Label label1;
        private TextBox textBox2;
        private TextBox textBox1;
        private Panel panel1;
        private DataGridView dataGridView1;
        private System.Windows.Forms.Timer timer1;
        private ImageList ımageList1;
    }
}
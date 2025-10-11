namespace ÜrünYönetimSistemi
{
    partial class UrunIadeEt
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UrunIadeEt));
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
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // textBox5
            // 
            textBox5.Location = new Point(1, 36);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(125, 27);
            textBox5.TabIndex = 54;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(0, 1);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(125, 27);
            textBox4.TabIndex = 53;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(723, 86);
            label3.Name = "label3";
            label3.Size = new Size(182, 20);
            label3.TabIndex = 52;
            label3.Text = "İade Edilecek Miktarı Girin";
            // 
            // button5
            // 
            button5.ImageAlign = ContentAlignment.MiddleLeft;
            button5.ImageKey = "Cancel1.png";
            button5.ImageList = ımageList1;
            button5.Location = new Point(1069, 12);
            button5.Name = "button5";
            button5.Size = new Size(129, 63);
            button5.TabIndex = 51;
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
            button2.Location = new Point(917, 522);
            button2.Name = "button2";
            button2.Size = new Size(146, 70);
            button2.TabIndex = 49;
            button2.Text = "Toptancı Nakit Ödedi";
            button2.TextImageRelation = TextImageRelation.ImageBeforeText;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(72, 531);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(125, 27);
            textBox3.TabIndex = 48;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 534);
            label2.Name = "label2";
            label2.Size = new Size(59, 20);
            label2.TabIndex = 47;
            label2.Text = "Toplam";
            // 
            // button1
            // 
            button1.ImageAlign = ContentAlignment.TopLeft;
            button1.ImageKey = "Exclamation Mark.png";
            button1.ImageList = ımageList1;
            button1.Location = new Point(765, 522);
            button1.Name = "button1";
            button1.Size = new Size(146, 70);
            button1.TabIndex = 46;
            button1.Text = "İade Türünü Belirtmek İstemiyorum ";
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(11, 89);
            label1.Name = "label1";
            label1.Size = new Size(30, 20);
            label1.TabIndex = 45;
            label1.Text = "Bul";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(47, 83);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Ürün Adı Veya Barkod No Girin";
            textBox2.Size = new Size(417, 27);
            textBox2.TabIndex = 44;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(908, 82);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(155, 27);
            textBox1.TabIndex = 43;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // panel1
            // 
            panel1.Controls.Add(dataGridView1);
            panel1.Location = new Point(8, 118);
            panel1.Name = "panel1";
            panel1.Size = new Size(1055, 401);
            panel1.TabIndex = 42;
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
            // UrunIadeEt
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
            Name = "UrunIadeEt";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ürün İade Et";
            Load += UrunIadeEt_Load;
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
        private ImageList ımageList1;
    }
}
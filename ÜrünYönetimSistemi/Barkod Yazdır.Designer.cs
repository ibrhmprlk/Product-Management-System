namespace ÜrünYönetimSistemi
{
    partial class Barkod_Yazdır
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Barkod_Yazdır));
            dataGridView1 = new DataGridView();
            label1 = new Label();
            textBox6 = new TextBox();
            button1 = new Button();
            ımageList1 = new ImageList(components);
            panel2 = new Panel();
            panel1 = new Panel();
            pictureBox2 = new PictureBox();
            button2 = new Button();
            checkBox9 = new CheckBox();
            checkBox5 = new CheckBox();
            checkBox8 = new CheckBox();
            checkBox7 = new CheckBox();
            checkBox6 = new CheckBox();
            textBox10 = new TextBox();
            label14 = new Label();
            textBox4 = new TextBox();
            textBox3 = new TextBox();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label16 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            panel2.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(16, 106);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(646, 379);
            dataGridView1.TabIndex = 40;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CellContentClick += dataGridView1_CellClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(47, 52);
            label1.Name = "label1";
            label1.Size = new Size(212, 20);
            label1.TabIndex = 38;
            label1.Text = "Ürün Adı Veya Barkod No Girin";
            // 
            // textBox6
            // 
            textBox6.Location = new Point(49, 73);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(610, 27);
            textBox6.TabIndex = 39;
            // 
            // button1
            // 
            button1.ImageAlign = ContentAlignment.MiddleLeft;
            button1.ImageKey = "Cancel1.png";
            button1.ImageList = ımageList1;
            button1.Location = new Point(1321, 11);
            button1.Name = "button1";
            button1.Size = new Size(110, 61);
            button1.TabIndex = 41;
            button1.Text = "Kapat";
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
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
            ımageList1.Images.SetKeyName(13, "barkod2jpg.jpg");
            ımageList1.Images.SetKeyName(14, "Printer.png");
            // 
            // panel2
            // 
            panel2.Controls.Add(panel1);
            panel2.Controls.Add(textBox10);
            panel2.Controls.Add(label14);
            panel2.Location = new Point(696, 248);
            panel2.Name = "panel2";
            panel2.Size = new Size(635, 259);
            panel2.TabIndex = 57;
            // 
            // panel1
            // 
            panel1.Controls.Add(pictureBox2);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(checkBox9);
            panel1.Controls.Add(checkBox5);
            panel1.Controls.Add(checkBox8);
            panel1.Controls.Add(checkBox7);
            panel1.Controls.Add(checkBox6);
            panel1.Location = new Point(7, 68);
            panel1.Name = "panel1";
            panel1.Size = new Size(615, 180);
            panel1.TabIndex = 71;
            // 
            // pictureBox2
            // 
            pictureBox2.InitialImage = null;
            pictureBox2.Location = new Point(3, 33);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(287, 135);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 73;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // button2
            // 
            button2.ImageAlign = ContentAlignment.MiddleLeft;
            button2.ImageKey = "Printer.png";
            button2.ImageList = ımageList1;
            button2.Location = new Point(492, 33);
            button2.Name = "button2";
            button2.Size = new Size(120, 73);
            button2.TabIndex = 72;
            button2.Text = "Yazdır";
            button2.TextImageRelation = TextImageRelation.ImageBeforeText;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // checkBox9
            // 
            checkBox9.AutoSize = true;
            checkBox9.Location = new Point(296, 63);
            checkBox9.Name = "checkBox9";
            checkBox9.Size = new Size(60, 24);
            checkBox9.TabIndex = 3;
            checkBox9.Text = "İthal";
            checkBox9.UseVisualStyleBackColor = true;
            checkBox9.CheckedChanged += checkBox9_CheckedChanged;
            // 
            // checkBox5
            // 
            checkBox5.AutoSize = true;
            checkBox5.Location = new Point(296, 3);
            checkBox5.Name = "checkBox5";
            checkBox5.Size = new Size(182, 24);
            checkBox5.TabIndex = 70;
            checkBox5.Text = "Etikette Fiyat Görünsün";
            checkBox5.UseVisualStyleBackColor = true;
            checkBox5.CheckedChanged += checkBox5_CheckedChanged;
            // 
            // checkBox8
            // 
            checkBox8.AutoSize = true;
            checkBox8.Location = new Point(296, 33);
            checkBox8.Name = "checkBox8";
            checkBox8.Size = new Size(108, 24);
            checkBox8.TabIndex = 2;
            checkBox8.Text = "Yerli Üretim";
            checkBox8.UseVisualStyleBackColor = true;
            checkBox8.CheckedChanged += checkBox8_CheckedChanged;
            // 
            // checkBox7
            // 
            checkBox7.AutoSize = true;
            checkBox7.Location = new Point(162, 3);
            checkBox7.Name = "checkBox7";
            checkBox7.Size = new Size(126, 24);
            checkBox7.TabIndex = 1;
            checkBox7.Text = "Raf Etiketi Gör";
            checkBox7.UseVisualStyleBackColor = true;
            checkBox7.CheckedChanged += checkBox7_CheckedChanged;
            // 
            // checkBox6
            // 
            checkBox6.AutoSize = true;
            checkBox6.Location = new Point(15, 3);
            checkBox6.Name = "checkBox6";
            checkBox6.Size = new Size(106, 24);
            checkBox6.TabIndex = 0;
            checkBox6.Text = "Barkod Gör";
            checkBox6.UseVisualStyleBackColor = true;
            checkBox6.CheckedChanged += checkBox6_CheckedChanged;
            // 
            // textBox10
            // 
            textBox10.Location = new Point(126, 18);
            textBox10.Name = "textBox10";
            textBox10.Size = new Size(71, 27);
            textBox10.TabIndex = 69;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(7, 25);
            label14.Name = "label14";
            label14.Size = new Size(92, 20);
            label14.TabIndex = 68;
            label14.Text = "Kopya Sayısı";
            // 
            // textBox4
            // 
            textBox4.Location = new Point(822, 206);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(251, 27);
            textBox4.TabIndex = 48;
            textBox4.TextChanged += textBox4_TextChanged;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(822, 173);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(251, 27);
            textBox3.TabIndex = 47;
            textBox3.TextChanged += textBox3_TextChanged;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(822, 140);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(251, 27);
            textBox2.TabIndex = 46;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(822, 106);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(251, 27);
            textBox1.TabIndex = 45;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(697, 212);
            label5.Name = "label5";
            label5.Size = new Size(99, 20);
            label5.TabIndex = 44;
            label5.Text = "İndirimli Fiyat";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(697, 178);
            label4.Name = "label4";
            label4.Size = new Size(79, 20);
            label4.TabIndex = 43;
            label4.Text = "Satış Fiyatı";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(697, 144);
            label3.Name = "label3";
            label3.Size = new Size(67, 20);
            label3.TabIndex = 42;
            label3.Text = "Ürün Adı";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(696, 111);
            label2.Name = "label2";
            label2.Size = new Size(80, 20);
            label2.TabIndex = 41;
            label2.Text = "Barkod No";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(12, 76);
            label16.Name = "label16";
            label16.Size = new Size(30, 20);
            label16.TabIndex = 74;
            label16.Text = "Bul";
            // 
            // Barkod_Yazdır
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1450, 531);
            Controls.Add(label16);
            Controls.Add(textBox4);
            Controls.Add(panel2);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(textBox6);
            Controls.Add(label5);
            Controls.Add(dataGridView1);
            Controls.Add(label4);
            Controls.Add(label1);
            Controls.Add(label3);
            Controls.Add(label2);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Barkod_Yazdır";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Barkod Yazdır";
            Load += Barkod_Yazdır_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Label label1;
        private TextBox textBox6;
        private Button button1;
        private TextBox textBox4;
        private TextBox textBox3;
        private TextBox textBox2;
        private TextBox textBox1;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Panel panel2;
        private Panel panel1;
        private CheckBox checkBox5;
        private TextBox textBox10;
        private Label label14;
        private CheckBox checkBox9;
        private CheckBox checkBox8;
        private CheckBox checkBox7;
        private CheckBox checkBox6;
        private Button button2;
        private PictureBox pictureBox2;
        private ImageList ımageList1;
        private Label label16;
    }
}
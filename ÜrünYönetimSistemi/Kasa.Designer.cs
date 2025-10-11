namespace ÜrünYönetimSistemi
{
    partial class Kasa
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Kasa));
            panel1 = new Panel();
            button3 = new Button();
            label1 = new Label();
            dateTimePicker1 = new DateTimePicker();
            dateTimePicker2 = new DateTimePicker();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            timer1 = new System.Windows.Forms.Timer(components);
            dataGridView1 = new DataGridView();
            panel2 = new Panel();
            checkBox3 = new CheckBox();
            checkBox2 = new CheckBox();
            button1 = new Button();
            ımageList1 = new ImageList(components);
            checkBox1 = new CheckBox();
            label6 = new Label();
            textBox1 = new TextBox();
            label5 = new Label();
            textBox2 = new TextBox();
            label4 = new Label();
            textBox3 = new TextBox();
            dataGridView2 = new DataGridView();
            button2 = new Button();
            button4 = new Button();
            button5 = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(button3);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(dateTimePicker1);
            panel1.Controls.Add(dateTimePicker2);
            panel1.Location = new Point(46, 76);
            panel1.Name = "panel1";
            panel1.Size = new Size(566, 110);
            panel1.TabIndex = 3;
            // 
            // button3
            // 
            button3.Location = new Point(505, 35);
            button3.Name = "button3";
            button3.Size = new Size(58, 29);
            button3.TabIndex = 10;
            button3.Text = "Yenile";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(125, 20);
            label1.TabIndex = 2;
            label1.Text = "Tarih Ayarı Seçin";
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Location = new Point(5, 37);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(233, 27);
            dateTimePicker1.TabIndex = 0;
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Location = new Point(266, 37);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(233, 27);
            dateTimePicker2.TabIndex = 1;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 1000;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(21, 206);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(612, 289);
            dataGridView1.TabIndex = 4;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // panel2
            // 
            panel2.Controls.Add(checkBox3);
            panel2.Controls.Add(checkBox2);
            panel2.Controls.Add(button1);
            panel2.Controls.Add(checkBox1);
            panel2.Controls.Add(label6);
            panel2.Controls.Add(textBox1);
            panel2.Controls.Add(label5);
            panel2.Controls.Add(textBox2);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(textBox3);
            panel2.Location = new Point(659, 76);
            panel2.Name = "panel2";
            panel2.Size = new Size(610, 110);
            panel2.TabIndex = 5;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Location = new Point(44, 69);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(119, 24);
            checkBox3.TabIndex = 2;
            checkBox3.Text = "Sadece Gider";
            checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(44, 39);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(114, 24);
            checkBox2.TabIndex = 1;
            checkBox2.Text = "Sadece Gelir";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.ImageAlign = ContentAlignment.MiddleLeft;
            button1.ImageKey = "Combo Chart.png";
            button1.ImageList = ımageList1;
            button1.Location = new Point(457, 7);
            button1.Name = "button1";
            button1.Size = new Size(150, 93);
            button1.TabIndex = 13;
            button1.Text = "Gelir - Gider Grafiği";
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
            ımageList1.Images.SetKeyName(26, "Combo Chart.png");
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(44, 9);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(68, 24);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "Tümü";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(226, 78);
            label6.Name = "label6";
            label6.Size = new Size(59, 20);
            label6.TabIndex = 12;
            label6.Text = "Toplam";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(306, 7);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(125, 27);
            textBox1.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(225, 45);
            label5.Name = "label5";
            label5.Size = new Size(45, 20);
            label5.TabIndex = 11;
            label5.Text = "Gider";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(306, 40);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(125, 27);
            textBox2.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(225, 10);
            label4.Name = "label4";
            label4.Size = new Size(40, 20);
            label4.TabIndex = 10;
            label4.Text = "Gelir";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(306, 73);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(125, 27);
            textBox3.TabIndex = 9;
            // 
            // dataGridView2
            // 
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Location = new Point(659, 206);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.RowHeadersWidth = 51;
            dataGridView2.Size = new Size(612, 289);
            dataGridView2.TabIndex = 6;
            // 
            // button2
            // 
            button2.ImageAlign = ContentAlignment.MiddleRight;
            button2.ImageKey = "Export Excel.png";
            button2.ImageList = ımageList1;
            button2.Location = new Point(23, 506);
            button2.Name = "button2";
            button2.Size = new Size(610, 67);
            button2.TabIndex = 14;
            button2.Text = "Excel'e Aktar";
            button2.TextImageRelation = TextImageRelation.ImageBeforeText;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button4
            // 
            button4.ImageAlign = ContentAlignment.MiddleRight;
            button4.ImageKey = "Export Excel.png";
            button4.ImageList = ımageList1;
            button4.Location = new Point(659, 506);
            button4.Name = "button4";
            button4.Size = new Size(610, 67);
            button4.TabIndex = 15;
            button4.Text = "Excel'e Aktar";
            button4.TextImageRelation = TextImageRelation.ImageBeforeText;
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.ImageAlign = ContentAlignment.MiddleLeft;
            button5.ImageKey = "Cancel1.png";
            button5.ImageList = ımageList1;
            button5.Location = new Point(1343, 12);
            button5.Name = "button5";
            button5.Size = new Size(121, 63);
            button5.TabIndex = 16;
            button5.Text = "Kapat";
            button5.TextImageRelation = TextImageRelation.ImageBeforeText;
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // Kasa
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1496, 584);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button2);
            Controls.Add(dataGridView2);
            Controls.Add(panel2);
            Controls.Add(dataGridView1);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Kasa";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Kasa";
            Load += Kasa_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button button3;
        private Label label1;
        private DateTimePicker dateTimePicker1;
        private DateTimePicker dateTimePicker2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Timer timer1;
        private DataGridView dataGridView1;
        private Panel panel2;
        private CheckBox checkBox3;
        private CheckBox checkBox2;
        private CheckBox checkBox1;
        private DataGridView dataGridView2;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Button button1;
        private Button button2;
        private Button button4;
        private Button button5;
        private ImageList ımageList1;
    }
}
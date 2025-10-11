namespace ÜrünYönetimSistemi
{
    partial class Fiyat_Gör
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Fiyat_Gör));
            textBox6 = new TextBox();
            label1 = new Label();
            dataGridView1 = new DataGridView();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            textBox5 = new TextBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            panel1 = new Panel();
            button1 = new Button();
            ımageList1 = new ImageList(components);
            button2 = new Button();
            label29 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // textBox6
            // 
            textBox6.Location = new Point(49, 80);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(646, 27);
            textBox6.TabIndex = 9;
            textBox6.TextChanged += textBox6_TextChanged_1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(49, 57);
            label1.Name = "label1";
            label1.Size = new Size(212, 20);
            label1.TabIndex = 1;
            label1.Text = "Ürün Adı Veya Barkod No Girin";
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(17, 113);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(678, 300);
            dataGridView1.TabIndex = 37;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(3, 34);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(150, 27);
            textBox2.TabIndex = 38;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(2, 97);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(150, 27);
            textBox3.TabIndex = 39;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(3, 159);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(150, 27);
            textBox4.TabIndex = 40;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(3, 227);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(150, 27);
            textBox5.TabIndex = 41;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 11);
            label2.Name = "label2";
            label2.Size = new Size(71, 20);
            label2.TabIndex = 42;
            label2.Text = "Ürün Adı ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(2, 74);
            label3.Name = "label3";
            label3.Size = new Size(90, 20);
            label3.TabIndex = 43;
            label3.Text = "Mevcut Stok";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(2, 136);
            label4.Name = "label4";
            label4.Size = new Size(79, 20);
            label4.TabIndex = 44;
            label4.Text = "Satış Fiyatı";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(3, 204);
            label5.Name = "label5";
            label5.Size = new Size(95, 20);
            label5.TabIndex = 45;
            label5.Text = "İndirmli Fiyat";
            // 
            // panel1
            // 
            panel1.Controls.Add(button1);
            panel1.Controls.Add(textBox2);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(textBox3);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(textBox4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(textBox5);
            panel1.Controls.Add(label2);
            panel1.Location = new Point(723, 83);
            panel1.Name = "panel1";
            panel1.Size = new Size(170, 330);
            panel1.TabIndex = 46;
            // 
            // button1
            // 
            button1.ImageAlign = ContentAlignment.MiddleLeft;
            button1.ImageKey = "Broom.png";
            button1.ImageList = ımageList1;
            button1.Location = new Point(3, 260);
            button1.Name = "button1";
            button1.Size = new Size(150, 64);
            button1.TabIndex = 46;
            button1.Text = "Temizle";
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
            // 
            // button2
            // 
            button2.ImageAlign = ContentAlignment.MiddleLeft;
            button2.ImageKey = "Cancel1.png";
            button2.ImageList = ımageList1;
            button2.Location = new Point(973, 12);
            button2.Name = "button2";
            button2.Size = new Size(132, 64);
            button2.TabIndex = 47;
            button2.Text = "Kapat";
            button2.TextImageRelation = TextImageRelation.ImageBeforeText;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(13, 83);
            label29.Name = "label29";
            label29.Size = new Size(30, 20);
            label29.TabIndex = 62;
            label29.Text = "Bul";
            // 
            // Fiyat_Gör
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1118, 430);
            Controls.Add(label29);
            Controls.Add(button2);
            Controls.Add(panel1);
            Controls.Add(dataGridView1);
            Controls.Add(label1);
            Controls.Add(textBox6);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Fiyat_Gör";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Fiyat Gör";
            Load += Fiyat_Gör_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox6;
        private Label label1;
        private DataGridView dataGridView1;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private TextBox textBox5;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Panel panel1;
        private Button button1;
        private Button button2;
        private ImageList ımageList1;
        private Label label29;
    }
}
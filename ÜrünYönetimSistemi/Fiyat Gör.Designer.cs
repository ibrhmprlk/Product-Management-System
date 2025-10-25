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
            label29 = new Label();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // textBox6
            // 
            textBox6.Location = new Point(54, 33);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(669, 27);
            textBox6.TabIndex = 9;
            textBox6.TextChanged += textBox6_TextChanged_1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(54, 10);
            label1.Name = "label1";
            label1.Size = new Size(212, 20);
            label1.TabIndex = 1;
            label1.Text = "Ürün Adı Veya Barkod No Girin";
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.BorderStyle = BorderStyle.Fixed3D;
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.Raised;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(22, 66);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(701, 223);
            dataGridView1.TabIndex = 37;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(9, 34);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(314, 27);
            textBox2.TabIndex = 38;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(344, 34);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(207, 27);
            textBox3.TabIndex = 39;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(9, 105);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(206, 27);
            textBox4.TabIndex = 40;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(344, 105);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(207, 27);
            textBox5.TabIndex = 41;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(7, 11);
            label2.Name = "label2";
            label2.Size = new Size(71, 20);
            label2.TabIndex = 42;
            label2.Text = "Ürün Adı ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(341, 11);
            label3.Name = "label3";
            label3.Size = new Size(90, 20);
            label3.TabIndex = 43;
            label3.Text = "Mevcut Stok";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(7, 82);
            label4.Name = "label4";
            label4.Size = new Size(79, 20);
            label4.TabIndex = 44;
            label4.Text = "Satış Fiyatı";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(341, 82);
            label5.Name = "label5";
            label5.Size = new Size(95, 20);
            label5.TabIndex = 45;
            label5.Text = "İndirmli Fiyat";
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(button1);
            panel1.Controls.Add(textBox2);
            panel1.Controls.Add(label5);
            panel1.Controls.Add(textBox3);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(textBox4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(textBox5);
            panel1.Controls.Add(label2);
            panel1.Location = new Point(13, 294);
            panel1.Name = "panel1";
            panel1.Size = new Size(722, 145);
            panel1.TabIndex = 46;
            // 
            // button1
            // 
            button1.ImageAlign = ContentAlignment.MiddleLeft;
            button1.ImageKey = "Broom.png";
            button1.ImageList = ımageList1;
            button1.Location = new Point(565, 48);
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
            ımageList1.Images.SetKeyName(0, "3566778.png");
            ımageList1.Images.SetKeyName(1, "3584670.png");
            ımageList1.Images.SetKeyName(2, "4573148.png");
            ımageList1.Images.SetKeyName(3, "6008154.png");
            ımageList1.Images.SetKeyName(4, "8890278.png");
            ımageList1.Images.SetKeyName(5, "9359564.png");
            ımageList1.Images.SetKeyName(6, "18735398.png");
            ımageList1.Images.SetKeyName(7, "Add Product.png");
            ımageList1.Images.SetKeyName(8, "Add Shopping Cart.png");
            ımageList1.Images.SetKeyName(9, "Add.png");
            ımageList1.Images.SetKeyName(10, "Add1.png");
            ımageList1.Images.SetKeyName(11, "Analyze.png");
            ımageList1.Images.SetKeyName(12, "Australia.png");
            ımageList1.Images.SetKeyName(13, "Authentication.png");
            ımageList1.Images.SetKeyName(14, "Average Price.png");
            ımageList1.Images.SetKeyName(15, "Barcode.png");
            ımageList1.Images.SetKeyName(16, "Broom.png");
            ımageList1.Images.SetKeyName(17, "Canada.png");
            ımageList1.Images.SetKeyName(18, "Cancel.png");
            ımageList1.Images.SetKeyName(19, "Cancel1.png");
            ımageList1.Images.SetKeyName(20, "Card Payment.png");
            ımageList1.Images.SetKeyName(21, "Cash in Hand.png");
            ımageList1.Images.SetKeyName(22, "Chart.png");
            ımageList1.Images.SetKeyName(23, "Combo Chart.png");
            ımageList1.Images.SetKeyName(24, "Create.png");
            ımageList1.Images.SetKeyName(25, "Currency Exchange.png");
            ımageList1.Images.SetKeyName(26, "Customer.png");
            ımageList1.Images.SetKeyName(27, "Delete4.png");
            ımageList1.Images.SetKeyName(28, "Denmark.png");
            ımageList1.Images.SetKeyName(29, "Edit Pencil.png");
            ımageList1.Images.SetKeyName(30, "Exclamation Mark.png");
            ımageList1.Images.SetKeyName(31, "Export Excel.png");
            ımageList1.Images.SetKeyName(32, "Germany.png");
            ımageList1.Images.SetKeyName(33, "Get Cash.png");
            ımageList1.Images.SetKeyName(34, "Get Revenue.png");
            ımageList1.Images.SetKeyName(35, "Go Back.png");
            ımageList1.Images.SetKeyName(36, "Gold Pot.png");
            ımageList1.Images.SetKeyName(37, "images (1).jpg");
            ımageList1.Images.SetKeyName(38, "images (1).png");
            ımageList1.Images.SetKeyName(39, "Installment Plan.png");
            ımageList1.Images.SetKeyName(40, "istockphoto-1453791996-612x612.jpg");
            ımageList1.Images.SetKeyName(41, "Japan.png");
            ımageList1.Images.SetKeyName(42, "Left.png");
            ımageList1.Images.SetKeyName(43, "List View.png");
            ımageList1.Images.SetKeyName(44, "Login.png");
            ımageList1.Images.SetKeyName(45, "Lowest Price.png");
            ımageList1.Images.SetKeyName(46, "Medical History.png");
            ımageList1.Images.SetKeyName(47, "nakit+kredi kartı.jpg");
            ımageList1.Images.SetKeyName(48, "Online Money Transfer.png");
            ımageList1.Images.SetKeyName(49, "People.png");
            ımageList1.Images.SetKeyName(50, "Picture.png");
            ımageList1.Images.SetKeyName(51, "Printer.png");
            ımageList1.Images.SetKeyName(52, "Product.png");
            ımageList1.Images.SetKeyName(53, "Right.png");
            ımageList1.Images.SetKeyName(54, "Save.png");
            ımageList1.Images.SetKeyName(55, "Settings.png");
            ımageList1.Images.SetKeyName(56, "Svalbard Jan Mayen.png");
            ımageList1.Images.SetKeyName(57, "Sweden.png");
            ımageList1.Images.SetKeyName(58, "Switzerland.png");
            ımageList1.Images.SetKeyName(59, "TikTok Verified Account.png");
            ımageList1.Images.SetKeyName(60, "Total Sales.png");
            ımageList1.Images.SetKeyName(61, "Transaction.png");
            ımageList1.Images.SetKeyName(62, "United Kingdom.png");
            ımageList1.Images.SetKeyName(63, "USA.png");
            ımageList1.Images.SetKeyName(64, "View.png");
            ımageList1.Images.SetKeyName(65, "Vision.png");
            ımageList1.Images.SetKeyName(66, "Wrench.png");
            // 
            // label29
            // 
            label29.AutoSize = true;
            label29.Location = new Point(18, 36);
            label29.Name = "label29";
            label29.Size = new Size(30, 20);
            label29.TabIndex = 62;
            label29.Text = "Bul";
            // 
            // Fiyat_Gör
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(743, 443);
            Controls.Add(label29);
            Controls.Add(panel1);
            Controls.Add(dataGridView1);
            Controls.Add(label1);
            Controls.Add(textBox6);
            Cursor = Cursors.Hand;
            FormBorderStyle = FormBorderStyle.Fixed3D;
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
namespace ÜrünYönetimSistemi
{
    partial class Toplu_Ürün_Sil
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Toplu_Ürün_Sil));
            panel2 = new Panel();
            label2 = new Label();
            button2 = new Button();
            ımageList1 = new ImageList(components);
            dataGridView2 = new DataGridView();
            button1 = new Button();
            button3 = new Button();
            button4 = new Button();
            dataGridView1 = new DataGridView();
            label1 = new Label();
            comboBox1 = new ComboBox();
            textBox1 = new TextBox();
            checkBox1 = new CheckBox();
            label3 = new Label();
            label4 = new Label();
            panel1 = new Panel();
            label5 = new Label();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel2
            // 
            panel2.Controls.Add(label2);
            panel2.Controls.Add(button2);
            panel2.Controls.Add(dataGridView2);
            panel2.Location = new Point(754, 93);
            panel2.Name = "panel2";
            panel2.Size = new Size(563, 588);
            panel2.TabIndex = 39;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label2.Location = new Point(0, 0);
            label2.Name = "label2";
            label2.Size = new Size(126, 20);
            label2.TabIndex = 39;
            label2.Text = "Silinecek Ürünler";
            // 
            // button2
            // 
            button2.ImageAlign = ContentAlignment.MiddleLeft;
            button2.ImageKey = "Delete4.png";
            button2.ImageList = ımageList1;
            button2.Location = new Point(188, 42);
            button2.Name = "button2";
            button2.Size = new Size(224, 68);
            button2.TabIndex = 41;
            button2.Text = "Seçili Olan Ürünleri Sil";
            button2.TextImageRelation = TextImageRelation.ImageBeforeText;
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // ımageList1
            // 
            ımageList1.ColorDepth = ColorDepth.Depth32Bit;
            ımageList1.ImageStream = (ImageListStreamer)resources.GetObject("ımageList1.ImageStream");
            ımageList1.TransparentColor = Color.Transparent;
            ımageList1.Images.SetKeyName(0, "Delete3.png");
            ımageList1.Images.SetKeyName(1, "Go Back.png");
            ımageList1.Images.SetKeyName(2, "Cancel.png");
            ımageList1.Images.SetKeyName(3, "Cancel1.png");
            ımageList1.Images.SetKeyName(4, "Delete4.png");
            // 
            // dataGridView2
            // 
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Location = new Point(5, 116);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.ReadOnly = true;
            dataGridView2.RowHeadersWidth = 51;
            dataGridView2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView2.Size = new Size(553, 428);
            dataGridView2.TabIndex = 37;
            dataGridView2.CellContentClick += dataGridView2_CellClick;
            // 
            // button1
            // 
            button1.Location = new Point(644, 209);
            button1.Name = "button1";
            button1.Size = new Size(104, 428);
            button1.TabIndex = 40;
            button1.Text = "Ürünleri Silinecekler Tablosuna Ekle";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button3
            // 
            button3.ImageAlign = ContentAlignment.MiddleLeft;
            button3.ImageKey = "Cancel1.png";
            button3.ImageList = ımageList1;
            button3.Location = new Point(1408, 3);
            button3.Name = "button3";
            button3.Size = new Size(126, 57);
            button3.TabIndex = 41;
            button3.Text = "Kapat";
            button3.TextAlign = ContentAlignment.MiddleRight;
            button3.TextImageRelation = TextImageRelation.ImageBeforeText;
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.ImageAlign = ContentAlignment.MiddleLeft;
            button4.ImageKey = "Go Back.png";
            button4.ImageList = ımageList1;
            button4.Location = new Point(1408, 66);
            button4.Name = "button4";
            button4.Size = new Size(126, 57);
            button4.TabIndex = 42;
            button4.Text = "Geri Dön";
            button4.TextAlign = ContentAlignment.MiddleRight;
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(5, 116);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(553, 428);
            dataGridView1.TabIndex = 37;
            dataGridView1.CellContentClick += dataGridView1_CellClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(62, 20);
            label1.TabIndex = 38;
            label1.Text = "Ürünler";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(360, 67);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(198, 28);
            comboBox1.TabIndex = 41;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(39, 68);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(274, 27);
            textBox1.TabIndex = 39;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(5, 550);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(250, 24);
            checkBox1.TabIndex = 40;
            checkBox1.Text = "Stok Sayısı 0 Olan Ürünleri Listele";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged_1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(360, 42);
            label3.Name = "label3";
            label3.Size = new Size(152, 20);
            label3.TabIndex = 42;
            label3.Text = "Ürün Grubu İle Arama";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(37, 45);
            label4.Name = "label4";
            label4.Size = new Size(212, 20);
            label4.TabIndex = 43;
            label4.Text = "Ürün Adı Veya Barkod No Girin";
            // 
            // panel1
            // 
            panel1.Controls.Add(label5);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(checkBox1);
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(comboBox1);
            panel1.Controls.Add(label1);
            panel1.Controls.Add(dataGridView1);
            panel1.Location = new Point(75, 93);
            panel1.Name = "panel1";
            panel1.Size = new Size(563, 588);
            panel1.TabIndex = 38;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(3, 73);
            label5.Name = "label5";
            label5.Size = new Size(30, 20);
            label5.TabIndex = 43;
            label5.Text = "Bul";
            // 
            // Toplu_Ürün_Sil
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1562, 723);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button1);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Toplu_Ürün_Sil";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Toplu Ürün Sil";
            Load += Toplu_Ürün_Sil_Load;
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private Panel panel2;
        private DataGridView dataGridView2;
        private Button button1;
        private Button button2;
        private Label label2;
        private Button button3;
        private Button button4;
        private DataGridView dataGridView1;
        private Label label1;
        private ComboBox comboBox1;
        private TextBox textBox1;
        private CheckBox checkBox1;
        private Label label3;
        private Label label4;
        private Panel panel1;
        private ImageList ımageList1;
        private Label label5;
    }
}
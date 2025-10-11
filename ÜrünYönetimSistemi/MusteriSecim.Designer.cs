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
            button1 = new Button();
            ımageList1 = new ImageList(components);
            label1 = new Label();
            textBox2 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(22, 110);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 51;
            dataGridView1.Size = new Size(532, 309);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellClick += dataGridView1_CellClick;
            // 
            // button1
            // 
            button1.ImageAlign = ContentAlignment.MiddleLeft;
            button1.ImageKey = "Cancel1.png";
            button1.ImageList = ımageList1;
            button1.Location = new Point(602, 10);
            button1.Name = "button1";
            button1.Size = new Size(121, 63);
            button1.TabIndex = 1;
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
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(21, 81);
            label1.Name = "label1";
            label1.Size = new Size(30, 20);
            label1.TabIndex = 8;
            label1.Text = "Bul";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(57, 77);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Müşteri Adı veya GSM Telefon No Girin";
            textBox2.Size = new Size(497, 27);
            textBox2.TabIndex = 7;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // MusteriSecim
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(735, 436);
            Controls.Add(label1);
            Controls.Add(textBox2);
            Controls.Add(button1);
            Controls.Add(dataGridView1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MusteriSecim";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Müşteri Seçim";
            Load += MusteriSecim_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView1;
        private Button button1;
        private Label label1;
        private TextBox textBox2;
        private ImageList ımageList1;
    }
}
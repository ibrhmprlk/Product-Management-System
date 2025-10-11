namespace ÜrünYönetimSistemi
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            label1 = new Label();
            label2 = new Label();
            textBox1 = new TextBox();
            comboBox1 = new ComboBox();
            button1 = new Button();
            ımageList1 = new ImageList(components);
            button2 = new Button();
            button3 = new Button();
            comboBox2 = new ComboBox();
            SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // textBox1
            // 
            resources.ApplyResources(textBox1, "textBox1");
            textBox1.Name = "textBox1";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            resources.ApplyResources(comboBox1, "comboBox1");
            comboBox1.Name = "comboBox1";
            // 
            // button1
            // 
            resources.ApplyResources(button1, "button1");
            button1.ImageList = ımageList1;
            button1.Name = "button1";
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
            ımageList1.Images.SetKeyName(24, "Get Cash.png");
            ımageList1.Images.SetKeyName(25, "3584670.png");
            ımageList1.Images.SetKeyName(26, "Barcode.png");
            ımageList1.Images.SetKeyName(27, "4573148.png");
            ımageList1.Images.SetKeyName(28, "istockphoto-1453791996-612x612.jpg");
            ımageList1.Images.SetKeyName(29, "People.png");
            ımageList1.Images.SetKeyName(30, "Vision.png");
            ımageList1.Images.SetKeyName(31, "images (1).jpg");
            ımageList1.Images.SetKeyName(32, "18735398.png");
            ımageList1.Images.SetKeyName(33, "3566778.png");
            ımageList1.Images.SetKeyName(34, "8890278.png");
            ımageList1.Images.SetKeyName(35, "Settings.png");
            ımageList1.Images.SetKeyName(36, "Login.png");
            // 
            // button2
            // 
            resources.ApplyResources(button2, "button2");
            button2.ImageList = ımageList1;
            button2.Name = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            resources.ApplyResources(button3, "button3");
            button3.ImageList = ımageList1;
            button3.Name = "button3";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            resources.ApplyResources(comboBox2, "comboBox2");
            comboBox2.Name = "comboBox2";
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(comboBox2);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(comboBox1);
            Controls.Add(textBox1);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox textBox1;
        private ComboBox comboBox1;
        private Button button1;
        private Button button2;
        private Button button3;
        private ComboBox comboBox2;
        private ImageList ımageList1;
    }
}

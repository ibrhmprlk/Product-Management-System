using System;
using System.Windows.Forms;

namespace ÜrünYönetimSistemi
{
    public partial class frmYukleniyor : Form
    {
        public frmYukleniyor()
        {
            InitializeComponent();
        }

        private void frmYukleniyor_Load(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            timer1.Interval = 53; // ilerleme hızı
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Increment(2); // her tick’te yüzde 2 artır
            label1.Text = $"Veriler Yükleniyor...%{progressBar1.Value}"; // yüzdeyi göster

            if (progressBar1.Value >= 100)
            {
                timer1.Stop();
                label1.Text = "%100"; // son değer
                this.DialogResult = DialogResult.OK; // Form1'e bilgi gönder
                this.Close();
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}

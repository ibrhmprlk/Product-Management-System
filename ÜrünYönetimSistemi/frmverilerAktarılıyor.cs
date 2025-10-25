using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ÜrünYönetimSistemi
{
    public partial class frmverilerAktarılıyor : Form
    {
        public frmverilerAktarılıyor()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void frmverilerAktarılıyor_Load(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            label1.Text = "Veriler yedekleniyor...";
            timer1.Interval = 53; // hız ayarı
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressBar1.Increment(2); // her tick’te ilerleme
            label1.Text = $"Veriler yedekleniyor... %{progressBar1.Value}";

            if (progressBar1.Value >= 100)
            {
                timer1.Stop();
                label1.Text = "Aktarım tamamlandı!";
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}

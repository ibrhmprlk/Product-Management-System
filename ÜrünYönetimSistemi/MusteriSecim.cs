using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using ÜrünYönetimSistemi;

namespace ÜrünYönetimSistemi
{
    public partial class MusteriSecim : Form
    {
        private Satış_İşlemleri satisIslemleriFormu;
        private DataTable tablo; // Form düzeyinde DataTable tanımlandı

        public MusteriSecim(Satış_İşlemleri satisFormu)
        {
            InitializeComponent();
            dataGridView1.ReadOnly = true;
            this.satisIslemleriFormu = satisFormu;

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dataGridView1.CellClick += dataGridView1_CellClick;
        }

        private void MusteriSecim_Load(object sender, EventArgs e)
        {
            try
            {
                string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=ÜrünYönetimSistemi.accdb;";
                using (OleDbConnection connection = new OleDbConnection(connectionString))
                {
                    string query = "SELECT MusteriAdi, GsmTelefon, DevredenBorc FROM Musteriler";
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, connection);
                    tablo = new DataTable(); // DataTable nesnesini doldur
                    adapter.Fill(tablo);

                    dataGridView1.DataSource = tablo; // DataGridView'e DataTable'ı bağla

                    if (dataGridView1.Columns.Count > 0)
                    {
                        dataGridView1.Columns["MusteriAdi"].HeaderText = "Müşteri Adı";
                        dataGridView1.Columns["GsmTelefon"].HeaderText = "Gsm Telefon";
                        dataGridView1.Columns["DevredenBorc"].HeaderText = "Devreden Borç";

                        foreach (DataGridViewColumn column in dataGridView1.Columns)
                        {
                            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veri yüklenirken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dataGridView1.Rows.Count)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];

                try
                {
                    satisIslemleriFormu.textBox13.Text = selectedRow.Cells["MusteriAdi"]?.Value?.ToString() ?? string.Empty;
                    satisIslemleriFormu.textBox17.Text = selectedRow.Cells["GsmTelefon"]?.Value?.ToString() ?? string.Empty;
                    satisIslemleriFormu.textBox19.Text = selectedRow.Cells["DevredenBorc"]?.Value?.ToString() ?? string.Empty;

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Veri aktarılırken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

       

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (tablo == null) return;

            string filtre = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(filtre))
            {
                tablo.DefaultView.RowFilter = "";
            }
            else
            {
                tablo.DefaultView.RowFilter = $"MusteriAdi LIKE '%{filtre}%' OR GsmTelefon LIKE '%{filtre}%'";
            }

        }

    }
}
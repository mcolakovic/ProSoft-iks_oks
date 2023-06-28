using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Domain;
using Common;
using System.Threading;
using System.Diagnostics;

namespace KorisnickiInterfejs
{
    public partial class FrmKlijent : Form
    {
        string kod;
        string okod;
        bool semafor = false;
        public FrmKlijent()
        {
            InitializeComponent();
            try
            {
                Communication.Instance.Connect();
                Login();
                InitListener();
            }
            catch (Exception)
            {
                MessageBox.Show("Igra je u toku");
                Environment.Exit(0);
            }
        }

        private void InitListener()
        {
            try
            {
                Thread nit = new Thread(CitajPoruke);
                nit.IsBackground = true;
                nit.Start();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void CitajPoruke()
        {
            try
            {
                while (true)
                {
                    Request request = Communication.Instance.ReadMessage<Request>();
                    switch (request.Operations)
                    {
                        case Operations.ZapocniIgru:
                            kod = "x";
                            okod = "o";
                            semafor = true;
                            Invoke(new Action(() => lblText.Text = "Vi ste na potezu."));
                            break;
                        case Operations.Pratiigru:
                            kod = "o";
                            okod = "x";
                            break;
                        case Operations.Igra:
                            if (((Igra)request.RequestObject).Pozicija == 0)
                                Invoke(new Action(() =>
                                {
                                    txt11.Text = ((Igra)request.RequestObject).Kod;
                                    txt11.Enabled = false;
                                }));
                            if (((Igra)request.RequestObject).Pozicija == 1)
                                Invoke(new Action(() =>
                                {
                                    txt12.Text = ((Igra)request.RequestObject).Kod;
                                    txt12.Enabled = false;
                                }));
                            if (((Igra)request.RequestObject).Pozicija == 2)
                                Invoke(new Action(() =>
                                {
                                    txt13.Text = ((Igra)request.RequestObject).Kod;
                                    txt13.Enabled = false;
                                }));
                            if (((Igra)request.RequestObject).Pozicija == 3)
                                Invoke(new Action(() =>
                                {
                                    txt21.Text = ((Igra)request.RequestObject).Kod;
                                    txt21.Enabled = false;
                                }));
                            if (((Igra)request.RequestObject).Pozicija == 4)
                                Invoke(new Action(() =>
                                {
                                    txt22.Text = ((Igra)request.RequestObject).Kod;
                                    txt22.Enabled = false;
                                }));
                            if (((Igra)request.RequestObject).Pozicija == 5)
                                Invoke(new Action(() =>
                                {
                                    txt23.Text = ((Igra)request.RequestObject).Kod;
                                    txt23.Enabled = false;
                                }));
                            if (((Igra)request.RequestObject).Pozicija == 6)
                                Invoke(new Action(() =>
                                {
                                    txt31.Text = ((Igra)request.RequestObject).Kod;
                                    txt31.Enabled = false;
                                }));
                            if (((Igra)request.RequestObject).Pozicija == 7)
                                Invoke(new Action(() =>
                                {
                                    txt32.Text = ((Igra)request.RequestObject).Kod;
                                    txt32.Enabled = false;
                                }));
                            if (((Igra)request.RequestObject).Pozicija == 8)
                                Invoke(new Action(() =>
                                {
                                    txt33.Text = ((Igra)request.RequestObject).Kod;
                                    txt33.Enabled = false;
                                }));
                            semafor = true;
                            Invoke(new Action(() => lblText.Text = "Vas potez"));
                            if (PobjedaPoraz(okod))
                            {
                                Invoke(new Action(() => lblText.Text = "Poraz"));
                            }
                            if (Nerijeseno())
                            {
                                Invoke(new Action(() => lblText.Text = "Nerijeseno"));
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Login()
        {
            Request request = new Request
            {
                Operations = Operations.Login
            };
            try
            {
                Communication.Instance.SendMessage(request);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void txt11_Click(object sender, EventArgs e)
        {
            SaljiKod(txt11, 0);
        }

        private void txt12_Click(object sender, EventArgs e)
        {
            SaljiKod(txt12, 1);
        }

        private void txt13_Click(object sender, EventArgs e)
        {
            SaljiKod(txt13, 2);
        }

        private void txt21_Click(object sender, EventArgs e)
        {
            SaljiKod(txt21, 3);
        }

        private void txt22_Click(object sender, EventArgs e)
        {
            SaljiKod(txt22, 4);
        }

        private void txt23_Click(object sender, EventArgs e)
        {
            SaljiKod(txt23, 5);
        }

        private void txt31_Click(object sender, EventArgs e)
        {
            SaljiKod(txt31, 6);
        }

        private void txt32_Click(object sender, EventArgs e)
        {
            SaljiKod(txt32, 7);
        }

        private void txt33_Click(object sender, EventArgs e)
        {
            SaljiKod(txt33, 8);
        }

        private void SaljiKod(TextBox txt, int pozicija)
        {
            try
            {
                if (semafor)
                {
                    txt.Text = kod;
                    Igra igra = new Igra
                    {
                        Kod = kod,
                        Pozicija = pozicija
                    };
                    Communication.Instance.SendMessage(new Request
                    {
                        Operations = Operations.Igra,
                        RequestObject = igra
                    });
                    semafor = false;
                    lblText.Text = "";
                    if (PobjedaPoraz(kod))
                    {
                        lblText.Text = "Pobjeda";
                    }
                    if (Nerijeseno())
                    {
                        lblText.Text = "Nerijeseno";
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private bool Nerijeseno()
        {
            if (txt11.Text != "" && txt12.Text != "" && txt13.Text != "" && txt21.Text != "" && txt22.Text != "" && txt23.Text != "" && txt31.Text != "" && txt32.Text != "" && txt33.Text != "") return true;
            return false;
        }

        private bool PobjedaPoraz(String kod)
        {
            if (txt11.Text == kod && txt12.Text == kod && txt13.Text == kod) return true;
            if (txt21.Text == kod && txt22.Text == kod && txt23.Text == kod) return true;
            if (txt31.Text == kod && txt32.Text == kod && txt33.Text == kod) return true;
            if (txt11.Text == kod && txt21.Text == kod && txt31.Text == kod) return true;
            if (txt12.Text == kod && txt22.Text == kod && txt32.Text == kod) return true;
            if (txt13.Text == kod && txt23.Text == kod && txt33.Text == kod) return true;
            if (txt11.Text == kod && txt22.Text == kod && txt33.Text == kod) return true;
            if (txt13.Text == kod && txt22.Text == kod && txt31.Text == kod) return true;
            return false;
        }
    }
}

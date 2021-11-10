using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//estas bibliotecas são ativadas ao baixar e instalar seus pacotes no NuGet
using AForge.Video;
using AForge.Video.DirectShow;
using ZXing;
using System.Media;

namespace sistemaGerenciamentoEmpreendedor
{
    public partial class frmLogin : Form
    {     
        public frmLogin()
        {
            InitializeComponent();

            //inabilita alguns controles
            btnEntrar.Visible = false;
            txtQRCode.Visible = false;
        }

        //faz a instância dos eventos
        FilterInfoCollection filterInfoCollection;
        VideoCaptureDevice captureDevice;
        SoundPlayer som;

       
        //lê a imagem no pictureBox
        private void CaptureDevice_NewFrame(Object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox.Image = (Bitmap)eventArgs.Frame.Clone();
        }

       

        //Sai do sistema
        private void pbSair_Click(object sender, EventArgs e)
        {
            try
            {
             timer1.Stop(); //para o timer

             if (captureDevice.IsRunning) //caso a cam esteja funcionando
             {
                captureDevice.Stop(); // para a captura
             }

            }

            catch
            {

            }

            finally
            {
                Application.Exit(); //sai da aplicação
            }      
        }

        //desativa a captura do qrcode pela webcam, ativa o txt para escrever a senha
        private void pbSemCam_Click(object sender, EventArgs e) //implementa o botão sem cam
        {
            timer1.Stop(); //para o timer

            try
            {
             if (captureDevice.IsRunning) //caso a cam esteja funcionando
             {
                captureDevice.Stop(); //para a captura
             }

            }
            catch 
            {

            }

            finally
            {
                //deixa visível os seguintes controles
                txtQRCode.Visible = true;
                btnEntrar.Visible = true;

                //inabilita os seguintes controles
                pictureBox.Enabled = false;
                cboDevice.Visible = false;
                pbCam2.Visible = false;
                pictureBox.Visible = false;
            }  
        }

        //ocorre quando o usuário digita a senha no txt e clica no botão para entrar
        private void btnEntrar_Click(object sender, EventArgs e)
        {
            if (txtQRCode.Text == "admlua1030#$") //esta é a senha
            {
                this.Visible = false; // o frmLogin fecha

                frmGerenciamento fg = new frmGerenciamento();
                fg.Show(); // o próximo formulário abre

                som = new SoundPlayer(Properties.Resources.efeitoSonoro); //instancia o som para ser executado
                som.Play(); // o som toca
            }

            else
            {
                if (txtQRCode.Text == "") //caso txt vazio
                {
                    MessageBox.Show("Digite a senha para abrir o sistema!", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Information); //exibe esta msg para o usuário
                }

                else
                {
                    //caso senha errada
                    MessageBox.Show("Digite a senha correta para abrir o sistema!", "Atenção!", MessageBoxButtons.OK, MessageBoxIcon.Information); //exibe essa msg para o usuário
                    txtQRCode.Clear(); //limpa o txt do QRCode
                }
            }
        }

        //tudo o que inicia no evento Load do frm
        private void frmLogin_Load(object sender, EventArgs e)
        {
            try
            {
             filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice); //filtra as cams que o pc possui

             if (FilterCategory.VideoInputDevice == null)
             {
                timer1.Stop(); //para o timer

                if (captureDevice.IsRunning) //caso a cam esteja funcionando
                {
                    captureDevice.Stop(); //para a captura
                }

                //deixa visível os seguintes controles
                txtQRCode.Visible = true;
                btnEntrar.Visible = true;

                //inabilita os seguintes controles
                pictureBox.Enabled = false;
                cboDevice.Visible = false;
                pbCam2.Visible = false;
                pictureBox.Visible = false;
             }

             else
             {
                foreach (FilterInfo filterInfo in filterInfoCollection) //sempre permance neste laço para não terminar a filtragem
                    cboDevice.Items.Add(filterInfo.Name); //exibe as cams no comboBox
                cboDevice.SelectedIndex = 0; //inicia comboBox com a primeira cam

                txtQRCode.Clear(); // limpa o txt do qrcode

                captureDevice = new VideoCaptureDevice(filterInfoCollection[cboDevice.SelectedIndex].MonikerString); //instancia a captura da cam escolhida no comboBox
                captureDevice.NewFrame += CaptureDevice_NewFrame; //associa a cam escolhida
                captureDevice.Start(); //inicia a cam
                timer1.Start(); //inicia o timer
             }

            }
            catch
            {

            }
        }

        //mostra os eventos do timer, quando a imagem do pictureBox é reconhecida
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox.Image != null) //caso o pictureBox esteja com alguma imagem capturada
            {
                BarcodeReader barcodeReader = new BarcodeReader(); //instancia a leitura do qrcode
                Result result = barcodeReader.Decode((Bitmap)pictureBox.Image); //verifica se o resultado corresponde a um qrcode

                if (result != null) //se o resultado for ok
                {
                    txtQRCode.Text += result.ToString(); // concatena a leitura do qrcode e converte para o txt ler

                    if (txtQRCode.Text == "admlua1030#$") //se o qrcode for esse
                    {
                        som = new SoundPlayer(Properties.Resources.efeitoSonoro); //instancia o efeito sonoro
                        som.Play(); //ativa o efeito sonoro

                        timer1.Stop(); //para o timer
                        captureDevice.Stop(); //para a captura da cam

                        this.Visible = false; // o frmLogin fecha

                        frmGerenciamento fg = new frmGerenciamento(); //instancia o novo frm a ser aberto
                        fg.Show(); // o próximo formulário abre
                    }                  
                }
            }
        }
    }
}

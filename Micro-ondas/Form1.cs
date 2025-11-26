using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace Micro_ondas
{
    public partial class Form1 : Form
    {
        private int timerSeconds = 0;

        private EstadoMicroondas estado = EstadoMicroondas.Parado;
        private int potencia = 10; 

        private string progressoAquecimento = "";
        private string charAquecimento = ".";

        private ProgramaAquecimento programaSelecionado = null;
        public Form1()
        {
            InitializeComponent();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += TimerTick;

            trackBar1.Minimum = 1;
            trackBar1.Maximum = 10;
            trackBar1.Value = 10;
        }

        private enum EstadoMicroondas
        {
            Parado = 0,
            Executando = 1,
            Pausado = 2
        }

        private class ProgramaAquecimento
        {
            public string Nome { get; set; }
            public string Alimento { get; set; }
            public int TempoSegundos { get; set; }
            public int Potencia { get; set; }
            public string charAquecimento { get; set; }
            public string Instrucoes { get; set; }
        }

        private List<ProgramaAquecimento> programas = new List<ProgramaAquecimento>
        {
            new ProgramaAquecimento { Nome="Pipoca",        Alimento="Pipoca(de micro-ondas)",    TempoSegundos=180, Potencia=7, charAquecimento="*", Instrucoes=" Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento." },
            new ProgramaAquecimento { Nome="Leite",         Alimento="Leite",                     TempoSegundos=300, Potencia=5, charAquecimento="#", Instrucoes="Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras." },
            new ProgramaAquecimento { Nome="Carnes de Boi", Alimento="Carne em pedaço ou fatias", TempoSegundos=840, Potencia=4, charAquecimento="@", Instrucoes="Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme." },
            new ProgramaAquecimento { Nome="Frango",        Alimento="Frango (qualquer corte)",   TempoSegundos=480, Potencia=7, charAquecimento="~", Instrucoes="Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme." },
            new ProgramaAquecimento { Nome="Feijão",        Alimento="Feijão congelado",          TempoSegundos=480, Potencia=9, charAquecimento="%", Instrucoes="Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas." }
        };

        private void SelPrograma(ProgramaAquecimento p)
        {
            programaSelecionado = p;

            timerSeconds = p.TempoSegundos;
            AtualizarDisplayTempo();

            potencia = p.Potencia;
            trackBar1.Value = potencia;
            label2.Text = potencia.ToString();

            label1.Text = p.Instrucoes;

            charAquecimento = p.charAquecimento;

            textBox1.Enabled = false;
            trackBar1.Enabled = false;

            lockButton();

        }

        private void button13_Click(object sender, EventArgs e) => SelPrograma(programas[0]);
        private void button14_Click(object sender, EventArgs e) => SelPrograma(programas[1]);
        private void button15_Click(object sender, EventArgs e) => SelPrograma(programas[2]);
        private void button16_Click(object sender, EventArgs e) => SelPrograma(programas[3]);
        private void button17_Click(object sender, EventArgs e) => SelPrograma(programas[4]);

        private void TimerTick(object sender, EventArgs e)
        {
            if (estado != EstadoMicroondas.Executando)
            {
                timer.Stop();
                return;
            }

            if (timerSeconds > 0)
            {
                timerSeconds--;

                AtualizarDisplayTempo();

                progressoAquecimento += new string(charAquecimento[0], potencia);

                progressoAquecimento = progressoAquecimento + ' ';
                label4.Text = progressoAquecimento;

                if (timerSeconds == 0)
                    FinalizarAquecimento();
            }
            else
            {
                FinalizarAquecimento();
            }
        }

        private void AtualizarDisplayTempo()
        {
            int minutos = timerSeconds / 60;
            int segundos = timerSeconds % 60;

            textBox1.Text = $"{minutos:00}:{segundos:00}";
        }

        private void FinalizarAquecimento()
        {
            timer.Stop();
            estado = EstadoMicroondas.Parado;

            label4.Text = progressoAquecimento + " Aquecimento concluído";

            textBox1.Enabled = true;
            trackBar1.Enabled = true;
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (estado == EstadoMicroondas.Executando)
            {
                timer.Stop();
                estado = EstadoMicroondas.Pausado;
                label4.Text = "Pausado";

                if (programaSelecionado == null)
                {
                    textBox1.Enabled = true;
                    trackBar1.Enabled = true;
                }


            }
            else if (estado == EstadoMicroondas.Pausado)
            {

                estado = EstadoMicroondas.Parado;
                timer.Stop();
                timerSeconds = 0;

                textBox1.Text = "00:00";
                label4.Text = "";
                label1.Text = "";
                potencia = 10;
                trackBar1.Value = potencia;
                label2.Text = potencia.ToString();
                programaSelecionado = null;
                charAquecimento = ".";

                unlockButton_Click();

                textBox1.Enabled = true;
                trackBar1.Enabled = true;
            }
            if (estado == EstadoMicroondas.Parado)
            {
                textBox1.Text = "00:00";
                label4.Text = "";
                label1.Text = "";
                potencia = 10;
                trackBar1.Value = potencia;
                label2.Text = potencia.ToString();
                textBox1.Enabled = true;
                trackBar1.Enabled = true;
                programaSelecionado = null;
                charAquecimento = ".";

                unlockButton_Click();
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            LerTempoDaTela();

            textBox1.Enabled = false;
            trackBar1.Enabled = false;
            lockButton();

            if (programaSelecionado != null)
            {
                if (estado == EstadoMicroondas.Pausado)
                {
                    estado = EstadoMicroondas.Executando;
                    timer.Start();
                }
                else if (estado == EstadoMicroondas.Parado)
                {
                    Iniciar();
                }
                return;
            }

            if (estado == EstadoMicroondas.Parado && timerSeconds == 0 )
                timerSeconds += 30;
            else if (estado == EstadoMicroondas.Executando)
                timerSeconds += 30;

            if (!ValidarPotencia() || !ValidarTempo())
            {
                estado = EstadoMicroondas.Parado;
                timerSeconds = 0;

                textBox1.Text = "00:00";
                label4.Text = "";
                label1.Text = "";

                textBox1.Enabled = true;
                trackBar1.Enabled = true;
                programaSelecionado = null;
                charAquecimento = ".";
                unlockButton_Click();

                return;
            }

            if (estado == EstadoMicroondas.Pausado)
            {
                estado = EstadoMicroondas.Executando;
                timer.Start();
                label4.Text = "Aquecendo";
            }
            else
            {
                Iniciar();
            }
        }

        private void LerTempoDaTela()
        {
            try
            {
                string[] partes = textBox1.Text.Split(':');
                int minutos = int.Parse(partes[0]);
                int segundos = int.Parse(partes[1]);

                timerSeconds = minutos * 60 + segundos;
            }
            catch
            {
                timerSeconds = 0;
            }
        }

        private void Iniciar()
        {
            if (!ValidarTempo() || !ValidarPotencia())
                return;

            estado = EstadoMicroondas.Executando;
            timer.Start();

            textBox1.Enabled = false;
            trackBar1.Enabled = false;
            lockButton();

            progressoAquecimento = "";
            label4.Text = "Aquecendo...";
        }

        private bool ValidarTempo()
        {

            if (programaSelecionado != null)
            {
                return true;
            }

                if (timerSeconds < 1 || timerSeconds > 120)
            {
                timer.Stop();
                MessageBox.Show("O tempo deve estar entre 1 e 120 segundos.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private bool ValidarPotencia()
        {

            if (programaSelecionado != null)
            {
                return true;
            }

            if (potencia < 1 || potencia > 10)
            {
                MessageBox.Show("A potência deve estar entre 1 e 10.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void AtualizaTimer(string digito)
        {
            label4.Text = "";
            if (estado != EstadoMicroondas.Parado)
                return;

            string tempoAux = textBox1.Text.Replace(":", "");
            tempoAux += digito;

            if (tempoAux.Length > 4)
                tempoAux = tempoAux.Substring(tempoAux.Length - 4);

            string min = tempoAux.Length >= 2 ? tempoAux.Substring(0, 2) : "00";
            string seg = tempoAux.Length >= 4 ? tempoAux.Substring(2, 2) : "00";

            int s = int.Parse(seg);
            if (s > 59) s = 59;

            textBox1.Text = $"{min}:{s:00}";
        }

        private void button1_Click(object sender, EventArgs e) => AtualizaTimer("1");
        private void button2_Click(object sender, EventArgs e) => AtualizaTimer("2");
        private void button3_Click(object sender, EventArgs e) => AtualizaTimer("3");
        private void button4_Click(object sender, EventArgs e) => AtualizaTimer("4");
        private void button5_Click(object sender, EventArgs e) => AtualizaTimer("5");
        private void button6_Click(object sender, EventArgs e) => AtualizaTimer("6");
        private void button7_Click(object sender, EventArgs e) => AtualizaTimer("7");
        private void button8_Click(object sender, EventArgs e) => AtualizaTimer("8");
        private void button9_Click(object sender, EventArgs e) => AtualizaTimer("9");
        private void button11_Click(object sender, EventArgs e) => AtualizaTimer("0");

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label4.Text = "";
            label1.Text = "";
            potencia = trackBar1.Value;
            label2.Text = potencia.ToString();
        }

        private void lockButton()
        {
            button13.Enabled = false;
            button14.Enabled = false;
            button15.Enabled = false;
            button16.Enabled = false;
            button17.Enabled = false;
        }

        private void unlockButton_Click()
        {
            button13.Enabled = true;
            button14.Enabled = true;
            button15.Enabled = true;
            button16.Enabled = true;
            button17.Enabled = true;
        }
    }
}

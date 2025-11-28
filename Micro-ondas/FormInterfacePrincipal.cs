using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace Micro_ondas
{
    public partial class FormInterfacePrincipal : Form
    {
        private int timerSeconds = 0;
        private EstadoMicroondas estado = EstadoMicroondas.Parado;
        private int potencia = 10;
        private string progressoAquecimento = "";
        private string charAquecimento = ".";

        private ProgramaAquecimento programaSelecionado = null;

        public FormInterfacePrincipal()
        {
            InitializeComponent();
            CarregarProgramasCustom();
            AtualizarDropdownProgramas();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += TimerTick;

            trackBarPotencia.Minimum = 1;
            trackBarPotencia.Maximum = 10;
            trackBarPotencia.Value = potencia;

            labelPotenciaValue.Text = potencia.ToString();

            unlockButton();
            unlockNumericButton();
        }

        private enum EstadoMicroondas
        {
            Parado = 0,
            Executando = 1,
            Pausado = 2
        }

        private List<ProgramaAquecimento> programas = new List<ProgramaAquecimento>
        {
            new ProgramaAquecimento { Nome="Pipoca",        Alimento="Pipoca(de micro-ondas)",    TempoSegundos=180, Potencia=7, charAquecimento="*", Instrucoes="Observar o barulho de estouros do milho, caso houver um intervalo de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento." },
            new ProgramaAquecimento { Nome="Leite",         Alimento="Leite",                     TempoSegundos=300, Potencia=5, charAquecimento="#", Instrucoes="Cuidado com aquecimento de líquidos, o choque térmico aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras." },
            new ProgramaAquecimento { Nome="Carnes de Boi", Alimento="Carne em pedaço ou fatias", TempoSegundos=840, Potencia=4, charAquecimento="@", Instrucoes="Interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme." },
            new ProgramaAquecimento { Nome="Frango",        Alimento="Frango (qualquer corte)",   TempoSegundos=480, Potencia=7, charAquecimento="~", Instrucoes="interrompa o processo na metade e vire o conteúdo com a parte de baixo para cima para o descongelamento uniforme." },
            new ProgramaAquecimento { Nome="Feijão",        Alimento="Feijão congelado",          TempoSegundos=480, Potencia=9, charAquecimento="%", Instrucoes="Deixe o recipiente destampado e em casos de plástico, cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas." }
        };

        private List<ProgramaAquecimento> programasCustom = new List<ProgramaAquecimento>();


        private void SelPrograma(ProgramaAquecimento p)
        {
            programaSelecionado = p;
            timerSeconds = p.TempoSegundos;
            AtualizarDisplayTempo();

            potencia = p.Potencia;
            trackBarPotencia.Value = potencia;
            labelPotenciaValue.Text = potencia.ToString();

            labelInstrucao.Text = p.Instrucoes;

            charAquecimento = p.charAquecimento;

            textBoxCounter.Enabled = false;
            trackBarPotencia.Enabled = false;

            lockNumericButton();
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

                progressoAquecimento += new string(charAquecimento[0], potencia) + " ";
                labelProgresso.Text = progressoAquecimento;

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
            textBoxCounter.Text = $"{minutos:00}:{segundos:00}";
        }

        private void FinalizarAquecimento()
        {
            timer.Stop();
            estado = EstadoMicroondas.Parado;

            labelProgresso.Text = progressoAquecimento + " Aquecimento concluído";

            textBoxCounter.Enabled = true;
            trackBarPotencia.Enabled = true;

            programaSelecionado = null;
            progressoAquecimento = "";
            charAquecimento = ".";
            unlockButton();
            unlockNumericButton();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (estado == EstadoMicroondas.Executando)
            {
                timer.Stop();
                estado = EstadoMicroondas.Pausado;
                labelProgresso.Text = "Pausado";

                if (programaSelecionado == null)
                {
                    textBoxCounter.Enabled = true;
                    trackBarPotencia.Enabled = true;
                }
            }
            else if (estado == EstadoMicroondas.Pausado)
            {
                ResetarMicroondas();
            }
            else if (estado == EstadoMicroondas.Parado)
            {
                ResetarMicroondas();
            }
        }

        private void ResetarMicroondas()
        {
            estado = EstadoMicroondas.Parado;
            timer.Stop();
            timerSeconds = 0;

            textBoxCounter.Text = "00:00";
            labelProgresso.Text = "";
            labelInstrucao.Text = "";
            potencia = 10;
            trackBarPotencia.Value = potencia;
            labelPotenciaValue.Text = potencia.ToString();
            programaSelecionado = null;
            charAquecimento = ".";

            unlockButton();
            unlockNumericButton();

            textBoxCounter.Enabled = true;
            trackBarPotencia.Enabled = true;

            progressoAquecimento = "";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            LerTempoDaTela();

            textBoxCounter.Enabled = false;
            trackBarPotencia.Enabled = false;
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

            if (estado == EstadoMicroondas.Parado && timerSeconds == 0)
                timerSeconds += 30;
            else if (estado == EstadoMicroondas.Executando)
                timerSeconds += 30;

            if (!ValidarPotencia() || !ValidarTempo())
            {
                ResetarMicroondas();
                return;
            }

            if (estado == EstadoMicroondas.Pausado)
            {
                estado = EstadoMicroondas.Executando;
                timer.Start();
                labelProgresso.Text = "Aquecendo";
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
                string[] partes = textBoxCounter.Text.Split(':');
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

            textBoxCounter.Enabled = false;
            trackBarPotencia.Enabled = false;
            lockButton();

            progressoAquecimento = "";
            labelProgresso.Text = "Aquecendo...";
        }

        private bool ValidarTempo()
        {
            if (programaSelecionado != null)
                return true;

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
                return true;

            if (potencia < 1 || potencia > 10)
            {
                MessageBox.Show("A potência deve estar entre 1 e 10.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void AtualizaTimer(string digito)
        {
            labelProgresso.Text = "";
            if (estado != EstadoMicroondas.Parado)
                return;

            string tempoAux = textBoxCounter.Text.Replace(":", "") + digito;

            if (tempoAux.Length > 4)
                tempoAux = tempoAux.Substring(tempoAux.Length - 4);

            string min = tempoAux.Length >= 2 ? tempoAux.Substring(0, 2) : "00";
            string seg = tempoAux.Length >= 4 ? tempoAux.Substring(2, 2) : "00";

            int s = int.Parse(seg);
            if (s > 59) s = 59;

            textBoxCounter.Text = $"{min}:{s:00}";
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
            labelProgresso.Text = "";
            labelInstrucao.Text = "";
            potencia = trackBarPotencia.Value;
            labelPotenciaValue.Text = potencia.ToString();
        }

        private void lockButton()
        {
            buttonPipoca.Enabled = false;
            buttonLeite.Enabled = false;
            buttonCarneBoi.Enabled = false;
            buttonFrango.Enabled = false;
            buttonFeijao.Enabled = false;
            comboProgramas.Enabled = false;
        }

        private void unlockButton()
        {
            buttonPipoca.Enabled = true;
            buttonLeite.Enabled = true;
            buttonCarneBoi.Enabled = true;
            buttonFrango.Enabled = true;
            buttonFeijao.Enabled = true;
            comboProgramas.Enabled = true;

        }
        private void lockNumericButton()
        {
            buttonNum0.Enabled = false;
            buttonNum1.Enabled = false;
            buttonNum2.Enabled = false;
            buttonNum3.Enabled = false;
            buttonNum4.Enabled = false;
            buttonNum5.Enabled = false;
            buttonNum6.Enabled = false;
            buttonNum7.Enabled = false;
            buttonNum8.Enabled = false;
            buttonNum9.Enabled = false;

        }
        private void unlockNumericButton()
        {
            buttonNum0.Enabled = true;
            buttonNum1.Enabled = true;
            buttonNum2.Enabled = true;
            buttonNum3.Enabled = true;
            buttonNum4.Enabled = true;
            buttonNum5.Enabled = true;
            buttonNum6.Enabled = true;
            buttonNum7.Enabled = true;
            buttonNum8.Enabled = true;
            buttonNum9.Enabled = true;

        }

        private void buttonCadastrar_Click(object sender, EventArgs e)
        {
            var lista = programas.Concat(programasCustom).ToList();

            var form = new FormCadastroPrograma(lista);
            if (form.ShowDialog() == DialogResult.OK)
            {
                programasCustom.Add(form.NovoPrograma);
                SalvarPrograma();
                AtualizarDropdownProgramas();
                comboProgramas.SelectedIndex = comboProgramas.Items.Count - 1;
            }
        }
        private string caminhoJson = "programas_custom.json";

        private void SalvarPrograma()
        {
            string json = JsonConvert.SerializeObject(programasCustom, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(caminhoJson, json);
        }
        private void CarregarProgramasCustom()
        {
            if (!File.Exists(caminhoJson))
                return;

            try
            {
                string json = File.ReadAllText(caminhoJson);
                programasCustom = JsonConvert.DeserializeObject<List<ProgramaAquecimento>>(json)
                                  ?? new List<ProgramaAquecimento>();
            }
            catch
            {
                programasCustom = new List<ProgramaAquecimento>();
            }
        }
        private void AtualizarDropdownProgramas()
        {
            comboProgramas.Items.Clear();

            foreach (var p in programasCustom)
            {
                comboProgramas.Items.Add(new ItemPrograma
                {
                    Programa = p,
                    Custom = true
                });
            }
        }

        private void comboProgramas_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();

            var item = (ItemPrograma)comboProgramas.Items[e.Index];

            Font fonte = item.Custom
                ? new Font(e.Font, FontStyle.Italic)
                : e.Font;

            e.Graphics.DrawString(
                item.Programa.Nome,
                fonte,
                new SolidBrush(e.ForeColor),
                e.Bounds
            );

            e.DrawFocusRectangle();
        }

        private void comboProgramas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboProgramas.SelectedIndex < 0)
                return;

            var item = (ItemPrograma)comboProgramas.SelectedItem;

            SelPrograma(item.Programa);
        }
    }
}

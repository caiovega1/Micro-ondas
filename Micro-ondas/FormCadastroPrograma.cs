using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Micro_ondas
{
    public partial class FormCadastroPrograma : Form
    {
        public ProgramaAquecimento NovoPrograma { get; private set; }

        public FormCadastroPrograma(List<ProgramaAquecimento> existentes)
        {
            InitializeComponent();
            listaExistentes = existentes;
        }

        private List<ProgramaAquecimento> listaExistentes;

        private void buttonSalvar_Click(object sender, EventArgs e)
        {
            string nome = textNome.Text.Trim();
            string alimento = textAlimento.Text.Trim();
            string tempoStr = textTempo.Text.Trim();
            string potenciaStr = textPotencia.Text.Trim();
            string caractere = textAquecimentoChar.Text.Trim();
            string instrucoes = textInstrucao.Text.Trim();

            if (string.IsNullOrEmpty(nome) ||
                string.IsNullOrEmpty(alimento) ||
                string.IsNullOrEmpty(tempoStr) ||
                string.IsNullOrEmpty(potenciaStr) ||
                string.IsNullOrEmpty(caractere))
            {
                MessageBox.Show("Todos os campos são obrigatórios, exceto instruções.");
                return;
            }

            if (caractere == "." || caractere.Length != 1)
            {
                MessageBox.Show("Caractere inválido.");
                return;
            }

            if (listaExistentes.Any(p => p.charAquecimento == caractere))
            {
                MessageBox.Show("Este caractere já está sendo usado em outro programa.");
                return;
            }

            if (!int.TryParse(tempoStr, out int tempo) || tempo < 1)
            {
                MessageBox.Show("Tempo inválido.");
                return;
            }

            if (!int.TryParse(potenciaStr, out int potencia) || potencia < 1 || potencia > 10)
            {
                MessageBox.Show("Potência inválida.");
                return;
            }

            NovoPrograma = new ProgramaAquecimento
            {
                Nome = nome,
                Alimento = alimento,
                TempoSegundos = tempo,
                Potencia = potencia,
                charAquecimento = caractere,
                Instrucoes = instrucoes
            };

            DialogResult = DialogResult.OK;

            Close();
        }
    }
}

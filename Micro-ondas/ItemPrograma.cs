using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro_ondas
{
    public class ItemPrograma
    {
        public ProgramaAquecimento Programa { get; set; }
        public bool Custom { get; set; }

        public override string ToString()
        {
            return Programa.Nome;
        }
    }
}

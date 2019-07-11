using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorridaKart.Model
{
    public class Corrida
    {
        public int Posicao { get; set; }

        public DateTime Hora { get; set; }

        public string CodigoPiloto { get; set; }

        public string Piloto { get; set; }

        public int NumeroVolta { get; set; }

        public DateTime TempoVolta { get; set; }

        public double VelocidadeMediaVolta { get; set; }

        public DateTime MelhorVollta { get; set; }

        public bool MelhorVoltaCorrida { get; set; }

        public TimeSpan TempoAposVencedor { get; set; }
    }
}

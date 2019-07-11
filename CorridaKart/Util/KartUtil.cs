using CorridaKart.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorridaKart.Util
{
    public static class KartUtil
    {
        public static List<Corrida> ProcessarArquivo(string caminhoArquivo)
        {
            if (!File.Exists(caminhoArquivo))
                throw new Exception("Arquivo não encontrado.");

            List<Corrida> dadosCorrida = new List<Corrida>();
            bool cabecalho = true;
            string[] linhas = System.IO.File.ReadAllLines(caminhoArquivo, Encoding.GetEncoding("iso-8859-1"));

            foreach (string linha in linhas)
            {
                if (cabecalho)
                {
                    if (!ValidarCabecalho(linha))
                        throw new Exception("Cabeçalho inválido");

                    cabecalho = false;
                }
                else
                {
                    dadosCorrida.Add(PreencherDados(linha));
                }
            }

            return RetornaResultadoCorrida(dadosCorrida);
        }

        private static bool ValidarCabecalho(string cabecalho)
        {
            return cabecalho.Contains("Hora") && cabecalho.Contains("Piloto") && cabecalho.Contains("Nº Volta") &&
                cabecalho.Contains("Tempo Volta") && cabecalho.Contains("Velocidade média da volta");
        }

        private static Corrida PreencherDados(string linha)
        {
            Corrida corrida = new Corrida
            {
                Hora = DateTime.Parse(linha.Substring(0, 18)),
                CodigoPiloto = linha.Substring(18, 3),
                Piloto = linha.Substring(21, 36).TrimStart(),
                NumeroVolta = int.Parse(linha.Substring(55, 6).Trim()),
                TempoVolta = TratarTempo(linha.Substring(61, 12).Trim()),
                VelocidadeMediaVolta = double.Parse(linha.Substring(73, linha.Length - 73))
            };

            return corrida;
        }

        private static DateTime TratarTempo(string tempo)
        {
            DateTime data = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour,
                int.Parse(tempo.Substring(0, tempo.IndexOf(':'))),
                int.Parse(tempo.Substring(tempo.IndexOf(':') + 1, tempo.Length - 1 - tempo.IndexOf('.') - 1)),
                int.Parse(tempo.Substring(tempo.IndexOf('.') + 1, 3)));

            return data;
        }

        private static List<Corrida> RetornaResultadoCorrida(List<Corrida> dadosCorrida)
        {
            var result = (from dados in dadosCorrida
                          group dados by dados.CodigoPiloto into pilotos
                          select new
                          {
                              Posicao = 0,
                              Hora = pilotos.Max(x => x.Hora),
                              CodigoPiloto = pilotos.Max(x => x.CodigoPiloto),
                              Piloto = pilotos.Max(x => x.Piloto),
                              NumeroVolta = pilotos.Max(x => x.NumeroVolta),
                              TempoVolta = DateTime.Now,
                              VelocidadeMediaVolta = pilotos.Average(x => x.VelocidadeMediaVolta),
                          }).ToList();

            var resultado = new List<Corrida>();

            for (int i = 0; i < result.Count; i++)
            {
                Corrida corrida = new Corrida
                {
                    CodigoPiloto = result[i].CodigoPiloto,
                    Piloto = result[i].Piloto,
                    NumeroVolta = result[i].NumeroVolta,
                    TempoVolta = SomarTempo(result[i].CodigoPiloto, dadosCorrida),
                    VelocidadeMediaVolta = result[i].VelocidadeMediaVolta,
                    MelhorVollta = CalcularMelhorVolta(result[i].CodigoPiloto, dadosCorrida)
                };

                resultado.Add(corrida);
            }

            CalcularPosicao(resultado);
            DefinirMelhorVoltaDaCorrida(resultado);
            DefinirTempoAposVencedor(resultado);

            return resultado;
        }

        private static DateTime SomarTempo(string codigoPiloto, List<Corrida> dados)
        {
            var voltas = dados.Where(d => d.CodigoPiloto == codigoPiloto).Select(s => s.TempoVolta).ToList();

            DateTime tempo = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            foreach (DateTime t in voltas)
            {
                tempo = tempo.AddMilliseconds(t.Millisecond);
                tempo = tempo.AddSeconds(t.Second);
                tempo = tempo.AddMinutes(t.Minute);
            }

            return tempo;
        }

        private static void CalcularPosicao(List<Corrida> dados)
        {
            var pos = (from t in dados
                       select t).OrderByDescending(c => c.NumeroVolta).OrderBy(c => c.TempoVolta).ToList();

            for (int i = 0; i < pos.Count(); i++)
            {
                dados.FirstOrDefault(d => d.CodigoPiloto == pos[i].CodigoPiloto).Posicao = i + 1;
            }
        }

        private static DateTime CalcularMelhorVolta(string codigoPiloto, List<Corrida> dados)
        {
            var voltas = dados.Where(d => d.CodigoPiloto == codigoPiloto).Select(s => s.TempoVolta).ToList();

            DateTime melhorVolta = new DateTime();

            foreach (DateTime t in voltas)
            {
                if (melhorVolta == DateTime.MinValue || melhorVolta > t)
                    melhorVolta = t;
            }

            return melhorVolta;
        }

        private static void DefinirMelhorVoltaDaCorrida(List<Corrida> dados)
        {
            var pos = (from t in dados
                       select t).OrderBy(c => c.MelhorVollta).FirstOrDefault();

            dados.FirstOrDefault(d => d.CodigoPiloto == pos.CodigoPiloto).MelhorVoltaCorrida = true;
        }

        private static void DefinirTempoAposVencedor(List<Corrida> dados)
        {
            DateTime tempoVencedorAux = dados.FirstOrDefault(t => t.Posicao == 1).TempoVolta;
            TimeSpan tempoVencedor = new TimeSpan(0, tempoVencedorAux.Hour, tempoVencedorAux.Minute, tempoVencedorAux.Second, tempoVencedorAux.Millisecond);

            foreach (Corrida corrida in dados)
            {
                if (corrida.Posicao > 1)
                {
                    TimeSpan tempo = new TimeSpan(0, corrida.TempoVolta.Hour, corrida.TempoVolta.Minute, corrida.TempoVolta.Second, corrida.TempoVolta.Millisecond);
                    corrida.TempoAposVencedor = tempo - tempoVencedor;
                }
            }
        }
    }
}

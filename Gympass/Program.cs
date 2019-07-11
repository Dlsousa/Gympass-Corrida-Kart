using CorridaKart.Model;
using CorridaKart.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gympass
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                List<Corrida> resultado = KartUtil.ProcessarArquivo(Directory.GetCurrentDirectory() + @"\Log.txt");

                foreach (Corrida corrida in resultado)
                {
                    Console.WriteLine($"Posição Chegada: {corrida.Posicao}");
                    Console.WriteLine($"Código Piloto: {corrida.CodigoPiloto}");
                    Console.WriteLine($"Nome Piloto: {corrida.Piloto}");
                    Console.WriteLine($"Qtde Voltas Completadas: {corrida.NumeroVolta}");
                    Console.WriteLine($"Tempo Total de Prova: {corrida.TempoVolta.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}");

                    if (corrida.MelhorVoltaCorrida)
                        Console.WriteLine($"Fez a Melhor Volta da Corrida");

                    Console.WriteLine($"Melhor Volta: {corrida.MelhorVollta.ToString("mm:ss.fff", CultureInfo.InvariantCulture)}");
                    Console.WriteLine($"Velocidade Média: {corrida.VelocidadeMediaVolta}");

                    if (corrida.Posicao > 1)
                        Console.WriteLine($"Tempo Após Vencedor: {corrida.TempoAposVencedor.ToString()}");

                    Console.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}

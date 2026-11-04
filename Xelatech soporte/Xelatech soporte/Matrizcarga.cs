using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public class MatrizCarga
    {
        private int[,] carga = new int[0, 7];
        private string[] dias = { "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb", "Dom" };
        private string[] diasLargos = { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

        private int IndiceTecnico(List<Tecnico> tecnicos, Tecnico buscado)
        {
            for (int i = 0; i < tecnicos.Count; i++)
            {
                if (tecnicos[i] == buscado)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Reconstruir(List<Tecnico> tecnicos, List<Solicitud> solicitudes)
        {
            carga = new int[tecnicos.Count, 7];
            foreach (Solicitud s in solicitudes)
            {
                if (s.Tecnico == null || !s.FechaAsignacion.HasValue)
                {
                    continue;
                }
                int fila = IndiceTecnico(tecnicos, s.Tecnico);
                if (fila == -1)
                {
                    continue;
                }
                int dia = ((int)s.FechaAsignacion.Value.DayOfWeek + 6) % 7;
                carga[fila, dia]++;
            }
        }

        public void Mostrar(List<Tecnico> tecnicos)
        {
            if (tecnicos.Count == 0)
            {
                Console.WriteLine("No hay técnicos registrados.");
                return;
            }

            Console.Write("Técnico".PadRight(24));
            foreach (string d in dias)
            {
                Console.Write(d.PadLeft(5));
            }
            Console.WriteLine("Total".PadLeft(7));
            Console.WriteLine(new string('-', 66));

            int[] totalDia = new int[7];
            int mayorTotal = -1;
            int filaMayor = 0;

            for (int i = 0; i < tecnicos.Count; i++)
            {
                string nombre = tecnicos[i].Codigo + " " + tecnicos[i].Nombre;
                if (nombre.Length > 23)
                {
                    nombre = nombre.Substring(0, 23);
                }
                Console.Write(nombre.PadRight(24));
                int totalTecnico = 0;
                for (int j = 0; j < 7; j++)
                {
                    Console.Write(carga[i, j].ToString().PadLeft(5));
                    totalTecnico += carga[i, j];
                    totalDia[j] += carga[i, j];
                }
                Console.WriteLine(totalTecnico.ToString().PadLeft(7));
                if (totalTecnico > mayorTotal)
                {
                    mayorTotal = totalTecnico;
                    filaMayor = i;
                }
            }
            Console.WriteLine(new string('-', 66));

            int diaMayor = 0;
            for (int j = 1; j < 7; j++)
            {
                if (totalDia[j] > totalDia[diaMayor])
                {
                    diaMayor = j;
                }
            }

            if (mayorTotal == 0)
            {
                Console.WriteLine("Todavía no hay solicitudes asignadas.");
                return;
            }
            Console.WriteLine("Técnico con mayor carga: " + tecnicos[filaMayor].Codigo + " " + tecnicos[filaMayor].Nombre + " (" + mayorTotal + " solicitudes)");
            Console.WriteLine("Día de mayor carga: " + diasLargos[diaMayor]);
            Console.WriteLine("Solicitudes asignadas ese día: " + totalDia[diaMayor]);
        }
    }
}
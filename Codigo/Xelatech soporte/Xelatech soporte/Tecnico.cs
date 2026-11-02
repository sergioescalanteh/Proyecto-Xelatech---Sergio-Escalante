using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public class Tecnico
    {
        public const int Limitetrabajo = 3;

        private int[] calificaciones = new int[5];

        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public TipoEquipo Especialidad { get; private set; }
        public bool Disponible { get; set; }
        public int TrabajosActivos { get; private set; }
        public int ServiciosFinalizados { get; private set; }

        public Tecnico(string codigo, string nombre, TipoEquipo especialidad)
        {
            Codigo = codigo;
            Nombre = nombre;
            Especialidad = especialidad;
            Disponible = true;
            TrabajosActivos = 0;
            ServiciosFinalizados = 0;
        }

        public bool PuedeRecibir(TipoEquipo tipo)
        {
            return Disponible && Especialidad == tipo && TrabajosActivos < Limitetrabajo;
        }

        public void TomarTrabajo()
        {
            if (!Disponible)
            {
                throw new ReglaNegocioException("El técnico no está disponible.");
            }
            if (TrabajosActivos >= Limitetrabajo)
            {
                throw new ReglaNegocioException("El técnico ya tiene " + Limitetrabajo + " trabajos activos.");
            }
            TrabajosActivos++;
        }

        public void LiberarTrabajo()
        {
            if (TrabajosActivos > 0)
            {
                TrabajosActivos--;
            }
        }

        public void FinalizarServicio(int calificacion)
        {
            if (calificacion < 1 || calificacion > 5)
            {
                throw new ReglaNegocioException("La calificación debe estar entre 1 y 5.");
            }
            calificaciones[calificacion - 1]++;
            ServiciosFinalizados++;
            LiberarTrabajo();
        }

        public int[] Calificaciones()
        {
            return calificaciones;
        }

        public double CalificacionPromedio()
        {
            int total = 0;
            int suma = 0;
            for (int i = 0; i < calificaciones.Length; i++)
            {
                total += calificaciones[i];
                suma += calificaciones[i] * (i + 1);
            }
            if (total == 0)
            {
                return 0;
            }
            return (double)suma / total;
        }

        public override string ToString()
        {
            string disp = Disponible ? "Sí" : "No";
            return Codigo + " | " + Nombre + " | " + Especialidad + " | Disponible: " + disp + " | Activos: " + TrabajosActivos + "/" + Limitetrabajo;
        }
    }
}
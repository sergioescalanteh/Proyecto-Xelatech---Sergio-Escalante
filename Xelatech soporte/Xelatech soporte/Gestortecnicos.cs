using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public class GestorTecnicos
    {
        private List<Tecnico> tecnicos = new List<Tecnico>();

        public List<Tecnico> ObtenerLista()
        {
            return tecnicos;
        }

        public Tecnico Registrar(string nombre, TipoEquipo especialidad)
        {
            Validador.TextoObligatorio(nombre, "nombre del técnico");

            string codigo = "TEC-" + (tecnicos.Count + 1).ToString("0000");
            Tecnico t = new Tecnico(codigo, nombre, especialidad);
            tecnicos.Add(t);
            return t;
        }

        public void Listar()
        {
            if (tecnicos.Count == 0)
            {
                Console.WriteLine("No hay técnicos registrados.");
                return;
            }
            foreach (Tecnico t in tecnicos)
            {
                Console.WriteLine(t);
            }
        }

        public Tecnico BuscarPorCodigo(string codigo)
        {
            foreach (Tecnico t in tecnicos)
            {
                if (t.Codigo == codigo)
                {
                    return t;
                }
            }
            return null;
        }

        public void CambiarDisponibilidad(string codigo, bool disponible)
        {
            Tecnico t = BuscarPorCodigo(codigo);
            if (t == null)
            {
                throw new ReglaNegocioException("No existe un técnico con ese código.");
            }
            t.Disponible = disponible;
        }

        public void MostrarCalificaciones()
        {
            if (tecnicos.Count == 0)
            {
                Console.WriteLine("No hay técnicos registrados.");
                return;
            }
            foreach (Tecnico t in tecnicos)
            {
                int[] notas = t.Calificaciones();
                Console.WriteLine(t.Codigo + " " + t.Nombre);
                for (int i = 0; i < notas.Length; i++)
                {
                    Console.WriteLine("  Nota " + (i + 1) + ": " + notas[i]);
                }
                Console.WriteLine("  Promedio: " + t.CalificacionPromedio().ToString("0.00"));
            }
        }

        public Tecnico BuscarDisponible(TipoEquipo tipo)
        {
            Tecnico elegido = null;
            foreach (Tecnico t in tecnicos)
            {
                if (t.PuedeRecibir(tipo))
                {
                    if (elegido == null || t.TrabajosActivos < elegido.TrabajosActivos)
                    {
                        elegido = t;
                    }
                }
            }
            return elegido;
        }
    }
}

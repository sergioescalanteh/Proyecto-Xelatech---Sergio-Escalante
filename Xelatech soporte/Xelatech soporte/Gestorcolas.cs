using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public class GestorColas
    {
        private Queue<Solicitud> colaUrgente = new Queue<Solicitud>();
        private Queue<Solicitud> colaNormal = new Queue<Solicitud>();

        public void Encolar(Solicitud s)
        {
            if (s.Prioridad == Prioridad.Urgente)
            {
                colaUrgente.Enqueue(s);
            }
            else
            {
                colaNormal.Enqueue(s);
            }
        }

        private void LimpiarFrente(Queue<Solicitud> cola)
        {
            while (cola.Count > 0 && cola.Peek().Estado != EstadoSolicitud.EnEspera)
            {
                cola.Dequeue();
            }
        }

        public Solicitud AtenderSiguiente()
        {
            LimpiarFrente(colaUrgente);
            if (colaUrgente.Count > 0)
            {
                return colaUrgente.Dequeue();
            }
            LimpiarFrente(colaNormal);
            if (colaNormal.Count > 0)
            {
                return colaNormal.Dequeue();
            }
            return null;
        }

        private Queue<Solicitud> PonerAlFrente(Queue<Solicitud> cola, Solicitud s)
        {
            Queue<Solicitud> nueva = new Queue<Solicitud>();
            nueva.Enqueue(s);
            foreach (Solicitud x in cola)
            {
                nueva.Enqueue(x);
            }
            return nueva;
        }

        public void ReinsertarAlFrente(Solicitud s)
        {
            if (s.Prioridad == Prioridad.Urgente)
            {
                colaUrgente = PonerAlFrente(colaUrgente, s);
            }
            else
            {
                colaNormal = PonerAlFrente(colaNormal, s);
            }
        }

        private int MostrarCola(string titulo, Queue<Solicitud> cola)
        {
            Console.WriteLine(titulo);
            int cantidad = 0;
            foreach (Solicitud s in cola)
            {
                if (s.Estado == EstadoSolicitud.EnEspera)
                {
                    cantidad++;
                    Console.WriteLine("  " + cantidad + ". " + s.Codigo + " | " + s.Cliente.Nombre + " | " + s.Equipo.Marca + " " + s.Equipo.Modelo + " | " + s.FechaRecepcion.ToString("dd/MM/yyyy HH:mm"));
                }
            }
            if (cantidad == 0)
            {
                Console.WriteLine("  (vacía)");
            }
            return cantidad;
        }

        public void MostrarColas()
        {
            int urgentes = MostrarCola("Cola URGENTE:", colaUrgente);
            int normales = MostrarCola("Cola NORMAL:", colaNormal);
            Console.WriteLine("Total en espera: " + (urgentes + normales));
        }
    }
}
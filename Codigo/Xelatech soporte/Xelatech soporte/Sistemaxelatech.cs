using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Xelatech_soporte
{
    public class SistemaXelaTech
    {
        public GestorClientes Clientes { get; private set; }
        public GestorTecnicos Tecnicos { get; private set; }
        public GestorEquipos Equipos { get; private set; }
        public GestorSolicitudes Solicitudes { get; private set; }
        public GestorRepuestos Repuestos { get; private set; }
        public GestorColas Colas { get; private set; }
        public MatrizCarga Matriz { get; private set; }

        public List<Solicitud> ListaOrdenada { get; set; }

        public SistemaXelaTech()
        {
            Clientes = new GestorClientes();
            Tecnicos = new GestorTecnicos();
            Equipos = new GestorEquipos();
            Solicitudes = new GestorSolicitudes();
            Repuestos = new GestorRepuestos();
            Colas = new GestorColas();
            Matriz = new MatrizCarga();
            ListaOrdenada = null;
        }

        public Solicitud CrearSolicitud(string codigoCliente, string codigoEquipo, string descripcion, Prioridad prioridad)
        {
            return CrearSolicitud(codigoCliente, codigoEquipo, descripcion, prioridad, DateTime.Now);
        }

        public Solicitud CrearSolicitud(string codigoCliente, string codigoEquipo, string descripcion, Prioridad prioridad, DateTime fecha)
        {
            Cliente cliente = Clientes.BuscarPorCodigo(codigoCliente);
            Equipo equipo = Equipos.BuscarPorCodigo(codigoEquipo);

            Solicitud s = Solicitudes.Crear(cliente, equipo, descripcion, prioridad, fecha);
            Colas.Encolar(s);
            s.CambiarEstado(EstadoSolicitud.EnEspera, "Solicitud encolada");
            ListaOrdenada = null;
            return s;
        }

        public string AtenderSiguiente()
        {
            return AtenderSiguiente(DateTime.Now);
        }

        public string AtenderSiguiente(DateTime fechaAsignacion)
        {
            Solicitud s = Colas.AtenderSiguiente();
            if (s == null)
            {
                return "No hay solicitudes pendientes en las colas.";
            }

            Tecnico t = Tecnicos.BuscarDisponible(s.Equipo.Tipo);
            if (t == null)
            {
                Colas.ReinsertarAlFrente(s);
                return "No hay un técnico adecuado disponible para la solicitud " + s.Codigo + ". Vuelve al frente de su cola.";
            }

            s.AsignarTecnico(t, fechaAsignacion);
            s.CambiarEstado(EstadoSolicitud.Asignada, "Asignada a " + t.Codigo);
            return "Solicitud " + s.Codigo + " (" + s.Prioridad + ") asignada a " + t.Codigo + " " + t.Nombre + ".";
        }
    }
}

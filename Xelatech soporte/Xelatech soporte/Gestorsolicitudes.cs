using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Xelatech_soporte
{
    public class GestorSolicitudes
    {
        private List<Solicitud> solicitudes = new List<Solicitud>();

        public List<Solicitud> ObtenerLista()
        {
            return solicitudes;
        }

        public bool EquipoTieneSolicitudActiva(Equipo equipo)
        {
            foreach (Solicitud s in solicitudes)
            {
                if (s.Equipo == equipo && s.EsActiva())
                {
                    return true;
                }
            }
            return false;
        }

        public Solicitud Crear(Cliente cliente, Equipo equipo, string descripcion, Prioridad prioridad, DateTime fecha)
        {
            if (cliente == null)
            {
                throw new ReglaNegocioException("El cliente no existe.");
            }
            if (equipo == null)
            {
                throw new ReglaNegocioException("El equipo no existe.");
            }
            if (equipo.Propietario != cliente)
            {
                throw new ReglaNegocioException("El equipo no pertenece a ese cliente.");
            }
            Validador.TextoObligatorio(descripcion, "descripción del problema");
            if (EquipoTieneSolicitudActiva(equipo))
            {
                throw new ReglaNegocioException("El equipo ya tiene una solicitud activa.");
            }

            string codigo = "SOL-" + (solicitudes.Count + 1).ToString("0000");
            Solicitud s = new Solicitud(codigo, cliente, equipo, descripcion, prioridad, fecha);
            solicitudes.Add(s);
            return s;
        }

        public Solicitud BuscarPorCodigo(string codigo)
        {
            foreach (Solicitud s in solicitudes)
            {
                if (s.Codigo == codigo)
                {
                    return s;
                }
            }
            return null;
        }

        private Solicitud Exigir(string codigo)
        {
            Solicitud s = BuscarPorCodigo(codigo);
            if (s == null)
            {
                throw new ReglaNegocioException("No existe una solicitud con ese código.");
            }
            return s;
        }

        private void ExigirActiva(Solicitud s)
        {
            if (!s.EsActiva())
            {
                throw new ReglaNegocioException("No se puede modificar una solicitud entregada o cancelada.");
            }
        }

        public void ActualizarTotales()
        {
            foreach (Solicitud s in solicitudes)
            {
                if (s.EsActiva())
                {
                    s.CalcularTotal();
                }
            }
        }

        public void Listar()
        {
            if (solicitudes.Count == 0)
            {
                Console.WriteLine("No hay solicitudes registradas.");
                return;
            }
            ActualizarTotales();
            foreach (Solicitud s in solicitudes)
            {
                Console.WriteLine(s);
            }
        }

        public void CambiarEstado(string codigo, EstadoSolicitud nuevo, string observacion)
        {
            Solicitud s = Exigir(codigo);
            if (nuevo == EstadoSolicitud.Entregada)
            {
                throw new ReglaNegocioException("Para entregar un equipo use la opción Entregar equipo.");
            }
            if (nuevo == EstadoSolicitud.EnEspera || nuevo == EstadoSolicitud.Asignada)
            {
                throw new ReglaNegocioException("Ese estado se asigna solo al encolar o atender la solicitud.");
            }
            s.CambiarEstado(nuevo, observacion);
        }

        public EstadoSolicitud Deshacer(string codigo)
        {
            Solicitud s = Exigir(codigo);
            if (s.Estado == EstadoSolicitud.EnEspera || s.Estado == EstadoSolicitud.Asignada)
            {
                throw new ReglaNegocioException("No se puede deshacer el encolado ni la asignación del técnico.");
            }
            return s.DeshacerUltimoCambio();
        }

        public void RegistrarDiagnostico(string codigo, string descripcion, int horas, List<string> acciones)
        {
            Solicitud s = Exigir(codigo);
            ExigirActiva(s);
            if (s.Estado != EstadoSolicitud.EnDiagnostico && s.Estado != EstadoSolicitud.EnReparacion)
            {
                throw new ReglaNegocioException("Primero cambie la solicitud al estado En diagnóstico.");
            }
            Diagnostico d = new Diagnostico("DIA-" + s.Codigo.Substring(4), descripcion, horas);
            foreach (string accion in acciones)
            {
                d.AgregarAccion(accion);
            }
            s.RegistrarDiagnostico(d);
        }

        public void RegistrarManoObra(string codigo, decimal costo)
        {
            Solicitud s = Exigir(codigo);
            ExigirActiva(s);
            Validador.NoNegativo(costo, "costo de mano de obra");
            s.CostoManoObra = costo;
        }

        public void AgregarRepuesto(string codigo, Repuesto repuesto, int cantidad)
        {
            Solicitud s = Exigir(codigo);
            ExigirActiva(s);
            if (s.Estado != EstadoSolicitud.EnDiagnostico && s.Estado != EstadoSolicitud.EnReparacion)
            {
                throw new ReglaNegocioException("Los repuestos se registran en diagnóstico o reparación.");
            }
            s.AgregarRepuesto(repuesto, cantidad);
        }

        public decimal Entregar(string codigo, int calificacion)
        {
            Solicitud s = Exigir(codigo);
            if (s.Estado != EstadoSolicitud.ListaParaEntregar)
            {
                throw new ReglaNegocioException("La solicitud debe estar Lista para entregar.");
            }
            if (calificacion < 1 || calificacion > 5)
            {
                throw new ReglaNegocioException("La calificación debe estar entre 1 y 5.");
            }
            s.CalcularTotal();
            s.CambiarEstado(EstadoSolicitud.Entregada, "Equipo entregado");
            if (s.Tecnico != null)
            {
                s.Tecnico.FinalizarServicio(calificacion);
            }
            return s.Total;
        }

        public void Cancelar(string codigo)
        {
            Solicitud s = Exigir(codigo);
            if (s.Estado == EstadoSolicitud.Entregada)
            {
                throw new ReglaNegocioException("No se puede cancelar una solicitud ya entregada.");
            }
            if (s.Estado == EstadoSolicitud.Cancelada)
            {
                throw new ReglaNegocioException("La solicitud ya está cancelada.");
            }
            s.CambiarEstado(EstadoSolicitud.Cancelada, "Solicitud cancelada");
        }

        public void MostrarHistorialEquipo(Equipo equipo)
        {
            bool hayDatos = false;
            foreach (Solicitud s in solicitudes)
            {
                if (s.Equipo == equipo)
                {
                    Console.WriteLine(s);
                    hayDatos = true;
                }
            }
            if (!hayDatos)
            {
                Console.WriteLine("Este equipo todavía no tiene solicitudes.");
            }
        }
    }
}

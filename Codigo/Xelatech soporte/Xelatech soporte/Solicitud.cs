using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public class EstadoCambio
    {
        public EstadoSolicitud EstadoAnterior { get; private set; }
        public DateTime Fecha { get; private set; }
        public string Observacion { get; private set; }

        public EstadoCambio(EstadoSolicitud estadoAnterior, DateTime fecha, string observacion)
        {
            EstadoAnterior = estadoAnterior;
            Fecha = fecha;
            Observacion = observacion;
        }

        public override string ToString()
        {
            string texto = Fecha.ToString("dd/MM/yyyy HH:mm") + " | Estado anterior: " + EstadoAnterior;
            if (!string.IsNullOrWhiteSpace(Observacion))
            {
                texto += " | " + Observacion;
            }
            return texto;
        }
    }

    public class Diagnostico
    {
        private List<string> acciones = new List<string>();

        public string Codigo { get; private set; }
        public string Descripcion { get; private set; }
        public int TiempoEstimadoHoras { get; private set; }
        public DateTime Fecha { get; private set; }

        public Diagnostico(string codigo, string descripcion, int tiempoEstimadoHoras)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                throw new ReglaNegocioException("La descripción del diagnóstico es obligatoria.");
            }
            if (tiempoEstimadoHoras < 0)
            {
                throw new ReglaNegocioException("El tiempo estimado no puede ser negativo.");
            }
            Codigo = codigo;
            Descripcion = descripcion;
            TiempoEstimadoHoras = tiempoEstimadoHoras;
            Fecha = DateTime.Now;
        }

        public void AgregarAccion(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                throw new ReglaNegocioException("La acción no puede estar vacía.");
            }
            acciones.Add(texto);
        }

        public List<string> Acciones()
        {
            return acciones;
        }
    }

    public class Solicitud
    {
        private const decimal RECARGO_URGENCIA = 0.20m;
        private const decimal DESCUENTO = 0.10m;

        private Stack<EstadoCambio> historial = new Stack<EstadoCambio>();
        private List<RepuestoUsado> repuestosUsados = new List<RepuestoUsado>();
        private decimal costoManoObra;

        public string Codigo { get; private set; }
        public Cliente Cliente { get; private set; }
        public Equipo Equipo { get; private set; }
        public string Descripcion { get; private set; }
        public DateTime FechaRecepcion { get; private set; }
        public DateTime? FechaAsignacion { get; private set; }
        public Prioridad Prioridad { get; private set; }
        public EstadoSolicitud Estado { get; private set; }
        public Tecnico Tecnico { get; private set; }
        public Diagnostico Diagnostico { get; private set; }
        public int TiempoEstimadoHoras { get; private set; }
        public decimal CostoRepuestos { get; private set; }
        public decimal Total { get; private set; }

        public Solicitud(string codigo, Cliente cliente, Equipo equipo, string descripcion, Prioridad prioridad)
            : this(codigo, cliente, equipo, descripcion, prioridad, DateTime.Now)
        {
        }

        public Solicitud(string codigo, Cliente cliente, Equipo equipo, string descripcion, Prioridad prioridad, DateTime fechaRecepcion)
        {
            if (cliente == null || equipo == null)
            {
                throw new ReglaNegocioException("La solicitud necesita un cliente y un equipo registrados.");
            }
            if (string.IsNullOrWhiteSpace(descripcion))
            {
                throw new ReglaNegocioException("La descripción del problema es obligatoria.");
            }
            Codigo = codigo;
            Cliente = cliente;
            Equipo = equipo;
            Descripcion = descripcion;
            Prioridad = prioridad;
            Estado = EstadoSolicitud.Recibida;
            FechaRecepcion = fechaRecepcion;
            TiempoEstimadoHoras = equipo.TiempoEstimadoBase();
            cliente.SumarSolicitud();
        }

        public decimal CostoManoObra
        {
            get { return costoManoObra; }
            set
            {
                if (value < 0)
                {
                    throw new ReglaNegocioException("El costo de mano de obra no puede ser negativo.");
                }
                costoManoObra = value;
            }
        }

        public List<RepuestoUsado> RepuestosUsados()
        {
            return repuestosUsados;
        }

        private bool TransicionValida(EstadoSolicitud nuevo)
        {
            switch (Estado)
            {
                case EstadoSolicitud.Recibida:
                    return nuevo == EstadoSolicitud.EnEspera || nuevo == EstadoSolicitud.Cancelada;
                case EstadoSolicitud.EnEspera:
                    return nuevo == EstadoSolicitud.Asignada || nuevo == EstadoSolicitud.Cancelada;
                case EstadoSolicitud.Asignada:
                    return nuevo == EstadoSolicitud.EnDiagnostico || nuevo == EstadoSolicitud.Cancelada;
                case EstadoSolicitud.EnDiagnostico:
                    return nuevo == EstadoSolicitud.EnReparacion || nuevo == EstadoSolicitud.Cancelada;
                case EstadoSolicitud.EnReparacion:
                    return nuevo == EstadoSolicitud.ListaParaEntregar || nuevo == EstadoSolicitud.Cancelada;
                case EstadoSolicitud.ListaParaEntregar:
                    return nuevo == EstadoSolicitud.Entregada || nuevo == EstadoSolicitud.Cancelada;
                default:
                    return false;
            }
        }

        public void CambiarEstado(EstadoSolicitud nuevo, string observacion)
        {
            if (!TransicionValida(nuevo))
            {
                throw new ReglaNegocioException("No se puede pasar de " + Estado + " a " + nuevo + ".");
            }
            historial.Push(new EstadoCambio(Estado, DateTime.Now, observacion));
            Estado = nuevo;

            if (nuevo == EstadoSolicitud.Cancelada && Tecnico != null)
            {
                Tecnico.LiberarTrabajo();
            }
        }

        public void CambiarEstado(EstadoSolicitud nuevo)
        {
            CambiarEstado(nuevo, "");
        }

        public EstadoSolicitud DeshacerUltimoCambio()
        {
            if (historial.Count == 0)
            {
                throw new ReglaNegocioException("No hay cambios de estado para deshacer.");
            }
            if (Estado == EstadoSolicitud.Entregada || Estado == EstadoSolicitud.Cancelada)
            {
                throw new ReglaNegocioException("No se puede deshacer una solicitud entregada o cancelada.");
            }
            EstadoCambio ultimo = historial.Pop();
            Estado = ultimo.EstadoAnterior;
            return Estado;
        }

        public void ConsultarHistorial()
        {
            if (historial.Count == 0)
            {
                Console.WriteLine("La solicitud no tiene cambios de estado.");
                return;
            }
            foreach (EstadoCambio cambio in historial)
            {
                Console.WriteLine(cambio);
            }
        }

        public void AsignarTecnico(Tecnico tecnico)
        {
            AsignarTecnico(tecnico, DateTime.Now);
        }

        public void AsignarTecnico(Tecnico tecnico, DateTime fecha)
        {
            if (tecnico == null)
            {
                throw new ReglaNegocioException("No se indicó el técnico.");
            }
            if (Tecnico != null)
            {
                throw new ReglaNegocioException("La solicitud ya tiene un técnico asignado.");
            }
            if (!tecnico.PuedeRecibir(Equipo.Tipo))
            {
                throw new ReglaNegocioException("El técnico no puede recibir esta solicitud.");
            }
            tecnico.TomarTrabajo();
            Tecnico = tecnico;
            FechaAsignacion = fecha;
        }

        public void RegistrarDiagnostico(Diagnostico diagnostico)
        {
            if (diagnostico == null)
            {
                throw new ReglaNegocioException("El diagnóstico no puede ser nulo.");
            }
            Diagnostico = diagnostico;
            TiempoEstimadoHoras = diagnostico.TiempoEstimadoHoras;
        }

        public void AgregarRepuesto(Repuesto repuesto, int cantidad)
        {
            if (!EsActiva())
            {
                throw new ReglaNegocioException("No se pueden agregar repuestos a una solicitud cerrada.");
            }
            if (repuesto == null)
            {
                throw new ReglaNegocioException("No se indicó el repuesto.");
            }
            repuesto.Descontar(cantidad);
            repuestosUsados.Add(new RepuestoUsado(repuesto, cantidad));
        }

        public decimal CalcularTotal()
        {
            CostoRepuestos = 0;
            foreach (RepuestoUsado r in repuestosUsados)
            {
                CostoRepuestos += r.Subtotal();
            }

            decimal subtotal = Equipo.CalcularCostoRevision() + costoManoObra + CostoRepuestos;

            if (Prioridad == Prioridad.Urgente)
            {
                subtotal += subtotal * RECARGO_URGENCIA;
            }
            if (Cliente.TieneDescuento())
            {
                subtotal -= subtotal * DESCUENTO;
            }

            Total = subtotal;
            return Total;
        }

        public bool EsActiva()
        {
            return Estado != EstadoSolicitud.Entregada && Estado != EstadoSolicitud.Cancelada;
        }

        public void MostrarDetalle()
        {
            Console.WriteLine("Código: " + Codigo);
            Console.WriteLine("Cliente: " + Cliente.Nombre);
            Console.WriteLine("Equipo: " + Equipo);
            Console.WriteLine("Problema: " + Descripcion);
            Console.WriteLine("Recepción: " + FechaRecepcion.ToString("dd/MM/yyyy HH:mm"));
            Console.WriteLine("Prioridad: " + Prioridad);
            Console.WriteLine("Estado: " + Estado);
            if (Tecnico == null)
            {
                Console.WriteLine("Técnico: sin asignar");
            }
            else
            {
                Console.WriteLine("Técnico: " + Tecnico.Codigo + " " + Tecnico.Nombre);
            }
            if (Diagnostico == null)
            {
                Console.WriteLine("Diagnóstico: sin registrar");
            }
            else
            {
                Console.WriteLine("Diagnóstico: " + Diagnostico.Descripcion + " (" + Diagnostico.TiempoEstimadoHoras + " h)");
                foreach (string accion in Diagnostico.Acciones())
                {
                    Console.WriteLine("  - " + accion);
                }
            }
            Console.WriteLine("Tiempo estimado: " + TiempoEstimadoHoras + " h");
            if (repuestosUsados.Count == 0)
            {
                Console.WriteLine("Repuestos: ninguno");
            }
            else
            {
                Console.WriteLine("Repuestos:");
                foreach (RepuestoUsado r in repuestosUsados)
                {
                    Console.WriteLine("  - " + r);
                }
            }
            Console.WriteLine("Mano de obra: Q" + costoManoObra.ToString("N2"));
            Console.WriteLine("Repuestos: Q" + CostoRepuestos.ToString("N2"));
            Console.WriteLine("Total: Q" + Total.ToString("N2"));
        }

        public override string ToString()
        {
            return Codigo + " | " + Cliente.Nombre + " | " + Equipo.Marca + " " + Equipo.Modelo + " | " + Prioridad + " | " + Estado + " | Q" + Total.ToString("N2");
        }
    }
}
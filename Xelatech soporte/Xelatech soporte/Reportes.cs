using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public class Reportes
    {
        private SistemaXelaTech sistema;

        public Reportes(SistemaXelaTech sistema)
        {
            this.sistema = sistema;
        }

        private void Titulo(string texto)
        {
            Console.WriteLine("============================================");
            Console.WriteLine(" " + texto);
            Console.WriteLine("============================================");
        }

        private string Cortar(string texto, int largo)
        {
            if (texto.Length > largo)
            {
                return texto.Substring(0, largo);
            }
            return texto;
        }

        private string NombreOrden(bool ascendente)
        {
            return ascendente ? "Ascendente" : "Descendente";
        }

        private void MostrarTablaSolicitudes(List<Solicitud> lista)
        {
            Console.WriteLine("No.".PadRight(5) + "Código".PadRight(10) + "Cliente".PadRight(22) + "Estado".PadRight(20) + "Fecha".PadRight(18) + "Horas".PadRight(7) + "Total (Q)".PadLeft(11));
            Console.WriteLine(new string('-', 93));
            for (int i = 0; i < lista.Count; i++)
            {
                Solicitud s = lista[i];
                Console.WriteLine((i + 1).ToString().PadRight(5) + s.Codigo.PadRight(10) + Cortar(s.Cliente.Nombre, 20).PadRight(22) + s.Estado.ToString().PadRight(20) + s.FechaRecepcion.ToString("dd/MM/yyyy HH:mm").PadRight(18) + s.TiempoEstimadoHoras.ToString().PadRight(7) + s.Total.ToString("N2").PadLeft(11));
            }
            Console.WriteLine(new string('-', 93));
            Console.WriteLine("Total de solicitudes listadas: " + lista.Count);
        }

        public void Reporte1()
        {
            Titulo("SOLICITUDES EN ESPERA POR PRIORIDAD");
            sistema.Colas.MostrarColas();
        }

        public void Reporte2()
        {
            Titulo("SOLICITUDES ACTIVAS, FINALIZADAS Y CANCELADAS");
            List<Solicitud> todas = sistema.Solicitudes.ObtenerLista();
            if (todas.Count == 0)
            {
                Console.WriteLine("No hay solicitudes registradas.");
                return;
            }
            sistema.Solicitudes.ActualizarTotales();

            List<Solicitud> activas = new List<Solicitud>();
            List<Solicitud> finalizadas = new List<Solicitud>();
            List<Solicitud> canceladas = new List<Solicitud>();

            foreach (Solicitud s in todas)
            {
                if (s.Estado == EstadoSolicitud.Cancelada)
                {
                    canceladas.Add(s);
                }
                else if (s.Estado == EstadoSolicitud.Entregada)
                {
                    finalizadas.Add(s);
                }
                else
                {
                    activas.Add(s);
                }
            }

            Console.WriteLine("ACTIVAS (" + activas.Count + ")");
            foreach (Solicitud s in activas) { Console.WriteLine("  " + s); }
            Console.WriteLine("FINALIZADAS (" + finalizadas.Count + ")");
            foreach (Solicitud s in finalizadas) { Console.WriteLine("  " + s); }
            Console.WriteLine("CANCELADAS (" + canceladas.Count + ")");
            foreach (Solicitud s in canceladas) { Console.WriteLine("  " + s); }
        }

        public void Reporte3()
        {
            Titulo("TÉCNICOS DISPONIBLES Y CON CARGA COMPLETA");
            List<Tecnico> tecnicos = sistema.Tecnicos.ObtenerLista();
            if (tecnicos.Count == 0)
            {
                Console.WriteLine("No hay técnicos registrados.");
                return;
            }

            Console.WriteLine("Disponibles para recibir solicitudes:");
            int disponibles = 0;
            foreach (Tecnico t in tecnicos)
            {
                if (t.Disponible && t.TrabajosActivos < Tecnico.Limitetrabajo)
                {
                    Console.WriteLine("  " + t);
                    disponibles++;
                }
            }
            if (disponibles == 0) { Console.WriteLine("  (ninguno)"); }

            Console.WriteLine("Con carga completa:");
            int completos = 0;
            foreach (Tecnico t in tecnicos)
            {
                if (t.TrabajosActivos >= Tecnico.Limitetrabajo)
                {
                    Console.WriteLine("  " + t);
                    completos++;
                }
            }
            if (completos == 0) { Console.WriteLine("  (ninguno)"); }
        }

        public void Reporte4()
        {
            Titulo("TÉCNICO CON MÁS SERVICIOS FINALIZADOS");
            List<Tecnico> tecnicos = sistema.Tecnicos.ObtenerLista();
            if (tecnicos.Count == 0)
            {
                Console.WriteLine("No hay técnicos registrados.");
                return;
            }

            Tecnico mejor = tecnicos[0];
            if (tecnicos.Count > 1)
            {
                Console.WriteLine("Algoritmo: Selection Sort | Criterio: Servicios finalizados | Orden: Descendente");
                mejor = Ordenamientos.SelectionSort(tecnicos, false)[0];
            }

            if (mejor.ServiciosFinalizados == 0)
            {
                Console.WriteLine("Todavía no hay servicios finalizados.");
                return;
            }
            Console.WriteLine(mejor.Codigo + " " + mejor.Nombre + " con " + mejor.ServiciosFinalizados + " servicios finalizados.");
        }

        public void Reporte5()
        {
            Titulo("EQUIPOS ATENDIDOS POR TIPO");
            int[] contadores = new int[3];
            foreach (Solicitud s in sistema.Solicitudes.ObtenerLista())
            {
                if (s.Tecnico != null)
                {
                    contadores[(int)s.Equipo.Tipo]++;
                }
            }
            Console.WriteLine("Computadoras portátiles:    " + contadores[0]);
            Console.WriteLine("Computadoras de escritorio: " + contadores[1]);
            Console.WriteLine("Impresoras:                 " + contadores[2]);
            Console.WriteLine("(se cuentan las solicitudes que ya tienen técnico asignado)");
        }


        public void OrdenarSolicitudesPorCosto(bool ascendente)
        {
            Titulo("SOLICITUDES ORDENADAS POR COSTO");
            sistema.Solicitudes.ActualizarTotales();
            List<Solicitud> ordenada = Ordenamientos.BubbleSort(sistema.Solicitudes.ObtenerLista(), ascendente);
            Console.WriteLine("Algoritmo: Bubble Sort");
            Console.WriteLine("Criterio: Costo total");
            Console.WriteLine("Orden: " + NombreOrden(ascendente));
            MostrarTablaSolicitudes(ordenada);
        }
        public void OrdenarSolicitudesPorFechaOTiempo(string criterio, bool ascendente)
        {
            Titulo("SOLICITUDES ORDENADAS");
            sistema.Solicitudes.ActualizarTotales();
            List<Solicitud> ordenada = Ordenamientos.QuickSort(sistema.Solicitudes.ObtenerLista(), criterio, ascendente);
            sistema.ListaOrdenada = ordenada;

            string nombreCriterio = "Código";
            if (criterio == "Fecha") { nombreCriterio = "Fecha de recepción"; }
            if (criterio == "Tiempo") { nombreCriterio = "Tiempo estimado"; }

            Console.WriteLine("Algoritmo: Quick Sort");
            Console.WriteLine("Criterio: " + nombreCriterio);
            Console.WriteLine("Orden: " + NombreOrden(ascendente));
            MostrarTablaSolicitudes(ordenada);
        }

        public void OrdenarTecnicosPorServicios(bool ascendente)
        {
            Titulo("TÉCNICOS ORDENADOS POR SERVICIOS");
            List<Tecnico> ordenada = Ordenamientos.SelectionSort(sistema.Tecnicos.ObtenerLista(), ascendente);
            Console.WriteLine("Algoritmo: Selection Sort");
            Console.WriteLine("Criterio: Servicios finalizados");
            Console.WriteLine("Orden: " + NombreOrden(ascendente));
            Console.WriteLine(new string('-', 60));
            foreach (Tecnico t in ordenada)
            {
                Console.WriteLine(t.Codigo + " | " + t.Nombre.PadRight(22) + " | Servicios: " + t.ServiciosFinalizados);
            }
        }

        public void OrdenarRepuestosPorExistencia(bool ascendente)
        {
            Titulo("REPUESTOS ORDENADOS POR EXISTENCIA");
            List<Repuesto> ordenada = Ordenamientos.ShellSort(sistema.Repuestos.ObtenerLista(), ascendente);
            Console.WriteLine("Algoritmo: Shell Sort");
            Console.WriteLine("Criterio: Existencia disponible");
            Console.WriteLine("Orden: " + NombreOrden(ascendente));
            Console.WriteLine(new string('-', 60));
            foreach (Repuesto r in ordenada)
            {
                Console.WriteLine(r.Codigo + " | " + r.Nombre.PadRight(18) + " | Existencia: " + r.Existencia);
            }
        }

        public void Reporte9()
        {
            Titulo("REPUESTOS EN EXISTENCIA MÍNIMA O AGOTADOS");
            sistema.Repuestos.MostrarBajoMinimo();
        }

        public void Reporte10()
        {
            Titulo("INGRESOS POR SERVICIOS FINALIZADOS");
            decimal total = 0;
            int cantidad = 0;
            foreach (Solicitud s in sistema.Solicitudes.ObtenerLista())
            {
                if (s.Estado == EstadoSolicitud.Entregada)
                {
                    total += s.Total;
                    cantidad++;
                }
            }
            Console.WriteLine("Solicitudes entregadas: " + cantidad);
            Console.WriteLine("Ingresos totales: Q" + total.ToString("N2"));
        }

        public void BuscarClientes(string texto)
        {
            List<Cliente> resultados = Busquedas.SecuencialClientes(sistema.Clientes.ObtenerLista(), texto);
            Console.WriteLine("Búsqueda secuencial de clientes por nombre parcial: " + texto);
            if (resultados.Count == 0)
            {
                Console.WriteLine("  No se encontraron clientes.");
            }
            foreach (Cliente c in resultados) { Console.WriteLine("  " + c); }
        }

        public void BuscarSolicitudesPorEstado(EstadoSolicitud estado)
        {
            List<Solicitud> resultados = Busquedas.SecuencialSolicitudes(sistema.Solicitudes.ObtenerLista(), estado);
            Console.WriteLine("Búsqueda secuencial de solicitudes por estado: " + estado);
            if (resultados.Count == 0)
            {
                Console.WriteLine("  No se encontraron solicitudes con ese estado.");
            }
            foreach (Solicitud s in resultados) { Console.WriteLine("  " + s); }
        }

        public void BuscarEquiposPorMarca(string marca)
        {
            List<Equipo> resultados = Busquedas.SecuencialEquipos(sistema.Equipos.ObtenerLista(), marca);
            Console.WriteLine("Búsqueda secuencial de equipos por marca: " + marca);
            if (resultados.Count == 0)
            {
                Console.WriteLine("  No se encontraron equipos de esa marca.");
            }
            foreach (Equipo e in resultados) { Console.WriteLine("  " + e); }
        }

        public void BuscarSolicitudPorCodigo(string codigo)
        {
            if (sistema.ListaOrdenada == null)
            {
                throw new ReglaNegocioException("Primero ordene las solicitudes por código (menú Ordenamientos, opción 4).");
            }
            Solicitud s = Busquedas.BinariaSolicitud(sistema.ListaOrdenada, codigo);
            Console.WriteLine("Búsqueda binaria de solicitud por código: " + codigo);
            if (s == null)
            {
                Console.WriteLine("  No se encontró la solicitud.");
            }
            else
            {
                Console.WriteLine("  " + s);
            }
        }

        public void Reporte11(string nombre, EstadoSolicitud estado, string marca, string codigo)
        {
            Titulo("RESULTADOS DE LAS BÚSQUEDAS");
            BuscarClientes(nombre);
            BuscarSolicitudesPorEstado(estado);
            BuscarEquiposPorMarca(marca);

            Console.WriteLine("Búsqueda binaria de solicitud por código: " + codigo);
            List<Solicitud> todas = sistema.Solicitudes.ObtenerLista();
            if (todas.Count == 0)
            {
                Console.WriteLine("  No hay solicitudes registradas.");
                return;
            }
            List<Solicitud> ordenada = todas;
            if (todas.Count > 1)
            {
                ordenada = Ordenamientos.QuickSort(todas, "Codigo", true);
            }
            Solicitud s = Busquedas.BinariaSolicitud(ordenada, codigo);
            if (s == null)
            {
                Console.WriteLine("  No se encontró la solicitud.");
            }
            else
            {
                Console.WriteLine("  " + s);
            }
        }

        public void Reporte12()
        {
            Titulo("MATRIZ SEMANAL DE CARGA");
            sistema.Matriz.Reconstruir(sistema.Tecnicos.ObtenerLista(), sistema.Solicitudes.ObtenerLista());
            sistema.Matriz.Mostrar(sistema.Tecnicos.ObtenerLista());
        }
    }
}
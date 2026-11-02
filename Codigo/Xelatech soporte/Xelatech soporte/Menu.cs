using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Xelatech_soporte
{
    public class Menu
    {
        private SistemaXelaTech sistema = new SistemaXelaTech();
        private Reportes reportes;

        public Menu()
        {
            reportes = new Reportes(sistema);
        }
        private int LeerEntero(string mensaje, int min, int max)
        {
            int valor;
            while (true)
            {
                Console.Write(mensaje);
                if (int.TryParse(Console.ReadLine(), out valor) && valor >= min && valor <= max)
                {
                    return valor;
                }
                Console.WriteLine("Entrada inválida. Ingrese un número entre " + min + " y " + max + ".");
            }
        }

        private decimal LeerDecimal(string mensaje)
        {
            decimal valor;
            while (true)
            {
                Console.Write(mensaje);
                if (decimal.TryParse(Console.ReadLine(), out valor) && valor >= 0)
                {
                    return valor;
                }
                Console.WriteLine("Ingrese un número válido mayor o igual a 0.");
            }
        }

        private string LeerTexto(string mensaje)
        {
            Console.Write(mensaje);
            string texto = Console.ReadLine();
            if (texto == null)
            {
                return "";
            }
            return texto.Trim();
        }

        private bool LeerSiNo(string mensaje)
        {
            Console.Write(mensaje + " (S/N): ");
            string resp = Console.ReadLine();
            return resp != null && resp.Trim().ToUpper() == "S";
        }

        private bool LeerAscendente()
        {
            Console.WriteLine("1. Ascendente");
            Console.WriteLine("2. Descendente");
            return LeerEntero("Seleccione el orden: ", 1, 2) == 1;
        }

        private void Pausa()
        {
            Console.WriteLine();
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        private void Encabezado(string titulo)
        {
            Console.Clear();
            Console.WriteLine("============================================");
            Console.WriteLine("  " + titulo);
            Console.WriteLine("============================================");
        }

        private TipoEquipo LeerEspecialidad()
        {
            Console.WriteLine("Especialidad:");
            Console.WriteLine("1. Computadora portátil");
            Console.WriteLine("2. Computadora de escritorio");
            Console.WriteLine("3. Impresora");
            int op = LeerEntero("Seleccione una opción: ", 1, 3);
            if (op == 1) return TipoEquipo.Portatil;
            if (op == 2) return TipoEquipo.Escritorio;
            return TipoEquipo.Impresora;
        }

        private EstadoSolicitud LeerEstado()
        {
            EstadoSolicitud[] estados = (EstadoSolicitud[])Enum.GetValues(typeof(EstadoSolicitud));
            for (int i = 0; i < estados.Length; i++)
            {
                Console.WriteLine((i + 1) + ". " + estados[i]);
            }
            int op = LeerEntero("Seleccione el estado: ", 1, estados.Length);
            return estados[op - 1];
        }
        public void Iniciar()
        {
            int opcion;
            do
            {
                Console.Clear();
                Console.WriteLine("============================================");
                Console.WriteLine("             XELATECH SOPORTE ");
                Console.WriteLine("============================================");
                Console.WriteLine("1. Gestión de clientes");
                Console.WriteLine("2. Gestión de técnicos");
                Console.WriteLine("3. Gestión de equipos");
                Console.WriteLine("4. Gestión de solicitudes");
                Console.WriteLine("5. Gestión de repuestos");
                Console.WriteLine("6. Atención de colas");
                Console.WriteLine("7. Ordenamientos");
                Console.WriteLine("8. Búsquedas");
                Console.WriteLine("9. Reportes");
                Console.WriteLine("10. Generar datos de prueba");
                Console.WriteLine("11. Salir");
                opcion = LeerEntero("Seleccione una opción: ", 1, 11);

                switch (opcion)
                {
                    case 1: MenuClientes(); break;
                    case 2: MenuTecnicos(); break;
                    case 3: MenuEquipos(); break;
                    case 4: MenuSolicitudes(); break;
                    case 5: MenuRepuestos(); break;
                    case 6: MenuColas(); break;
                    case 7: MenuOrdenamientos(); break;
                    case 8: MenuBusquedas(); break;
                    case 9: MenuReportes(); break;
                    case 10: MenuDatos(); break;
                    case 11: Console.WriteLine("Saliendo del programa..."); break;
                }
            } while (opcion != 11);
        }
        private void MenuClientes()
        {
            int opcion;
            do
            {
                Encabezado("GESTIÓN DE CLIENTES");
                Console.WriteLine("1. Registrar cliente");
                Console.WriteLine("2. Consultar listado de clientes");
                Console.WriteLine("3. Consultar cliente por código");
                Console.WriteLine("4. Actualizar información de contacto");
                Console.WriteLine("0. Volver");
                opcion = LeerEntero("Seleccione una opción: ", 0, 4);
                Console.WriteLine();

                try
                {
                    switch (opcion)
                    {
                        case 1:
                            {
                                string nombre = LeerTexto("Nombre completo: ");
                                string telefono = LeerTexto("Teléfono (8 dígitos): ");
                                string correo = LeerTexto("Correo electrónico: ");
                                string tipo = LeerTexto("Tipo de cliente (Individual, Empresa, Estudiante): ");
                                Cliente c = sistema.Clientes.Registrar(nombre, telefono, correo, tipo);
                                Console.WriteLine("Cliente registrado con el código " + c.Codigo);
                                break;
                            }
                        case 2:
                            sistema.Clientes.Listar();
                            break;
                        case 3:
                            {
                                Cliente c = sistema.Clientes.BuscarPorCodigo(LeerTexto("Código del cliente: "));
                                if (c == null)
                                {
                                    Console.WriteLine("No se encontró el cliente.");
                                }
                                else
                                {
                                    Console.WriteLine(c + " | Solicitudes: " + c.CantidadSolicitudes);
                                }
                                break;
                            }
                        case 4:
                            {
                                string codigo = LeerTexto("Código del cliente: ");
                                string telefono = LeerTexto("Nuevo teléfono (8 dígitos): ");
                                string correo = LeerTexto("Nuevo correo: ");
                                sistema.Clientes.ActualizarContacto(codigo, telefono, correo);
                                Console.WriteLine("Contacto actualizado.");
                                break;
                            }
                    }
                }
                catch (ReglaNegocioException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (opcion != 0)
                {
                    Pausa();
                }
            } while (opcion != 0);
        }

        private void MenuTecnicos()
        {
            int opcion;
            do
            {
                Encabezado("GESTIÓN DE TÉCNICOS");
                Console.WriteLine("1. Registrar técnico");
                Console.WriteLine("2. Consultar técnicos y especialidades");
                Console.WriteLine("3. Cambiar disponibilidad");
                Console.WriteLine("4. Ver distribución de calificaciones");
                Console.WriteLine("0. Volver");
                opcion = LeerEntero("Seleccione una opción: ", 0, 4);
                Console.WriteLine();

                try
                {
                    switch (opcion)
                    {
                        case 1:
                            {
                                string nombre = LeerTexto("Nombre completo: ");
                                TipoEquipo especialidad = LeerEspecialidad();
                                Tecnico t = sistema.Tecnicos.Registrar(nombre, especialidad);
                                Console.WriteLine("Técnico registrado con el código " + t.Codigo);
                                break;
                            }
                        case 2:
                            sistema.Tecnicos.Listar();
                            break;
                        case 3:
                            {
                                string codigo = LeerTexto("Código del técnico: ");
                                bool disponible = LeerSiNo("¿Está disponible?");
                                sistema.Tecnicos.CambiarDisponibilidad(codigo, disponible);
                                Console.WriteLine("Disponibilidad actualizada.");
                                break;
                            }
                        case 4:
                            sistema.Tecnicos.MostrarCalificaciones();
                            break;
                    }
                }
                catch (ReglaNegocioException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (opcion != 0)
                {
                    Pausa();
                }
            } while (opcion != 0);
        }

        private void MenuEquipos()
        {
            int opcion;
            do
            {
                Encabezado("GESTIÓN DE EQUIPOS");
                Console.WriteLine("1. Registrar computadora portátil");
                Console.WriteLine("2. Registrar computadora de escritorio");
                Console.WriteLine("3. Registrar impresora");
                Console.WriteLine("4. Consultar equipos registrados");
                Console.WriteLine("5. Ver historial de un equipo");
                Console.WriteLine("0. Volver");
                opcion = LeerEntero("Seleccione una opción: ", 0, 5);
                Console.WriteLine();

                try
                {
                    switch (opcion)
                    {
                        case 1:
                        case 2:
                        case 3:
                            {
                                Cliente propietario = sistema.Clientes.BuscarPorCodigo(LeerTexto("Código del cliente propietario: "));
                                string serie = LeerTexto("Número de serie: ");
                                string marca = LeerTexto("Marca: ");
                                string modelo = LeerTexto("Modelo: ");
                                string estado = LeerTexto("Estado físico recibido: ");
                                string accesorios = LeerTexto("Accesorios entregados (o escriba Ninguno): ");
                                Equipo e;

                                if (opcion == 1)
                                {
                                    bool cargador = LeerSiNo("¿Incluye cargador?");
                                    e = sistema.Equipos.RegistrarLaptop(propietario, serie, marca, modelo, estado, accesorios, cargador);
                                }
                                else if (opcion == 2)
                                {
                                    bool todoEnUno = LeerSiNo("¿Es todo en uno?");
                                    e = sistema.Equipos.RegistrarDesktop(propietario, serie, marca, modelo, estado, accesorios, todoEnUno);
                                }
                                else
                                {
                                    string tecnologia = LeerTexto("Tecnología de impresión (Láser, Inyección de tinta): ");
                                    e = sistema.Equipos.RegistrarImpresora(propietario, serie, marca, modelo, estado, accesorios, tecnologia);
                                }
                                Console.WriteLine("Equipo registrado con el código " + e.Codigo);
                                break;
                            }
                        case 4:
                            sistema.Equipos.Listar();
                            break;
                        case 5:
                            {
                                Equipo e = sistema.Equipos.BuscarPorCodigo(LeerTexto("Código del equipo: "));
                                if (e == null)
                                {
                                    Console.WriteLine("No se encontró el equipo.");
                                }
                                else
                                {
                                    sistema.Solicitudes.MostrarHistorialEquipo(e);
                                }
                                break;
                            }
                    }
                }
                catch (ReglaNegocioException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (opcion != 0)
                {
                    Pausa();
                }
            } while (opcion != 0);
        }

        private void MenuSolicitudes()
        {
            int opcion;
            do
            {
                Encabezado("GESTIÓN DE SOLICITUDES");
                Console.WriteLine("1. Crear solicitud");
                Console.WriteLine("2. Consultar solicitudes");
                Console.WriteLine("3. Registrar diagnóstico y acciones");
                Console.WriteLine("4. Registrar repuestos y costo de reparación");
                Console.WriteLine("5. Actualizar estado");
                Console.WriteLine("6. Consultar historial de estados");
                Console.WriteLine("7. Deshacer último cambio");
                Console.WriteLine("8. Entregar equipo");
                Console.WriteLine("9. Cancelar solicitud");
                Console.WriteLine("0. Volver");
                opcion = LeerEntero("Seleccione una opción: ", 0, 9);
                Console.WriteLine();

                try
                {
                    switch (opcion)
                    {
                        case 1:
                            {
                                string cliente = LeerTexto("Código del cliente: ");
                                string equipo = LeerTexto("Código del equipo: ");
                                string descripcion = LeerTexto("Descripción del problema: ");
                                Console.WriteLine("1. Normal");
                                Console.WriteLine("2. Urgente");
                                int p = LeerEntero("Prioridad: ", 1, 2);
                                Prioridad prioridad = (p == 2) ? Prioridad.Urgente : Prioridad.Normal;
                                Solicitud s = sistema.CrearSolicitud(cliente, equipo, descripcion, prioridad);
                                Console.WriteLine("Solicitud " + s.Codigo + " creada y colocada en la cola " + (prioridad == Prioridad.Urgente ? "urgente." : "normal."));
                                break;
                            }
                        case 2:
                            {
                                sistema.Solicitudes.Listar();
                                string codigo = LeerTexto("Código para ver el detalle (Enter para omitir): ");
                                if (codigo != "")
                                {
                                    Solicitud s = sistema.Solicitudes.BuscarPorCodigo(codigo);
                                    Console.WriteLine();
                                    if (s == null)
                                    {
                                        Console.WriteLine("No se encontró la solicitud.");
                                    }
                                    else
                                    {
                                        sistema.Solicitudes.ActualizarTotales();
                                        s.MostrarDetalle();
                                    }
                                }
                                break;
                            }
                        case 3:
                            {
                                string codigo = LeerTexto("Código de la solicitud: ");
                                string descripcion = LeerTexto("Descripción del diagnóstico: ");
                                int horas = LeerEntero("Tiempo estimado de reparación (horas): ", 0, 500);
                                List<string> acciones = new List<string>();
                                string accion;
                                do
                                {
                                    accion = LeerTexto("Acción realizada (Enter para terminar): ");
                                    if (accion != "")
                                    {
                                        acciones.Add(accion);
                                    }
                                } while (accion != "");
                                sistema.Solicitudes.RegistrarDiagnostico(codigo, descripcion, horas, acciones);
                                Console.WriteLine("Diagnóstico registrado.");
                                break;
                            }
                        case 4:
                            {
                                string codigo = LeerTexto("Código de la solicitud: ");
                                decimal mano = LeerDecimal("Costo de mano de obra (Q): ");
                                sistema.Solicitudes.RegistrarManoObra(codigo, mano);

                                string codigoRepuesto;
                                do
                                {
                                    codigoRepuesto = LeerTexto("Código del repuesto utilizado (Enter para terminar): ");
                                    if (codigoRepuesto != "")
                                    {
                                        Repuesto r = sistema.Repuestos.BuscarPorCodigo(codigoRepuesto);
                                        if (r == null)
                                        {
                                            Console.WriteLine("No existe ese repuesto.");
                                        }
                                        else
                                        {
                                            int cantidad = LeerEntero("Cantidad utilizada: ", 1, 10000);
                                            try
                                            {
                                                sistema.Solicitudes.AgregarRepuesto(codigo, r, cantidad);
                                                Console.WriteLine("Repuesto agregado.");
                                                string alerta = sistema.Repuestos.Alerta(r);
                                                if (alerta != "")
                                                {
                                                    Console.WriteLine(alerta);
                                                }
                                            }
                                            catch (ReglaNegocioException ex)
                                            {
                                                Console.WriteLine("Error: " + ex.Message);
                                            }
                                        }
                                    }
                                } while (codigoRepuesto != "");

                                Solicitud s = sistema.Solicitudes.BuscarPorCodigo(codigo);
                                Console.WriteLine("Total actual de la solicitud: Q" + s.CalcularTotal().ToString("N2"));
                                break;
                            }
                        case 5:
                            {
                                string codigo = LeerTexto("Código de la solicitud: ");
                                Console.WriteLine("1. En diagnóstico");
                                Console.WriteLine("2. En reparación");
                                Console.WriteLine("3. Lista para entregar");
                                int op = LeerEntero("Nuevo estado: ", 1, 3);
                                EstadoSolicitud nuevo = EstadoSolicitud.EnDiagnostico;
                                if (op == 2) nuevo = EstadoSolicitud.EnReparacion;
                                if (op == 3) nuevo = EstadoSolicitud.ListaParaEntregar;
                                string observacion = LeerTexto("Observación (opcional): ");
                                sistema.Solicitudes.CambiarEstado(codigo, nuevo, observacion);
                                Console.WriteLine("Estado actualizado a " + nuevo + ".");
                                break;
                            }
                        case 6:
                            {
                                Solicitud s = sistema.Solicitudes.BuscarPorCodigo(LeerTexto("Código de la solicitud: "));
                                if (s == null)
                                {
                                    Console.WriteLine("No se encontró la solicitud.");
                                }
                                else
                                {
                                    Console.WriteLine("Estado actual: " + s.Estado);
                                    Console.WriteLine("Historial (del más reciente al más antiguo):");
                                    s.ConsultarHistorial();
                                }
                                break;
                            }
                        case 7:
                            {
                                EstadoSolicitud restaurado = sistema.Solicitudes.Deshacer(LeerTexto("Código de la solicitud: "));
                                Console.WriteLine("Cambio deshecho. Estado actual: " + restaurado);
                                break;
                            }
                        case 8:
                            {
                                string codigo = LeerTexto("Código de la solicitud: ");
                                int calificacion = LeerEntero("Calificación del servicio (1 a 5): ", 1, 5);
                                decimal total = sistema.Solicitudes.Entregar(codigo, calificacion);
                                Console.WriteLine("Equipo entregado. Total a pagar: Q" + total.ToString("N2"));
                                break;
                            }
                        case 9:
                            {
                                sistema.Solicitudes.Cancelar(LeerTexto("Código de la solicitud: "));
                                Console.WriteLine("Solicitud cancelada.");
                                break;
                            }
                    }
                }
                catch (ReglaNegocioException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (opcion != 0)
                {
                    Pausa();
                }
            } while (opcion != 0);
        }

        private void MenuRepuestos()
        {
            int opcion;
            do
            {
                Encabezado("GESTIÓN DE REPUESTOS");
                Console.WriteLine("1. Registrar repuesto");
                Console.WriteLine("2. Consultar inventario");
                Console.WriteLine("3. Reponer existencias");
                Console.WriteLine("4. Ver repuestos con existencia mínima o agotados");
                Console.WriteLine("0. Volver");
                opcion = LeerEntero("Seleccione una opción: ", 0, 4);
                Console.WriteLine();

                try
                {
                    switch (opcion)
                    {
                        case 1:
                            {
                                string nombre = LeerTexto("Nombre: ");
                                string categoria = LeerTexto("Categoría: ");
                                int existencia = LeerEntero("Existencia: ", 0, 100000);
                                int minima = LeerEntero("Existencia mínima: ", 0, 100000);
                                decimal costo = LeerDecimal("Costo unitario (Q): ");
                                decimal precio = LeerDecimal("Precio de venta (Q): ");
                                Repuesto r = sistema.Repuestos.Registrar(nombre, categoria, existencia, minima, costo, precio);
                                Console.WriteLine("Repuesto registrado con el código " + r.Codigo);
                                break;
                            }
                        case 2:
                            sistema.Repuestos.Listar();
                            break;
                        case 3:
                            {
                                string codigo = LeerTexto("Código del repuesto: ");
                                int cantidad = LeerEntero("Cantidad a reponer: ", 1, 100000);
                                sistema.Repuestos.Reponer(codigo, cantidad);
                                Console.WriteLine("Existencias actualizadas.");
                                break;
                            }
                        case 4:
                            sistema.Repuestos.MostrarBajoMinimo();
                            break;
                    }
                }
                catch (ReglaNegocioException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (opcion != 0)
                {
                    Pausa();
                }
            } while (opcion != 0);
        }


        private void MenuColas()
        {
            int opcion;
            do
            {
                Encabezado("ATENCIÓN DE COLAS");
                Console.WriteLine("1. Ver cola de solicitudes urgentes y normales");
                Console.WriteLine("2. Atender siguiente solicitud");
                Console.WriteLine("3. Mostrar matriz de carga semanal");
                Console.WriteLine("0. Volver");
                opcion = LeerEntero("Seleccione una opción: ", 0, 3);
                Console.WriteLine();

                try
                {
                    switch (opcion)
                    {
                        case 1:
                            sistema.Colas.MostrarColas();
                            break;
                        case 2:
                            Console.WriteLine(sistema.AtenderSiguiente());
                            break;
                        case 3:
                            reportes.Reporte12();
                            break;
                    }
                }
                catch (ReglaNegocioException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (opcion != 0)
                {
                    Pausa();
                }
            } while (opcion != 0);
        }


        private void MenuOrdenamientos()
        {
            int opcion;
            do
            {
                Encabezado("ORDENAMIENTOS");
                Console.WriteLine("1. Bubble Sort: solicitudes por costo total");
                Console.WriteLine("2. Selection Sort: técnicos por servicios finalizados");
                Console.WriteLine("3. Shell Sort: repuestos por existencia");
                Console.WriteLine("4. Quick Sort: solicitudes por fecha, tiempo estimado o código");
                Console.WriteLine("0. Volver");
                opcion = LeerEntero("Seleccione una opción: ", 0, 4);
                Console.WriteLine();

                try
                {
                    switch (opcion)
                    {
                        case 1:
                            reportes.OrdenarSolicitudesPorCosto(LeerAscendente());
                            break;
                        case 2:
                            reportes.OrdenarTecnicosPorServicios(LeerAscendente());
                            break;
                        case 3:
                            reportes.OrdenarRepuestosPorExistencia(LeerAscendente());
                            break;
                        case 4:
                            {
                                Console.WriteLine("1. Fecha de recepción");
                                Console.WriteLine("2. Tiempo estimado");
                                Console.WriteLine("3. Código");
                                int c = LeerEntero("Criterio: ", 1, 3);
                                string criterio = "Fecha";
                                if (c == 2) criterio = "Tiempo";
                                if (c == 3) criterio = "Codigo";
                                bool asc = LeerAscendente();
                                Console.WriteLine();
                                reportes.OrdenarSolicitudesPorFechaOTiempo(criterio, asc);
                                break;
                            }
                    }
                }
                catch (ReglaNegocioException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (opcion != 0)
                {
                    Pausa();
                }
            } while (opcion != 0);
        }


        private void MenuBusquedas()
        {
            int opcion;
            do
            {
                Encabezado("BÚSQUEDAS");
                Console.WriteLine("1. Buscar clientes por nombre parcial");
                Console.WriteLine("2. Buscar solicitudes por estado");
                Console.WriteLine("3. Buscar equipos por marca");
                Console.WriteLine("4. Buscar solicitud por código (búsqueda binaria)");
                Console.WriteLine("0. Volver");
                opcion = LeerEntero("Seleccione una opción: ", 0, 4);
                Console.WriteLine();

                try
                {
                    switch (opcion)
                    {
                        case 1:
                            reportes.BuscarClientes(LeerTexto("Nombre o parte del nombre: "));
                            break;
                        case 2:
                            reportes.BuscarSolicitudesPorEstado(LeerEstado());
                            break;
                        case 3:
                            reportes.BuscarEquiposPorMarca(LeerTexto("Marca: "));
                            break;
                        case 4:
                            reportes.BuscarSolicitudPorCodigo(LeerTexto("Código de la solicitud (ejemplo SOL-0001): "));
                            break;
                    }
                }
                catch (ReglaNegocioException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (opcion != 0)
                {
                    Pausa();
                }
            } while (opcion != 0);
        }


        private void MenuReportes()
        {
            int opcion;
            do
            {
                Encabezado("REPORTES");
                Console.WriteLine("1. Solicitudes en espera por prioridad");
                Console.WriteLine("2. Solicitudes activas, finalizadas y canceladas");
                Console.WriteLine("3. Técnicos disponibles y con carga completa");
                Console.WriteLine("4. Técnico con más servicios finalizados");
                Console.WriteLine("5. Equipos atendidos por tipo");
                Console.WriteLine("6. Solicitudes ordenadas por costo total");
                Console.WriteLine("7. Solicitudes ordenadas por fecha o tiempo estimado");
                Console.WriteLine("8. Repuestos ordenados por existencia");
                Console.WriteLine("9. Repuestos con existencia mínima o agotados");
                Console.WriteLine("10. Ingresos totales por servicios finalizados");
                Console.WriteLine("11. Resultados de las búsquedas");
                Console.WriteLine("12. Matriz semanal de carga de trabajo");
                Console.WriteLine("0. Volver");
                opcion = LeerEntero("Seleccione una opción: ", 0, 12);
                Console.WriteLine();

                try
                {
                    switch (opcion)
                    {
                        case 1: reportes.Reporte1(); break;
                        case 2: reportes.Reporte2(); break;
                        case 3: reportes.Reporte3(); break;
                        case 4: reportes.Reporte4(); break;
                        case 5: reportes.Reporte5(); break;
                        case 6: reportes.OrdenarSolicitudesPorCosto(LeerAscendente()); break;
                        case 7:
                            {
                                Console.WriteLine("1. Fecha de recepción");
                                Console.WriteLine("2. Tiempo estimado");
                                string criterio = (LeerEntero("Criterio: ", 1, 2) == 1) ? "Fecha" : "Tiempo";
                                bool asc = LeerAscendente();
                                Console.WriteLine();
                                reportes.OrdenarSolicitudesPorFechaOTiempo(criterio, asc);
                                break;
                            }
                        case 8: reportes.OrdenarRepuestosPorExistencia(LeerAscendente()); break;
                        case 9: reportes.Reporte9(); break;
                        case 10: reportes.Reporte10(); break;
                        case 11:
                            {
                                string nombre = LeerTexto("Nombre parcial del cliente: ");
                                Console.WriteLine("Estado de las solicitudes a buscar:");
                                EstadoSolicitud estado = LeerEstado();
                                string marca = LeerTexto("Marca del equipo: ");
                                string codigo = LeerTexto("Código de solicitud (búsqueda binaria): ");
                                Console.WriteLine();
                                reportes.Reporte11(nombre, estado, marca, codigo);
                                break;
                            }
                        case 12: reportes.Reporte12(); break;
                    }
                }
                catch (ReglaNegocioException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (opcion != 0)
                {
                    Pausa();
                }
            } while (opcion != 0);
        }

        private void MenuDatos()
        {
            int opcion;
            do
            {
                Encabezado("GENERACIÓN DE DATOS");
                Console.WriteLine("Seleccione la cantidad de datos que desea generar:");
                Console.WriteLine("1. Cantidad pequeña");
                Console.WriteLine("2. Cantidad mediana");
                Console.WriteLine("3. Cantidad grande");
                Console.WriteLine("0. Volver");
                opcion = LeerEntero("Seleccione una opción: ", 0, 3);
                Console.WriteLine();

                try
                {
                    if (opcion != 0)
                    {
                        Console.WriteLine(GeneradorDatos.Generar(sistema, opcion));
                    }
                }
                catch (ReglaNegocioException ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                if (opcion != 0)
                {
                    Pausa();
                }
            } while (opcion != 0);
        }
    }
}
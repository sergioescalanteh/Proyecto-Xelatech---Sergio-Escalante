using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public static class GeneradorDatos
    {
        private static Random random = new Random();

        private static string[] nombres = { "Ana", "Luis", "María", "Carlos", "Sofía", "Pedro", "Lucía", "Jorge", "Elena", "Diego" };
        private static string[] apellidos = { "López", "Pérez", "García", "Ajú", "Castillo", "Morales", "Ixcot", "Tzoc", "Méndez", "Cifuentes" };
        private static string[] tipos = { "Individual", "Empresa", "Estudiante" };
        private static string[] marcas = { "HP", "Dell", "Lenovo", "Acer", "Asus", "Epson", "Canon", "Brother" };
        private static string[] modelos = { "Serie A", "Serie B", "Pro 15", "Basic 200", "Plus X", "Elite 7" };
        private static string[] problemas = { "No enciende", "Funciona lento", "Pantalla dañada", "No imprime", "Se sobrecalienta", "Ruido extraño", "No carga" };
        private static string[] repuestosNombres = { "Batería", "Pantalla", "Teclado", "Disco SSD", "Memoria RAM", "Cargador", "Cabezal", "Tóner", "Fuente de poder", "Ventilador" };
        private static string[] categorias = { "Energía", "Almacenamiento", "Pantallas", "Impresión", "Enfriamiento" };

        private static string NombreAleatorio()
        {
            return nombres[random.Next(nombres.Length)] + " " + apellidos[random.Next(apellidos.Length)];
        }

        public static string Generar(SistemaXelaTech sistema, int nivel)
        {
            int cantClientes = 5, cantTecnicos = 3, cantRepuestos = 6, cantEquipos = 6;
            if (nivel == 2) { cantClientes = 10; cantTecnicos = 5; cantRepuestos = 10; cantEquipos = 12; }
            if (nivel == 3) { cantClientes = 20; cantTecnicos = 8; cantRepuestos = 15; cantEquipos = 25; }

            for (int i = 0; i < cantClientes; i++)
            {
                string nombre = NombreAleatorio();
                string telefono = "5" + random.Next(1000000, 9999999).ToString();
                string correo = nombre.Replace(" ", "").ToLower() + (sistema.Clientes.ObtenerLista().Count + 1) + "@correo.com";
                sistema.Clientes.Registrar(nombre, telefono, correo, tipos[random.Next(tipos.Length)]);
            }

            for (int i = 0; i < cantTecnicos; i++)
            {
                TipoEquipo especialidad = (TipoEquipo)(i % 3);
                sistema.Tecnicos.Registrar(NombreAleatorio(), especialidad);
            }

            for (int i = 0; i < cantRepuestos; i++)
            {
                string nombre = repuestosNombres[i % repuestosNombres.Length];
                int existencia = random.Next(2, 20);
                int minima = random.Next(1, 5);
                decimal costo = random.Next(20, 200);
                decimal precio = Math.Round(costo * 1.3m, 2);
                sistema.Repuestos.Registrar(nombre, categorias[random.Next(categorias.Length)], existencia, minima, costo, precio);
            }

            List<Cliente> clientes = sistema.Clientes.ObtenerLista();
            int equiposCreados = 0;
            for (int i = 0; i < cantEquipos; i++)
            {
                Cliente dueño = clientes[random.Next(clientes.Count)];
                string serie = "SN" + (100000 + sistema.Equipos.ObtenerLista().Count + 1);
                string marca = marcas[random.Next(marcas.Length)];
                string modelo = modelos[random.Next(modelos.Length)];
                int tipo = random.Next(3);

                if (tipo == 0)
                {
                    sistema.Equipos.RegistrarLaptop(dueño, serie, marca, modelo, "Buen estado", "Ninguno", random.Next(2) == 0);
                }
                else if (tipo == 1)
                {
                    sistema.Equipos.RegistrarDesktop(dueño, serie, marca, modelo, "Buen estado", "Cable de poder", random.Next(2) == 0);
                }
                else
                {
                    sistema.Equipos.RegistrarImpresora(dueño, serie, marca, modelo, "Buen estado", "Cable USB", random.Next(2) == 0 ? "Inyección de tinta" : "Láser");
                }
                equiposCreados++;
            }

            List<Equipo> equipos = sistema.Equipos.ObtenerLista();
            int solicitudesCreadas = 0;
            for (int i = equipos.Count - equiposCreados; i < equipos.Count; i++)
            {
                if (random.Next(100) < 85)
                {
                    Equipo e = equipos[i];
                    Prioridad prioridad = random.Next(100) < 30 ? Prioridad.Urgente : Prioridad.Normal;
                    DateTime fecha = DateTime.Now.AddDays(-random.Next(0, 7)).AddHours(-random.Next(0, 24));
                    sistema.CrearSolicitud(e.Propietario.Codigo, e.Codigo, problemas[random.Next(problemas.Length)], prioridad, fecha);
                    solicitudesCreadas++;
                }
            }

            int aAtender = solicitudesCreadas * 2 / 3;
            for (int i = 0; i < aAtender; i++)
            {
                sistema.AtenderSiguiente(DateTime.Now.AddDays(-random.Next(0, 7)));
            }

            foreach (Solicitud s in sistema.Solicitudes.ObtenerLista())
            {
                if (s.Estado == EstadoSolicitud.Asignada)
                {
                    Avanzar(sistema, s, random.Next(0, 5));
                }
            }
            sistema.Solicitudes.ActualizarTotales();

            return "Datos generados: " + cantClientes + " clientes, " + cantTecnicos + " técnicos, " + cantRepuestos + " repuestos, " + equiposCreados + " equipos y " + solicitudesCreadas + " solicitudes.";
        }

        private static void Avanzar(SistemaXelaTech sistema, Solicitud s, int etapas)
        {
            try
            {
                if (etapas >= 1)
                {
                    s.CambiarEstado(EstadoSolicitud.EnDiagnostico, "Diagnóstico iniciado");
                    Diagnostico d = new Diagnostico("DIA-" + s.Codigo.Substring(4), problemas[random.Next(problemas.Length)], random.Next(1, 6));
                    d.AgregarAccion("Revisión general del equipo");
                    s.RegistrarDiagnostico(d);
                }
                if (etapas >= 2)
                {
                    s.CambiarEstado(EstadoSolicitud.EnReparacion, "Reparación iniciada");
                    s.CostoManoObra = random.Next(50, 300);
                    List<Repuesto> repuestos = sistema.Repuestos.ObtenerLista();
                    Repuesto r = repuestos[random.Next(repuestos.Count)];
                    if (r.Existencia > 0)
                    {
                        s.AgregarRepuesto(r, random.Next(1, Math.Min(3, r.Existencia) + 1));
                    }
                }
                if (etapas >= 3)
                {
                    s.CambiarEstado(EstadoSolicitud.ListaParaEntregar, "Reparación terminada");
                }
                if (etapas >= 4)
                {
                    sistema.Solicitudes.Entregar(s.Codigo, random.Next(1, 6));
                }
            }
            catch (ReglaNegocioException)
            {
            }
        }
    }
}

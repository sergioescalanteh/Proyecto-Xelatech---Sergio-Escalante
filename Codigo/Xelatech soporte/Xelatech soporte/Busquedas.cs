using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Xelatech_soporte
{
    public static class Busquedas
    {
        public static List<Cliente> SecuencialClientes(List<Cliente> clientes, string texto)
        {
            Validador.TextoObligatorio(texto, "texto a buscar");
            string buscado = texto.Trim().ToLower();
            List<Cliente> resultados = new List<Cliente>();

            for (int i = 0; i < clientes.Count; i++)
            {
                if (clientes[i].Nombre.ToLower().Contains(buscado))
                {
                    resultados.Add(clientes[i]);
                }
            }
            return resultados;
        }

        public static List<Solicitud> SecuencialSolicitudes(List<Solicitud> solicitudes, EstadoSolicitud estado)
        {
            List<Solicitud> resultados = new List<Solicitud>();

            for (int i = 0; i < solicitudes.Count; i++)
            {
                if (solicitudes[i].Estado == estado)
                {
                    resultados.Add(solicitudes[i]);
                }
            }
            return resultados;
        }

        public static List<Equipo> SecuencialEquipos(List<Equipo> equipos, string marca)
        {
            Validador.TextoObligatorio(marca, "marca");
            string buscada = marca.Trim().ToLower();
            List<Equipo> resultados = new List<Equipo>();

            for (int i = 0; i < equipos.Count; i++)
            {
                if (equipos[i].Marca.ToLower() == buscada)
                {
                    resultados.Add(equipos[i]);
                }
            }
            return resultados;
        }

        public static bool EstaOrdenadaPorCodigo(List<Solicitud> solicitudes)
        {
            for (int i = 0; i < solicitudes.Count - 1; i++)
            {
                if (string.CompareOrdinal(solicitudes[i].Codigo, solicitudes[i + 1].Codigo) > 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static Solicitud BinariaSolicitud(List<Solicitud> solicitudes, string codigoBuscado)
        {
            if (solicitudes.Count == 0)
            {
                throw new ReglaNegocioException("No hay solicitudes registradas.");
            }
            if (!EstaOrdenadaPorCodigo(solicitudes))
            {
                throw new ReglaNegocioException("Ordene las solicitudes por código antes de buscar.");
            }

            int bajo = 0;
            int alto = solicitudes.Count - 1;

            while (bajo <= alto)
            {
                int medio = bajo + (alto - bajo) / 2;
                int comparacion = string.CompareOrdinal(solicitudes[medio].Codigo, codigoBuscado);

                if (comparacion == 0)
                {
                    return solicitudes[medio];
                }
                else if (comparacion < 0)
                {
                    bajo = medio + 1;
                }
                else
                {
                    alto = medio - 1;
                }
            }
            return null;
        }
    }
}
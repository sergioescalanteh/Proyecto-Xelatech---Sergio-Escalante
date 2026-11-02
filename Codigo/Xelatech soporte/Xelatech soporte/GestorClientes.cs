using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public class GestorClientes
    {
        private List<Cliente> clientes = new List<Cliente>();

        public List<Cliente> ObtenerLista()
        {
            return clientes;
        }

        public Cliente Registrar(string nombre, string telefono, string correo, string tipo)
        {
            Validador.TextoObligatorio(nombre, "nombre");
            Validador.TelefonoValido(telefono);
            Validador.CorreoValido(correo);
            Validador.TextoObligatorio(tipo, "tipo de cliente");

            string codigo = "CLI-" + (clientes.Count + 1).ToString("0000");
            Cliente c = new Cliente(codigo, nombre, telefono, correo, tipo);
            clientes.Add(c);
            return c;
        }

        public void Listar()
        {
            if (clientes.Count == 0)
            {
                Console.WriteLine("No hay clientes registrados.");
                return;
            }
            foreach (Cliente c in clientes)
            {
                Console.WriteLine(c + " | Solicitudes: " + c.CantidadSolicitudes);
            }
        }

        public Cliente BuscarPorCodigo(string codigo)
        {
            foreach (Cliente c in clientes)
            {
                if (c.Codigo == codigo)
                {
                    return c;
                }
            }
            return null;
        }

        public void ActualizarContacto(string codigo, string telefono, string correo)
        {
            Cliente c = BuscarPorCodigo(codigo);
            if (c == null)
            {
                throw new ReglaNegocioException("No existe un cliente con ese código.");
            }
            Validador.TelefonoValido(telefono);
            Validador.CorreoValido(correo);
            c.ActualizarContacto(telefono, correo);
        }
    }
}

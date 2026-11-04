using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public class Cliente
    {
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public string Telefono { get; private set; }
        public string Correo { get; private set; }
        public string Tipo { get; private set; }
        public int CantidadSolicitudes { get; private set; }

        public Cliente(string codigo, string nombre, string telefono, string correo, string tipo)
        {
            Codigo = codigo;
            Nombre = nombre;
            Telefono = telefono;
            Correo = correo;
            Tipo = tipo;
            CantidadSolicitudes = 0;
        }

        public void ActualizarContacto(string telefono, string correo)
        {
            Telefono = telefono;
            Correo = correo;
        }

        public void SumarSolicitud()
        {
            CantidadSolicitudes++;
        }

        public bool TieneDescuento()
        {
            return CantidadSolicitudes >= 5;
        }

        public override string ToString()
        {
            return Codigo + " | " + Nombre + " | " + Telefono + " | " + Correo + " | " + Tipo;
        }
    }
}

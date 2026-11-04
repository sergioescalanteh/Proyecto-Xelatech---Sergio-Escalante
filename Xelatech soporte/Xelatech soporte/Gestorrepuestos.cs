using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Xelatech_soporte
{
    public class GestorRepuestos
    {
        private List<Repuesto> repuestos = new List<Repuesto>();

        public List<Repuesto> ObtenerLista()
        {
            return repuestos;
        }

        public Repuesto Registrar(string nombre, string categoria, int existencia, int existenciaMinima, decimal costoUnitario, decimal precioVenta)
        {
            Validador.TextoObligatorio(nombre, "nombre del repuesto");
            Validador.TextoObligatorio(categoria, "categoría");
            Validador.NoNegativo(existencia, "existencia");
            Validador.NoNegativo(existenciaMinima, "existencia mínima");
            Validador.NoNegativo(costoUnitario, "costo unitario");
            Validador.NoNegativo(precioVenta, "precio de venta");

            string codigo = "REP-" + (repuestos.Count + 1).ToString("0000");
            Repuesto r = new Repuesto(codigo, nombre, categoria, existencia, existenciaMinima, costoUnitario, precioVenta);
            repuestos.Add(r);
            return r;
        }

        public Repuesto BuscarPorCodigo(string codigo)
        {
            foreach (Repuesto r in repuestos)
            {
                if (r.Codigo == codigo)
                {
                    return r;
                }
            }
            return null;
        }

        public void Reponer(string codigo, int cantidad)
        {
            Repuesto r = BuscarPorCodigo(codigo);
            if (r == null)
            {
                throw new ReglaNegocioException("No existe un repuesto con ese código.");
            }
            r.Reponer(cantidad);
        }

        public void Listar()
        {
            if (repuestos.Count == 0)
            {
                Console.WriteLine("No hay repuestos registrados.");
                return;
            }
            foreach (Repuesto r in repuestos)
            {
                Console.WriteLine(r);
            }
        }

        public List<Repuesto> ObtenerBajoMinimo()
        {
            List<Repuesto> resultado = new List<Repuesto>();
            foreach (Repuesto r in repuestos)
            {
                if (r.EstaBajoMinimo())
                {
                    resultado.Add(r);
                }
            }
            return resultado;
        }

        public void MostrarBajoMinimo()
        {
            List<Repuesto> lista = ObtenerBajoMinimo();
            if (lista.Count == 0)
            {
                Console.WriteLine("No hay repuestos en existencia mínima ni agotados.");
                return;
            }
            foreach (Repuesto r in lista)
            {
                Console.WriteLine(r + " | " + (r.EstaAgotado() ? "AGOTADO" : "EXISTENCIA MÍNIMA"));
            }
        }

        public string Alerta(Repuesto r)
        {
            if (r.EstaAgotado())
            {
                return "ALERTA: el repuesto " + r.Nombre + " se agotó.";
            }
            if (r.EstaBajoMinimo())
            {
                return "ALERTA: el repuesto " + r.Nombre + " llegó a su existencia mínima (" + r.Existencia + ").";
            }
            return "";
        }
    }
}
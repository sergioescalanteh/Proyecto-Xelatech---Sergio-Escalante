using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public class Repuesto
    {
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public string Categoria { get; private set; }
        public int Existencia { get; private set; }
        public int ExistenciaMinima { get; private set; }
        public decimal CostoUnitario { get; private set; }
        public decimal PrecioVenta { get; private set; }
        public int CantidadUtilizada { get; private set; }

        public Repuesto(string codigo, string nombre, string categoria, int existencia, int existenciaMinima, decimal costoUnitario, decimal precioVenta)
        {
            if (existencia < 0 || existenciaMinima < 0 || costoUnitario < 0 || precioVenta < 0)
            {
                throw new ReglaNegocioException("Las existencias, costos y precios no pueden ser negativos.");
            }
            Codigo = codigo;
            Nombre = nombre;
            Categoria = categoria;
            Existencia = existencia;
            ExistenciaMinima = existenciaMinima;
            CostoUnitario = costoUnitario;
            PrecioVenta = precioVenta;
            CantidadUtilizada = 0;
        }

        public void Descontar(int cantidad)
        {
            if (cantidad <= 0)
            {
                throw new ReglaNegocioException("La cantidad debe ser mayor a cero.");
            }
            if (cantidad > Existencia)
            {
                throw new ReglaNegocioException("No hay suficiente existencia de " + Nombre + ". Disponible: " + Existencia);
            }
            Existencia -= cantidad;
            CantidadUtilizada += cantidad;
        }

        public void Reponer(int cantidad)
        {
            if (cantidad <= 0)
            {
                throw new ReglaNegocioException("La cantidad a reponer debe ser mayor a cero.");
            }
            Existencia += cantidad;
        }

        public bool EstaBajoMinimo()
        {
            return Existencia <= ExistenciaMinima;
        }

        public bool EstaAgotado()
        {
            return Existencia == 0;
        }

        public override string ToString()
        {
            return Codigo + " | " + Nombre + " | " + Categoria + " | Existencia: " + Existencia + " (mínimo " + ExistenciaMinima + ") | Q" + PrecioVenta.ToString("N2");
        }
    }

    public class RepuestoUsado
    {
        public Repuesto Repuesto { get; private set; }
        public int Cantidad { get; private set; }

        public RepuestoUsado(Repuesto repuesto, int cantidad)
        {
            Repuesto = repuesto;
            Cantidad = cantidad;
        }

        public decimal Subtotal()
        {
            return Repuesto.PrecioVenta * Cantidad;
        }

        public override string ToString()
        {
            return Repuesto.Nombre + " x" + Cantidad + " = Q" + Subtotal().ToString("N2");
        }
    }
}

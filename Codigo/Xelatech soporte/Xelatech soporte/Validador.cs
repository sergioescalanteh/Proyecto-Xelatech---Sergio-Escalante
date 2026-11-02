using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public static class Validador
    {
        public static void TextoObligatorio(string valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                throw new ReglaNegocioException("El campo " + campo + " es obligatorio.");
            }
        }

        public static void NoNegativo(decimal valor, string campo)
        {
            if (valor < 0)
            {
                throw new ReglaNegocioException("El valor de " + campo + " no puede ser negativo.");
            }
        }

        public static void TelefonoValido(string telefono)
        {
            TextoObligatorio(telefono, "teléfono");
            if (telefono.Length != 8)
            {
                throw new ReglaNegocioException("El teléfono debe tener 8 dígitos.");
            }
            foreach (char c in telefono)
            {
                if (!char.IsDigit(c))
                {
                    throw new ReglaNegocioException("El teléfono solo puede contener números.");
                }
            }
        }

        public static void CorreoValido(string correo)
        {
            TextoObligatorio(correo, "correo electrónico");
            int arroba = correo.IndexOf('@');
            if (arroba < 1 || correo.Contains(" ") || correo.IndexOf('.', arroba) < arroba + 2 || correo.EndsWith("."))
            {
                throw new ReglaNegocioException("El correo electrónico no tiene un formato válido.");
            }
        }
    }
}
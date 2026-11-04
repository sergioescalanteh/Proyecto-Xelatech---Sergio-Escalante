using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public class GestorEquipos
    {
        private List<Equipo> equipos = new List<Equipo>();

        public List<Equipo> ObtenerLista()
        {
            return equipos;
        }

        private string SiguienteCodigo()
        {
            return "EQU-" + (equipos.Count + 1).ToString("0000");
        }

        public bool ExisteSerie(string serie)
        {
            foreach (Equipo e in equipos)
            {
                if (e.Serie.Trim().ToLower() == serie.Trim().ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        private void ValidarComunes(Cliente propietario, string serie, string marca, string modelo, string estadoFisico, string accesorios)
        {
            if (propietario == null)
            {
                throw new ReglaNegocioException("El cliente propietario no existe.");
            }
            Validador.TextoObligatorio(serie, "número de serie");
            Validador.TextoObligatorio(marca, "marca");
            Validador.TextoObligatorio(modelo, "modelo");
            Validador.TextoObligatorio(estadoFisico, "estado físico");
            Validador.TextoObligatorio(accesorios, "accesorios entregados");
            if (ExisteSerie(serie))
            {
                throw new ReglaNegocioException("Ya existe un equipo con ese número de serie.");
            }
        }

        public Equipo RegistrarLaptop(Cliente propietario, string serie, string marca, string modelo, string estadoFisico, string accesorios, bool tieneCargador)
        {
            ValidarComunes(propietario, serie, marca, modelo, estadoFisico, accesorios);
            Equipo e = new Laptop(SiguienteCodigo(), serie, marca, modelo, propietario, estadoFisico, accesorios, tieneCargador);
            equipos.Add(e);
            return e;
        }

        public Equipo RegistrarDesktop(Cliente propietario, string serie, string marca, string modelo, string estadoFisico, string accesorios, bool esTodoEnUno)
        {
            ValidarComunes(propietario, serie, marca, modelo, estadoFisico, accesorios);
            Equipo e = new Desktop(SiguienteCodigo(), serie, marca, modelo, propietario, estadoFisico, accesorios, esTodoEnUno);
            equipos.Add(e);
            return e;
        }

        public Equipo RegistrarImpresora(Cliente propietario, string serie, string marca, string modelo, string estadoFisico, string accesorios, string tecnologia)
        {
            ValidarComunes(propietario, serie, marca, modelo, estadoFisico, accesorios);
            Validador.TextoObligatorio(tecnologia, "tecnología de impresión");
            Equipo e = new Impresora(SiguienteCodigo(), serie, marca, modelo, propietario, estadoFisico, accesorios, tecnologia);
            equipos.Add(e);
            return e;
        }

        public Equipo BuscarPorCodigo(string codigo)
        {
            foreach (Equipo e in equipos)
            {
                if (e.Codigo == codigo)
                {
                    return e;
                }
            }
            return null;
        }

        public void Listar()
        {
            if (equipos.Count == 0)
            {
                Console.WriteLine("No hay equipos registrados.");
                return;
            }
            foreach (Equipo e in equipos)
            {
                Console.WriteLine(e);
            }
        }
    }
}
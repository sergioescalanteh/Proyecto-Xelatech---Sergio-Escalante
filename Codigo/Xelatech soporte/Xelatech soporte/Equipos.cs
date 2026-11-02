using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public abstract class Equipo
    {
        protected string codigo;
        protected string serie;
        protected string marca;
        protected string modelo;
        protected Cliente propietario;
        protected string estadoFisico;
        protected string accesorios;

        public string Codigo { get { return codigo; } }
        public string Serie { get { return serie; } }
        public string Marca { get { return marca; } }
        public string Modelo { get { return modelo; } }
        public Cliente Propietario { get { return propietario; } }
        public string EstadoFisico { get { return estadoFisico; } }
        public string Accesorios { get { return accesorios; } }

        public abstract TipoEquipo Tipo { get; }

        public Equipo(string codigo, string serie, string marca, string modelo, Cliente propietario, string estadoFisico, string accesorios)
        {
            this.codigo = codigo;
            this.serie = serie;
            this.marca = marca;
            this.modelo = modelo;
            this.propietario = propietario;
            this.estadoFisico = estadoFisico;
            this.accesorios = accesorios;
        }

        public abstract decimal CalcularCostoRevision();
        public abstract string ChecklistDiagnostico();
        public abstract int TiempoEstimadoBase();

        public override string ToString()
        {
            return codigo + " | " + Tipo + " | " + marca + " " + modelo + " | Serie: " + serie + " | Dueño: " + propietario.Nombre;
        }
    }

    public class Laptop : Equipo
    {
        private bool tieneCargador;

        public bool TieneCargador { get { return tieneCargador; } }

        public override TipoEquipo Tipo
        {
            get { return TipoEquipo.Portatil; }
        }

        public Laptop(string codigo, string serie, string marca, string modelo, Cliente propietario, string estadoFisico, string accesorios, bool tieneCargador)
            : base(codigo, serie, marca, modelo, propietario, estadoFisico, accesorios)
        {
            this.tieneCargador = tieneCargador;
        }

        public override decimal CalcularCostoRevision()
        {
            decimal costo = 50m;
            if (tieneCargador)
            {
                costo += 10m;
            }
            return costo;
        }

        public override string ChecklistDiagnostico()
        {
            string lista = "Batería, pantalla, teclado, puertos";
            if (tieneCargador)
            {
                return lista + ", cargador incluido";
            }
            return lista + ", sin cargador";
        }

        public override int TiempoEstimadoBase()
        {
            return 2;
        }
    }

    public class Desktop : Equipo
    {
        private bool esTodoEnUno;

        public bool EsTodoEnUno { get { return esTodoEnUno; } }

        public override TipoEquipo Tipo
        {
            get { return TipoEquipo.Escritorio; }
        }

        public Desktop(string codigo, string serie, string marca, string modelo, Cliente propietario, string estadoFisico, string accesorios, bool esTodoEnUno)
            : base(codigo, serie, marca, modelo, propietario, estadoFisico, accesorios)
        {
            this.esTodoEnUno = esTodoEnUno;
        }

        public override decimal CalcularCostoRevision()
        {
            if (esTodoEnUno)
            {
                return 80m;
            }
            return 60m;
        }

        public override string ChecklistDiagnostico()
        {
            if (esTodoEnUno)
            {
                return "Pantalla integrada, disco, memoria, fuente, puertos";
            }
            return "Fuente de poder, disco, memoria, tarjeta madre, puertos";
        }

        public override int TiempoEstimadoBase()
        {
            if (esTodoEnUno)
            {
                return 4;
            }
            return 3;
        }
    }

    public class Impresora : Equipo
    {
        private string tecnologia;

        public string Tecnologia { get { return tecnologia; } }

        public override TipoEquipo Tipo
        {
            get { return TipoEquipo.Impresora; }
        }

        public Impresora(string codigo, string serie, string marca, string modelo, Cliente propietario, string estadoFisico, string accesorios, string tecnologia)
            : base(codigo, serie, marca, modelo, propietario, estadoFisico, accesorios)
        {
            this.tecnologia = tecnologia;
        }

        public override decimal CalcularCostoRevision()
        {
            return 40m;
        }

        public override string ChecklistDiagnostico()
        {
            return "Cabezal o tóner (" + tecnologia + "), rodillos, papel, conectividad";
        }

        public override int TiempoEstimadoBase()
        {
            return 2;
        }
    }
}
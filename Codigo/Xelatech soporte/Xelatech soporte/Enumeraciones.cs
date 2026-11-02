using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Xelatech_soporte
{
    public enum Prioridad
    {
        Normal,
        Urgente
    }

    public enum TipoEquipo
    {
        Portatil,
        Escritorio,
        Impresora
    }

    public enum EstadoSolicitud
    {
        Recibida,
        EnEspera,
        Asignada,
        EnDiagnostico,
        EnReparacion,
        ListaParaEntregar,
        Entregada,
        Cancelada
    }
}

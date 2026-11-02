using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    public static class Ordenamientos
    {
        private static void ValidarCantidad(int cantidad)
        {
            if (cantidad < 2)
            {
                throw new ReglaNegocioException("Datos insuficientes para ordenar.");
            }
        }

        public static List<Solicitud> BubbleSort(List<Solicitud> lista, bool ascendente)
        {
            ValidarCantidad(lista.Count);
            List<Solicitud> copia = new List<Solicitud>(lista);
            int n = copia.Count;

            for (int i = 0; i < n - 1; i++)
            {
                bool huboCambio = false;
                for (int j = 0; j < n - 1 - i; j++)
                {
                    bool intercambiar;
                    if (ascendente)
                    {
                        intercambiar = copia[j].Total > copia[j + 1].Total;
                    }
                    else
                    {
                        intercambiar = copia[j].Total < copia[j + 1].Total;
                    }

                    if (intercambiar)
                    {
                        Solicitud temporal = copia[j];
                        copia[j] = copia[j + 1];
                        copia[j + 1] = temporal;
                        huboCambio = true;
                    }
                }
                if (!huboCambio)
                {
                    break;
                }
            }
            return copia;
        }

        public static List<Tecnico> SelectionSort(List<Tecnico> lista, bool ascendente)
        {
            ValidarCantidad(lista.Count);
            List<Tecnico> copia = new List<Tecnico>(lista);
            int n = copia.Count;

            for (int i = 0; i < n - 1; i++)
            {
                int posicion = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (ascendente && copia[j].ServiciosFinalizados < copia[posicion].ServiciosFinalizados)
                    {
                        posicion = j;
                    }
                    else if (!ascendente && copia[j].ServiciosFinalizados > copia[posicion].ServiciosFinalizados)
                    {
                        posicion = j;
                    }
                }
                if (posicion != i)
                {
                    Tecnico temporal = copia[i];
                    copia[i] = copia[posicion];
                    copia[posicion] = temporal;
                }
            }
            return copia;
        }

        public static List<Repuesto> ShellSort(List<Repuesto> lista, bool ascendente)
        {
            ValidarCantidad(lista.Count);
            List<Repuesto> copia = new List<Repuesto>(lista);
            int n = copia.Count;
            int salto = n / 2;

            while (salto > 0)
            {
                for (int i = salto; i < n; i++)
                {
                    Repuesto temporal = copia[i];
                    int j = i;
                    while (j >= salto && DebeIrAntes(temporal, copia[j - salto], ascendente))
                    {
                        copia[j] = copia[j - salto];
                        j = j - salto;
                    }
                    copia[j] = temporal;
                }
                salto = salto / 2;
            }
            return copia;
        }

        private static bool DebeIrAntes(Repuesto a, Repuesto b, bool ascendente)
        {
            if (ascendente)
            {
                return a.Existencia < b.Existencia;
            }
            return a.Existencia > b.Existencia;
        }

        public static List<Solicitud> QuickSort(List<Solicitud> lista, string criterio, bool ascendente)
        {
            ValidarCantidad(lista.Count);
            List<Solicitud> copia = new List<Solicitud>(lista);
            QuickSortRecursivo(copia, 0, copia.Count - 1, criterio, ascendente);
            return copia;
        }

        private static void QuickSortRecursivo(List<Solicitud> lista, int bajo, int alto, string criterio, bool ascendente)
        {
            if (bajo < alto)
            {
                int pivote = Particionar(lista, bajo, alto, criterio, ascendente);
                QuickSortRecursivo(lista, bajo, pivote - 1, criterio, ascendente);
                QuickSortRecursivo(lista, pivote + 1, alto, criterio, ascendente);
            }
        }

        private static int Particionar(List<Solicitud> lista, int bajo, int alto, string criterio, bool ascendente)
        {
            Solicitud pivote = lista[alto];
            int i = bajo - 1;

            for (int j = bajo; j < alto; j++)
            {
                int comparacion = Comparar(lista[j], pivote, criterio);
                if ((ascendente && comparacion <= 0) || (!ascendente && comparacion >= 0))
                {
                    i++;
                    Intercambiar(lista, i, j);
                }
            }
            Intercambiar(lista, i + 1, alto);
            return i + 1;
        }

        private static void Intercambiar(List<Solicitud> lista, int a, int b)
        {
            Solicitud temporal = lista[a];
            lista[a] = lista[b];
            lista[b] = temporal;
        }

        public static int Comparar(Solicitud a, Solicitud b, string criterio)
        {
            if (criterio == "Fecha")
            {
                return a.FechaRecepcion.CompareTo(b.FechaRecepcion);
            }
            if (criterio == "Tiempo")
            {
                return a.TiempoEstimadoHoras.CompareTo(b.TiempoEstimadoHoras);
            }
            return string.CompareOrdinal(a.Codigo, b.Codigo);
        }
    }
}
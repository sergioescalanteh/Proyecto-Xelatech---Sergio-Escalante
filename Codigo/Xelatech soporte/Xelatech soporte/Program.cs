using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xelatech_soporte
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = new System.Text.UTF8Encoding(false);
            Menu menu = new Menu();
            menu.Iniciar();
        }
    }
}
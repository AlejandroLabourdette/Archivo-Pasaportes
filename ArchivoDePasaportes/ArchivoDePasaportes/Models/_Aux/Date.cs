using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchivoDePasaportes.Models._Aux
{
    public static class DateInSpanish
    {    
        public static string NameOfMonth(int month)
        {
            switch (month)
            {
                case 1:
                    return "Enero";
                case 2:
                    return "Febrero";
                case 3:
                    return "Marzo";
                case 4:
                    return "Abril";
                case 5:
                    return "Mayo";
                case 6:
                    return "Junio";
                case 7:
                    return "Julio";
                case 8:
                    return "Agosto";
                case 9:
                    return "Septiembre";
                case 10:
                    return "Octubre";
                case 11:
                    return "Noviembre";
                case 12:
                    return "Diciembre";
            }
            throw new Exception("El mes insertado no es correcto");
        }
        public static string ToShortDate(int day, int month, int year)
        {
            return day + "/" + month + "/" + year;
        }
        public static string ToLongDate(int day, int month, int year)
        {
            return NameOfMonth(month) + " " + day + ", " + year;
        }
    }
}

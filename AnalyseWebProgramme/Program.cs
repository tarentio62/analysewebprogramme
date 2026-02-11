using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseWebProgramme
{
    class Program
    {
        static void Main(string[] args)
        {


            List<cRencontre> mesRencontres = cRencontre.ChargerDepuisFootao("http://www.footao.tv/");


            cRencontre.InsererRencontreBdd(mesRencontres);
         
            Console.ReadKey();

        }

       
    }
}

using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AnalyseWebProgramme
{
    public class cRencontre
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string EquipeA { get; set; }
        public string EquipeB { get; set; }
        public string EquipeALogoLien { get; set; }
        public string EquipeBLogoLien { get; set; }
        public string InfoSup { get; set; }
        public string Competition { get; set; }
        public string Chaine { get; set; }




        public static List<cRencontre> ChargerDepuisFootao(string lien)
        {

            List<cRencontre> mesRencontres = new List<cRencontre>();

            DateTime dateRencontre = DateTime.Now;
            HtmlWeb htmlWeb = new HtmlWeb();



            // Creates an HtmlDocument object from an URL
            HtmlAgilityPack.HtmlDocument document = htmlWeb.Load(lien);

            // Targets a specific node
            HtmlNode someNode = document.GetElementbyId("jrs");

            // If there is no node with that Id, someNode will be null
            if (someNode != null)
            {

                foreach (HtmlNode divDay in someNode.ChildNodes)
                {

                    if (divDay.Attributes.Contains("id") && divDay.Attributes["id"].Value.Contains("j"))
                    {


                        // Extracts all links within that node
                        IEnumerable<HtmlNode> allDiv = divDay.ChildNodes;

                        try
                        {

                            dateRencontre = Convert.ToDateTime(divDay.Attributes["id"].Value.Substring(1, 2)+"/" +divDay.Attributes["id"].Value.Substring(3, 2)+"/20" + divDay.Attributes["id"].Value.Substring(5, 2));
                        }
                        catch (Exception)
                        {
                        }

                        // Outputs the href for external links
                        foreach (HtmlNode divd in allDiv)
                        {
                            if (divd.Name != "div")
                                continue;

                            cRencontre rencontre = new cRencontre();
                            rencontre.Date = dateRencontre;
                            foreach (HtmlNode div in divd.Descendants())
                            {

                            if (div.Attributes.Contains("class") && (div.Attributes["class"].Value == "dt" || div.Attributes["class"].Value == "rd") && !div.Attributes.Contains("href") && div.ParentNode.Attributes["class"].Value != "ap dtl")
                            {
                                rencontre.Date = rencontre.Date.AddHours(int.Parse(div.InnerText.Substring(0, 2)));
                                rencontre.Date = rencontre.Date.AddMinutes(int.Parse(div.InnerText.Substring(3, 2)));
                            }
                            else if (div.Attributes.Contains("src") && div.Name=="img")
                            {
                                rencontre.Chaine += div.Attributes["class"].Value.Replace("imgch ", "") + "|";
                            }
                            else if (div.Attributes.Contains("class") && div.Attributes["class"].Value == "rnc")
                            {

                                if (WebUtility.HtmlDecode(div.ChildNodes.ToList()[0].InnerText).Contains('·'))
                                {
                                    if (div.ChildNodes.ToList()[0].ChildNodes.Count > 0)
                                    {

                                    rencontre.EquipeA = WebUtility.HtmlDecode(div.ChildNodes.ToList()[0].ChildNodes.First().InnerText).Split(new char[] { '·' })[0].ToString();
                                    rencontre.EquipeB = WebUtility.HtmlDecode(div.ChildNodes.ToList()[0].ChildNodes.First().InnerText).Split(new char[] { '·' })[1].ToString();

                                    if (div.ChildNodes.ToList()[0].ChildNodes.Count > 1 && div.ChildNodes.ToList()[0].ChildNodes[1].Attributes["class"].Value == "agen")
                                    {
                                        rencontre.InfoSup = WebUtility.HtmlDecode(div.ChildNodes.ToList()[0].ChildNodes[1].InnerText.ToString());
                                    }

                                    }
                                    else
                                    {
                                        rencontre.EquipeA = WebUtility.HtmlDecode(div.ChildNodes.ToList()[0].InnerText).Split(new char[] { '·' })[0].ToString();
                                        rencontre.EquipeB = WebUtility.HtmlDecode(div.ChildNodes.ToList()[0].InnerText).Split(new char[] { '·' })[1].ToString();

                                        if (div.ChildNodes.ToList()[0].ChildNodes.Count > 1 && div.ChildNodes.ToList()[0].ChildNodes[1].Attributes["class"].Value == "agen")
                                        {
                                            rencontre.InfoSup = WebUtility.HtmlDecode(div.ChildNodes.ToList()[0].ChildNodes[1].InnerText.ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    rencontre.InfoSup = WebUtility.HtmlDecode(div.ChildNodes.ToList()[0].InnerText);

                                    if (div.ChildNodes.ToList()[0].ChildNodes.Count > 1 && div.ChildNodes.ToList()[0].ChildNodes[1].Attributes["class"].Value == "agen")
                                    {

                                        rencontre.InfoSup += " " + WebUtility.HtmlDecode(div.ChildNodes.ToList()[0].ChildNodes[1].InnerText.ToString());
                                    
                                    }

                                 

                                }

                            
                            }
                            }
                            if (((rencontre.EquipeA !="" && rencontre.EquipeB !="") || rencontre.InfoSup !="") && rencontre.EquipeA !=null)
                            mesRencontres.Add(rencontre);
                        }
                    }
                 
                }

            }
            return mesRencontres;
        }


        public static void InsererRencontreBdd(List<cRencontre> rencontres)
        {

            
            DBConnect bdd = new DBConnect();

     




              if (bdd.OpenConnection() == true)
                {
                   
            foreach (cRencontre renc in rencontres)
            {

                string query = "INSERT INTO rencontres (date,equipeA,equipeB,info,competition,chaine) VALUES(@date,@equipeA,@equipeB,@info,@competition,@chaine)";

                //open connection
                //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query,bdd.connection);

                    cmd.Parameters.Add("@date", renc.Date);
                    cmd.Parameters.Add("@equipeA", renc.EquipeA);
                    cmd.Parameters.Add("@equipeB", renc.EquipeB);
                    cmd.Parameters.Add("@info", renc.InfoSup);
                    cmd.Parameters.Add("@competition", renc.Competition);
                    cmd.Parameters.Add("@chaine", renc.Chaine);
                

                    //Execute command
                    cmd.ExecuteNonQuery();

              
                }


            }
           


        }
    }
}
        

        


using OfficeAgent.Cryption;
using OfficeAgent.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SigortaSatis.Controllers
{
    public class HomeController : Controller
    {
        public class PktList
        {
            public Guid ID { get; set; }
            public string PKTNAME { get; set; }
            public string PKTIMG { get; set; }
            public string PKTFIYAT { get; set; }
            public Guid PKTTIPI { get; set; }
        }

        public ActionResult Index()
        {
            DataSet dsPKT = new DataSet();
            //string iID = Session["iIDv"].ToString();
            using (DataVw dMan = new DataVw())
            {
                dsPKT = dMan.ExecuteView_S("PKT", "*","", "", "");
            }

            List<PktList> paketList = new List<PktList>();
            foreach (DataRow dr in dsPKT.Tables[0].Rows)
            {
                paketList.Add(new PktList
                {
                    ID = (Guid)dr["ID"],
                    PKTNAME = dr["PKTNAME"].ToString(),
                    PKTIMG = dr["PKTIMG"].ToString(),
                    PKTFIYAT = dr["PKTFIYAT"].ToString(),
                    PKTTIPI = (Guid)dr["PKTTIPI"]
                });
            }

            ViewBag.PaketList = paketList;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult PktIslem( FormCollection collection)
        {
            //4c38a353-bc02-4506-8f04-2bc71f5eb300       Ev
            //43cd5b24-535e-4447-9789-690f463de639       Araba

            string pktID= collection[0]; 
            DataSet dsPKT = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsPKT = dMan.ExecuteView_S("PKT", "PKTTIPI", pktID, "", "ID=");
            }
                              //PKTTYPID

            Session["PKTID"] = pktID;
            string pktTyp = dsPKT.Tables[0].Rows[0][0].ToString();

            if (Convert.ToBoolean(Session["IsAuthenticated"]))
            {
                //Session["USRIDv"]

                if (pktTyp == "43cd5b24-535e-4447-9789-690f463de639")
                {
                    return Redirect("/Paket/CarPkt");
                }
                else
                {
                    return Redirect("/Paket/HomePkt");
                }
            }
            else
            {
                return Redirect("/Account/Login");
            }
        }
    }
}
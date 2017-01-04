using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using OfficeAgent.Data;
using System.IO;
using OfficeAgent.Object;

namespace SigortaSatis.Controllers
{
    public class PaketController : Controller
    {
        DataSet dsPKTTYP = new DataSet();
        DataSet dsPKT = new DataSet();

        //
        // GET: /Paket/
        public ActionResult Pacekt()
        {
            using (DataVw dMan = new DataVw())
            {
                dsPKTTYP = dMan.ExecuteView_S("PKTTYPE", "*", "", "", "");
            }

            List<SelectListItem> pktTyp = new List<SelectListItem>();
            foreach (DataRow dr in dsPKTTYP.Tables[0].Rows)
            {
                pktTyp.Add(new SelectListItem { Text = Convert.ToString(dr["TPYENAME"]), Value = dr["ID"].ToString() });
            }

            ViewBag.PktTyp = pktTyp;

            return View();
        }

        [HttpPost]
        public ActionResult PktAdd(string txtPKTNAME, string txtPKTFIYAT, HttpPostedFileBase file, FormCollection collection)
        {
            string filefo = "";
            string pktTyp = collection["PktTyp"];

            using (DataVw dMan = new DataVw())
            {
                dsPKT = dMan.ExecuteView_S("PKT", "*", "", "", "");
            }

            if (txtPKTNAME.ToString() == "" || txtPKTFIYAT.ToString() == "")
            {
                Session["useraddsuccess"] = false;
                ViewBag.addmessage = "Eksik veri girişi! Tüm Alanları Doldurunuz.";
                return Redirect("/Account/Pacekt");
            }
            else
            {
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/pkt"), pic);
                    string pathd = "~/images/pkt/" + pic;
                    // file is uploaded
                    file.SaveAs(path);
                    filefo = pathd;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                }
                DataRow newrow = dsPKT.Tables[0].NewRow();
                newrow["ID"] = Guid.NewGuid();
                newrow["PKTNAME"] = txtPKTNAME;
                newrow["PKTFIYAT"] = txtPKTFIYAT;
                newrow["PKTTIPI"] = pktTyp;
                newrow["PKTIMG"] = filefo;
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("PKT", newrow, dsPKT.Tables[0]);
                Session["useraddsuccess"] = true;
                ViewBag.addmessageinfo = veri;
                return Redirect("/Account/Admin");
            }
        }

        public ActionResult HomePkt()
        {
            
            return View();
        }

        public ActionResult CarPkt()
        {
            return View();
        }

        [HttpPost]
        public ActionResult HomePktAdd(string txtTAPUNO, string txtADRES)
        {
            using (DataVw dMan = new DataVw())
            {
                dsHOME = dMan.ExecuteView_S("HOMESAFETY", "*", "", "", "");
            }

            if (txtTAPUNO.ToString() == "" || txtADRES.ToString() == "")
            {
                Session["useraddsuccess"] = false;
                ViewBag.addmessage = "Eksik veri girişi! Tüm Alanları Doldurunuz.";
                return Redirect("/Account/Pacekt");
            }
            else
            {
                DataRow newrow = dsHOME.Tables[0].NewRow();
                newrow["ID"] = Guid.NewGuid();
                newrow["TAPUNO"] = txtTAPUNO;
                newrow["ADRES"] = txtADRES;
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("HOMESAFETY", newrow, dsHOME.Tables[0]);
                Session["useraddsuccess"] = true;
                ViewBag.addmessageinfo = veri;
                return Redirect("/Account/Admin");
            }
        }

        [HttpPost]
        public ActionResult CarPktAdd(string txtPLAKA, string txtSASENO)
        {
            using (DataVw dMan = new DataVw())
            {
                dsCAR = dMan.ExecuteView_S("CARSAFETY", "*", "", "", "");
            }

            if (txtPLAKA.ToString() == "" || txtSASENO.ToString() == "")
            {
                Session["useraddsuccess"] = false;
                ViewBag.addmessage = "Eksik veri girişi! Tüm Alanları Doldurunuz.";
                return Redirect("/Account/Pacekt");
            }
            else
            {
                DataRow newrow = dsCAR.Tables[0].NewRow();
                newrow["ID"] = Guid.NewGuid();
                newrow["PLAKA"] = txtPLAKA;
                newrow["SASENO"] = txtSASENO;
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("CARSAFETY", newrow, dsCAR.Tables[0]);
                Session["useraddsuccess"] = true;
                ViewBag.addmessageinfo = veri;
                return Redirect("/Account/Admin");
            }
        }

        public DataSet dsHOME { get; set; }

        public DataSet dsCAR { get; set; }
    }
}
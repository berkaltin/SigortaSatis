using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using SigortaSatis.Models;
using OfficeAgent.Configuration;
using OfficeAgent.Cryption;
using OfficeAgent.Data;
using OfficeAgent.Object;
using OfficeAgent;
using System.Data;
using System.IO;


namespace SigortaSatis.Controllers
{
    
    public class AccountController : Controller
    {
        DataSet dsUser = new DataSet();
      
        LoginInfo _li;

        public LoginInfo UserInfo
        {
            get { return _li; }
        }
        public class UserList
        {
            public Guid ID { get; set; }
            public string USRNM { get; set; }
            public string PWD { get; set; }
            public string FULNM { get; set; }
            public string EMAIL { get; set; }
            public string CARDNO { get; set; }
            public string CVC { get; set; }
            public string STKDAY { get; set; }
            public string STKMONTH { get; set; }
        }
        public class CarPaket
        {
            public Guid ID { get; set; }
            public string PKTID { get; set; }
            public string USRID { get; set; }
            public string PLAKA { get; set; }
            public string SASENO { get; set; }
           
        }
        public class HomePaket
        {
            public Guid ID { get; set; }
            public string PKTID { get; set; }
            public string USRID { get; set; }
            public string TAPUNO { get; set; }
            public string ADRES { get; set; }

        }


      
        public ActionResult Register() { return View(); }
        
        public ActionResult Manege() { return View(); }
        public ActionResult PaketSatis(string txtPLAKA, string txtSASENO)
        {
            DataSet dsCARSAFETY = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsCARSAFETY = dMan.ExecuteView_S("CARSAFETY", "*", "", "", "");
            }
            string USRID = Session["USRIDv"].ToString();
            DataRow newrow = dsCARSAFETY.Tables[0].NewRow();
            newrow["ID"] = Guid.NewGuid();
            newrow["PKTID"] = Session["PKTID"].ToString();
            newrow["USRID"] = USRID;
            newrow["PLAKA"] = txtPLAKA ;
            newrow["SASENO"] = txtSASENO;
            AgentGc data = new AgentGc();
            string veri = data.DataAdded("CARSAFETY", newrow, dsCARSAFETY.Tables[0]);

            
            return Redirect("/Home/Index");
        }
        public ActionResult HomeSatis(string txtTAPUNO, string txtADRES)
        {
            DataSet dsHOMEAFETY = new DataSet();
            using (DataVw dMan = new DataVw())
            {
                dsHOMEAFETY = dMan.ExecuteView_S("HOMESAFETY", "*", "", "", "");
            }
            string USRID = Session["USRIDv"].ToString();
            DataRow newrow = dsHOMEAFETY.Tables[0].NewRow();
            newrow["ID"] = Guid.NewGuid();
            newrow["PKTID"] = Session["PKTID"].ToString();
            newrow["USRID"] = USRID;
            newrow["TAPUNO"] = txtTAPUNO;
            newrow["ADRES"] = txtADRES;
            AgentGc data = new AgentGc();
            string veri = data.DataAdded("HOMESAFETY", newrow, dsHOMEAFETY.Tables[0]);


            return Redirect("/Home/Index");
        }

        [HttpPost]
        public ActionResult UserAdd(string txtUSRNM, string txtFULNM, string txtPWD, string txtEMAIL, string txtCARDNO, string txtCVC, string txtSTKDAY, string txtSTKMONTH, HttpPostedFileBase file)
        {
            string filefo = "";
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", "", "", "");
            }

            if (txtUSRNM.ToString() == "" || txtFULNM.ToString() == "" || txtPWD.ToString() == "" || txtEMAIL.ToString() == "" || txtCARDNO.ToString() == "" || txtCVC.ToString() == "" || txtSTKDAY.ToString() == "" || txtSTKMONTH.ToString() == "")
            {
                Session["useraddsuccess"] = false;
                ViewBag.addmessage = "Eksik veri girişi! Tüm Alanları Doldurunuz.";
                return Redirect("/Account/Register");
            }
            else
            {
                if (file != null)
                {
                    string pic = System.IO.Path.GetFileName(file.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/images/avatar"), pic);
                    string pathd = "~/images/avatar/" + pic;
                    // file is uploaded
                    file.SaveAs(path);
                    filefo = pathd;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }

                }
                DataRow newrow = dsUser.Tables[0].NewRow();
                newrow["ID"] = Guid.NewGuid();
                newrow["USRNM"] = txtUSRNM;
                newrow["PWD"] = CryptionHelper.Encrypt(txtPWD, "tb");
                newrow["FULNM"] = txtFULNM;
                newrow["EMAIL"] = txtEMAIL;
                newrow["IS_ADMIN"] = 1;
                newrow["IS_SYSADM"] = 0;
                if (filefo == "")
                {
                    newrow["AVATAR"] = "~/images/avatar/nullavatar.jpg";
                }
                else
                {
                    newrow["AVATAR"] = filefo;
                }
                newrow["AVATAR"] = filefo;
                newrow["CARDNO"] = txtCARDNO;
                newrow["CVC"] = txtCVC;
                newrow["STKDAY"] = txtSTKDAY;
                newrow["STKMONTH"] = txtSTKMONTH;
                newrow["EDATE"] = DateTime.Now;
                //newrow["EUSRID"] = null;
                newrow["UDATE"] = DateTime.Now;
                //newrow["UUSRID"] = null;
                newrow["NOTE"] = "En Son Kayıt İşlemi Gerçekleştirdi.";
                AgentGc data = new AgentGc();
                string veri = data.DataAdded("USR", newrow, dsUser.Tables[0]);
                Session["useraddsuccess"] = true;
                ViewBag.addmessageinfo = veri;
                return Redirect("/Account/Login");
            }
        }
        public ActionResult Control(string txtUsername, string txtPassword) 
        {
            HomeController HomeCont = new HomeController();
            UserManager uMan = new UserManager(txtUsername, txtPassword);
            _li = uMan.CheckLogin();

            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", txtUsername, "", "USRNM =");
            }

            if (dsUser.Tables[0].Rows.Count > 0)
            {
                DataRow row = dsUser.Tables[0].Rows[0];



                if (txtUsername.ToString() == row["USRNM"].ToString() && txtPassword.ToString() == CryptionHelper.Decrypt(row["PWD"].ToString(), "tb").ToString())
                {
                    Session["USRSTATUS"] = row["IS_ADMIN"].ToString();
                    Session["USRSTATUSADM"] = row["IS_SYSADM"].ToString();
                    Session["USRIDv"] = row["ID"].ToString();
                    Session["name"] = row["FULNM"].ToString();
                    //Session["admin"] = true;
                    //Session["loginError"] = true;
                    Session["IsAuthenticated"] = true;
                    Session["ADMIN"] = row["IS_SYSADM"].ToString();

                    if (row["IS_SYSADM"].ToString() == "True")
                    {
                        Session["IS_SYSADM"] = true;
                        //Session["loginRoles"] = true;
                        //Session["admin"] = true;
                        if (row["AVATAR"].ToString() == "")
                        {
                            Session["avatarimg"] = "~/images/profil/nullavatar.jpg";
                        }
                        else
                        {
                            Session["avatarimg"] = row["AVATAR"].ToString();
                        }


                        return Redirect("/Account/Admin");
                    }
                    else  
                    {
                        Session["IsAuthenticated"] = true;
                        Session["loginRoles"] = false;
                        Session["CUST"] = true;
                        Session["IS_ADMIN"] = true;
                        if (row["AVATAR"].ToString() == "")
                        {
                            Session["avatarimg"] = "~/images/profil/nullavatar.jpg";
                        }
                        else
                        {
                            Session["avatarimg"] = row["AVATAR"].ToString();
                        }
                        return Redirect("/Account/Cust");
                       
                    }

                   
                    
                }

                Session["loginError"] = true;
                Session["IsAuthenticated"] = false;

                //int loginErrorCount = Convert.ToInt32(Session["wrongpiece"]);

                //Session["wrongpiece"] = loginErrorCount + 1;
                //Session["wrongdate"] = DateTime.Now;
                //Session["IP"] = GetIp();

                return Redirect("/Account/Login");
            }
            else
            {
                Session["loginError"] = true;
                return Redirect("/Account/Login");
            }

            
        }
        public ActionResult Login() {

           
            return View();
        }
        public ActionResult Cust()
        {

            DataSet UserCarPKT = new DataSet();
            string USRID = Session["USRIDv"].ToString();

            using (DataVw dMan = new DataVw())
            {
                UserCarPKT = dMan.ExecuteView_S("CARv", "*", USRID, "", "USRID =");
            }

            List<CarPaket> CarPKT = new List<CarPaket>();
            foreach (DataRow dr in UserCarPKT.Tables[0].Rows)
            {

                CarPKT.Add(new CarPaket
                {
                    ID = (Guid)dr["ID"],
                    PKTID = dr["PKTNAME"].ToString(),
                    USRID = dr["USRNM"].ToString(),
                    PLAKA = dr["PLAKA"].ToString(),
                    SASENO = dr["SASENO"].ToString(),
                    

                });
            }
            ViewBag.CARPKT = CarPKT;
           
            DataSet UserHomePKT = new DataSet();
            

            using (DataVw dMan = new DataVw())
            {
                UserHomePKT = dMan.ExecuteView_S("HOMEv", "*", USRID, "", "USRID =");
            }

            List<HomePaket> HomePKT = new List<HomePaket>();
            foreach (DataRow dr in UserHomePKT.Tables[0].Rows)
            {

                HomePKT.Add(new HomePaket
                {
                    ID = (Guid)dr["ID"],
                    PKTID = dr["PKTNAME"].ToString(),
                    USRID = dr["USRNM"].ToString(),
                    TAPUNO = dr["TAPUNO"].ToString(),
                    ADRES = dr["ADRES"].ToString(),


                });
            }
            ViewBag.HOMEPKT = HomePKT;
            return View();
        }
        public ActionResult Admin() {

            DataSet dsUser = new DataSet();
            string USRID = Session["USRIDv"].ToString();
            using (DataVw dMan = new DataVw())
            {
                dsUser = dMan.ExecuteView_S("USR", "*", USRID, "", "ID = ");
            }

            List<UserList> userList = new List<UserList>();
            foreach (DataRow dr in dsUser.Tables[0].Rows)
            {
                userList.Add(new UserList
                {
                    ID = (Guid)dr["ID"],
                    USRNM = dr["USRNM"].ToString(),
                    PWD = CryptionHelper.Decrypt(dr["PWD"].ToString(), "tb"),
                    EMAIL = dr["EMAIL"].ToString(),
                    FULNM = dr["FULNM"].ToString(),
                    CARDNO = dr["CARDNO"].ToString(),
                    CVC = dr["CVC"].ToString(),
                    STKDAY = dr["STKDAY"].ToString(),
                    STKMONTH = dr["STKMONTH"].ToString()
                });
            }

            ViewBag.UserList = userList;

            return View();
        
        }
        public ActionResult LogOff()
        {
            Session.Abandon();
            //AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using prjshoppincar.Models;

namespace prjshoppincar.Controllers
{
    public class homeController : Controller
    {
        // GET: home
        dbShoppingCarEntities db = new dbShoppingCarEntities();
        public ActionResult Index()
        {
            var products = db.tProduct.ToList();  //傳給網頁顯示
            if(Session["Member"]==null)
            {
                return View("Index", "_Layout", products);
            }else
            {
                return View("Index", "_LayoutMember", products);
            }
            
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]

        public ActionResult Login(string fUserId,string fPwd)
        {
            var me = db.tMember.Where(m => m.fUserId == fUserId && m.fPwd == fPwd).FirstOrDefault();
            if(me==null)
            {
                ViewBag.Message = "帳/密錯誤,登入失敗!!";
                return View();
            }else
            {
                Session["Member"] = me;
                Session["MemberName"] = me.fName;
                Session["Welcome"] = me.fName+"歡迎光臨";
                return RedirectToAction("Index");
            }
        }
    }
}
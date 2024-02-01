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
    }
}
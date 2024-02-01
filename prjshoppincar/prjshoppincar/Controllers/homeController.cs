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
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(tMember t)
        {
            if(ModelState.IsValid)
            {
                var me = db.tMember.Where(m => m.fUserId == t.fUserId).FirstOrDefault();
                if(me==null)
                {
                    db.tMember.Add(t);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }else
                {
                    ViewBag.Message = "帳號已存在,註冊失敗!!";
                    return View();
                }
            }else
            {
                ViewBag.Message = "驗證未通過!!";
                return View();
            }
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
        public ActionResult SoppingCar()
        {
            var fUserId = ((tMember)Session["Member"]).fUserId;  //物件型態(session)直接轉換
            var ods = db.tOrderDetail.Where(m => m.fUserId == fUserId && m.fIsApproved == "否").ToList();
            return View("SoppingCar","_LayoutMember",ods);
        }
        public ActionResult AddCar(string fPId)
        {
            var fUserId = ((tMember)Session["Member"]).fUserId;
            var currentCar = db.tOrderDetail.Where(m => m.fUserId == fUserId&&m.fPId==fPId&&m.fIsApproved=="否").FirstOrDefault();
            if(currentCar==null)
            {
                var product = db.tProduct.Where(m => m.fPId == fPId).FirstOrDefault();
                tOrderDetail od = new tOrderDetail();
                od.fUserId = fUserId;
                od.fPId = fPId;
                od.fName = product.fName;
                od.fPrice = product.fPrice;
                od.fQty = 1;
                od.fIsApproved = "否";
                db.tOrderDetail.Add(od);
            }
            else
            {
                currentCar.fQty += 1;
            }
            db.SaveChanges();
            return RedirectToAction("SoppingCar");
        }
        public ActionResult Delete(int fId)
        {
            var od = db.tOrderDetail.Where(m => m.fId == fId).FirstOrDefault();
            db.tOrderDetail.Remove(od);
            db.SaveChanges();
            return RedirectToAction("SoppingCar");
        }
        [HttpPost]
        public ActionResult SoppingCar(string fReceiver,string fEmail,string fAddress)
        {
            var fUserId = ((tMember)Session["Member"]).fUserId;
            //1.須完成 訂單表
            tOrder order = new tOrder();
            string guid = Guid.NewGuid().ToString();
            order.fOrderGuid=guid;
            order.fUserId = fUserId;
            order.fReceiver = fReceiver;
            order.fEmail = fEmail;
            order.fAddress = fAddress;
            order.fDate = DateTime.Now;
            db.tOrder.Add(order);
            var ods = db.tOrderDetail.Where(m => m.fUserId == fUserId && m.fIsApproved == "否").ToList();
            foreach(var item in ods)
            {
                item.fOrderGuid = guid;
                item.fIsApproved = "是";
            }
            db.SaveChanges();
            return RedirectToAction("OrderList");
        }
        public ActionResult OrderList()
        {
            var fUserId = ((tMember)Session["Member"]).fUserId;
            var orders = db.tOrder.Where(m => m.fUserId == fUserId).ToList();
            return View("OrderList", "_LayoutMember", orders);
        }

        //處理詳細歷史訂單資料按鈕
        public ActionResult OrderDetail(string fOrderGuid)
        {
            var ods = db.tOrderDetail.Where(m => m.fOrderGuid == fOrderGuid).ToList();
            Session["Guid"] = fOrderGuid;
            return View("OrderDetail", "_LayoutMember", ods);
        }
    }
}
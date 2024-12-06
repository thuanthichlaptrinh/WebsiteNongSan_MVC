using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebsiteNongSan.Areas.Admin.Controllers
{
    public class ThongBaoController : Controller
    {
        // GET: Admin/ThongBao
        public ActionResult BaoLoi()
        {
            return View();
        }
    }
}
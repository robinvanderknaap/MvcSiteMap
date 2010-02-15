using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace MvcSiteMap.Controllers
{
    public class NestedPagesController : Controller
    {
        public ActionResult Item(string id)
        {
            ViewData["id"] = id;
            return View();
        }

    }
}

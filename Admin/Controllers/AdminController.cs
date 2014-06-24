using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return PartialView();
        }

        //GET: Admin/Table
        public ActionResult Table()
        {
            return PartialView();
        }

        //GET: Admin/EditRow
        public ActionResult EditRow()
        {
            return PartialView();
        }

        //GET: Admin/DeleteDialog
        public ActionResult DeleteDialog()
        {
            return PartialView();
        }

    }
}
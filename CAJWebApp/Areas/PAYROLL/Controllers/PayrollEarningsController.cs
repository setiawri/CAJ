using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using CAJWebApp.Areas.PAYROLL.Models;
using CAJWebApp.Controllers;
using LIBUtil;

namespace CAJWebApp.Areas.PAYROLL.Controllers
{
    public class PayrollEarningsController : Controller
    { 
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PAYROLL/PayrollEarnings
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(db.PayrollEarningsModel);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PAYROLL/PayrollEarnings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PAYROLL/PayrollEarnings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PayrollEarningsModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Create, null, model.Name))
                    ModelState.AddModelError(PayrollEarningsModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    db.PayrollEarningsModel.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PAYROLL/PayrollEarnings/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            return View(db.PayrollEarningsModel.Find(id));
        }

        // POST: PAYROLL/PayrollEarnings/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PayrollEarningsModel modifiedModel)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Edit, modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(PayrollEarningsModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else 
                {
                    PayrollEarningsModel originalModel = db.PayrollEarningsModel.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Util.webAppendChange(log, originalModel.Name, modifiedModel.Name, ActivityLogsController.editStringFormat(PayrollEarningsModel.COL_Name.Display));

                    if (!string.IsNullOrEmpty(log))
                    {
                        db.Entry(modifiedModel).State = EntityState.Modified;
                        ActivityLogsController.AddEditLog(db, Session, modifiedModel.Id, log);
                        db.SaveChanges();
                    }

                    return RedirectToAction(nameof(Index));
                }
            }

            return View(modifiedModel);
        }

        /******************************************************************************************************************************************************/

        public bool isExists(EnumActions action, Guid? id, object value)
        {
            var result = action == EnumActions.Create
                ? get(db).Where(x => x.Name.ToLower() == value.ToString().ToLower()).FirstOrDefault()
                : get(db).Where(x => x.Name.ToLower() == value.ToString().ToLower() && x.Id != id).FirstOrDefault();
            return result != null;
        }

        public JsonResult getDropdownlistData()
        {
            List<object> result = new List<object>();
            foreach (var item in get(db))
                result.Add(new { item.Id, item.Name });

            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }

        public static List<PayrollEarningsModel> get(DBContext db)
        {
            return db.PayrollEarningsModel.AsNoTracking().ToList();
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {
            controller.ViewBag.PayrollEarnings = new SelectList(get(db), PayrollEarningsModel.COL_Id.Name, PayrollEarningsModel.COL_Name.Name);
        }

        /******************************************************************************************************************************************************/
    }
}
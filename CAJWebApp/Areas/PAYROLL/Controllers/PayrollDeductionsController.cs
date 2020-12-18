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
    public class PayrollDeductionsController : Controller
    { 
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PAYROLL/PayrollDeductions
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(db.PayrollDeductionsModel);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PAYROLL/PayrollDeductions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PAYROLL/PayrollDeductions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PayrollDeductionsModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Create, null, model.Name))
                    ModelState.AddModelError(PayrollDeductionsModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    db.PayrollDeductionsModel.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PAYROLL/PayrollDeductions/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            return View(db.PayrollDeductionsModel.Find(id));
        }

        // POST: PAYROLL/PayrollDeductions/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PayrollDeductionsModel modifiedModel)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Edit, modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(PayrollDeductionsModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else 
                {
                    PayrollDeductionsModel originalModel = db.PayrollDeductionsModel.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Util.webAppendChange(log, originalModel.Name, modifiedModel.Name, ActivityLogsController.editStringFormat(PayrollDeductionsModel.COL_Name.Display));

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

        public static List<PayrollDeductionsModel> get(DBContext db)
        {
            return db.PayrollDeductionsModel.AsNoTracking().OrderBy(x => x.Name).ToList();
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {
            controller.ViewBag.PayrollDeductions = new SelectList(get(db), PayrollDeductionsModel.COL_Id.Name, PayrollDeductionsModel.COL_Name.Name);
        }

        /******************************************************************************************************************************************************/
    }
}
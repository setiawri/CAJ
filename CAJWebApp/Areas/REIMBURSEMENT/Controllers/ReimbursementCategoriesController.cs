using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using CAJWebApp.Areas.Reimbursement.Models;
using CAJWebApp.Controllers;
using LIBUtil;

namespace CAJWebApp.Areas.Reimbursement.Controllers
{
    public class ReimbursementCategoriesController : Controller
    { 
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PAYROLL/ReimbursementCategories
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(db.ReimbursementCategoriesModel);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PAYROLL/ReimbursementCategories/Create
        public ActionResult Create()
        {
            return View(new ReimbursementCategoriesModel());
        }

        // POST: PAYROLL/ReimbursementCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReimbursementCategoriesModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Create, null, model.Name))
                    ModelState.AddModelError(ReimbursementCategoriesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    db.ReimbursementCategoriesModel.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PAYROLL/ReimbursementCategories/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            return View(db.ReimbursementCategoriesModel.Find(id));
        }

        // POST: PAYROLL/ReimbursementCategories/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReimbursementCategoriesModel modifiedModel)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Edit, modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(ReimbursementCategoriesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else 
                {
                    ReimbursementCategoriesModel originalModel = db.ReimbursementCategoriesModel.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Util.webAppendChange(log, originalModel.Name, modifiedModel.Name, ActivityLogsController.editStringFormat(ReimbursementCategoriesModel.COL_Name.Display));

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

        public static List<ReimbursementCategoriesModel> get(DBContext db)
        {
            return db.ReimbursementCategoriesModel.AsNoTracking().ToList();
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {
            controller.ViewBag.ReimbursementCategories = new SelectList(get(db), ReimbursementCategoriesModel.COL_Id.Name, ReimbursementCategoriesModel.COL_Name.Name);
        }

        /******************************************************************************************************************************************************/
    }
}
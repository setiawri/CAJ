﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using CAJWebApp.Areas.PAYROLL.Models;
using CAJWebApp.Controllers;
using LIBUtil;

namespace CAJWebApp.Areas.PAYROLL.Controllers
{
    public class PayrollDebtsController : Controller
    { 
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PAYROLL/PayrollDebts
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(db.PayrollDebtsModel);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PAYROLL/PayrollDebts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PAYROLL/PayrollDebts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PayrollDebtsModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Create, null, model.Name))
                    ModelState.AddModelError(PayrollDebtsModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    db.PayrollDebtsModel.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PAYROLL/PayrollDebts/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            return View(db.PayrollDebtsModel.Find(id));
        }

        // POST: PAYROLL/PayrollDebts/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PayrollDebtsModel modifiedModel)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Edit, modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(PayrollDebtsModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else 
                {
                    PayrollDebtsModel originalModel = db.PayrollDebtsModel.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Util.webAppendChange(log, originalModel.Name, modifiedModel.Name, ActivityLogsController.editStringFormat(PayrollDebtsModel.COL_Name.Display));

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

        public static List<PayrollDebtsModel> get(DBContext db)
        {
            return db.PayrollDebtsModel.AsNoTracking().OrderBy(x => x.Name).ToList();
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {
            controller.ViewBag.PayrollDebts = new SelectList(get(db), PayrollDebtsModel.COL_Id.Name, PayrollDebtsModel.COL_Name.Name);
        }

        /******************************************************************************************************************************************************/
    }
}
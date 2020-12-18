using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CAJWebApp.Areas.Reimbursement.Models;
using CAJWebApp.Controllers;
using LIBUtil;

namespace CAJWebApp.Areas.Reimbursement.Controllers
{
    public class ReimbursementPaymentDatesController : Controller
    { 
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: Reimbursement/ReimbursementPaymentDates
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(db.ReimbursementPaymentDatesModel);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: Reimbursement/ReimbursementPaymentDates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reimbursement/ReimbursementPaymentDates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReimbursementPaymentDatesModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Create, null, model.PayDate))
                    ModelState.AddModelError(ReimbursementPaymentDatesModel.COL_PayDate.Name, $"{model.PayDate} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    db.ReimbursementPaymentDatesModel.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: Reimbursement/ReimbursementPaymentDates/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            return View(db.ReimbursementPaymentDatesModel.Find(id));
        }

        // POST: Reimbursement/ReimbursementPaymentDates/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReimbursementPaymentDatesModel modifiedModel)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Edit, modifiedModel.Id, modifiedModel.PayDate))
                    ModelState.AddModelError(ReimbursementPaymentDatesModel.COL_PayDate.Name, $"{modifiedModel.PayDate} sudah terdaftar");
                else 
                {
                    ReimbursementPaymentDatesModel originalModel = db.ReimbursementPaymentDatesModel.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Util.webAppendChange(log, originalModel.PayDate, modifiedModel.PayDate, ActivityLogsController.editIntFormat(ReimbursementPaymentDatesModel.COL_PayDate.Display));

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

        public bool isExists(EnumActions action, Guid? id, byte value)
        {
            var result = action == EnumActions.Create
                ? db.ReimbursementPaymentDatesModel.AsNoTracking().Where(x => x.PayDate == value).FirstOrDefault()
                : db.ReimbursementPaymentDatesModel.AsNoTracking().Where(x => x.PayDate == value && x.Id != id).FirstOrDefault();
            return result != null;
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {            
            controller.ViewBag.ReimbursementPaymentDates = new SelectList(
                    db.ReimbursementPaymentDatesModel.AsNoTracking().OrderBy(x => x.PayDate).ToList(), 
                    ReimbursementPaymentDatesModel.COL_Id.Name, ReimbursementPaymentDatesModel.COL_PayDate.Name
                );
        }

        /******************************************************************************************************************************************************/
    }
}
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CAJWebApp.Areas.PAYROLL.Models;
using CAJWebApp.Controllers;
using LIBUtil;

namespace CAJWebApp.Areas.PAYROLL.Controllers
{
    public class PayrollPaymentDatesController : Controller
    { 
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PAYROLL/PayrollPaymentDates
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(db.PayrollPaymentDatesModel);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PAYROLL/PayrollPaymentDates/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PAYROLL/PayrollPaymentDates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PayrollPaymentDatesModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Create, null, model.PayDate))
                    ModelState.AddModelError(PayrollPaymentDatesModel.COL_PayDate.Name, $"{model.PayDate} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    db.PayrollPaymentDatesModel.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PAYROLL/PayrollPaymentDates/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            return View(db.PayrollPaymentDatesModel.Find(id));
        }

        // POST: PAYROLL/PayrollPaymentDates/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PayrollPaymentDatesModel modifiedModel)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Edit, modifiedModel.Id, modifiedModel.PayDate))
                    ModelState.AddModelError(PayrollPaymentDatesModel.COL_PayDate.Name, $"{modifiedModel.PayDate} sudah terdaftar");
                else 
                {
                    PayrollPaymentDatesModel originalModel = db.PayrollPaymentDatesModel.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Util.webAppendChange(log, originalModel.PayDate, modifiedModel.PayDate, ActivityLogsController.editIntFormat(PayrollPaymentDatesModel.COL_PayDate.Display));

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
                ? db.PayrollPaymentDatesModel.AsNoTracking().Where(x => x.PayDate == value).FirstOrDefault()
                : db.PayrollPaymentDatesModel.AsNoTracking().Where(x => x.PayDate == value && x.Id != id).FirstOrDefault();
            return result != null;
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {            
            controller.ViewBag.PayrollPaymentDates = new SelectList(
                    db.PayrollPaymentDatesModel.AsNoTracking().OrderBy(x => x.PayDate).ToList(), 
                    PayrollPaymentDatesModel.COL_Id.Name, PayrollPaymentDatesModel.COL_PayDate.Name
                );
        }

        /******************************************************************************************************************************************************/
    }
}
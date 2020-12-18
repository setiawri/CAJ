using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CAJWebApp.Models;
using LIBUtil;

namespace CAJWebApp.Controllers
{
    public class MaritalStatusesController : Controller
    { 
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PAYROLL/MaritalStatuses
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(db.MaritalStatusesModel);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PAYROLL/MaritalStatuses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PAYROLL/MaritalStatuses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MaritalStatusesModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Create, null, model.Name))
                    ModelState.AddModelError(MaritalStatusesModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    db.MaritalStatusesModel.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PAYROLL/MaritalStatuses/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            return View(db.MaritalStatusesModel.Find(id));
        }

        // POST: PAYROLL/MaritalStatuses/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MaritalStatusesModel modifiedModel)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Edit, modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(MaritalStatusesModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else 
                {
                    MaritalStatusesModel originalModel = db.MaritalStatusesModel.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Util.webAppendChange(log, originalModel.Name, modifiedModel.Name, ActivityLogsController.editStringFormat(MaritalStatusesModel.COL_Name.Display));

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
                ? db.MaritalStatusesModel.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower()).FirstOrDefault()
                : db.MaritalStatusesModel.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower() && x.Id != id).FirstOrDefault();
            return result != null;
        }

        /******************************************************************************************************************************************************/
    }
}
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CAJWebApp.Models;
using LIBUtil;

namespace CAJWebApp.Controllers
{
    public class BanksController : Controller
    { 
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PAYROLL/Banks
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(db.BanksModel);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PAYROLL/Banks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PAYROLL/Banks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BanksModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Create, null, model.Name))
                    ModelState.AddModelError(BanksModel.COL_Name.Name, $"{model.Name} sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    db.BanksModel.Add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PAYROLL/Banks/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            return View(db.BanksModel.Find(id));
        }

        // POST: PAYROLL/Banks/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BanksModel modifiedModel)
        {
            if (ModelState.IsValid)
            {
                if (isExists(EnumActions.Edit, modifiedModel.Id, modifiedModel.Name))
                    ModelState.AddModelError(BanksModel.COL_Name.Name, $"{modifiedModel.Name} sudah terdaftar");
                else 
                {
                    BanksModel originalModel = db.BanksModel.AsNoTracking().Where(x => x.Id == modifiedModel.Id).FirstOrDefault();

                    string log = string.Empty;
                    log = Util.webAppendChange(log, originalModel.Name, modifiedModel.Name, ActivityLogsController.editStringFormat(BanksModel.COL_Name.Display));

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
                ? db.BanksModel.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower()).FirstOrDefault()
                : db.BanksModel.AsNoTracking().Where(x => x.Name.ToLower() == value.ToString().ToLower() && x.Id != id).FirstOrDefault();
            return result != null;
        }

        public static void setDropDownListViewBag(DBContext db, ControllerBase controller)
        {
            controller.ViewBag.Banks = new SelectList(db.BanksModel.AsNoTracking().OrderBy(x => x.Name).ToList(), BanksModel.COL_Id.Name, BanksModel.COL_Name.Name);
        }

        /******************************************************************************************************************************************************/
    }
}
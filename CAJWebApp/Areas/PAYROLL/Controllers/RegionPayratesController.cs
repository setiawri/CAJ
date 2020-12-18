using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CAJWebApp.Areas.PAYROLL.Models;
using CAJWebApp.Controllers;
using CAJWebApp.Models;
using LIBUtil;

namespace CAJWebApp.Areas.PAYROLL.Controllers
{
    public class RegionPayratesController : Controller
    {
        private readonly DBContext db = new DBContext();
        
        /* INDEX **********************************************************************************************************************************************/

        // GET: PAYROLL/RegionPayrates
        public ActionResult Index(int? rss)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            return View(get());
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PAYROLL/RegionPayrates/Create
        public ActionResult Create()
        {
            populateViewBag();
            return View();
        }

        // POST: PAYROLL/RegionPayrates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegionPayratesModel model)
        {
            if (ModelState.IsValid)
            {
                if (isExists(null, model.Regions_Id))
                    ModelState.AddModelError(RegionPayratesModel.COL_Regions_Id.Name, "Sudah terdaftar");
                else
                {
                    model.Id = Guid.NewGuid();
                    add(model);
                    ActivityLogsController.AddCreateLog(db, Session, model.Id);
                    db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
            }

            populateViewBag();
            return View(model);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PAYROLL/RegionPayrates/Edit/{id}
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
                return RedirectToAction(nameof(Index));

            populateViewBag();
            return View(get((Guid)id));
        }

        // POST: PAYROLL/RegionPayrates/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegionPayratesModel modifiedModel)
        {
            if (ModelState.IsValid)
            {
                if (isExists(modifiedModel.Id, modifiedModel.Regions_Id))
                    ModelState.AddModelError(RegionPayratesModel.COL_Regions_Id.Name, "Sudah terdaftar");
                else
                {
                    RegionPayratesModel originalModel = get(modifiedModel.Id);

                    string log = string.Empty;

                    log = Util.webAppendChange(log, originalModel.Regions_Name, modifiedModel.Regions_Name, ActivityLogsController.editStringFormat(RegionPayratesModel.COL_Regions_Id.Display));
                    log = Util.webAppendChange(log, originalModel.MinimumPayrate, modifiedModel.MinimumPayrate, ActivityLogsController.editIntFormat(RegionPayratesModel.COL_MinimumPayrate.Display));
                    log = Util.webAppendChange(log, originalModel.RegularPayrate, modifiedModel.RegularPayrate, ActivityLogsController.editIntFormat(RegionPayratesModel.COL_RegularPayrate.Display));
                    log = Util.webAppendChange(log, originalModel.RegularOvertimeHourlyPayrate, modifiedModel.RegularOvertimeHourlyPayrate, ActivityLogsController.editIntFormat(RegionPayratesModel.COL_RegularOvertimeHourlyPayrate.Display));
                    log = Util.webAppendChange(log, originalModel.HolidayPayrate, modifiedModel.HolidayPayrate, ActivityLogsController.editIntFormat(RegionPayratesModel.COL_HolidayPayrate.Display));
                    log = Util.webAppendChange(log, originalModel.HolidayOvertimeHourlyPayrate, modifiedModel.HolidayOvertimeHourlyPayrate, ActivityLogsController.editIntFormat(RegionPayratesModel.COL_HolidayOvertimeHourlyPayrate.Display));

                    if (!string.IsNullOrEmpty(log))
                    {
                        update(modifiedModel);
                        ActivityLogsController.AddEditLog(db, Session, modifiedModel.Id, log);
                        db.SaveChanges();
                    }

                    return RedirectToAction(nameof(Index));
                }
            }

            populateViewBag();
            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/

        public bool isExists(Guid? id, object value)
        {
            return 1 == db.Database.SqlQuery<int>(@"
	                    IF EXISTS (SELECT id FROM DWSystem.RegionPayrates WHERE Regions_Id = @Regions_Id AND (@id IS NULL OR id != @id))
		                    SELECT 1
	                    ELSE
		                    SELECT 0
                    ", 
                    DBConnection.getSqlParameter(RegionPayratesModel.COL_Regions_Id.Name, value), 
                    DBConnection.getSqlParameter(RegionPayratesModel.COL_Id.Name, id)
                ).FirstOrDefault();
        }

        private void populateViewBag()
        {
            ViewBag.Regions = new SelectList(db.RegionsModel.OrderBy(x => x.Name).ToList(), RegionsModel.COL_Id.Name, RegionsModel.COL_Name.Name);
        }

        private List<RegionPayratesModel> get() { return getData(db, null, null); }
        private RegionPayratesModel get(Guid id) { return getData(db, id, null).FirstOrDefault(); }
        public static RegionPayratesModel get(DBContext db, Guid regions_Id) { return getData(db, null, regions_Id).FirstOrDefault(); }
        private static List<RegionPayratesModel> getData(DBContext db, Guid? id, Guid? regions_Id)
        {
            return db.Database.SqlQuery<RegionPayratesModel>(@"
                        SELECT RegionPayrates.*,
                            Regions.Name AS Regions_Name
                        FROM DWSystem.RegionPayrates
                            LEFT JOIN DWSystem.Regions ON Regions.Id = RegionPayrates.Regions_Id
                        WHERE 1=1
                            AND (@Id IS NULL OR RegionPayrates.Id = @Id)
                            AND (@Regions_Id IS NULL OR RegionPayrates.Regions_Id = @Regions_Id)
                    ", 
                    DBConnection.getSqlParameter(RegionPayratesModel.COL_Id.Name, id),
                    DBConnection.getSqlParameter(RegionPayratesModel.COL_Regions_Id.Name, regions_Id)
                ).ToList();
        }

        private void add(RegionPayratesModel model)
        {
            db.Database.ExecuteSqlCommand(@"
                    INSERT INTO DWSystem.RegionPayrates (
                        Id,
                        Regions_Id,
                        MinimumPayrate,
                        RegularPayrate,
                        RegularOvertimeHourlyPayrate,
                        HolidayPayrate,
                        HolidayOvertimeHourlyPayrate
                    ) VALUES (
                        @Id,
                        @Regions_Id,
                        @MinimumPayrate,
                        @RegularPayrate,
                        @RegularOvertimeHourlyPayrate,
                        @HolidayPayrate,
                        @HolidayOvertimeHourlyPayrate
                    )
                ",
                DBConnection.getSqlParameter(RegionPayratesModel.COL_Id.Name, Guid.NewGuid()),
                DBConnection.getSqlParameter(RegionPayratesModel.COL_Regions_Id.Name, model.Regions_Id),
                DBConnection.getSqlParameter(RegionPayratesModel.COL_MinimumPayrate.Name, model.MinimumPayrate),
                DBConnection.getSqlParameter(RegionPayratesModel.COL_RegularPayrate.Name, model.RegularPayrate),
                DBConnection.getSqlParameter(RegionPayratesModel.COL_RegularOvertimeHourlyPayrate.Name, model.RegularOvertimeHourlyPayrate),
                DBConnection.getSqlParameter(RegionPayratesModel.COL_HolidayPayrate.Name, model.HolidayPayrate),
                DBConnection.getSqlParameter(RegionPayratesModel.COL_HolidayOvertimeHourlyPayrate.Name, model.HolidayOvertimeHourlyPayrate)
            );
        }

        private void update(RegionPayratesModel modifiedModel)
        {
            db.Database.ExecuteSqlCommand(@"
                    UPDATE DWSystem.RegionPayrates 
                    SET 
                        Regions_Id = @Regions_Id,
                        MinimumPayrate = @MinimumPayrate,
                        RegularPayrate = @RegularPayrate,
                        RegularOvertimeHourlyPayrate = @RegularOvertimeHourlyPayrate,
                        HolidayPayrate = @HolidayPayrate,
                        HolidayOvertimeHourlyPayrate = @HolidayOvertimeHourlyPayrate
                    WHERE Id=@Id;
                ",
                DBConnection.getSqlParameter(RegionPayratesModel.COL_Id.Name, modifiedModel.Id),
                DBConnection.getSqlParameter(RegionPayratesModel.COL_Regions_Id.Name, modifiedModel.Regions_Id),
                DBConnection.getSqlParameter(RegionPayratesModel.COL_MinimumPayrate.Name, modifiedModel.MinimumPayrate),
                DBConnection.getSqlParameter(RegionPayratesModel.COL_RegularPayrate.Name, modifiedModel.RegularPayrate),
                DBConnection.getSqlParameter(RegionPayratesModel.COL_RegularOvertimeHourlyPayrate.Name, modifiedModel.RegularOvertimeHourlyPayrate),
                DBConnection.getSqlParameter(RegionPayratesModel.COL_HolidayPayrate.Name, modifiedModel.HolidayPayrate),
                DBConnection.getSqlParameter(RegionPayratesModel.COL_HolidayOvertimeHourlyPayrate.Name, modifiedModel.HolidayOvertimeHourlyPayrate)
            );
        }

        public JsonResult calculatePayrates(decimal? minimumPayrate)
        {
            int regularPayrate = 0;
            int holidayPayrate = 0;
            int regularOvertimeHourlyPayrate = 0;
            int holidayOvertimeHourlyPayrate = 0;

            if (minimumPayrate != null)
            {
                SettingsModel model = SettingsController.getModel(db);

                //Round up to the nearest 50 
                regularPayrate = (int)Math.Ceiling(((int)((decimal)minimumPayrate * model.MultiplierRegularPayrate)) / 50.0) * 50;
                holidayPayrate = (int)Math.Ceiling(((int)((decimal)minimumPayrate * model.MultiplierHolidayPayrate)) / 50.0) * 50;
                regularOvertimeHourlyPayrate = (int)Math.Ceiling(((int)((decimal)minimumPayrate * model.MultiplierRegularOvertimeHourlyPayrate)) / 50.0) * 50;
                holidayOvertimeHourlyPayrate = (int)Math.Ceiling(((int)((decimal)minimumPayrate * model.MultiplierHolidayOvertimeHourlyPayrate)) / 50.0) * 50;
            }

            return Json(new
            {
                regularPayrate,
                holidayPayrate,
                regularOvertimeHourlyPayrate,
                holidayOvertimeHourlyPayrate
            }, JsonRequestBehavior.AllowGet);
        }

        /******************************************************************************************************************************************************/
    }
}
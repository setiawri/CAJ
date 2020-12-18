using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CAJWebApp.Models;
using LIBUtil;

namespace CAJWebApp.Controllers
{
    public class SettingsController : Controller
    {
        private readonly DBContext db = new DBContext();

        /******************************************************************************************************************************************************/

        public static SettingsModel getModel(DBContext db)
        {
            return get(db);
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: Settings/Edit
        public ActionResult Edit()
        {
            return View(get(db));
        }

        // POST: Settings/Edit/{modifiedModel,uploadFiles}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SettingsModel modifiedModel, List<HttpPostedFileBase> uploadFiles)
        {
            if (ModelState.IsValid)
            {
                //Prepare changes
                SettingsModel originalModel = get(db);
                string log = string.Empty;
                log += addLog(SettingsModel.COL_WorkHoursPerDay.Id, originalModel.WorkHoursPerDay, modifiedModel.WorkHoursPerDay, "Update: '{1}'");
                log += addLog(SettingsModel.COL_WorkHoursPerDay.Id, originalModel.WorkHoursPerDay_Notes, modifiedModel.WorkHoursPerDay_Notes, "Notes: '{1}'");
                log += addLog(SettingsModel.COL_WorkDaysPerMonth.Id, originalModel.WorkDaysPerMonth, modifiedModel.WorkDaysPerMonth, "Update: '{1}'");
                log += addLog(SettingsModel.COL_WorkDaysPerMonth.Id, originalModel.WorkDaysPerMonth_Notes, modifiedModel.WorkDaysPerMonth_Notes, "Notes: '{1}'");
                log += addLog(SettingsModel.COL_LeaveDaysPerYear.Id, originalModel.LeaveDaysPerYear, modifiedModel.LeaveDaysPerYear, "Update: '{1}'");
                log += addLog(SettingsModel.COL_LeaveDaysPerYear.Id, originalModel.LeaveDaysPerYear_Notes, modifiedModel.LeaveDaysPerYear_Notes, "Notes: '{1}'");
                log += addLog(SettingsModel.COL_DepositAmountPerMonth.Id, originalModel.DepositAmountPerMonth, modifiedModel.DepositAmountPerMonth, "Update: '{1}'");
                log += addLog(SettingsModel.COL_DepositAmountPerMonth.Id, originalModel.DepositAmountPerMonth_Notes, modifiedModel.DepositAmountPerMonth_Notes, "Notes: '{1}'");
                log += addLog(SettingsModel.COL_DepositAmountTotal.Id, originalModel.DepositAmountTotal, modifiedModel.DepositAmountTotal, "Update: '{1}'");
                log += addLog(SettingsModel.COL_DepositAmountTotal.Id, originalModel.DepositAmountTotal_Notes, modifiedModel.DepositAmountTotal_Notes, "Notes: '{1}'");
                log += addLog(SettingsModel.COL_DepositFirstMonthAfterJoin.Id, originalModel.DepositFirstMonthAfterJoin, modifiedModel.DepositFirstMonthAfterJoin, "Update: '{1}'");
                log += addLog(SettingsModel.COL_DepositFirstMonthAfterJoin.Id, originalModel.DepositFirstMonthAfterJoin_Notes, modifiedModel.DepositFirstMonthAfterJoin_Notes, "Notes: '{1}'");
                log += addLog(SettingsModel.COL_PayrollApproverName.Id, originalModel.PayrollApproverName, modifiedModel.PayrollApproverName, "Update: '{1}'");
                log += addLog(SettingsModel.COL_PayrollApproverName.Id, originalModel.PayrollApproverName_Notes, modifiedModel.PayrollApproverName_Notes, "Notes: '{1}'");
                log += addLog(SettingsModel.COL_PayrollApproverTitle.Id, originalModel.PayrollApproverTitle, modifiedModel.PayrollApproverTitle, "Update: '{1}'");
                log += addLog(SettingsModel.COL_PayrollApproverTitle.Id, originalModel.PayrollApproverTitle_Notes, modifiedModel.PayrollApproverTitle_Notes, "Notes: '{1}'");
                log += addLog(SettingsModel.COL_MultiplierRegularPayrate.Id, originalModel.MultiplierRegularPayrate, modifiedModel.MultiplierRegularPayrate, "Update: '{1}'");
                log += addLog(SettingsModel.COL_MultiplierRegularPayrate.Id, originalModel.MultiplierRegularPayrate_Notes, modifiedModel.MultiplierRegularPayrate_Notes, "Notes: '{1}'");
                log += addLog(SettingsModel.COL_MultiplierHolidayPayrate.Id, originalModel.MultiplierHolidayPayrate, modifiedModel.MultiplierHolidayPayrate, "Update: '{1}'");
                log += addLog(SettingsModel.COL_MultiplierHolidayPayrate.Id, originalModel.MultiplierHolidayPayrate_Notes, modifiedModel.MultiplierHolidayPayrate_Notes, "Notes: '{1}'");
                log += addLog(SettingsModel.COL_MultiplierRegularOvertimeHourlyPayrate.Id, originalModel.MultiplierRegularOvertimeHourlyPayrate, modifiedModel.MultiplierRegularOvertimeHourlyPayrate, "Update: '{1}'");
                log += addLog(SettingsModel.COL_MultiplierRegularOvertimeHourlyPayrate.Id, originalModel.MultiplierRegularOvertimeHourlyPayrate_Notes, modifiedModel.MultiplierRegularOvertimeHourlyPayrate_Notes, "Notes: '{1}'");
                log += addLog(SettingsModel.COL_MultiplierHolidayOvertimeHourlyPayrate.Id, originalModel.MultiplierHolidayOvertimeHourlyPayrate, modifiedModel.MultiplierHolidayOvertimeHourlyPayrate, "Update: '{1}'");
                log += addLog(SettingsModel.COL_MultiplierHolidayOvertimeHourlyPayrate.Id, originalModel.MultiplierHolidayOvertimeHourlyPayrate_Notes, modifiedModel.MultiplierHolidayOvertimeHourlyPayrate_Notes, "Notes: '{1}'");

                log += addLog(SettingsModel.COL_PaymentPercentageMonth1.Id, originalModel.PaymentPercentageMonth1, modifiedModel.PaymentPercentageMonth1, "Update: '{1}'");
                log += addLog(SettingsModel.COL_PaymentPercentageMonth1.Id, originalModel.PaymentPercentageMonth1_Notes, modifiedModel.PaymentPercentageMonth1_Notes, "Notes: '{1}'");
                log += addLog(SettingsModel.COL_PaymentPercentageMonth2.Id, originalModel.PaymentPercentageMonth2, modifiedModel.PaymentPercentageMonth2, "Update: '{1}'");
                log += addLog(SettingsModel.COL_PaymentPercentageMonth2.Id, originalModel.PaymentPercentageMonth2_Notes, modifiedModel.PaymentPercentageMonth2_Notes, "Notes: '{1}'");
                log += addLog(SettingsModel.COL_PaymentPercentageMonth3.Id, originalModel.PaymentPercentageMonth3, modifiedModel.PaymentPercentageMonth3, "Update: '{1}'");
                log += addLog(SettingsModel.COL_PaymentPercentageMonth3.Id, originalModel.PaymentPercentageMonth3_Notes, modifiedModel.PaymentPercentageMonth3_Notes, "Notes: '{1}'");

                //signature file upload
                string originalFilename = originalModel.PayrollApproverSignature;
                string newFilenamePath = System.IO.Path.Combine(Server.MapPath(Helper.IMAGEFOLDERPATH), originalFilename);
                if (uploadFiles != null && uploadFiles[0] != null)
                {
                    HttpPostedFileBase uploadFile = uploadFiles[0];

                    //delete original file
                    if (!string.IsNullOrEmpty(originalFilename) && System.IO.File.Exists(originalFilename))
                        System.IO.File.Delete(newFilenamePath);

                    //upload new file
                    uploadFile.SaveAs(System.IO.Path.Combine(Server.MapPath(Helper.IMAGEFOLDERPATH), uploadFile.FileName));

                    //prepare changes
                    modifiedModel.PayrollApproverSignature = uploadFile.FileName;
                    log += addLog(SettingsModel.COL_PayrollApproverSignature.Id, originalModel.PayrollApproverSignature, modifiedModel.PayrollApproverSignature, "Update: '{1}'");
                } 
                else
                {
                    modifiedModel.PayrollApproverSignature = originalModel.PayrollApproverSignature; //retain original value since no new file is uploaded
                }
                log += addLog(SettingsModel.COL_PayrollApproverSignature.Id, originalModel.PayrollApproverSignature_Notes, modifiedModel.PayrollApproverSignature_Notes, "Notes: '{1}'");

                //Update Database
                if (!string.IsNullOrEmpty(log))
                {
                    update(modifiedModel); //update setting values
                    db.SaveChanges(); //insert activity logs

                    return RedirectToAction(nameof(Edit));
                }
            }

            return View(modifiedModel);
        }

        /* METHODS ********************************************************************************************************************************************/
        #region METHODS

        public static SettingsModel get(DBContext db)
        {
            List<SettingsModel> models = db.Database.SqlQuery<SettingsModel>(@"
                SELECT
                    ISNULL(Settings_WorkHoursPerDay.Value_Int,0) AS WorkHoursPerDay,
                    ISNULL(Settings_WorkHoursPerDay.Notes,'') AS WorkHoursPerDay_Notes,
                    ISNULL(Settings_WorkDaysPerMonth.Value_Int,0) AS WorkDaysPerMonth,
                    ISNULL(Settings_WorkDaysPerMonth.Notes,'') AS WorkDaysPerMonth_Notes,
                    ISNULL(Settings_LeaveDaysPerYear.Value_Int,0) AS LeaveDaysPerYear,
                    ISNULL(Settings_LeaveDaysPerYear.Notes,'') AS LeaveDaysPerYear_Notes,
                    ISNULL(Settings_DepositAmountPerMonth.Value_Int,0) AS DepositAmountPerMonth,
                    ISNULL(Settings_DepositAmountPerMonth.Notes,'') AS DepositAmountPerMonth_Notes,
                    ISNULL(Settings_DepositAmountTotal.Value_Int,0) AS DepositAmountTotal,
                    ISNULL(Settings_DepositAmountTotal.Notes,'') AS DepositAmountTotal_Notes,
                    ISNULL(Settings_DepositFirstMonthAfterJoin.Value_Int,0) AS DepositFirstMonthAfterJoin,
                    ISNULL(Settings_DepositFirstMonthAfterJoin.Notes,'') AS DepositFirstMonthAfterJoin_Notes,
                    ISNULL(Settings_PayrollApproverName.Value_String,'') AS PayrollApproverName,
                    ISNULL(Settings_PayrollApproverName.Notes,'') AS PayrollApproverName_Notes,
                    ISNULL(Settings_PayrollApproverTitle.Value_String,'') AS PayrollApproverTitle,
                    ISNULL(Settings_PayrollApproverTitle.Notes,'') AS PayrollApproverTitle_Notes,
                    ISNULL(Settings_PayrollApproverSignature.Value_String,'') AS PayrollApproverSignature,
                    ISNULL(Settings_PayrollApproverSignature.Notes,'') AS PayrollApproverSignature_Notes,
                    ISNULL(Settings_MultiplierRegularPayrate.Value_Decimal,0) AS MultiplierRegularPayrate,
                    ISNULL(Settings_MultiplierRegularPayrate.Notes,'') AS MultiplierRegularPayrate_Notes,
                    ISNULL(Settings_MultiplierRegularOvertimeHourlyPayrate.Value_Decimal,0) AS MultiplierRegularOvertimeHourlyPayrate,
                    ISNULL(Settings_MultiplierRegularOvertimeHourlyPayrate.Notes,'') AS MultiplierRegularOvertimeHourlyPayrate_Notes,
                    ISNULL(Settings_MultiplierHolidayPayrate.Value_Decimal,0) AS MultiplierHolidayPayrate,
                    ISNULL(Settings_MultiplierHolidayPayrate.Notes,'') AS MultiplierHolidayPayrate_Notes,
                    ISNULL(Settings_MultiplierHolidayOvertimeHourlyPayrate.Value_Decimal,0) AS MultiplierHolidayOvertimeHourlyPayrate,
                    ISNULL(Settings_MultiplierHolidayOvertimeHourlyPayrate.Notes,'') AS MultiplierHolidayOvertimeHourlyPayrate_Notes,
                    ISNULL(Settings_PaymentPercentageMonth1.Value_Int,0) AS PaymentPercentageMonth1,
                    ISNULL(Settings_PaymentPercentageMonth1.Notes,'') AS PaymentPercentageMonth1_Notes,
                    ISNULL(Settings_PaymentPercentageMonth2.Value_Int,0) AS PaymentPercentageMonth2,
                    ISNULL(Settings_PaymentPercentageMonth2.Notes,'') AS PaymentPercentageMonth2_Notes,
                    ISNULL(Settings_PaymentPercentageMonth3.Value_Int,0) AS PaymentPercentageMonth3,
                    ISNULL(Settings_PaymentPercentageMonth3.Notes,'') AS PaymentPercentageMonth3_Notes
                FROM DWSystem.Settings Settings_WorkHoursPerDay
                    LEFT JOIN DWSystem.Settings Settings_WorkDaysPerMonth ON Settings_WorkDaysPerMonth.Id = @WorkDaysPerMonthId
                    LEFT JOIN DWSystem.Settings Settings_LeaveDaysPerYear ON Settings_LeaveDaysPerYear.Id = @LeaveDaysPerYearId
                    LEFT JOIN DWSystem.Settings Settings_DepositAmountPerMonth ON Settings_DepositAmountPerMonth.Id = @DepositAmountPerMonthId
                    LEFT JOIN DWSystem.Settings Settings_DepositAmountTotal ON Settings_DepositAmountTotal.Id = @DepositAmountTotalId
                    LEFT JOIN DWSystem.Settings Settings_DepositFirstMonthAfterJoin ON Settings_DepositFirstMonthAfterJoin.Id = @DepositFirstMonthAfterJoinId
                    LEFT JOIN DWSystem.Settings Settings_PayrollApproverName ON Settings_PayrollApproverName.Id = @PayrollApproverNameId
                    LEFT JOIN DWSystem.Settings Settings_PayrollApproverTitle ON Settings_PayrollApproverTitle.Id = @PayrollApproverTitleId
                    LEFT JOIN DWSystem.Settings Settings_PayrollApproverSignature ON Settings_PayrollApproverSignature.Id = @PayrollApproverSignatureId
                    LEFT JOIN DWSystem.Settings Settings_MultiplierRegularPayrate ON Settings_MultiplierRegularPayrate.Id = @MultiplierRegularPayrateId
                    LEFT JOIN DWSystem.Settings Settings_MultiplierRegularOvertimeHourlyPayrate ON Settings_MultiplierRegularOvertimeHourlyPayrate.Id = @MultiplierRegularOvertimeHourlyPayrateId
                    LEFT JOIN DWSystem.Settings Settings_MultiplierHolidayPayrate ON Settings_MultiplierHolidayPayrate.Id = @MultiplierHolidayPayrateId
                    LEFT JOIN DWSystem.Settings Settings_MultiplierHolidayOvertimeHourlyPayrate ON Settings_MultiplierHolidayOvertimeHourlyPayrate.Id = @MultiplierHolidayOvertimeHourlyPayrateId
                    LEFT JOIN DWSystem.Settings Settings_PaymentPercentageMonth1 ON Settings_PaymentPercentageMonth1.Id = @PaymentPercentageMonth1Id
                    LEFT JOIN DWSystem.Settings Settings_PaymentPercentageMonth2 ON Settings_PaymentPercentageMonth2.Id = @PaymentPercentageMonth2Id
                    LEFT JOIN DWSystem.Settings Settings_PaymentPercentageMonth3 ON Settings_PaymentPercentageMonth3.Id = @PaymentPercentageMonth3Id
                WHERE Settings_WorkHoursPerDay.Id = @WorkHoursPerDayId
                ", getSqlParametersForQuery()).ToList();

            return models.Count == 0 ? null : models[0];
        }

        private void update(SettingsModel modifiedModel)
        {
            db.Database.ExecuteSqlCommand(@"
                    UPDATE DWSystem.Settings SET Value_Int=@WorkHoursPerDay, Notes=@WorkHoursPerDay_Notes WHERE Id=@WorkHoursPerDayId;
                    UPDATE DWSystem.Settings SET Value_Int=@WorkDaysPerMonth, Notes=@WorkDaysPerMonth_Notes WHERE Id=@WorkDaysPerMonthId;
                    UPDATE DWSystem.Settings SET Value_Int=@LeaveDaysPerYear, Notes=@LeaveDaysPerYear_Notes WHERE Id=@LeaveDaysPerYearId;
                    UPDATE DWSystem.Settings SET Value_Int=@DepositAmountPerMonth, Notes=@DepositAmountPerMonth_Notes WHERE Id=@DepositAmountPerMonthId;
                    UPDATE DWSystem.Settings SET Value_Int=@DepositAmountTotal, Notes=@DepositAmountTotal_Notes WHERE Id=@DepositAmountTotalId;
                    UPDATE DWSystem.Settings SET Value_Int=@DepositFirstMonthAfterJoin, Notes=@DepositFirstMonthAfterJoin_Notes WHERE Id=@DepositFirstMonthAfterJoinId;
                    UPDATE DWSystem.Settings SET Value_String=@PayrollApproverName, Notes=@PayrollApproverName_Notes WHERE Id=@PayrollApproverNameId;
                    UPDATE DWSystem.Settings SET Value_String=@PayrollApproverTitle, Notes=@PayrollApproverTitle_Notes WHERE Id=@PayrollApproverTitleId;
                    UPDATE DWSystem.Settings SET Value_String=@PayrollApproverSignature, Notes=@PayrollApproverSignature_Notes WHERE Id=@PayrollApproverSignatureId;
                    UPDATE DWSystem.Settings SET Value_Decimal=@MultiplierRegularPayrate, Notes=@MultiplierRegularPayrate_Notes WHERE Id=@MultiplierRegularPayrateId;
                    UPDATE DWSystem.Settings SET Value_Decimal=@MultiplierRegularOvertimeHourlyPayrate, Notes=@MultiplierRegularOvertimeHourlyPayrate_Notes WHERE Id=@MultiplierRegularOvertimeHourlyPayrateId;
                    UPDATE DWSystem.Settings SET Value_Decimal=@MultiplierHolidayPayrate, Notes=@MultiplierHolidayPayrate_Notes WHERE Id=@MultiplierHolidayPayrateId;
                    UPDATE DWSystem.Settings SET Value_Decimal=@MultiplierHolidayOvertimeHourlyPayrate, Notes=@MultiplierHolidayOvertimeHourlyPayrate_Notes WHERE Id=@MultiplierHolidayOvertimeHourlyPayrateId;
                    UPDATE DWSystem.Settings SET Value_Int=@PaymentPercentageMonth1, Notes=@PaymentPercentageMonth1_Notes WHERE Id=@PaymentPercentageMonth1Id;
                    UPDATE DWSystem.Settings SET Value_Int=@PaymentPercentageMonth2, Notes=@PaymentPercentageMonth2_Notes WHERE Id=@PaymentPercentageMonth2Id;
                    UPDATE DWSystem.Settings SET Value_Int=@PaymentPercentageMonth3, Notes=@PaymentPercentageMonth3_Notes WHERE Id=@PaymentPercentageMonth3Id;
                ", getSqlParametersForUpdate(modifiedModel));
        }

        public static SqlParameter[] getSqlParametersForQuery()
        {
            return new SqlParameter[] {
                new SqlParameter(SettingsModel.COL_WorkHoursPerDay.Name+"Id", SettingsModel.COL_WorkHoursPerDay.Id),
                new SqlParameter(SettingsModel.COL_WorkDaysPerMonth.Name+"Id", SettingsModel.COL_WorkDaysPerMonth.Id),
                new SqlParameter(SettingsModel.COL_LeaveDaysPerYear.Name+"Id", SettingsModel.COL_LeaveDaysPerYear.Id),
                new SqlParameter(SettingsModel.COL_DepositAmountPerMonth.Name+"Id", SettingsModel.COL_DepositAmountPerMonth.Id),
                new SqlParameter(SettingsModel.COL_DepositAmountTotal.Name+"Id", SettingsModel.COL_DepositAmountTotal.Id),
                new SqlParameter(SettingsModel.COL_DepositFirstMonthAfterJoin.Name+"Id", SettingsModel.COL_DepositFirstMonthAfterJoin.Id),
                new SqlParameter(SettingsModel.COL_PayrollApproverName.Name+"Id", SettingsModel.COL_PayrollApproverName.Id),
                new SqlParameter(SettingsModel.COL_PayrollApproverTitle.Name+"Id", SettingsModel.COL_PayrollApproverTitle.Id),
                new SqlParameter(SettingsModel.COL_PayrollApproverSignature.Name+"Id", SettingsModel.COL_PayrollApproverSignature.Id),
                new SqlParameter(SettingsModel.COL_MultiplierRegularPayrate.Name+"Id", SettingsModel.COL_MultiplierRegularPayrate.Id),
                new SqlParameter(SettingsModel.COL_MultiplierRegularOvertimeHourlyPayrate.Name+"Id", SettingsModel.COL_MultiplierRegularOvertimeHourlyPayrate.Id),
                new SqlParameter(SettingsModel.COL_MultiplierHolidayPayrate.Name+"Id", SettingsModel.COL_MultiplierHolidayPayrate.Id),
                new SqlParameter(SettingsModel.COL_MultiplierHolidayOvertimeHourlyPayrate.Name+"Id", SettingsModel.COL_MultiplierHolidayOvertimeHourlyPayrate.Id),
                new SqlParameter(SettingsModel.COL_PaymentPercentageMonth1.Name+"Id", SettingsModel.COL_PaymentPercentageMonth1.Id),
                new SqlParameter(SettingsModel.COL_PaymentPercentageMonth2.Name+"Id", SettingsModel.COL_PaymentPercentageMonth2.Id),
                new SqlParameter(SettingsModel.COL_PaymentPercentageMonth3.Name+"Id", SettingsModel.COL_PaymentPercentageMonth3.Id)
            };
        }

        public SqlParameter[] getSqlParametersForUpdate(SettingsModel model)
        {
            return getSqlParametersForQuery().Concat(new SqlParameter[] {
                new SqlParameter(SettingsModel.COL_WorkHoursPerDay.Name, Util.wrapNullable(model.WorkHoursPerDay)),
                new SqlParameter(SettingsModel.COL_WorkHoursPerDay_Notes.Name, Util.wrapNullable(model.WorkHoursPerDay_Notes)),
                new SqlParameter(SettingsModel.COL_WorkDaysPerMonth.Name, Util.wrapNullable(model.WorkDaysPerMonth)),
                new SqlParameter(SettingsModel.COL_WorkDaysPerMonth_Notes.Name, Util.wrapNullable(model.WorkDaysPerMonth_Notes)),
                new SqlParameter(SettingsModel.COL_LeaveDaysPerYear.Name, Util.wrapNullable(model.LeaveDaysPerYear)),
                new SqlParameter(SettingsModel.COL_LeaveDaysPerYear_Notes.Name, Util.wrapNullable(model.LeaveDaysPerYear_Notes)),
                new SqlParameter(SettingsModel.COL_DepositAmountPerMonth.Name, Util.wrapNullable(model.DepositAmountPerMonth)),
                new SqlParameter(SettingsModel.COL_DepositAmountPerMonth_Notes.Name, Util.wrapNullable(model.DepositAmountPerMonth_Notes)),
                new SqlParameter(SettingsModel.COL_DepositAmountTotal.Name, Util.wrapNullable(model.DepositAmountTotal)),
                new SqlParameter(SettingsModel.COL_DepositAmountTotal_Notes.Name, Util.wrapNullable(model.DepositAmountTotal_Notes)),
                new SqlParameter(SettingsModel.COL_DepositFirstMonthAfterJoin.Name, Util.wrapNullable(model.DepositFirstMonthAfterJoin)),
                new SqlParameter(SettingsModel.COL_DepositFirstMonthAfterJoin_Notes.Name, Util.wrapNullable(model.DepositFirstMonthAfterJoin_Notes)),
                new SqlParameter(SettingsModel.COL_PayrollApproverName.Name, Util.wrapNullable(model.PayrollApproverName)),
                new SqlParameter(SettingsModel.COL_PayrollApproverName_Notes.Name, Util.wrapNullable(model.PayrollApproverName_Notes)),
                new SqlParameter(SettingsModel.COL_PayrollApproverTitle.Name, Util.wrapNullable(model.PayrollApproverTitle)),
                new SqlParameter(SettingsModel.COL_PayrollApproverTitle_Notes.Name, Util.wrapNullable(model.PayrollApproverTitle_Notes)),
                new SqlParameter(SettingsModel.COL_PayrollApproverSignature.Name, Util.wrapNullable(model.PayrollApproverSignature)),
                new SqlParameter(SettingsModel.COL_PayrollApproverSignature_Notes.Name, Util.wrapNullable(model.PayrollApproverSignature_Notes)),
                new SqlParameter(SettingsModel.COL_MultiplierRegularPayrate.Name, Util.wrapNullable(model.MultiplierRegularPayrate)),
                new SqlParameter(SettingsModel.COL_MultiplierRegularPayrate_Notes.Name, Util.wrapNullable(model.MultiplierRegularPayrate_Notes)),
                new SqlParameter(SettingsModel.COL_MultiplierRegularOvertimeHourlyPayrate.Name,  Util.wrapNullable(model.MultiplierRegularOvertimeHourlyPayrate)),
                new SqlParameter(SettingsModel.COL_MultiplierRegularOvertimeHourlyPayrate_Notes.Name, Util.wrapNullable(model.MultiplierRegularOvertimeHourlyPayrate_Notes)),
                new SqlParameter(SettingsModel.COL_MultiplierHolidayPayrate.Name,  Util.wrapNullable(model.MultiplierHolidayPayrate)),
                new SqlParameter(SettingsModel.COL_MultiplierHolidayPayrate_Notes.Name, Util.wrapNullable(model.MultiplierHolidayPayrate_Notes)),
                new SqlParameter(SettingsModel.COL_MultiplierHolidayOvertimeHourlyPayrate.Name,  Util.wrapNullable(model.MultiplierHolidayOvertimeHourlyPayrate)),
                new SqlParameter(SettingsModel.COL_MultiplierHolidayOvertimeHourlyPayrate_Notes.Name, Util.wrapNullable(model.MultiplierHolidayOvertimeHourlyPayrate_Notes)),
                new SqlParameter(SettingsModel.COL_PaymentPercentageMonth1.Name,  Util.wrapNullable(model.PaymentPercentageMonth1)),
                new SqlParameter(SettingsModel.COL_PaymentPercentageMonth1_Notes.Name, Util.wrapNullable(model.PaymentPercentageMonth1_Notes)),
                new SqlParameter(SettingsModel.COL_PaymentPercentageMonth2.Name,  Util.wrapNullable(model.PaymentPercentageMonth2)),
                new SqlParameter(SettingsModel.COL_PaymentPercentageMonth2_Notes.Name, Util.wrapNullable(model.PaymentPercentageMonth2_Notes)),
                new SqlParameter(SettingsModel.COL_PaymentPercentageMonth3.Name,  Util.wrapNullable(model.PaymentPercentageMonth3)),
                new SqlParameter(SettingsModel.COL_PaymentPercentageMonth3_Notes.Name, Util.wrapNullable(model.PaymentPercentageMonth3_Notes))
            }).ToArray();
        }

        private string addLog(Guid reffId, object oldValue, object newValue, string format)
        {
            string log = string.Empty;
            log = Util.webAppendChange(log, oldValue, newValue, format);
            if (!string.IsNullOrEmpty(log))
                ActivityLogsController.Add(db, Session, reffId, log);

            return log;
        }

        //improvement: when there is uploaded file, show the image of that one. otherwise, show new uploaded file
        public string getImageUrl(string previousImageName, string newImageName)
        {
            if(!string.IsNullOrEmpty(newImageName))
                return Helper.getImageUrl(newImageName, Request, Server);
            else
                return Helper.getImageUrl(previousImageName, Request, Server);
        }

        #endregion METHODS
        /******************************************************************************************************************************************************/
    }
}
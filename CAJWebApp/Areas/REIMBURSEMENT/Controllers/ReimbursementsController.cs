using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Newtonsoft.Json;
using CAJWebApp.Areas.Reimbursement.Models;
using CAJWebApp.Areas.PAYROLL.Controllers;
using CAJWebApp.Controllers;
using CAJWebApp.Models;
using LIBUtil;
using LIBExcel;

namespace CAJWebApp.Areas.Reimbursement.Controllers
{
    public class ReimbursementsController : Controller
    {
        private readonly DBContext db = new DBContext();
        
        /* INDEX **********************************************************************************************************************************************/

        // GET: PAYROLL/Reimbursements
        public ActionResult Index(int? rss, DateTime? PayPeriod, int? year, int? month, int? payDate, int? approval, string FILTER_Banks_Id, string search, string periodChange)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            ReimbursementPaymentDatesController.setDropDownListViewBag(db, this);
            BanksController.setDropDownListViewBag(db, this);

            DateTime payPeriod = Helper.setFilterViewBag(this, PayPeriod, year, month, payDate, approval, FILTER_Banks_Id, search, periodChange, null);

            List<ReimbursementsModel> models = get(payPeriod, approval, FILTER_Banks_Id);
            ViewBag.TotalApprovedPayableAmount = string.Format("{0:N0}", models.Where(x => x.ApprovalOperator_ID != null).Sum(x => x.PayableAmount));
            ViewBag.TotalApprovedDueAmount = string.Format("{0:N0}", models.Where(x => x.ApprovalOperator_ID != null).Sum(x => x.PayableAmount - x.PaymentAmount));

            return View(models);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PAYROLL/Reimbursements/Create
        public ActionResult Create(int? year, int? month, int? payDate, int? approval, string Banks_Id, string search)
        {
            if (year == null || month == null)
                return RedirectToAction(nameof(Index));

            DateTime payPeriod = Helper.setFilterViewBag(this, null, year, month, payDate, approval, Banks_Id, search, null, null);
            PayrollEmployeesController.setDropDownListViewBag(db, this, payPeriod, EnumActionTypes.Reimbursement);

            return View(new ReimbursementsModel());
        }

        // POST: PAYROLL/Reimbursements/Generate/id
        public ActionResult Generate(Guid id, int year, int month, int? payDate, int? approval, string Banks_Id, string search)
        {
            Guid Reimbursements_Id = Guid.NewGuid();
            DateTime payPeriod = Helper.setFilterViewBag(this, null, year, month, payDate, approval, Banks_Id, search, null, null);
            add(Reimbursements_Id, id, payPeriod);

            return RedirectToAction(nameof(Edit), new { id = Reimbursements_Id, year = year, month = month, approval = approval, Banks_Id = Banks_Id, search = search });
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PAYROLL/Reimbursements/Edit/{id}
        public ActionResult Edit(Guid? id, int? year, int? month, int? approval, string Banks_Id, string search)
        {
            if(id == null)
                return RedirectToAction(nameof(Index));

            setEditViewBag(year, month, approval, Banks_Id, search);
            return View(get((Guid)id));
        }

        // POST: PAYROLL/Reimbursements/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReimbursementsModel modifiedModel)
        {
            //Util.debug(ModelState, ViewData);
            
            if (ModelState.IsValid)
            {
                ReimbursementsModel originalModel = get(modifiedModel.Id);

                string log = string.Empty;
                log = Util.webAppendChange(log, originalModel.EmployeeFullName, modifiedModel.EmployeeFullName, ActivityLogsController.editStringFormat(ReimbursementsModel.COL_EmployeeFullName.Display));
                log = Util.webAppendChange(log, originalModel.CustomerName, modifiedModel.CustomerName, ActivityLogsController.editStringFormat(ReimbursementsModel.COL_CustomerName.Display));
                log = Util.webAppendChange(log, originalModel.JoinDate, modifiedModel.JoinDate, ActivityLogsController.editDateFormat(ReimbursementsModel.COL_JoinDate.Display));
                log = Util.webAppendChange(log, originalModel.RowNumber, modifiedModel.RowNumber, ActivityLogsController.editIntFormat(ReimbursementsModel.COL_RowNumber.Display));
                log = Util.webAppendChange(log, originalModel.PaymentDate, modifiedModel.PaymentDate, ActivityLogsController.editIntFormat(ReimbursementsModel.COL_PaymentDate.Display));
                log = Util.webAppendChange(log, originalModel.Notes, modifiedModel.Notes, ActivityLogsController.editStringFormat(ReimbursementsModel.COL_Notes.Display));

                log = processItems(modifiedModel, log);

                log = Util.webAppendChange(log, originalModel.ApproverName, modifiedModel.ApproverName, ActivityLogsController.editIntFormat(ReimbursementsModel.COL_ApproverName.Display));
                log = Util.webAppendChange(log, originalModel.ApproverTitle, modifiedModel.ApproverTitle, ActivityLogsController.editIntFormat(ReimbursementsModel.COL_ApproverTitle.Display));
                log = Util.webAppendChange(log, originalModel.ApproverSignatureFilename, modifiedModel.ApproverSignatureFilename, ActivityLogsController.editIntFormat(ReimbursementsModel.COL_ApproverSignatureFilename.Display));

                if (!string.IsNullOrEmpty(log))
                {
                    update(modifiedModel);
                    ActivityLogsController.AddEditLog(db, Session, modifiedModel.Id, log);
                    db.SaveChanges();
                }

                //approval
                if(originalModel.ApprovalOperator_ID != modifiedModel.ApprovalOperator_ID)
                    update_ApprovalOperator_ID(originalModel.ApprovalOperator_ID == null ? EnumActions.Approve.ToString() : EnumActions.Cancel.ToString(), modifiedModel.Id);

                return RedirectToAction(nameof(Index), new { 
                    year = modifiedModel.FILTER_PayPeriodYear, 
                    month = modifiedModel.FILTER_PayPeriodMonth, 
                    search = modifiedModel.FILTER_Search 
                });
            }

            setEditViewBag(modifiedModel.FILTER_PayPeriodYear, modifiedModel.FILTER_PayPeriodMonth, modifiedModel.FILTER_Approval, modifiedModel.FILTER_Banks_Id, modifiedModel.FILTER_Search);
            return View(modifiedModel);
        }

        private void setEditViewBag(int? year, int? month, int? approval, string Banks_Id, string search)
        {
            ReimbursementCategoriesController.setDropDownListViewBag(db, this);

            Helper.setFilterViewBag(this, null, year, month, null, approval, Banks_Id, search, null, null);
        }

        /* PRINT **********************************************************************************************************************************************/

        // GET: REIMBURSEMENT/Reimbursements/Print/{id, year, month, paymentDate, approval, banks_id, search}
        public ActionResult Print(Guid? id, int? year, int? month, int? approval, string Banks_Id, string search)
        {
            return View(get(id, Helper.setFilterViewBag(this, null, year, month, null, approval, Banks_Id, search, null, null), approval, Banks_Id));
        }

        public ActionResult PrintToPdf(Guid? id, string name, DateTime? payPeriod, int? year, int? month, int? payDate, string search)
        {
            string filename = "Reimbursement";

            if (!string.IsNullOrEmpty(name))
                filename += " " + name;

            if (payPeriod != null)
                filename += string.Format(" {0:yyyy-MM}", payPeriod);

            filename += ".pdf";

            return new Rotativa.ActionAsPdf("Print", new { id = id, year = year, month = month, payDate = payDate, search = search })
            { FileName = filename };
        }

        /* METHODS ********************************************************************************************************************************************/

        private string processItems(ReimbursementsModel modifiedModel, string log)
        {
            if (!string.IsNullOrEmpty(modifiedModel.ReimbursementItemsListString))
            {
                List<ReimbursementItemsModel> originalReimbursementItemsList = ReimbursementItemsController.get(db, modifiedModel.Id);
                modifiedModel.ReimbursementItemsList = JsonConvert.DeserializeObject<List<ReimbursementItemsModel>>(modifiedModel.ReimbursementItemsListString);
                byte rowNo = 0;

                //remove all items without amount
                for (int i = modifiedModel.ReimbursementItemsList.Count - 1; i >= 0; i--)
                {
                    if (modifiedModel.ReimbursementItemsList[i].Amount == null || modifiedModel.ReimbursementItemsList[i].Amount == 0)
                        modifiedModel.ReimbursementItemsList.RemoveAt(i);
                }

                foreach (ReimbursementItemsModel item in modifiedModel.ReimbursementItemsList)
                {
                    item.RowNumber = ++rowNo;

                    if (!item.Id.HasValue) //new item
                    {
                        item.Id = Guid.NewGuid();
                        item.Reimbursements_Id = modifiedModel.Id;

                        db.ReimbursementItemsModel.Add(item);
                        log = Util.append(log, string.Format("Add {0}: '{1}' [{2:N0}]", item.CategoryName, item.Description, item.Amount), "<BR>");
                    }
                    else //existing item
                    {
                        ReimbursementItemsModel oldItem = originalReimbursementItemsList.Where(x => x.Id == item.Id).FirstOrDefault();

                        if (oldItem.CategoryName != item.CategoryName ||
                            oldItem.Description != item.Description ||
                            oldItem.Amount != item.Amount)
                        {
                            log = Util.append(log, string.Format("Edit {0}: '{1}' [{2:N0}] to {3}: '{4}' [{5:N0}]",
                                oldItem.CategoryName, oldItem.Description, oldItem.Amount,
                                item.CategoryName, item.Description, item.Amount), "<BR>"); ;

                            oldItem.RowNumber = rowNo;
                            oldItem.ReimbursementCategories_Id = item.ReimbursementCategories_Id;
                            oldItem.CategoryName = item.CategoryName;
                            oldItem.Description = item.Description;
                            oldItem.Amount = item.Amount;
                            db.Entry(oldItem).State = EntityState.Modified;
                        }
                    }

                }
                foreach (ReimbursementItemsModel item in originalReimbursementItemsList)
                {
                    if (!modifiedModel.ReimbursementItemsList.Any(x => x.Id == item.Id))
                    {
                        db.ReimbursementItemsModel.Remove(item);
                        log = Util.append(log, string.Format("Delete {0}: '{1}' [{2:N0}]", item.CategoryName, item.Description, item.Amount), "<BR>");
                    }
                }            
            }

            return log;
        }

        private List<ReimbursementsModel> get() { return getData(null, null, null, null); }
        private ReimbursementsModel get(Guid id) { return getData(id, null, null, null).FirstOrDefault(); }
        public List<ReimbursementsModel> get(DateTime? payPeriod, int? approvalStatus, string Banks_Id) { return getData(null, payPeriod, approvalStatus, Banks_Id); }
        private List<ReimbursementsModel> get(Guid? id, DateTime? payPeriod, int? approvalStatus, string Banks_Id) { return getData(id, payPeriod, approvalStatus, Banks_Id); }
        private List<ReimbursementsModel> getData(Guid? id, DateTime? payPeriod, int? approvalStatus, string banks_Id)
        {
            Guid? Banks_Id = null;
            if (!string.IsNullOrEmpty(banks_Id))
                Banks_Id = new Guid(banks_Id);

            List<ReimbursementsModel> models = db.Database.SqlQuery<ReimbursementsModel>(@"
                        SELECT Reimbursements.*,
                            CONVERT(BIGINT,ISNULL(ReimbursementItems.Amount,0)) AS PayableAmount,
							CONVERT(BIGINT,ISNULL(Payments.Amount,0)) AS PaymentAmount,
                            NULL AS ReimbursementItemsList,
                            '' AS ReimbursementItemsListString,
                            NULL AS FILTER_PayPeriodYear,
                            NULL AS FILTER_PayPeriodMonth,
                            NULL AS FILTER_Approval,
                            NULL AS FILTER_Banks_Id,
                            NULL AS FILTER_Search
                        FROM DWSystem.Reimbursements
                            LEFT JOIN DWSystem.PayrollEmployees ON PayrollEmployees.Id = Reimbursements.PayrollEmployees_Id
							LEFT JOIN (
									SELECT ReimbursementItems.Reimbursements_Id,ISNULL(SUM(ReimbursementItems.Amount),0) Amount
									FROM DWSystem.ReimbursementItems 
									GROUP BY ReimbursementItems.Reimbursements_Id
								) ReimbursementItems ON ReimbursementItems.Reimbursements_Id = Reimbursements.Id
							LEFT JOIN (
									SELECT PayrollPayments.Reimbursements_Id,ISNULL(SUM(PayrollPayments.Amount),0) Amount
									FROM DWSystem.PayrollPayments 
									WHERE PayrollPayments.Cancelled = 0
									GROUP BY PayrollPayments.Reimbursements_Id
								) Payments ON Payments.Reimbursements_Id = Reimbursements.Id
                        WHERE 1=1
                            AND (@Id IS NULL OR Reimbursements.Id = @Id)
							AND (@Id IS NOT NULL OR (
								(@Banks_Id IS NULL OR PayrollEmployees.Banks_Id = @Banks_Id)
								AND (@PayPeriod IS NULL OR Reimbursements.PayPeriod = @PayPeriod)
								AND (@FILTER_ApprovalStatus IS NULL OR 
										(@FILTER_ApprovalStatus = 0 AND Reimbursements.ApprovalOperator_ID IS NULL)
										OR (@FILTER_ApprovalStatus = 1 AND Reimbursements.ApprovalOperator_ID IS NOT NULL)
									)
							))
						ORDER BY Reimbursements.RowNumber ASC, Reimbursements.EmployeeFullName ASC
                    ", 
                    DBConnection.getSqlParameter(ReimbursementsModel.COL_Id.Name, id),
                    DBConnection.getSqlParameter(ReimbursementsModel.COL_PayPeriod.Name, payPeriod),
                    DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Banks_Id.Name, Banks_Id),
                    DBConnection.getSqlParameter("FILTER_ApprovalStatus", approvalStatus)
                ).ToList();

            foreach(ReimbursementsModel model in models)
            {
                model.ReimbursementItemsList = ReimbursementItemsController.get(db, model.Id);
                model.PaymentList = new PayrollPaymentsController().get(null, null, null, false, null, model.Id, null);
            }

            return models;
        }

        private void add(Guid id, Guid payrollEmployees_Id, DateTime payPeriod)
        {
            PayrollEmployeesModel payrollEmployee = new PayrollEmployeesController().get(payrollEmployees_Id);

            //prevent error when pay date is not valid such as feb 29, nov 31, etc.
            DateTime PaymentDate = new DateTime(payPeriod.Year, payPeriod.Month, 1).AddMonths(1);
            try
            {
                PaymentDate = new DateTime(PaymentDate.Year, PaymentDate.Month, payrollEmployee.ReimbursementPaymentDates_PayDate, 0, 0, 0);
            }
            catch
            {
                PaymentDate = (DateTime)Util.getLastDayOfSelectedMonth(PaymentDate);
            }

            db.Database.ExecuteSqlCommand(@"                   
                    INSERT INTO DWSystem.Reimbursements (
                        Id,
                        PayrollEmployees_Id,
                        EmployeeFullName,
                        JoinDate,
                        Customer_CustomerID,
                        CustomerName,
                        PayPeriod,
						ApproverName,
						ApproverTitle,
                        ApproverSignatureFilename,
						PaymentDate,
						RowNumber
                    ) VALUES (
                        @Id,
                        @PayrollEmployees_Id,
                        @EmployeeFullName,
                        @JoinDate,
                        @Customer_CustomerID,
                        @CustomerName,
                        @PayPeriod,
						(SELECT ISNULL(Value_String,'') FROM DWSystem.Settings WHERE Settings.Id = @ApproverName),
						(SELECT ISNULL(Value_String,'') FROM DWSystem.Settings WHERE Settings.Id = @ApproverTitle),
						(SELECT ISNULL(Value_String,'') FROM DWSystem.Settings WHERE Settings.Id = @ApproverSignatureFilename),
						@PaymentDate,
						(SELECT ISNULL(MAX(Reimbursements.RowNumber),0)+1 FROM DWSystem.Reimbursements WHERE Reimbursements.PayPeriod = @PayPeriod)
                    )
                ",
                DBConnection.getSqlParameter(ReimbursementsModel.COL_Id.Name, id),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_PayrollEmployees_Id.Name, payrollEmployee.Id),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_EmployeeFullName.Name, payrollEmployee.Fullname),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_JoinDate.Name, payrollEmployee.JoinDate),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_Customer_CustomerID.Name, payrollEmployee.Customer_CustomerID),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_CustomerName.Name, payrollEmployee.Customer_Name),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_PayPeriod.Name, payPeriod),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_ApproverName.Name, SettingsModel.COL_PayrollApproverName.Id),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_ApproverTitle.Name, SettingsModel.COL_PayrollApproverTitle.Id),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_ApproverSignatureFilename.Name, SettingsModel.COL_PayrollApproverSignature.Id),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_PaymentDate.Name, PaymentDate)
            );
        }

        private void update(ReimbursementsModel modifiedModel)
        {
            db.Database.ExecuteSqlCommand(@"
                    UPDATE DWSystem.Reimbursements 
                    SET 
                        EmployeeFullName = @EmployeeFullName,
                        JoinDate = @JoinDate,
                        Customer_CustomerID = @Customer_CustomerID,
                        CustomerName = @CustomerName,
                        PayPeriod = @PayPeriod,
                        ApproverName = @ApproverName,
                        ApproverTitle = @ApproverTitle,
                        ApproverSignatureFilename = @ApproverSignatureFilename,
                        ApprovalOperator_ID = @ApprovalOperator_ID,
                        Notes = @Notes,
                        PaymentDate = @PaymentDate,
                        RowNumber = @RowNumber
                    WHERE Id=@Id;
                ",
                DBConnection.getSqlParameter(ReimbursementsModel.COL_Id.Name, modifiedModel.Id),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_EmployeeFullName.Name, modifiedModel.EmployeeFullName),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_JoinDate.Name, modifiedModel.JoinDate),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_Customer_CustomerID.Name, modifiedModel.Customer_CustomerID),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_CustomerName.Name, modifiedModel.CustomerName),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_PayPeriod.Name, modifiedModel.PayPeriod),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_ApproverName.Name, modifiedModel.ApproverName),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_ApproverTitle.Name, modifiedModel.ApproverTitle),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_ApproverSignatureFilename.Name, modifiedModel.ApproverSignatureFilename),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_ApprovalOperator_ID.Name, modifiedModel.ApprovalOperator_ID),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_Notes.Name, modifiedModel.Notes),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_PaymentDate.Name, modifiedModel.PaymentDate),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_RowNumber.Name, modifiedModel.RowNumber)
            );
        }

        public JsonResult update_ApprovalOperator_ID(string key, Guid id)
        {
            int? ApprovalOperator_ID = null;
            if(key == EnumActions.Approve.ToString()) 
                ApprovalOperator_ID = OperatorController.getUserId(Session);

            db.Database.ExecuteSqlCommand(@"
                    UPDATE DWSystem.Reimbursements 
                    SET 
                        ApprovalOperator_ID = @ApprovalOperator_ID
                    WHERE Id=@Id;
                ",
                DBConnection.getSqlParameter(ReimbursementsModel.COL_Id.Name, id),
                DBConnection.getSqlParameter(ReimbursementsModel.COL_ApprovalOperator_ID.Name, ApprovalOperator_ID)
            );

            ActivityLogsController.Add(db, Session, id, key == EnumActions.Approve.ToString() ? "Approved" : "Approval Cancelled");
            db.SaveChanges();

            return Json(new { status = "200" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRegenerateData(Guid id)
        {
            ReimbursementsModel model = get(id);
            PayrollEmployeesModel payrollEmployee = new PayrollEmployeesController().get(model.PayrollEmployees_Id);
            model.EmployeeFullName = payrollEmployee.Fullname;
            model.Customer_CustomerID = payrollEmployee.Customer_CustomerID;
            model.CustomerName = payrollEmployee.Customer_Name;
            model.JoinDate = payrollEmployee.JoinDate;

            SettingsModel settings = SettingsController.get(db);
            model.ApproverName = settings.PayrollApproverName;
            model.ApproverTitle = settings.PayrollApproverTitle;
            model.ApproverSignatureFilename = settings.PayrollApproverSignature;

            return Json(new { model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJsonTotalMonthsSinceJoin(DateTime JoinDate, DateTime PayPeriod) {
            string TotalMonthsSinceJoin = Helper.GetTotalMonthsSinceJoin(JoinDate, PayPeriod);
            return Json(new { TotalMonthsSinceJoin }, JsonRequestBehavior.AllowGet);
        }

        /* REPORT *********************************************************************************************************************************************/

        public ActionResult Report(int year, int month, int? approval, string Banks_Id)
        {
            DateTime payPeriod = new DateTime(year, month, 1, 0, 0, 0);
            string filename = string.Format("Reimbursement {0:yyyy-MM}.xlsx", payPeriod);

            if (!string.IsNullOrEmpty(Banks_Id))
                filename += " " + db.BanksModel.Where(x => x.Id.ToString() == Banks_Id).FirstOrDefault().Name;

            if (approval == 0)
                filename += " Approved";
            else if (approval == 1)
                filename += " Not Approved";

            return Excel.GenerateExcelReport(filename, CompileExcelPackage(get(payPeriod, approval, Banks_Id)));
        }

        public ExcelPackage CompileExcelPackage(List<ReimbursementsModel> models)
        {
            ExcelPackage excelPackage = new ExcelPackage();

            Excel.SetWorkbookProperties(excelPackage);
            var workbook = excelPackage.Workbook;
            var ws = workbook.Worksheets.Add("Sheet1");

            /***********************************************************************************************************************************************
             * BUILD HEADERS
             **********************************************************************************************************************************************/

            int colIdxHeader = 0;
            int rowIdxHeaderGroup = 1;
            int rowIdxHeader = 2;

            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 5, "No", null, 0, 1, null);
            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 30, "Nama", null, 0, 1, null);
            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 30, "Counter", null, 0, 1, null);

            Dictionary<Guid?, string> availableItems = new Dictionary<Guid?, string>();
            List<ReimbursementItemsModel> reimbursementItems = db.ReimbursementItemsModel.ToList();
            List<ExcelCellFormat> headers = new List<ExcelCellFormat>();
            foreach (ReimbursementsModel model in models)
            {
                if(model.ReimbursementItemsList != null)
                { 
                    foreach (ReimbursementItemsModel item in model.ReimbursementItemsList)
                    {
                        if(!availableItems.ContainsKey(item.ReimbursementCategories_Id))
                        {
                            string name = reimbursementItems.Find(x => x.ReimbursementCategories_Id == item.ReimbursementCategories_Id).CategoryName;
                            availableItems.Add(item.ReimbursementCategories_Id, name);
                            headers.Add(new ExcelCellFormat(15, ++colIdxHeader, name));
                            headers.Add(new ExcelCellFormat(15, ++colIdxHeader, "Deskripsi"));
                        }
                    }
                }
            }
            if (headers.Count > 0)
                Excel.editCellGroup(ws, rowIdxHeaderGroup, colIdxHeader - (headers.Count - 1), 10, "LIST", null, headers.ToArray());

            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "Total", null, 0, 1, null);
            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "Payment", null, 0, 1, null);
            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "Due", null, 0, 1, null);

            //header styling
            ws.Cells[rowIdxHeaderGroup, 1, rowIdxHeaderGroup, colIdxHeader].Style.Font.Bold = true;
            ws.Cells[rowIdxHeader, 1, rowIdxHeader, colIdxHeader].Style.Font.Bold = true;
            ws.Cells[rowIdxHeaderGroup, 1, rowIdxHeaderGroup, colIdxHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            ws.Cells[rowIdxHeader, 1, rowIdxHeader, colIdxHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var headerBorder = ws.Cells[rowIdxHeaderGroup, 1, rowIdxHeader, colIdxHeader].Style.Border;
            headerBorder.Left.Style = headerBorder.Top.Style = headerBorder.Right.Style = headerBorder.Bottom.Style = ExcelBorderStyle.Thin;
            var headerFill = ws.Cells[rowIdxHeaderGroup, 1, rowIdxHeader, colIdxHeader].Style.Fill;
            headerFill.PatternType = ExcelFillStyle.Solid;
            headerFill.BackgroundColor.SetColor(Color.LightGray);

            /***********************************************************************************************************************************************
             * POPULATE DATA
             **********************************************************************************************************************************************/

            int rowIdx = rowIdxHeader + 1;
            int colIdx = 0;
            ws.View.FreezePanes(rowIdx, 1);
            string description = "";
            if (models.Count > 0)
            {
                foreach (ReimbursementsModel item in models.OrderBy(x => x.RowNumber))
                {
                    colIdx = 0;
                    Excel.editCell(ws, rowIdx, ++colIdx, item.RowNumber, null);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.EmployeeFullName, null);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.CustomerName, null);

                    foreach (ReimbursementItemsModel payrollitem in item.ReimbursementItemsList)
                    {
                        ExcelCellFormat header = headers.Find(x => x.Text == payrollitem.CategoryName);
                        object cellValue = Excel.getCellValue(ws, rowIdx, header.ColumnIndex);
                        int amount = (int)payrollitem.Amount + (cellValue == null ? 0 : Convert.ToInt32(cellValue));

                        Excel.editCell(ws, rowIdx, header.ColumnIndex, amount, "#,##0", ExcelHorizontalAlignment.Right);
                        description = Util.wrapNullable<string>(Excel.getCellValue(ws, rowIdx, header.ColumnIndex + 1)) ?? "";
                        description = Util.append(description, payrollitem.Description ?? "", ",");
                        Excel.editCell(ws, rowIdx, header.ColumnIndex + 1, description, "");
                    }
                    colIdx += headers.Count;

                    Excel.editCell(ws, rowIdx, ++colIdx, item.PayableAmount, "#,##0", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.PaymentAmount, "#,##0", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.PayableAmount - item.PaymentAmount, "#,##0", ExcelHorizontalAlignment.Right);

                    rowIdx++;
                }
            }
            else
            {
                ws.Cells[rowIdx, 1].Value = "No Data Available";
                ws.Cells[rowIdx, 1, rowIdx, colIdxHeader].Merge = true;
                ws.Cells[rowIdx, 1, rowIdx, colIdxHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }

            Excel.setCellBorders(ws, rowIdxHeader + 1, 1, rowIdx, colIdx, ExcelBorderStyle.Thin);

            return excelPackage;
        }

        /******************************************************************************************************************************************************/
    }
}
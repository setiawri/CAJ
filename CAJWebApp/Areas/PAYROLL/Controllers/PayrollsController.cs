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
using CAJWebApp.Areas.PAYROLL.Models;
using CAJWebApp.Controllers;
using CAJWebApp.Models;
using LIBUtil;
using LIBWebMVC;
using LIBExcel;

namespace CAJWebApp.Areas.PAYROLL.Controllers
{
    public class PayrollsController : Controller
    {
        private readonly DBContext db = new DBContext();
        
        /* INDEX **********************************************************************************************************************************************/

        // GET: PAYROLL/Payrolls
        public ActionResult Index(int? rss, DateTime? PayPeriod, int? year, int? month, int? payDate, int? approval, string Banks_Id, string search, string periodChange, string FILTER_Keyword)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            PayrollPaymentDatesController.setDropDownListViewBag(db, this);
            BanksController.setDropDownListViewBag(db, this);

            DateTime payPeriod = Helper.setFilterViewBag(this, PayPeriod, year, month, payDate, approval, Banks_Id, search, periodChange, null, FILTER_Keyword, null);

            if (rss != null)
            {
                ViewBag.RemoveDatatablesStateSave = rss;
                return View();
            }
            else
            {
                List<PayrollsModel> models = get(payPeriod, payDate, approval, Banks_Id, FILTER_Keyword);
                ViewBag.TotalApprovedPayableAmount = string.Format("{0:N0}", models.Where(x => x.ApprovalOperator_ID != null && x.PayableAmount > 0).Sum(x => x.PayableAmount));
                ViewBag.TotalApprovedDueAmount = string.Format("{0:N0}", models.Where(x => x.ApprovalOperator_ID != null && x.PayableAmount > 0).Sum(x => x.PayableAmount - x.PaymentAmount));
                ViewBag.FILTER_Keyword = FILTER_Keyword;

                return View(models);
            }
        }

        // POST: PAYROLL/Payrolls
        [HttpPost]
        public ActionResult Index(DateTime? PayPeriod, int? year, int? month, int? payDate, int? approval, string FILTER_Banks_Id, string search, string periodChange, string FILTER_Keyword)
        {
            PayrollPaymentDatesController.setDropDownListViewBag(db, this);
            BanksController.setDropDownListViewBag(db, this);

            DateTime payPeriod = Helper.setFilterViewBag(this, PayPeriod, year, month, payDate, approval, FILTER_Banks_Id, search, periodChange, null, FILTER_Keyword, null);

            List<PayrollsModel> models = get(payPeriod, payDate, approval, FILTER_Banks_Id, FILTER_Keyword);
            ViewBag.TotalApprovedPayableAmount = string.Format("{0:N0}", models.Where(x => x.ApprovalOperator_ID != null && x.PayableAmount > 0).Sum(x => x.PayableAmount));
            ViewBag.TotalApprovedDueAmount = string.Format("{0:N0}", models.Where(x => x.ApprovalOperator_ID != null && x.PayableAmount > 0).Sum(x => x.PayableAmount - x.PaymentAmount));

            return View(models);
        }

        /* CREATE *********************************************************************************************************************************************/

        // GET: PAYROLL/Payrolls/Create
        public ActionResult Create(int? year, int? month, int? payDate, int? approval, string Banks_Id, string search, string FILTER_Keyword)
        {
            if (year == null || month == null)
                return RedirectToAction(nameof(Index));

            DateTime payPeriod = Helper.setFilterViewBag(this, null, year, month, payDate, approval, Banks_Id, search, null, null, FILTER_Keyword, null);
            PayrollEmployeesController.setDropDownListViewBag(db, this, payPeriod, EnumActionTypes.Payroll);

            return View(new PayrollsModel());
        }

        // POST: PAYROLL/Payrolls/Create/id
        [HttpPost]
        public ActionResult Create(Guid PayrollEmployees_Id, int year, int month, int? payDate, int? approval, string Banks_Id, string search, string FILTER_Keyword)
        {
            Guid Payrolls_Id = Guid.NewGuid();
            DateTime payPeriod = Helper.setFilterViewBag(this, null, year, month, payDate, approval, Banks_Id, search, null, null, FILTER_Keyword, null);
            add(Payrolls_Id, PayrollEmployees_Id, payPeriod);

            if (UtilWebMVC.hasBootboxMessage(this))
            {
                ViewBag.PayrollEmployees_Id = PayrollEmployees_Id;
                PayrollEmployeesController.setDropDownListViewBag(db, this, payPeriod, EnumActionTypes.Payroll);
                return View();
            }
            else
            {
                return RedirectToAction(nameof(Edit), new { id = Payrolls_Id, year = year, month = month, payDate = payDate, approval = approval, Banks_Id = Banks_Id, search = search, FILTER_Keyword = FILTER_Keyword });
            }
        }

        /* EDIT ***********************************************************************************************************************************************/

        // GET: PAYROLL/Payrolls/Edit/{id}
        public ActionResult Edit(Guid? id, int? year, int? month, int? payDate, int? approval, string Banks_Id, string search, string FILTER_Keyword)
        {
            if(id == null)
                return RedirectToAction(nameof(Index));

            PayrollsModel model = get((Guid)id);

            //cannot edit approved payroll
            if (model.ApprovalOperator_ID != null)
                return RedirectToAction(nameof(Index));

            setEditViewBag(year, month, payDate, approval, Banks_Id, search, FILTER_Keyword);
            return View(model);
        }

        // POST: PAYROLL/Payrolls/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PayrollsModel modifiedModel, string redirectType, string FILTER_Keyword)
        {
            //Util.debug(ModelState, ViewData);
            
            if(modifiedModel.WorkHourPerDay == 0)
                ModelState.AddModelError("WorkHourPerDay", "Jam Kerja / Hari harus lebih dari 0");

            if (ModelState.IsValid)
            {
                PayrollsModel originalModel = get(modifiedModel.Id);

                string log = string.Empty;
                log = Util.webAppendChange(log, originalModel.EmployeeFullName, modifiedModel.EmployeeFullName, ActivityLogsController.editStringFormat(PayrollsModel.COL_EmployeeFullName.Display));
                log = Util.webAppendChange(log, originalModel.CustomerName, modifiedModel.CustomerName, ActivityLogsController.editStringFormat(PayrollsModel.COL_CustomerName.Display));
                log = Util.webAppendChange(log, originalModel.JoinDate, modifiedModel.JoinDate, ActivityLogsController.editDateFormat(PayrollsModel.COL_JoinDate.Display));
                log = Util.webAppendChange(log, originalModel.RowNumber, modifiedModel.RowNumber, ActivityLogsController.editIntFormat(PayrollsModel.COL_RowNumber.Display));
                log = Util.webAppendChange(log, originalModel.PayrollPaymentDates_PayDate, modifiedModel.PayrollPaymentDates_PayDate, ActivityLogsController.editIntFormat(PayrollsModel.COL_PayrollPaymentDates_PayDate.Display));
                log = Util.webAppendChange(log, originalModel.WorkHourPerDay, modifiedModel.WorkHourPerDay, ActivityLogsController.editIntFormat(PayrollsModel.COL_WorkHourPerDay.Display));
                log = Util.webAppendChange(log, originalModel.RegularWorkDay, modifiedModel.RegularWorkDay, ActivityLogsController.editDecimalFormat(PayrollsModel.COL_RegularWorkDay.Display));
                log = Util.webAppendChange(log, originalModel.RegularWorkHour, modifiedModel.RegularWorkHour, ActivityLogsController.editDecimalFormat(PayrollsModel.COL_RegularWorkHour.Display));
                log = Util.webAppendChange(log, originalModel.RegularPayrate, modifiedModel.RegularPayrate, ActivityLogsController.editIntFormat(PayrollsModel.COL_RegularPayrate.Display));
                log = Util.webAppendChange(log, originalModel.RegularOvertimeWorkHour, modifiedModel.RegularOvertimeWorkHour, ActivityLogsController.editIntFormat(PayrollsModel.COL_RegularOvertimeWorkHour.Display));
                log = Util.webAppendChange(log, originalModel.RegularOvertimeHourlyPayrate, modifiedModel.RegularOvertimeHourlyPayrate, ActivityLogsController.editIntFormat(PayrollsModel.COL_RegularOvertimeHourlyPayrate.Display));
                log = Util.webAppendChange(log, originalModel.HolidayWorkDay, modifiedModel.HolidayWorkDay, ActivityLogsController.editDecimalFormat(PayrollsModel.COL_HolidayWorkDay.Display));
                log = Util.webAppendChange(log, originalModel.HolidayPayrate, modifiedModel.HolidayPayrate, ActivityLogsController.editIntFormat(PayrollsModel.COL_HolidayPayrate.Display));
                log = Util.webAppendChange(log, originalModel.HolidayOvertimeWorkHour, modifiedModel.HolidayOvertimeWorkHour, ActivityLogsController.editIntFormat(PayrollsModel.COL_HolidayOvertimeWorkHour.Display));
                log = Util.webAppendChange(log, originalModel.HolidayOvertimeHourlyPayrate, modifiedModel.HolidayOvertimeHourlyPayrate, ActivityLogsController.editIntFormat(PayrollsModel.COL_HolidayOvertimeHourlyPayrate.Display));
                log = Util.webAppendChange(log, originalModel.Notes, modifiedModel.Notes, ActivityLogsController.editStringFormat(PayrollsModel.COL_Notes.Display));
                log = Util.webAppendChange(log, originalModel.PaymentPercentage, modifiedModel.PaymentPercentage, ActivityLogsController.editIntFormat(PayrollsModel.COL_PaymentPercentage.Display));

                log = processEarnings(modifiedModel, log);
                log = processDeductions(modifiedModel, log);

                log = Util.webAppendChange(log, originalModel.DebtStartingBalance, modifiedModel.DebtStartingBalance, ActivityLogsController.editIntFormat(PayrollsModel.COL_DebtStartingBalance.Display));
                log = processDebts(modifiedModel, log);

                log = Util.webAppendChange(log, originalModel.MandatoryDepositStartingBalance, modifiedModel.MandatoryDepositStartingBalance, ActivityLogsController.editIntFormat(PayrollsModel.COL_MandatoryDepositStartingBalance.Display));
                log = Util.webAppendChange(log, originalModel.MandatoryDepositUpdateAmount, modifiedModel.MandatoryDepositUpdateAmount, ActivityLogsController.editIntFormat(PayrollsModel.COL_MandatoryDepositUpdateAmount.Display));

                log = Util.webAppendChange(log, originalModel.LeaveDaysStartingBalance, modifiedModel.LeaveDaysStartingBalance, ActivityLogsController.editIntFormat(PayrollsModel.COL_LeaveDaysStartingBalance.Display));
                log = Util.webAppendChange(log, originalModel.LeaveDaysAdjustment, modifiedModel.LeaveDaysAdjustment, ActivityLogsController.editIntFormat(PayrollsModel.COL_LeaveDaysAdjustment.Display));

                log = Util.webAppendChange(log, originalModel.ApproverName, modifiedModel.ApproverName, ActivityLogsController.editIntFormat(PayrollsModel.COL_ApproverName.Display));
                log = Util.webAppendChange(log, originalModel.ApproverTitle, modifiedModel.ApproverTitle, ActivityLogsController.editIntFormat(PayrollsModel.COL_ApproverTitle.Display));
                log = Util.webAppendChange(log, originalModel.ApproverSignatureFilename, modifiedModel.ApproverSignatureFilename, ActivityLogsController.editIntFormat(PayrollsModel.COL_ApproverSignatureFilename.Display));

                if (!string.IsNullOrEmpty(log))
                {
                    modifiedModel.MandatoryDeposit_PayrollItems_Id = PayrollItemsController.update(db, originalModel, modifiedModel);
                    update(modifiedModel);
                    ActivityLogsController.AddEditLog(db, Session, modifiedModel.Id, log);
                    db.SaveChanges();
                }

                //approval
                if(originalModel.ApprovalOperator_ID != modifiedModel.ApprovalOperator_ID)
                    update_ApprovalOperator_ID(originalModel.ApprovalOperator_ID != null, modifiedModel.Id);

                //improvement: user SubmitAction property in the model to redirect user to the correct page.
                //in this case, redirect to print page. However, the print page need button to go back to index page.
                //Currently, having buttons in the page gets printed to pdf. Need to find a fix to not print outside the print div.
                //if(modifiedModel.SubmitAction == EnumActions.Print)
                //    return RedirectToAction(nameof(Print));
                //else

                if(redirectType == "Update")
                    return RedirectToAction(nameof(Index), new { 
                        FILTER_Keyword = FILTER_Keyword,
                        year = modifiedModel.FILTER_PayPeriodYear, 
                        month = modifiedModel.FILTER_PayPeriodMonth, 
                        payDate = modifiedModel.FILTER_PayDate, 
                        search = modifiedModel.FILTER_Search 
                    });
                else
                    return RedirectToAction(nameof(Create), new
                    {
                        FILTER_Keyword = FILTER_Keyword,
                        year = modifiedModel.FILTER_PayPeriodYear,
                        month = modifiedModel.FILTER_PayPeriodMonth,
                        payDate = modifiedModel.FILTER_PayDate,
                        search = modifiedModel.FILTER_Search
                    });
            }

            setEditViewBag(modifiedModel.FILTER_PayPeriodYear, modifiedModel.FILTER_PayPeriodMonth, modifiedModel.FILTER_PayDate, modifiedModel.FILTER_Approval, modifiedModel.FILTER_Banks_Id, modifiedModel.FILTER_Search, FILTER_Keyword);
            return View(modifiedModel);
        }

        private void setEditViewBag(int? year, int? month, int? payDate, int? approval, string Banks_Id, string search, string FILTER_Keyword)
        {
            PayrollEarningsController.setDropDownListViewBag(db, this);
            PayrollDeductionsController.setDropDownListViewBag(db, this);
            PayrollDebtsController.setDropDownListViewBag(db, this);

            Helper.setFilterViewBag(this, null, year, month, payDate, approval, Banks_Id, search, null, null, FILTER_Keyword, null);
        }

        /* PRINT **********************************************************************************************************************************************/

        // GET: PAYROLL/Payrolls/Print/{id, year, month, paymentDate}
        public ActionResult Print(Guid? id, int? year, int? month, int? payDate, int? approval, string Banks_Id, string search, string FILTER_Keyword)
        {
            if(id == null && year == null && month == null)
                return RedirectToAction(nameof(Index));

            return View(get(id, Helper.setFilterViewBag(this, null, year, month, payDate, approval, Banks_Id, search, null, null, FILTER_Keyword, null), payDate, approval, Banks_Id));
        }

        public ActionResult PrintToPdf(Guid? id, string name, DateTime? payPeriod, int? year, int? month, int? payDate, string search)
        {
            string filename = "Slip Gaji";

            if (!string.IsNullOrEmpty(name))
                filename += " " + name;

            if (payPeriod != null)
                filename += string.Format(" {0:yyyy-MM}", payPeriod);

            filename += ".pdf";

            return new Rotativa.ActionAsPdf(nameof(Print), new { id = id, year = year, month = month, payDate = payDate, search = search })
            { FileName = filename };
        }

        /* METHODS ********************************************************************************************************************************************/

        private string processEarnings(PayrollsModel modifiedModel, string log)
        {
            if (!string.IsNullOrEmpty(modifiedModel.PayrollEarningsListString))
            {
                List<PayrollItemsModel> originalPayrollEarningsList = PayrollItemsController.GetEarnings(db, modifiedModel.Id);
                modifiedModel.PayrollEarningsList = JsonConvert.DeserializeObject<List<PayrollItemsModel>>(modifiedModel.PayrollEarningsListString);
                byte rowNo = 0;

                //remove all items without amount
                for (int i= modifiedModel.PayrollEarningsList.Count-1; i>=0; i--)
                {
                    if (modifiedModel.PayrollEarningsList[i].Amount == null || modifiedModel.PayrollEarningsList[i].Amount == 0)
                        modifiedModel.PayrollEarningsList.RemoveAt(i);
                }

                foreach (PayrollItemsModel item in modifiedModel.PayrollEarningsList)
                {
                    item.RowNo = ++rowNo;

                    if (!item.Id.HasValue) //new item
                    {
                        item.Id = Guid.NewGuid();
                        item.Payrolls_Id = modifiedModel.Id;

                        db.PayrollItemsModel.Add(item);
                        log = Util.append(log, string.Format("Add TAMBAHAN {0}: '{1}' [{2:N0}]", item.CategoryName, item.Description, item.Amount), "<BR>");
                    }
                    else //existing item
                    {
                        PayrollItemsModel oldItem = originalPayrollEarningsList.Where(x => x.Id == item.Id).FirstOrDefault();

                        if (oldItem.CategoryName != item.CategoryName ||
                            oldItem.Description != item.Description ||
                            oldItem.Amount != item.Amount)
                        {
                            log = Util.append(log, string.Format("Edit TAMBAHAN {0}: '{1}' [{2:N0}] to {3}: '{4}' [{5:N0}]",
                                oldItem.CategoryName, oldItem.Description, oldItem.Amount,
                                item.CategoryName, item.Description, item.Amount), "<BR>"); ;

                            oldItem.RowNo = rowNo;
                            oldItem.PayrollEarnings_Id = item.PayrollEarnings_Id;
                            oldItem.CategoryName = item.CategoryName;
                            oldItem.Description = item.Description;
                            oldItem.Amount = item.Amount;
                            db.Entry(oldItem).State = EntityState.Modified;
                        }
                    }
                }

                foreach (PayrollItemsModel item in originalPayrollEarningsList)
                {
                    if (!modifiedModel.PayrollEarningsList.Any(x => x.Id == item.Id))
                    {
                        db.PayrollItemsModel.Remove(item);
                        log = Util.append(log, string.Format("Delete TAMBAHAN {0}: '{1}' [{2:N0}]", item.CategoryName, item.Description, item.Amount), "<BR>");
                    }
                }            
            }

            return log;
        }

        private string processDeductions(PayrollsModel modifiedModel, string log)
        {
            if (!string.IsNullOrEmpty(modifiedModel.PayrollDeductionsListString))
            {
                List<PayrollItemsModel> originalPayrollDeductionsList = PayrollItemsController.GetDeductions(db, modifiedModel.Id);
                modifiedModel.PayrollDeductionsList = JsonConvert.DeserializeObject<List<PayrollItemsModel>>(modifiedModel.PayrollDeductionsListString);
                byte rowNo = 0;

                //remove all items without amount
                for (int i = modifiedModel.PayrollDeductionsList.Count - 1; i >= 0; i--)
                {
                    if (modifiedModel.PayrollDeductionsList[i].Amount == null || modifiedModel.PayrollDeductionsList[i].Amount == 0)
                        modifiedModel.PayrollDeductionsList.RemoveAt(i);
                }

                foreach (PayrollItemsModel item in modifiedModel.PayrollDeductionsList)
                {
                    item.RowNo = ++rowNo;

                    if (!item.Id.HasValue) //new item
                    {
                        item.Id = Guid.NewGuid();
                        item.Payrolls_Id = modifiedModel.Id;

                        db.PayrollItemsModel.Add(item);
                        log = Util.append(log, string.Format("Add POTONGAN {0}: '{1}' [{2:N0}]", item.CategoryName, item.Description, -1 * item.Amount), "<BR>");
                    }
                    else //existing item
                    {
                        PayrollItemsModel oldItem = originalPayrollDeductionsList.Where(x => x.Id == item.Id).FirstOrDefault();

                        if (oldItem.CategoryName != item.CategoryName ||
                            oldItem.Description != item.Description ||
                            oldItem.Amount != item.Amount)
                        {
                            log = Util.append(log, string.Format("Edit POTONGAN {0}: '{1}' [{2:N0}] to {3}: '{4}' [{5:N0}]",
                                oldItem.CategoryName, oldItem.Description, -1 * oldItem.Amount,
                                item.CategoryName, item.Description, -1 * item.Amount), "<BR>"); ;

                            oldItem.RowNo = rowNo;
                            oldItem.PayrollDeductions_Id = item.PayrollDeductions_Id;
                            oldItem.CategoryName = item.CategoryName;
                            oldItem.Description = item.Description;
                            oldItem.Amount = item.Amount;
                            db.Entry(oldItem).State = EntityState.Modified;
                        }
                    }
                }

                foreach (PayrollItemsModel item in originalPayrollDeductionsList)
                {
                    if (!modifiedModel.PayrollDeductionsList.Any(x => x.Id == item.Id))
                    {
                        db.PayrollItemsModel.Remove(item);
                        log = Util.append(log, string.Format("Delete POTONGAN {0}: '{1}' [{2:N0}]", item.CategoryName, item.Description, -1 * item.Amount), "<BR>");
                    }
                }
            }

            return log;
        }

        private string processDebts(PayrollsModel modifiedModel, string log)
        {
            if (!string.IsNullOrEmpty(modifiedModel.PayrollDebtsListString))
            {
                List<PayrollItemsModel> originalPayrollDebtsList = PayrollItemsController.GetDebts(db, modifiedModel.Id);
                modifiedModel.PayrollDebtsList = JsonConvert.DeserializeObject<List<PayrollItemsModel>>(modifiedModel.PayrollDebtsListString);
                byte rowNo = 0;

                //remove all items without amount
                for (int i = modifiedModel.PayrollDebtsList.Count - 1; i >= 0; i--)
                {
                    if (modifiedModel.PayrollDebtsList[i].Amount == null || modifiedModel.PayrollDebtsList[i].Amount == 0)
                        modifiedModel.PayrollDebtsList.RemoveAt(i);
                }

                foreach (PayrollItemsModel item in modifiedModel.PayrollDebtsList)
                {
                    item.RowNo = ++rowNo;

                    if (!item.Id.HasValue) //new item
                    {
                        item.Id = Guid.NewGuid();
                        item.Payrolls_Id = modifiedModel.Id;

                        db.PayrollItemsModel.Add(item);
                        log = Util.append(log, string.Format("Add HUTANG {0}: '{1}' [{2:N0}]", item.CategoryName, item.Description, item.Amount), "<BR>");
                    }
                    else //existing item
                    {
                        PayrollItemsModel oldItem = originalPayrollDebtsList.Where(x => x.Id == item.Id).FirstOrDefault();

                        if (oldItem.CategoryName != item.CategoryName ||
                            oldItem.Description != item.Description ||
                            oldItem.Amount != item.Amount)
                        {
                            log = Util.append(log, string.Format("Edit HUTANG {0}: '{1}' [{2:N0}] to {3}: '{4}' [{5:N0}]",
                                oldItem.CategoryName, oldItem.Description, oldItem.Amount,
                                item.CategoryName, item.Description, item.Amount), "<BR>"); ;

                            oldItem.RowNo = rowNo;
                            oldItem.PayrollDebts_Id = item.PayrollDebts_Id;
                            oldItem.CategoryName = item.CategoryName;
                            oldItem.Description = item.Description;
                            oldItem.Amount = item.Amount;
                            db.Entry(oldItem).State = EntityState.Modified;
                        }
                    }
                }

                foreach (PayrollItemsModel item in originalPayrollDebtsList)
                {
                    if (!modifiedModel.PayrollDebtsList.Any(x => x.Id == item.Id))
                    {
                        db.PayrollItemsModel.Remove(item);
                        log = Util.append(log, string.Format("Delete HUTANG {0}: '{1}' [{2:N0}]", item.CategoryName, item.Description, item.Amount), "<BR>");
                    }
                }
            }

            return log;
        }

        public bool isExists(Guid? id, Guid PayrollEmployees_Id, DateTime PayPeriod)
        {
            return 1 == db.Database.SqlQuery<int>(@"
	                    IF EXISTS (SELECT id FROM DWSystem.Payrolls 
                                    WHERE PayrollEmployees_Id = @PayrollEmployees_Id 
                                        AND PayPeriod = @PayPeriod
                                        AND (@id IS NULL OR id != @id))
		                    SELECT 1
	                    ELSE
		                    SELECT 0
                    ", 
                    DBConnection.getSqlParameter(PayrollsModel.COL_PayrollEmployees_Id.Name, PayrollEmployees_Id),
                    DBConnection.getSqlParameter(PayrollsModel.COL_PayPeriod.Name, PayPeriod),
                    DBConnection.getSqlParameter(PayrollsModel.COL_Id.Name, id)
                ).FirstOrDefault();
        }

        private List<PayrollsModel> get() { return getData(null, null, null, null, null, null); }
        private PayrollsModel get(Guid id) { return getData(id, null, null, null, null, null).FirstOrDefault(); }
        public List<PayrollsModel> get(DateTime? payPeriod, int? payDate, int? approvalStatus, string Banks_Id, string FILTER_Keyword) { return getData(null, payPeriod, payDate, approvalStatus, Banks_Id, FILTER_Keyword); }
        private List<PayrollsModel> get(Guid? id, DateTime? payPeriod, int? payDate, int? approvalStatus, string Banks_Id) { return getData(id, payPeriod, payDate, approvalStatus, Banks_Id, null); }
        private List<PayrollsModel> getData(Guid? id, DateTime? payPeriod, int? payDate, int? approvalStatus, string banks_Id, string FILTER_Keyword)
        {
            Guid? Banks_Id = null;
            if (!string.IsNullOrEmpty(banks_Id))
                Banks_Id = new Guid(banks_Id);

            db.Database.CommandTimeout = 1000;
            List<PayrollsModel> models = db.Database.SqlQuery<PayrollsModel>(@"
                        SELECT Payrolls.*,
							ISNULL(MandatoryDepositUpdate.Amount,0) * -1 AS MandatoryDepositUpdateAmount,
                            CEILING(RegularPayrate/(Payrolls.WorkHourPerDay*1.0)) AS RegularHourlyPayrate,
                            CEILING((
										CEILING(RegularWorkDay*RegularPayrate)
										+ CEILING(RegularWorkHour*CEILING(RegularPayrate/(Payrolls.WorkHourPerDay*1.0)))
										+ CEILING(RegularOvertimeWorkHour * RegularOvertimeHourlyPayrate)
										+ CEILING(HolidayWorkDay * HolidayPayrate)
										+ CEILING(HolidayOvertimeWorkHour * HolidayOvertimeHourlyPayrate)
									) * Payrolls.PaymentPercentage / 100
								) AS SalaryAmount,
                            ISNULL(Earnings.Amount,0) AS EarningsAmount,
                            ISNULL(Deductions.Amount,0) AS DeductionsAmount,
                            ISNULL(DebtUpdate.Amount,0) AS DebtUpdateAmount,
                            CONVERT(BIGINT, 
								CEILING(((
											CEILING(RegularWorkDay*RegularPayrate)
    										+ CEILING(RegularWorkHour*CEILING(RegularPayrate/(Payrolls.WorkHourPerDay*1.0)))
											+ CEILING(RegularOvertimeWorkHour * RegularOvertimeHourlyPayrate)
											+ CEILING(HolidayWorkDay * HolidayPayrate)
											+ CEILING(HolidayOvertimeWorkHour * HolidayOvertimeHourlyPayrate)
										) * Payrolls.PaymentPercentage / 100)
									) + ISNULL(Earnings.Amount,0) + ISNULL(Deductions.Amount,0) + ISNULL(DebtUpdate.Amount,0) + ISNULL(MandatoryDepositUpdate.Amount,0)
								) AS PayableAmount,
							CONVERT(BIGINT, ISNULL(Payments.Amount,0)) AS PaymentAmount,
                            NULL AS PayrollEarningsList,
                            '' AS PayrollEarningsListString,
                            NULL AS PayrollDeductionsList,
                            '' AS PayrollDeductionsListString,
                            NULL AS PayrollDebtsList,
                            '' AS PayrollDebtsListString,
                            NULL AS MandatoryDepositList
                        FROM DWSystem.Payrolls
                            LEFT JOIN DWSystem.PayrollEmployees ON PayrollEmployees.Id = Payrolls.PayrollEmployees_Id
                            LEFT JOIN DWSystem.PayrollItems MandatoryDepositUpdate ON MandatoryDepositUpdate.Id = Payrolls.MandatoryDeposit_PayrollItems_Id
							LEFT JOIN (
									SELECT PayrollItems.Payrolls_Id,ISNULL(SUM(PayrollItems.Amount),0) Amount
									FROM DWSystem.PayrollItems 
									WHERE PayrollItems.PayrollEarnings_Id IS NOT NULL
									GROUP BY PayrollItems.Payrolls_Id
								) Earnings ON Earnings.Payrolls_Id = Payrolls.Id
							LEFT JOIN (
									SELECT PayrollItems.Payrolls_Id,ISNULL(SUM(PayrollItems.Amount),0) Amount
									FROM DWSystem.PayrollItems 
									WHERE PayrollItems.PayrollDeductions_Id IS NOT NULL
									GROUP BY PayrollItems.Payrolls_Id
								) Deductions ON Deductions.Payrolls_Id = Payrolls.Id
							LEFT JOIN (
									SELECT PayrollItems.Payrolls_Id,ISNULL(SUM(PayrollItems.Amount),0) Amount
									FROM DWSystem.PayrollItems 
									WHERE PayrollItems.PayrollDebts_Id IS NOT NULL
									GROUP BY PayrollItems.Payrolls_Id
								) DebtUpdate ON DebtUpdate.Payrolls_Id = Payrolls.Id
							LEFT JOIN (
									SELECT PayrollPayments.Payrolls_Id,ISNULL(SUM(PayrollPayments.Amount),0) Amount
									FROM DWSystem.PayrollPayments 
									WHERE PayrollPayments.Cancelled = 0
									GROUP BY PayrollPayments.Payrolls_Id
								) Payments ON Payments.Payrolls_Id = Payrolls.Id
                        WHERE 1=1
                            AND (@Id IS NULL OR Payrolls.Id = @Id)
							AND (@Id IS NOT NULL OR (
                                (@FILTER_Keyword IS NULL OR (Payrolls.EmployeeFullName LIKE '%'+@FILTER_Keyword+'%' OR Payrolls.CustomerName LIKE '%'+@FILTER_Keyword+'%'))
								AND (@PayrollPaymentDates_PayDate IS NULL OR Payrolls.PayrollPaymentDates_PayDate=@PayrollPaymentDates_PayDate)
								AND (@Banks_Id IS NULL OR PayrollEmployees.Banks_Id = @Banks_Id)
								AND (@PayPeriod IS NULL OR Payrolls.PayPeriod = @PayPeriod)
								AND (@FILTER_ApprovalStatus IS NULL OR 
										(@FILTER_ApprovalStatus = 0 AND Payrolls.ApprovalOperator_ID IS NULL)
										OR (@FILTER_ApprovalStatus = 1 AND Payrolls.ApprovalOperator_ID IS NOT NULL)
									)
							))
						ORDER BY Payrolls.RowNumber ASC, Payrolls.EmployeeFullName ASC
                    ", 
                    DBConnection.getSqlParameter(PayrollsModel.COL_Id.Name, id),
                    DBConnection.getSqlParameter(PayrollsModel.COL_PayPeriod.Name, payPeriod),
                    DBConnection.getSqlParameter(PayrollsModel.COL_PayrollPaymentDates_PayDate.Name, payDate),
                    DBConnection.getSqlParameter(PayrollEmployeesModel.COL_Banks_Id.Name, Banks_Id),
                    DBConnection.getSqlParameter("FILTER_ApprovalStatus", approvalStatus),
                    DBConnection.getSqlParameter("FILTER_Keyword", FILTER_Keyword)
                ).ToList();

            foreach(PayrollsModel model in models)
            {
                model.PayrollEarningsList = PayrollItemsController.GetEarnings(db, model.Id);
                model.PayrollDeductionsList = PayrollItemsController.GetDeductions(db, model.Id);
                model.PayrollDebtsList = PayrollItemsController.GetDebts(db, model.Id);
                model.MandatoryDepositList = PayrollItemsController.GetMandatoryDeposits(db, model.Id);
                model.PayrollPaymentList = new PayrollPaymentsController().get(null, null, null, false, model.Id, null, null);
            }

            return models;
        }

        private void add(Guid id, Guid payrollEmployees_Id, DateTime payPeriod)
        {
            PayrollEmployeesModel payrollEmployee = new PayrollEmployeesController().get(payrollEmployees_Id);

            if (payrollEmployee.Regions_Id == null)
            {
                UtilWebMVC.setBootboxMessage(this, string.Format("Wilayah counter {0} tidak ditemukan", payrollEmployee.Customer_Name));
                return;
            }

            RegionPayratesModel regionPayrate = RegionPayratesController.get(db, (Guid)payrollEmployee.Regions_Id);

            if (regionPayrate == null)
            {
                UtilWebMVC.setBootboxMessage(this, string.Format("UMP untuk wilayah {0} tidak ditemukan", payrollEmployee.Regions_Name));
                return;
            }

            db.Database.ExecuteSqlCommand(@"                   
                    INSERT INTO DWSystem.Payrolls (
                        Id,
                        PayrollEmployees_Id,
                        EmployeeFullName,
                        JoinDate,
                        Customer_CustomerID,
                        CustomerName,
                        PayPeriod,
						WorkHourPerDay,
						RegularWorkDay,
                        RegularWorkHour,
						RegularPayrate,
						RegularOvertimeWorkHour,
						RegularOvertimeHourlyPayrate,
						HolidayWorkDay,
						HolidayPayrate,
						HolidayOvertimeWorkHour,
						HolidayOvertimeHourlyPayrate,
						LeaveDaysStartingBalance,
						LeaveDaysAdjustment,
						MandatoryDepositStartingBalance,
						DebtStartingBalance,
						ApproverName,
						ApproverTitle,
                        ApproverSignatureFilename,
						PayrollDepartments_Name,
						Regions_Name,
						PaymentPercentage,
						PayrollPaymentDates_PayDate,
						RowNumber
                    ) VALUES (
                        @Id,
                        @PayrollEmployees_Id,
                        @EmployeeFullName,
                        @JoinDate,
                        @Customer_CustomerID,
                        @CustomerName,
                        @PayPeriod,
						@WorkHourPerDay,
						@RegularWorkDay,
                        @RegularWorkHour,
						@RegularPayrate,
						@RegularOvertimeWorkHour,
						@RegularOvertimeHourlyPayrate,
						@HolidayWorkDay,
						@HolidayPayrate,
						@HolidayOvertimeWorkHour,
						@HolidayOvertimeHourlyPayrate,
						(SELECT ISNULL(SUM(Payrolls.LeaveDaysAdjustment),0)
                            FROM DWSystem.Payrolls 
                            WHERE Payrolls.PayrollEmployees_Id = @PayrollEmployees_Id
                                AND Payrolls.ApprovalOperator_ID IS NOT NULL
                                AND Payrolls.PayPeriod < @PayPeriod
                        ),
						@LeaveDaysAdjustment,
						(SELECT ISNULL(SUM(PayrollItems.Amount),0)*-1
							FROM DWSystem.Payrolls
								LEFT JOIN DWSystem.PayrollItems ON PayrollItems.Id = Payrolls.MandatoryDeposit_PayrollItems_Id
							WHERE Payrolls.PayrollEmployees_Id = @PayrollEmployees_Id
								AND Payrolls.ApprovalOperator_ID IS NOT NULL
                                AND Payrolls.PayPeriod < @PayPeriod
						),
						(SELECT ISNULL(SUM(PayrollItems.Amount),0)
							FROM DWSystem.PayrollItems
								LEFT JOIN DWSystem.Payrolls ON Payrolls.Id = PayrollItems.Payrolls_Id
								LEFT JOIN DWSystem.PayrollEmployees ON PayrollEmployees.Id = Payrolls.PayrollEmployees_Id
							WHERE PayrollItems.PayrollDebts_Id IS NOT NULL
								AND Payrolls.PayrollEmployees_Id = @PayrollEmployees_Id
								AND Payrolls.ApprovalOperator_ID IS NOT NULL
                                AND Payrolls.PayPeriod < @PayPeriod
						),
						(SELECT ISNULL(Value_String,'') FROM DWSystem.Settings WHERE Settings.Id = @ApproverName),
						(SELECT ISNULL(Value_String,'') FROM DWSystem.Settings WHERE Settings.Id = @ApproverTitle),
						(SELECT ISNULL(Value_String,'') FROM DWSystem.Settings WHERE Settings.Id = @ApproverSignatureFilename),
						@PayrollDepartments_Name,
						@Regions_Name,
						@PaymentPercentage,
						@PayrollPaymentDates_PayDate,
						(SELECT ISNULL(MAX(Payrolls.RowNumber),0)+1 FROM DWSystem.Payrolls WHERE Payrolls.PayPeriod = @PayPeriod)
                    )
                ",
                DBConnection.getSqlParameter(PayrollsModel.COL_Id.Name, id),
                DBConnection.getSqlParameter(PayrollsModel.COL_PayrollEmployees_Id.Name, payrollEmployee.Id),
                DBConnection.getSqlParameter(PayrollsModel.COL_EmployeeFullName.Name, payrollEmployee.Fullname),
                DBConnection.getSqlParameter(PayrollsModel.COL_JoinDate.Name, payrollEmployee.JoinDate),
                DBConnection.getSqlParameter(PayrollsModel.COL_Customer_CustomerID.Name, payrollEmployee.Customer_CustomerID),
                DBConnection.getSqlParameter(PayrollsModel.COL_CustomerName.Name, payrollEmployee.Customer_Name),
                DBConnection.getSqlParameter(PayrollsModel.COL_PayPeriod.Name, payPeriod),
                DBConnection.getSqlParameter(PayrollsModel.COL_WorkHourPerDay.Name, payrollEmployee.WorkHourPerDay),
                DBConnection.getSqlParameter(PayrollsModel.COL_RegularWorkDay.Name, 0),
                DBConnection.getSqlParameter(PayrollsModel.COL_RegularWorkHour.Name, 0),
                DBConnection.getSqlParameter(PayrollsModel.COL_RegularPayrate.Name, regionPayrate.RegularPayrate),
                DBConnection.getSqlParameter(PayrollsModel.COL_RegularOvertimeWorkHour.Name, 0),
                DBConnection.getSqlParameter(PayrollsModel.COL_RegularOvertimeHourlyPayrate.Name, regionPayrate.RegularOvertimeHourlyPayrate),
                DBConnection.getSqlParameter(PayrollsModel.COL_HolidayWorkDay.Name, 0),
                DBConnection.getSqlParameter(PayrollsModel.COL_HolidayPayrate.Name, regionPayrate.HolidayPayrate),
                DBConnection.getSqlParameter(PayrollsModel.COL_HolidayOvertimeWorkHour.Name, 0),
                DBConnection.getSqlParameter(PayrollsModel.COL_HolidayOvertimeHourlyPayrate.Name, regionPayrate.HolidayOvertimeHourlyPayrate),
                DBConnection.getSqlParameter(PayrollsModel.COL_LeaveDaysAdjustment.Name, 0),
                DBConnection.getSqlParameter(PayrollsModel.COL_ApproverName.Name, SettingsModel.COL_PayrollApproverName.Id),
                DBConnection.getSqlParameter(PayrollsModel.COL_ApproverTitle.Name, SettingsModel.COL_PayrollApproverTitle.Id),
                DBConnection.getSqlParameter(PayrollsModel.COL_ApproverSignatureFilename.Name, SettingsModel.COL_PayrollApproverSignature.Id),
                DBConnection.getSqlParameter(PayrollsModel.COL_PayrollDepartments_Name.Name, payrollEmployee.PayrollDepartments_Name),
                DBConnection.getSqlParameter(PayrollsModel.COL_Regions_Name.Name, payrollEmployee.Regions_Name),
                DBConnection.getSqlParameter(PayrollsModel.COL_PaymentPercentage.Name, 100),
                DBConnection.getSqlParameter(PayrollsModel.COL_PayrollPaymentDates_PayDate.Name, payrollEmployee.PayrollPaymentDates_PayDate)
            );

            SettingsModel settings = SettingsController.get(db);
            PayrollsModel model = get(id);

            //reset leave days on January. DISABLED because there are other rules (1 year minimum to receive leave days). Need fixed rules from client and create the mechanism.
            //if (payPeriod.Month == 1)
            //    model.LeaveDaysAdjustment = settings.LeaveDaysPerYear - model.LeaveDaysStartingBalance;

            int totalMonthAfterJoin = model.PayPeriod.Month - model.JoinDate.Month + 12 * (model.PayPeriod.Year - model.JoinDate.Year);
            if (model.MandatoryDepositStartingBalance < settings.DepositAmountTotal)
            {
                if (totalMonthAfterJoin >= settings.DepositFirstMonthAfterJoin)
                    model.MandatoryDeposit_PayrollItems_Id = PayrollItemsController.add(db, model, settings.DepositAmountPerMonth);
            }

            if (totalMonthAfterJoin == 0)
                model.PaymentPercentage = settings.PaymentPercentageMonth1;
            else if (totalMonthAfterJoin == 1)
                model.PaymentPercentage = settings.PaymentPercentageMonth2;
            else if (totalMonthAfterJoin == 2)
                model.PaymentPercentage = settings.PaymentPercentageMonth3;

            db.SaveChanges();
            update(model);
        }

        private void update(PayrollsModel modifiedModel)
        {
            db.Database.ExecuteSqlCommand(@"
                    UPDATE DWSystem.Payrolls 
                    SET 
                        EmployeeFullName = @EmployeeFullName,
                        JoinDate = @JoinDate,
                        Customer_CustomerID = @Customer_CustomerID,
                        CustomerName = @CustomerName,
                        PayPeriod = @PayPeriod,
                        WorkHourPerDay = @WorkHourPerDay,
                        RegularWorkDay = @RegularWorkDay,
                        RegularWorkHour = @RegularWorkHour,
                        RegularPayrate = @RegularPayrate,
                        RegularOvertimeWorkHour = @RegularOvertimeWorkHour,
                        RegularOvertimeHourlyPayrate = @RegularOvertimeHourlyPayrate,
                        HolidayWorkDay = @HolidayWorkDay,
                        HolidayPayrate = @HolidayPayrate,
                        HolidayOvertimeWorkHour = @HolidayOvertimeWorkHour,
                        HolidayOvertimeHourlyPayrate = @HolidayOvertimeHourlyPayrate,
                        LeaveDaysStartingBalance = @LeaveDaysStartingBalance,
                        LeaveDaysAdjustment = @LeaveDaysAdjustment,
                        MandatoryDepositStartingBalance = @MandatoryDepositStartingBalance,
                        MandatoryDeposit_PayrollItems_Id = @MandatoryDeposit_PayrollItems_Id,
                        DebtStartingBalance = @DebtStartingBalance,
                        ApproverName = @ApproverName,
                        ApproverTitle = @ApproverTitle,
                        ApproverSignatureFilename = @ApproverSignatureFilename,
                        ApprovalOperator_ID = @ApprovalOperator_ID,
                        PayrollDepartments_Name = @PayrollDepartments_Name,
                        Regions_Name = @Regions_Name,
                        PaymentPercentage = @PaymentPercentage,
                        Notes = @Notes,
                        PayrollPaymentDates_PayDate = @PayrollPaymentDates_PayDate,
                        RowNumber = @RowNumber
                    WHERE Id=@Id;
                ",
                DBConnection.getSqlParameter(PayrollsModel.COL_Id.Name, modifiedModel.Id),
                DBConnection.getSqlParameter(PayrollsModel.COL_EmployeeFullName.Name, modifiedModel.EmployeeFullName),
                DBConnection.getSqlParameter(PayrollsModel.COL_JoinDate.Name, modifiedModel.JoinDate),
                DBConnection.getSqlParameter(PayrollsModel.COL_Customer_CustomerID.Name, modifiedModel.Customer_CustomerID),
                DBConnection.getSqlParameter(PayrollsModel.COL_CustomerName.Name, modifiedModel.CustomerName),
                DBConnection.getSqlParameter(PayrollsModel.COL_PayPeriod.Name, modifiedModel.PayPeriod),
                DBConnection.getSqlParameter(PayrollsModel.COL_WorkHourPerDay.Name, modifiedModel.WorkHourPerDay),
                DBConnection.getSqlParameter(PayrollsModel.COL_RegularWorkDay.Name, modifiedModel.RegularWorkDay),
                DBConnection.getSqlParameter(PayrollsModel.COL_RegularWorkHour.Name, modifiedModel.RegularWorkHour),
                DBConnection.getSqlParameter(PayrollsModel.COL_RegularPayrate.Name, modifiedModel.RegularPayrate),
                DBConnection.getSqlParameter(PayrollsModel.COL_RegularOvertimeWorkHour.Name, modifiedModel.RegularOvertimeWorkHour),
                DBConnection.getSqlParameter(PayrollsModel.COL_RegularOvertimeHourlyPayrate.Name, modifiedModel.RegularOvertimeHourlyPayrate),
                DBConnection.getSqlParameter(PayrollsModel.COL_HolidayWorkDay.Name, modifiedModel.HolidayWorkDay),
                DBConnection.getSqlParameter(PayrollsModel.COL_HolidayPayrate.Name, modifiedModel.HolidayPayrate),
                DBConnection.getSqlParameter(PayrollsModel.COL_HolidayOvertimeWorkHour.Name, modifiedModel.HolidayOvertimeWorkHour),
                DBConnection.getSqlParameter(PayrollsModel.COL_HolidayOvertimeHourlyPayrate.Name, modifiedModel.HolidayOvertimeHourlyPayrate),
                DBConnection.getSqlParameter(PayrollsModel.COL_LeaveDaysStartingBalance.Name, modifiedModel.LeaveDaysStartingBalance),
                DBConnection.getSqlParameter(PayrollsModel.COL_LeaveDaysAdjustment.Name, modifiedModel.LeaveDaysAdjustment),
                DBConnection.getSqlParameter(PayrollsModel.COL_MandatoryDepositStartingBalance.Name, modifiedModel.MandatoryDepositStartingBalance),
                DBConnection.getSqlParameter(PayrollsModel.COL_MandatoryDeposit_PayrollItems_Id.Name, modifiedModel.MandatoryDeposit_PayrollItems_Id),
                DBConnection.getSqlParameter(PayrollsModel.COL_DebtStartingBalance.Name, modifiedModel.DebtStartingBalance),
                DBConnection.getSqlParameter(PayrollsModel.COL_ApproverName.Name, modifiedModel.ApproverName),
                DBConnection.getSqlParameter(PayrollsModel.COL_ApproverTitle.Name, modifiedModel.ApproverTitle),
                DBConnection.getSqlParameter(PayrollsModel.COL_ApproverSignatureFilename.Name, modifiedModel.ApproverSignatureFilename),
                DBConnection.getSqlParameter(PayrollsModel.COL_ApprovalOperator_ID.Name, modifiedModel.ApprovalOperator_ID),
                DBConnection.getSqlParameter(PayrollsModel.COL_PayrollDepartments_Name.Name, modifiedModel.PayrollDepartments_Name),
                DBConnection.getSqlParameter(PayrollsModel.COL_Regions_Name.Name, modifiedModel.Regions_Name),
                DBConnection.getSqlParameter(PayrollsModel.COL_PaymentPercentage.Name, modifiedModel.PaymentPercentage),
                DBConnection.getSqlParameter(PayrollsModel.COL_Notes.Name, modifiedModel.Notes),
                DBConnection.getSqlParameter(PayrollsModel.COL_PayrollPaymentDates_PayDate.Name, modifiedModel.PayrollPaymentDates_PayDate),
                DBConnection.getSqlParameter(PayrollsModel.COL_RowNumber.Name, modifiedModel.RowNumber)
            );
        }

        public JsonResult update_ApprovalOperator_ID(bool value, Guid id)
        {
            int? ApprovalOperator_ID = null;
            if(value) 
                ApprovalOperator_ID = OperatorController.getUserId(Session);

            db.Database.ExecuteSqlCommand(@"
                    UPDATE DWSystem.Payrolls 
                    SET 
                        ApprovalOperator_ID = @ApprovalOperator_ID
                    WHERE Id=@Id;
                ",
                DBConnection.getSqlParameter(PayrollsModel.COL_Id.Name, id),
                DBConnection.getSqlParameter(PayrollsModel.COL_ApprovalOperator_ID.Name, ApprovalOperator_ID)
            );

            ActivityLogsController.Add(db, Session, id, value ? "Approved" : "Approval Cancelled");
            db.SaveChanges();

            return Json(new { Message = "" });
        }

        public JsonResult GetRegenerateData(Guid id)
        {
            PayrollsModel model = get(id);
            PayrollEmployeesModel payrollEmployee = new PayrollEmployeesController().get(model.PayrollEmployees_Id);
            RegionPayratesModel regionPayrate = RegionPayratesController.get(db, (Guid)payrollEmployee.Regions_Id);
            model.EmployeeFullName = payrollEmployee.Fullname;
            model.Customer_CustomerID = payrollEmployee.Customer_CustomerID;
            model.CustomerName = payrollEmployee.Customer_Name;
            model.JoinDate = payrollEmployee.JoinDate;
            //WorkHourPerDay is not refreshed on purpose so it doesn't mess up current calculation. User must change it manually
            //PayrollPaymentDates_PayDate is skipped also. User must change it manually
            model.RegularPayrate = regionPayrate.RegularPayrate;
            model.RegularOvertimeHourlyPayrate = regionPayrate.RegularOvertimeHourlyPayrate;
            model.HolidayPayrate = regionPayrate.HolidayPayrate;
            model.HolidayOvertimeHourlyPayrate = regionPayrate.HolidayOvertimeHourlyPayrate;

            model.LeaveDaysStartingBalance = db.Database.SqlQuery<int>(@"
                        SELECT ISNULL(LeaveDays.StartingBalance,0)
                        FROM DWSystem.PayrollEmployees
                        LEFT JOIN (
	                            SELECT Payrolls.PayrollEmployees_Id, ISNULL(SUM(Payrolls.LeaveDaysAdjustment),0) AS StartingBalance
	                            FROM DWSystem.Payrolls
                                WHERE Payrolls.PayPeriod < @PayPeriod
                                    AND Payrolls.ApprovalOperator_ID IS NOT NULL
	                            GROUP BY PayrollEmployees_Id
                            ) LeaveDays ON LeaveDays.PayrollEmployees_Id = PayrollEmployees.Id
                        WHERE PayrollEmployees.Id = @PayrollEmployees_Id
                    ", 
                    new SqlParameter(PayrollsModel.COL_PayPeriod.Name, model.PayPeriod), 
                    new SqlParameter(PayrollsModel.COL_PayrollEmployees_Id.Name, model.PayrollEmployees_Id)
                ).FirstOrDefault();

            model.MandatoryDepositStartingBalance = db.Database.SqlQuery<int>(@"
                        SELECT ISNULL(MandatoryDeposit.StartingBalance,0)
                        FROM DWSystem.PayrollEmployees
                        LEFT JOIN (
	                            SELECT Payrolls.PayrollEmployees_Id, ISNULL(SUM(PayrollItems.Amount),0)*-1 AS StartingBalance
	                            FROM DWSystem.Payrolls
		                            LEFT JOIN DWSystem.PayrollItems ON PayrollItems.Id = Payrolls.MandatoryDeposit_PayrollItems_Id
                                WHERE Payrolls.PayPeriod < @PayPeriod
                                    AND Payrolls.ApprovalOperator_ID IS NOT NULL
	                            GROUP BY PayrollEmployees_Id
                            ) MandatoryDeposit ON MandatoryDeposit.PayrollEmployees_Id = PayrollEmployees.Id
                        WHERE PayrollEmployees.Id = @PayrollEmployees_Id
                    ",
                    new SqlParameter(PayrollsModel.COL_PayPeriod.Name, model.PayPeriod),
                    new SqlParameter(PayrollsModel.COL_PayrollEmployees_Id.Name, model.PayrollEmployees_Id)
                ).FirstOrDefault();

            model.DebtStartingBalance = db.Database.SqlQuery<int>(@"
                        SELECT ISNULL(Debt.StartingBalance,0)
                        FROM DWSystem.PayrollEmployees
                        LEFT JOIN (
	                            SELECT Payrolls.PayrollEmployees_Id, ISNULL(SUM(PayrollItems.Amount),0) AS StartingBalance
							    FROM DWSystem.PayrollItems
								    LEFT JOIN DWSystem.Payrolls ON Payrolls.Id = PayrollItems.Payrolls_Id
								    LEFT JOIN DWSystem.PayrollEmployees ON PayrollEmployees.Id = Payrolls.PayrollEmployees_Id
                                WHERE PayrollItems.PayrollDebts_Id IS NOT NULL
                                    AND Payrolls.ApprovalOperator_ID IS NOT NULL
                                    AND Payrolls.PayPeriod < @PayPeriod
	                            GROUP BY PayrollEmployees_Id
                            ) Debt ON Debt.PayrollEmployees_Id = PayrollEmployees.Id
                        WHERE PayrollEmployees.Id = @PayrollEmployees_Id
                    ",
                    new SqlParameter(PayrollsModel.COL_PayPeriod.Name, model.PayPeriod),
                    new SqlParameter(PayrollsModel.COL_PayrollEmployees_Id.Name, model.PayrollEmployees_Id)
                ).FirstOrDefault();

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

        public static string GetFormattedWorkDay(decimal workDay, int workHourPerDay)
        {
            string formattedString = "";

            decimal dayCount = Math.Floor(workDay);
            if (dayCount > 0)
                formattedString += string.Format("{0:N0} hari", dayCount);

            decimal hourCount = (workDay - dayCount) * workHourPerDay;
            if (hourCount > 0)
                formattedString += string.Format(" {0:N0} jam", hourCount);

            return formattedString;
        }

        /* REPORT *********************************************************************************************************************************************/

        public ActionResult Report(int year, int month, int? payDate, int? approval, string Banks_Id)
        {
            DateTime payPeriod = new DateTime(year, month, 1, 0, 0, 0);

            string filename = string.Format("Payroll {0:yyyy-MM}.xlsx", payPeriod);

            if (!string.IsNullOrEmpty(Banks_Id))
                filename += " " + db.BanksModel.Where(x => x.Id.ToString() == Banks_Id).FirstOrDefault().Name;

            if (payDate != null)
                filename += " Tanggal " + payDate;

            if (approval == 0)
                filename += " Approved";
            else if (approval == 1)
                filename += " Not Approved";

            return Excel.GenerateExcelReport(filename, CompileExcelPackage(get(payPeriod, payDate, approval, Banks_Id, null)));
        }

        public ExcelPackage CompileExcelPackage(List<PayrollsModel> models)
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
            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "Approved", null, 0, 1, null);

            Excel.editCellGroup(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "NORMAL", null, 
                    new ExcelCellFormat(10, colIdxHeader, "Hari"),
                    new ExcelCellFormat(10, ++colIdxHeader, "Jam"),
                    new ExcelCellFormat(10, ++colIdxHeader, "Rate/Hari"),
                    new ExcelCellFormat(10, ++colIdxHeader, "Jumlah")
                );

            Excel.editCellGroup(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "LEMBUR NORMAL", null,
                    new ExcelCellFormat(10, colIdxHeader, "Jam"),
                    new ExcelCellFormat(10, ++colIdxHeader, "Rate/Jam"),
                    new ExcelCellFormat(10, ++colIdxHeader, "Jumlah")
                );

            Excel.editCellGroup(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "HARI BESAR", null,
                    new ExcelCellFormat(10, colIdxHeader, "Hari"),
                    new ExcelCellFormat(10, ++colIdxHeader, "Rate/Hari"),
                    new ExcelCellFormat(10, ++colIdxHeader, "Jumlah")
                );

            Excel.editCellGroup(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "LEMBUR HARI BESAR", null,
                    new ExcelCellFormat(10, colIdxHeader, "Jam"),
                    new ExcelCellFormat(10, ++colIdxHeader, "Rate/Hari"),
                    new ExcelCellFormat(10, ++colIdxHeader, "Jumlah")
                );

            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "% Dibayar", null, 0, 1, null);
            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 15, "GAJI POKOK", null, 0, 1, null);

            //Payroll Earnings
            Dictionary<Guid?, string> availablePayrollEarnings = new Dictionary<Guid?, string>();
            List<PayrollEarningsModel> payrollEarnings = db.PayrollEarningsModel.ToList();
            List<ExcelCellFormat> payrollEarningHeaders = new List<ExcelCellFormat>();
            foreach (PayrollsModel payroll in models)
            {
                if(payroll.PayrollEarningsList != null)
                { 
                    foreach (PayrollItemsModel earning in payroll.PayrollEarningsList)
                    {
                        if(!availablePayrollEarnings.ContainsKey(earning.PayrollEarnings_Id))
                        {
                            string name = payrollEarnings.Find(x => x.Id == earning.PayrollEarnings_Id).Name;
                            availablePayrollEarnings.Add(earning.PayrollEarnings_Id, name);
                            payrollEarningHeaders.Add(new ExcelCellFormat(15, ++colIdxHeader, name));
                        }
                    }
                }
            }
            if (payrollEarningHeaders.Count > 0)
                Excel.editCellGroup(ws, rowIdxHeaderGroup, colIdxHeader - (payrollEarningHeaders.Count - 1), 10, "TAMBAHAN", null, payrollEarningHeaders.ToArray());

            //Payroll Deductions
            Dictionary<Guid?, string> availablePayrollDeductions = new Dictionary<Guid?, string>();
            List<PayrollDeductionsModel> payrollDeductions = db.PayrollDeductionsModel.ToList();
            List<ExcelCellFormat> payrollDeductionHeaders = new List<ExcelCellFormat>();
            foreach (PayrollsModel payroll in models)
            {
                if (payroll.PayrollDeductionsList != null)
                {
                    foreach (PayrollItemsModel Deduction in payroll.PayrollDeductionsList)
                    {
                        if (!availablePayrollDeductions.ContainsKey(Deduction.PayrollDeductions_Id))
                        {
                            string name = payrollDeductions.Find(x => x.Id == Deduction.PayrollDeductions_Id).Name;
                            availablePayrollDeductions.Add(Deduction.PayrollDeductions_Id, name);
                            payrollDeductionHeaders.Add(new ExcelCellFormat(15, ++colIdxHeader, name));
                        }
                    }
                }
            }
            if (payrollDeductionHeaders.Count > 0)
                Excel.editCellGroup(ws, rowIdxHeaderGroup, colIdxHeader - (payrollDeductionHeaders.Count - 1), 10, "POTONGAN", null, payrollDeductionHeaders.ToArray());

            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "Tabungan", null, 0, 1, null);
            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "Hutang", null, 0, 1, null);
            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "Total", null, 0, 1, null);

            Excel.editCellGroup(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "PAYMENT", null,
                    new ExcelCellFormat(10, colIdxHeader, "Jumlah"),
                    new ExcelCellFormat(15, ++colIdxHeader, "Tanggal"),
                    new ExcelCellFormat(10, ++colIdxHeader, "Bank"),
                    new ExcelCellFormat(15, ++colIdxHeader, "Rekening"),
                    new ExcelCellFormat(15, ++colIdxHeader, "Nama")
                );

            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "Due", null, 0, 1, null);

            Excel.editCellGroup(ws, rowIdxHeaderGroup, ++colIdxHeader, 10, "CUTI", null,
                    new ExcelCellFormat(7, colIdxHeader, "Update"),
                    new ExcelCellFormat(5, ++colIdxHeader, "Sisa")
                );

            Excel.editCell(ws, rowIdxHeaderGroup, ++colIdxHeader, 100, "Notes", null, 0, 1, null);

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
            if (models.Count > 0)
            {
                foreach (PayrollsModel item in models.OrderBy(x => x.RowNumber))
                {
                    colIdx = 0;
                    Excel.editCell(ws, rowIdx, ++colIdx, item.RowNumber, null);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.EmployeeFullName, null);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.CustomerName, null);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.ApprovalOperator_ID == null ? "" : "YA", "", ExcelHorizontalAlignment.Center);

                    Excel.editCell(ws, rowIdx, ++colIdx, item.RegularWorkDay, "#,##0.000", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.RegularWorkHour, "#,##0.000", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.RegularPayrate, "#,##0", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, 
                        Math.Ceiling(item.RegularWorkDay * item.RegularPayrate) + (item.RegularWorkHour * Math.Ceiling((decimal)item.RegularPayrate / item.WorkHourPerDay)), 
                        "#,##0", ExcelHorizontalAlignment.Right
                    );

                    Excel.editCell(ws, rowIdx, ++colIdx, item.RegularOvertimeWorkHour, "#,##0", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.RegularOvertimeHourlyPayrate, "#,##0", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, Math.Ceiling(item.RegularOvertimeWorkHour * item.RegularOvertimeHourlyPayrate), "#,##0", ExcelHorizontalAlignment.Right);

                    Excel.editCell(ws, rowIdx, ++colIdx, item.HolidayWorkDay, "#,##0.000", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.HolidayPayrate, "#,##0", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, Math.Ceiling(item.HolidayWorkDay * item.HolidayPayrate), "#,##0", ExcelHorizontalAlignment.Right);

                    Excel.editCell(ws, rowIdx, ++colIdx, item.HolidayOvertimeWorkHour, "#,##0", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.HolidayOvertimeHourlyPayrate, "#,##0", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, Math.Ceiling(item.HolidayOvertimeWorkHour * item.HolidayOvertimeHourlyPayrate), "#,##0", ExcelHorizontalAlignment.Right);

                    Excel.editCell(ws, rowIdx, ++colIdx, item.PaymentPercentage, null);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.SalaryAmount, "#,##0", ExcelHorizontalAlignment.Right);

                    //Payroll Earnings
                    foreach (PayrollItemsModel payrollitem in item.PayrollEarningsList)
                    {
                        string categoryName = payrollEarnings.Find(x => x.Id == payrollitem.PayrollEarnings_Id).Name;
                        ExcelCellFormat header = payrollEarningHeaders.Find(x => x.Text == categoryName);
                        object cellValue = Excel.getCellValue(ws, rowIdx, header.ColumnIndex);
                        int amount = (int)payrollitem.Amount + (cellValue == null ? 0 : Convert.ToInt32(cellValue));

                        Excel.editCell(ws, rowIdx, header.ColumnIndex, amount, "#,##0", ExcelHorizontalAlignment.Right);
                    }
                    colIdx += payrollEarningHeaders.Count;

                    //Payroll Deductions
                    foreach (PayrollItemsModel payrollitem in item.PayrollDeductionsList)
                    {
                        string categoryName = payrollDeductions.Find(x => x.Id == payrollitem.PayrollDeductions_Id).Name;
                        ExcelCellFormat header = payrollDeductionHeaders.Find(x => x.Text == categoryName);
                        object cellValue = Excel.getCellValue(ws, rowIdx, header.ColumnIndex);
                        int amount = (int)payrollitem.Amount + (cellValue == null ? 0 : Convert.ToInt32(cellValue));

                        Excel.editCell(ws, rowIdx, header.ColumnIndex, amount, "#,##0", ExcelHorizontalAlignment.Right);
                    }
                    colIdx += payrollDeductionHeaders.Count;

                    Excel.editCell(ws, rowIdx, ++colIdx, -1 * item.MandatoryDepositUpdateAmount, "#,##0", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.DebtUpdateAmount, "#,##0", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.PayableAmount, "#,##0", ExcelHorizontalAlignment.Right);

                    Excel.editCell(ws, rowIdx, ++colIdx, item.PaymentAmount, "#,##0", ExcelHorizontalAlignment.Right);

                    string paymentInfo = "";
                    string paymentDateInfo = "";
                    string bankInfo = "";
                    string accountNoInfo = "";
                    string accountNameInfo = "";
                    foreach (PayrollPaymentsModel payrollPayment in item.PayrollPaymentList)
                    {
                        paymentDateInfo = Util.append(paymentDateInfo, string.Format("{0:dd MMM yyyy}", payrollPayment.PaymentDate), ",");
                        bankInfo = Util.append(bankInfo, payrollPayment.Banks_Name, ",");
                        accountNoInfo = Util.append(accountNoInfo, payrollPayment.AccountNumber, ","); ;
                        accountNameInfo = Util.append(accountNameInfo, payrollPayment.AccountName, ","); ;

                        paymentInfo = Util.append(paymentInfo, string.Format("{0:dd MMM yyyy}: {1} {2} {3} {4:N0}",
                            payrollPayment.PaymentDate, payrollPayment.Banks_Name, payrollPayment.AccountNumber, payrollPayment.AccountName, payrollPayment.Amount)
                            , ",");
                    }
                    Excel.editCell(ws, rowIdx, ++colIdx, paymentDateInfo, "");
                    Excel.editCell(ws, rowIdx, ++colIdx, bankInfo, "");
                    Excel.editCell(ws, rowIdx, ++colIdx, accountNoInfo, "");
                    Excel.editCell(ws, rowIdx, ++colIdx, accountNameInfo, "");

                    Excel.editCell(ws, rowIdx, ++colIdx, item.PayableAmount - item.PaymentAmount, "#,##0", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.LeaveDaysAdjustment, "#,##0", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.LeaveDaysStartingBalance + item.LeaveDaysAdjustment, "#,##0", ExcelHorizontalAlignment.Right);
                    Excel.editCell(ws, rowIdx, ++colIdx, item.Notes, null);

                    rowIdx++;
                }
            }
            //else
            //{
            //    ws.Cells[rowIdx, 1].Value = "No Data Available";
            //    ws.Cells[rowIdx, 1, rowIdx, colIdxHeader].Merge = true;
            //    ws.Cells[rowIdx, 1, rowIdx, colIdxHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //}

            Excel.setCellBorders(ws, rowIdxHeader + 1, 1, rowIdx, colIdx, ExcelBorderStyle.Thin);

            return excelPackage;
        }

        /******************************************************************************************************************************************************/
    }
}
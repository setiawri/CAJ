using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using CAJWebApp.Areas.PAYROLL.Models;
using CAJWebApp.Areas.Reimbursement.Models;
using CAJWebApp.Areas.Reimbursement.Controllers;
using CAJWebApp.Controllers;
using CAJWebApp.Models;
using LIBUtil;

namespace CAJWebApp.Areas.PAYROLL.Controllers
{
    public class PayrollPaymentsController : Controller
    { 
        private readonly DBContext db = new DBContext();

        /* INDEX **********************************************************************************************************************************************/

        // GET: PAYROLL/PayrollPayments
        public ActionResult Index(int? rss, DateTime? PayPeriod, int? year, int? month, int? payDate, string Banks_Id, string search, string periodChange, int? Type)
        {
            ViewBag.RemoveDatatablesStateSave = rss;

            PayrollPaymentDatesController.setDropDownListViewBag(db, this);
            BanksController.setDropDownListViewBag(db, this);
            DateTime payPeriod = Helper.setFilterViewBag(this, PayPeriod, year, month, payDate, null, Banks_Id, search, periodChange, Type);

            List<PayrollPaymentsModel> models = get(payPeriod, payDate, Banks_Id, false, null, null, Type);
            ViewBag.TotalPaymentAmount = string.Format("{0:N0}", models.Where(x => x.Cancelled == false).Sum(x => x.Amount));

            return View(models);
        }

        /* CREATE PAYMENTS ************************************************************************************************************************************/

        public JsonResult CreatePayments(int year, int month, int? payDate, string Banks_Id, int actionType)
        {
            int insertCount = 0;
            string sql = "";

            DateTime payPeriod = new DateTime(year, month, 1, 0, 0, 0);

            if((EnumActionTypes)actionType == EnumActionTypes.Payroll)
            {
                List<PayrollsModel> models = new PayrollsController().get(payPeriod, payDate, null, Banks_Id);
                foreach (PayrollsModel model in models)
                {
                    if (model.ApprovalOperator_ID != null && model.PayableAmount - model.PaymentAmount > 0)
                    {
                        DateTime paymentDate = new DateTime(payPeriod.Year, payPeriod.Month, model.PayrollPaymentDates_PayDate).AddMonths(1);
                        sql = add(sql, model.Id, null, paymentDate, model.PayableAmount - model.PaymentAmount, model.PayrollEmployees_Id);
                        insertCount++;
                    }
                }
            } 
            else if((EnumActionTypes)actionType == EnumActionTypes.Reimbursement)
            {
                List<ReimbursementsModel> models = new ReimbursementsController().get(payPeriod, null, Banks_Id);
                foreach (ReimbursementsModel model in models)
                {
                    if (model.ApprovalOperator_ID != null && model.PayableAmount - model.PaymentAmount > 0)
                    {
                        sql = add(sql, null, model.Id, model.PaymentDate, model.PayableAmount - model.PaymentAmount, model.PayrollEmployees_Id);
                        insertCount++;
                    }
                }
            }

            if(!string.IsNullOrEmpty(sql))
            {
                db.Database.ExecuteSqlCommand(sql);
                db.SaveChanges();
            }

            return Json(new { status = "200", insertCount = string.Format("{0:N0}", insertCount) }, JsonRequestBehavior.AllowGet);
        }

        private string add(string sql, Guid? Payrolls_Id, Guid? Reimbursements_Id, DateTime PaymentDate, int Amount, Guid PayrollEmployees_Id)
        {
            Guid paymentId = Guid.NewGuid();

            PayrollEmployeesController payrollEmployeesController = new PayrollEmployeesController();
            PayrollEmployeesModel payrollEmployee = payrollEmployeesController.get(PayrollEmployees_Id);

            sql += string.Format(@"
                            INSERT INTO DWSystem.PayrollPayments (
									Id,
                                    Payrolls_Id,
									Reimbursements_Id,
									PaymentDate,
									Amount,
									Banks_Id,
									Banks_Name,
									AccountName,
									AccountNumber
								) VALUES ( '{0}', {1}, {2}, '{3}', '{4}', '{5}', '{6}', '{7}', '{8}');
                            ",
                    paymentId,
                    Payrolls_Id == null ? "null" : "'"+ Payrolls_Id + "'",
                    Reimbursements_Id == null ? "null" : "'" + Reimbursements_Id + "'",
                    PaymentDate,
                    Amount,
                    payrollEmployee.Banks_Id,
                    payrollEmployee.Banks_Name,
                    payrollEmployee.AccountName,
                    payrollEmployee.AccountNumber
                );

            Guid reffId = Payrolls_Id == null ? (Guid)Reimbursements_Id : (Guid)Payrolls_Id;
            ActivityLogsController.Add(db, Session, reffId, string.Format("New Payment: {0:N0}", Amount));
            ActivityLogsController.AddCreateLog(db, Session, paymentId);
            
            return sql;
        }

        /******************************************************************************************************************************************************/

        public List<PayrollPaymentsModel> get(DateTime? payPeriod, int? payDate, string banks_Id, bool onlyNotCancelled, Guid? Payrolls_Id, Guid? Reimbursements_Id, int? actionType)
        {
            Guid? Banks_Id = null;
            if (!string.IsNullOrEmpty(banks_Id))
                Banks_Id = new Guid(banks_Id);

            List<PayrollPaymentsModel> models = db.Database.SqlQuery<PayrollPaymentsModel>(@"
                        SELECT PayrollPayments.*,
							ISNULL(Payrolls.EmployeeFullName, Reimbursements.EmployeeFullName) AS EmployeeFullName,
							ISNULL(Payrolls.JoinDate, Reimbursements.JoinDate) AS JoinDate,
							ISNULL(Payrolls.PayPeriod, Reimbursements.PayPeriod) AS PayPeriod,
							ISNULL(Payrolls.CustomerName, Reimbursements.CustomerName) AS CustomerName,
							ISNULL(Payrolls.Regions_Name, '') AS Regions_Name,
							ISNULL(Payrolls.RowNumber, Reimbursements.RowNumber) AS RowNumber
						FROM DWSystem.PayrollPayments
							LEFT JOIN DWSystem.Payrolls ON Payrolls.Id = PayrollPayments.Payrolls_Id
							LEFT JOIN DWSystem.Reimbursements ON Reimbursements.Id = PayrollPayments.Reimbursements_Id
                        WHERE 1=1
							AND (@Payrolls_Id IS NULL OR PayrollPayments.Payrolls_Id = @Payrolls_Id)
							AND (@Reimbursements_Id IS NULL OR PayrollPayments.Reimbursements_Id = @Reimbursements_Id)
							AND (@Banks_Id IS NULL OR PayrollPayments.Banks_Id = @Banks_Id)
							AND (@FILTER_OnlyNotCancelled = 0 OR (@FILTER_OnlyNotCancelled = 1 AND PayrollPayments.Cancelled = 0))
							AND (@PayrollPaymentDates_PayDate IS NULL OR DAY(PayrollPayments.PaymentDate) = @PayrollPaymentDates_PayDate)
							AND (@PayPeriod IS NULL OR (PayrollPayments.Payrolls_Id IS NULL OR Payrolls.PayPeriod = @PayPeriod))
							AND (@PayPeriod IS NULL OR (PayrollPayments.Reimbursements_Id IS NULL OR Reimbursements.PayPeriod = @PayPeriod))
							AND (@FILTER_ActionType IS NULL 
                                OR (@FILTER_ActionType = 1 AND PayrollPayments.Payrolls_Id IS NOT NULL)
                                OR (@FILTER_ActionType = 2 AND PayrollPayments.Reimbursements_Id IS NOT NULL)
                            )
                        ORDER BY Payrolls.RowNumber ASC, PayrollPayments.PaymentDate ASC
                    ",
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Payrolls_Id.Name, Payrolls_Id),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Reimbursements_Id.Name, Reimbursements_Id),
                    DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Banks_Id.Name, Banks_Id),
                    DBConnection.getSqlParameter(PayrollsModel.COL_PayrollPaymentDates_PayDate.Name, payDate),
                    DBConnection.getSqlParameter(PayrollsModel.COL_PayPeriod.Name, payPeriod),
                    DBConnection.getSqlParameter("FILTER_OnlyNotCancelled", onlyNotCancelled),
                    DBConnection.getSqlParameter("FILTER_ActionType", actionType)
                ).ToList();

            return models;
        }

        public JsonResult update_Cancelled(Guid id, Guid Payrolls_Id, int Amount)
        {
            db.Database.ExecuteSqlCommand(@"
                    UPDATE DWSystem.PayrollPayments 
                    SET Cancelled = 1
                    WHERE Id=@Id;
                ",
                DBConnection.getSqlParameter(PayrollPaymentsModel.COL_Id.Name, id)
            );

            ActivityLogsController.Add(db, Session, id, "Cancelled");
            ActivityLogsController.Add(db, Session, Payrolls_Id, string.Format("Payment Cancelled: {0:N0}", Amount));
            db.SaveChanges();

            return Json(new { status = "200" }, JsonRequestBehavior.AllowGet);
        }

        /* REPORT *********************************************************************************************************************************************/

        public ActionResult Report(int year, int month, int? payDate, string Banks_Id, int? paymentType, int? Type)
        {
            DateTime payPeriod = new DateTime(year, month, 1, 0, 0, 0);

            var filename = string.Format("Payment Periode {0:yyyy-MM}{1}.xlsx", payPeriod, payDate == null ? "" : " Tanggal " + payDate);
            return Excel.GenerateExcelReport(filename, CompileExcelPackage(get(payPeriod, payDate, Banks_Id, true, null, null, Type)));
        }

        public ExcelPackage CompileExcelPackage(List<PayrollPaymentsModel> model)
        {
            ExcelPackage excelPackage = new ExcelPackage();

            Excel.SetWorkbookProperties(excelPackage);
            var workbook = excelPackage.Workbook;
            var ws = workbook.Worksheets.Add("Sheet1");

            //setting width column
            ws.Column(1).Width = 5;
            ws.Column(2).Width = 15;
            ws.Column(3).Width = 30;
            ws.Column(4).Width = 15;
            ws.Column(5).Width = 30;
            ws.Column(6).Width = 30;
            ws.Column(7).Width = 15;
            ws.Column(8).Width = 30;

            ws.Cells[1, 1].Value = "No";
            ws.Cells[1, 2].Value = "No Rekening";
            ws.Cells[1, 3].Value = "Nama Pegawai";
            ws.Cells[1, 4].Value = "Jumlah";
            ws.Cells[1, 5].Value = "Counter";
            ws.Cells[1, 6].Value = "Rekening Atas Nama";
            ws.Cells[1, 7].Value = "Nama Bank";
            ws.Cells[1, 8].Value = "Wilayah";
            ws.Cells[1, 1, 1, 8].Style.Font.Bold = true;
            ws.Cells[1, 1, 1, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            var headerBorder = ws.Cells[1, 1, 1, 8].Style.Border;
            headerBorder.Left.Style = headerBorder.Top.Style = headerBorder.Right.Style = headerBorder.Bottom.Style = ExcelBorderStyle.Thin;
            var headerFill = ws.Cells[1, 1, 1, 8].Style.Fill;
            headerFill.PatternType = ExcelFillStyle.Solid;
            headerFill.BackgroundColor.SetColor(Color.LightGray);

            ws.View.FreezePanes(2, 1);
            int rowIndex = 2;
            if (model.Count > 0)
            {
                foreach (var item in model.OrderBy(x => x.RowNumber))
                {
                    ws.Cells[rowIndex, 1].Value = item.RowNumber;
                    ws.Cells[rowIndex, 2].Value = item.AccountNumber;
                    ws.Cells[rowIndex, 3].Value = item.EmployeeFullName;
                    ws.Cells[rowIndex, 4].Value = item.Amount;
                    ws.Cells[rowIndex, 5].Value = item.CustomerName;
                    ws.Cells[rowIndex, 6].Value = item.AccountName;
                    ws.Cells[rowIndex, 7].Value = item.Banks_Name;
                    ws.Cells[rowIndex, 8].Value = item.Regions_Name;

                    var cellBorder = ws.Cells[rowIndex, 1, rowIndex, 8].Style.Border;
                    cellBorder.Left.Style = cellBorder.Top.Style = cellBorder.Right.Style = cellBorder.Bottom.Style = ExcelBorderStyle.Thin;

                    rowIndex++;
                }
            }
            else
            {
                ws.Cells[rowIndex, 1].Value = "No Data Available";
                ws.Cells[rowIndex, 1, rowIndex, 8].Merge = true;
                ws.Cells[rowIndex, 1, rowIndex, 8].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                var cellBorder = ws.Cells[rowIndex, 1, rowIndex, 8].Style.Border;
                cellBorder.Left.Style = cellBorder.Top.Style = cellBorder.Right.Style = cellBorder.Bottom.Style = ExcelBorderStyle.Thin;
                rowIndex++;
            }

            return excelPackage;
        }

        /******************************************************************************************************************************************************/
    }
}
using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LIBUtil;
using LIBWebMVC;

namespace CAJWebApp
{
    public struct ModelMember
    {
        public string Name;
        public Guid Id;
        public string Display;
    }

    public class Helper
    {
        private static readonly DBContext db = new DBContext();

        /* PUBLIC PROPERTIES **********************************************************************************************************************************/

        public const string APP_VERSION = "v201211";
        public const string COMPANYNAME = "CAJ";

        public const string IMAGEFOLDERURL = "/assets/img/";
        public const string IMAGEFOLDERPATH = "~"+ IMAGEFOLDERURL;
        public const string NOIMAGEFILE = "no-image.jpg";

        /* METHODS ********************************************************************************************************************************************/
        
        public static string getImageUrl(string imageName, HttpRequestBase Request, HttpServerUtilityBase Server)
        {
            string filename = NOIMAGEFILE;
            if (!string.IsNullOrEmpty(imageName))
            {
                string dir = Server.MapPath(IMAGEFOLDERPATH);
                string path = Path.Combine(dir, imageName);
                if (File.Exists(path))
                    filename = imageName;
            }

            return (Request.ApplicationPath + IMAGEFOLDERURL + filename).Replace("//", "/");
        }

        public static DateTime setFilterViewBag(ControllerBase controller, DateTime? PayPeriod, int? year, int? month, int? payDate, int? approval, string Banks_Id, string search, string periodChange, int? Type)
        {
            DateTime payPeriod;

            if (PayPeriod != null)
                payPeriod = (DateTime)PayPeriod;
            else if (month == null || year == null)
                payPeriod = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
            else
                payPeriod = new DateTime((int)year, (int)month, 1, 0, 0, 0);

            if (periodChange == EnumActions.Previous.ToString())
                payPeriod = payPeriod.AddMonths(-1);
            else if (periodChange == EnumActions.Next.ToString())
                payPeriod = payPeriod.AddMonths(1);

            var ViewBag = controller.ViewBag;
            ViewBag.PayPeriodYear = UtilWebMVC.validateParameter(payPeriod.Year);
            ViewBag.PayPeriodMonth = UtilWebMVC.validateParameter(payPeriod.Month);
            ViewBag.PayPeriod = UtilWebMVC.validateParameter(payPeriod);
            ViewBag.PayDate = UtilWebMVC.validateParameter(payDate);
            ViewBag.Approval = UtilWebMVC.validateParameter(approval);
            ViewBag.Banks_Id = UtilWebMVC.validateParameter(Banks_Id);
            ViewBag.Search = UtilWebMVC.validateParameter(search);
            ViewBag.Type = UtilWebMVC.validateParameter(Type);

            return payPeriod;
        }

        public static string GetTotalMonthsSinceJoin(DateTime JoinDate, DateTime PayPeriod)
        {
            int totalMonths = (PayPeriod.Month - JoinDate.Month) + 12 * (PayPeriod.Year - JoinDate.Year);
            if (totalMonths <= 12)
                return string.Format("{0} bulan", totalMonths);
            else if (totalMonths % 12 == 0)
                return string.Format("{0} tahun", totalMonths / 12);
            else
                return string.Format("{0} tahun {1} bulan", totalMonths / 12, totalMonths % 12);
        }

        /******************************************************************************************************************************************************/
    }
}
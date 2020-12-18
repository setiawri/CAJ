using System.Web.Mvc;

namespace CAJWebApp.Areas.PAYROLL
{
    public class PAYROLLAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PAYROLL";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PAYROLL_default",
                "PAYROLL/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
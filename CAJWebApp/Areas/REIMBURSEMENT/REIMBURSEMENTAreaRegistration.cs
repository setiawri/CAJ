using System.Web.Mvc;

namespace CAJWebApp.Areas.REIMBURSEMENT
{
    public class REIMBURSEMENTAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "REIMBURSEMENT";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "REIMBURSEMENT_default",
                "REIMBURSEMENT/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
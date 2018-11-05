namespace PandaWebApp.Controllers
{
    using PandaWebApp.Data;
    using SIS.MvcFramework;

    public class BaseController : Controller
    {
        public BaseController()
        {
            this.Context = new PandaDbContext();
        }

        public PandaDbContext Context { get; set; }
    }
}

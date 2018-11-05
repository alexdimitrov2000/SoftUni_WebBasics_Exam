namespace PandaWebApp.Controllers
{
    using PandaWebApp.Models.Enums;
    using PandaWebApp.ViewModels.Home;
    using SIS.HTTP.Responses;
    using System.Collections.Generic;
    using System.Linq;

    public class HomeController : BaseController
    {
        public IHttpResponse Index()
        {
            if (this.User.IsLoggedIn)
            {
                var userUsername = this.User.Username;

                var packageCollection = new IndexPackageCollectionViewModel();

                var pendingPackages = this.Context.Packages
                    .Where(p => p.Status == PackageStatus.Pending && p.Recipient.Username == userUsername)
                    .Select(p => new IndexPackageViewModel
                    {
                        Id = p.Id,
                        Description = p.Description
                    })
                    .ToList();

                var shippedPackages = this.Context.Packages
                    .Where(p => p.Status == PackageStatus.Shipped && p.Recipient.Username == userUsername)
                    .Select(p => new IndexPackageViewModel
                    {
                        Id = p.Id,
                        Description = p.Description
                    })
                    .ToList();

                var deliveredPackages = this.Context.Packages
                    .Where(p => p.Status == PackageStatus.Delivered && p.Recipient.Username == userUsername)
                    .Select(p => new IndexPackageViewModel
                    {
                        Id = p.Id,
                        Description = p.Description
                    })
                    .ToList();

                packageCollection.Pending = pendingPackages;
                packageCollection.Shipped = shippedPackages;
                packageCollection.Delivered = deliveredPackages;

                return this.View("Home/LoggedIndex", packageCollection);
            }

            return this.View();
        }
    }
}

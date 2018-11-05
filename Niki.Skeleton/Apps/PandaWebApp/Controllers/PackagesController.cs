using PandaWebApp.Models;
using PandaWebApp.Models.Enums;
using PandaWebApp.ViewModels.Home;
using PandaWebApp.ViewModels.Packages;
using SIS.HTTP.Responses;
using SIS.MvcFramework;
using System;
using System.Linq;

namespace PandaWebApp.Controllers
{
    public class PackagesController : BaseController
    {
        [Authorize]
        public IHttpResponse Details(int id)
        {
            var packageExists = this.Context.Packages.Any(p => p.Id == id);

            if (!packageExists)
            {
                return this.BadRequestErrorWithView("Invalid package ID");
            }

            var package = this.Context.Packages.First(p => p.Id == id);

            var packageViewModel = this.Context.Packages
                .Where(p => p.Id == id)
                .Select(p => new PackageDetailsViewModel
                {
                    Description = p.Description,
                    RecipientName = p.Recipient.Username,
                    ShippingAddress = p.ShippingAddress,
                    Status = p.Status.ToString(),
                    Weight = p.Weight.ToString()
                })
                .First();

            if (packageViewModel.Status == "Pending")
            {
                packageViewModel.EstimatedDeliveryDate = "N/A";
            }
            else if (packageViewModel.Status == "Shipped")
            {
                packageViewModel.EstimatedDeliveryDate = package.EstimatedDeliveryDate?.ToString("dd/MM/yyyy");
            }
            else
            {
                packageViewModel.EstimatedDeliveryDate = "Delivered";
            }

            return this.View(packageViewModel);
        }

        [Authorize("Admin")]
        public IHttpResponse Create()
        {
            var recipients = this.Context.Users
                .Select(u => u.Username)
                .ToList();

            var recipientNamesViewModel = new RecipientNamesViewModel
            {
                Recipients = recipients
            };

            return this.View(recipientNamesViewModel);
        }

        [Authorize("Admin")]
        [HttpPost]
        public IHttpResponse Create(PackageCreateInputModel model)
        {
            var description = model.Description;
            var weight = model.Weight;
            var shippingAddress = model.ShippingAddress;
            var recipientName = model.Recipient;

            var recipient = this.Context.Users
                .FirstOrDefault(u => u.Username == recipientName);

            if (recipient == null)
            {
                return this.BadRequestErrorWithView("Invalid recipient username.");
            }

            var package = new Package
            {
                Description = description,
                ShippingAddress = shippingAddress,
                Weight = double.Parse(weight),
                Recipient = recipient,
                Status = PackageStatus.Pending,
                EstimatedDeliveryDate = null
            };

            this.Context.Packages.Add(package);

            try
            {
                this.Context.SaveChanges();
            }
            catch (System.Exception e)
            {
                return this.BadRequestError(e.Message);
            }

            return this.Redirect("/");
        }

        [Authorize("Admin")]
        public IHttpResponse Pending()
        {
            var pendingPackages = this.Context.Packages
                    .Where(p => p.Status == PackageStatus.Pending)
                    .Select(p => new PackageViewModel
                    {
                        Id = p.Id,
                        Description = p.Description,
                        RecipientName = p.Recipient.Username,
                        ShippingAddress = p.ShippingAddress,
                        Weight = p.Weight.ToString("F2")
                    })
                    .ToList();

            var packagesCollection = new PackageCollectionViewModel
            {
                Packages = pendingPackages
            };

            return this.View(packagesCollection);
        }

        [Authorize("Admin")]
        public IHttpResponse Shipped()
        {
            var pendingPackages = this.Context.Packages
                       .Where(p => p.Status == PackageStatus.Shipped)
                       .Select(p => new PackageViewModel
                       {
                           Id = p.Id,
                           Description = p.Description,
                           RecipientName = p.Recipient.Username,
                           ShippingAddress = p.ShippingAddress,
                           Weight = p.Weight.ToString("F2")
                       })
                       .ToList();

            var packagesCollection = new PackageCollectionViewModel
            {
                Packages = pendingPackages
            };

            return this.View(packagesCollection);
        }

        [Authorize("Admin")]
        public IHttpResponse Delivered()
        {
            var pendingPackages = this.Context.Packages
                       .Where(p => p.Status == PackageStatus.Delivered || p.Status == PackageStatus.Acquired)
                       .Select(p => new PackageViewModel
                       {
                           Id = p.Id,
                           Description = p.Description,
                           RecipientName = p.Recipient.Username,
                           ShippingAddress = p.ShippingAddress,
                           Weight = p.Weight.ToString("F2")
                       })
                       .ToList();

            var packagesCollection = new PackageCollectionViewModel
            {
                Packages = pendingPackages
            };

            return this.View(packagesCollection);
        }

        [Authorize("Admin")]
        public IHttpResponse Ship(int id)
        {
            var packageExists = this.Context.Packages.Any(p => p.Id == id);

            if (!packageExists)
            {
                return this.BadRequestErrorWithView("Invalid package ID");
            }

            var package = this.Context.Packages.First(p => p.Id == id);

            package.Status = PackageStatus.Shipped;

            var randomDays = new Random().Next(20, 40);
            package.EstimatedDeliveryDate = DateTime.UtcNow.AddDays(randomDays);
            this.Context.SaveChanges();

            return this.Redirect("/");
        }

        [Authorize("Admin")]
        public IHttpResponse Deliver(int id)
        {
            var packageExists = this.Context.Packages.Any(p => p.Id == id);

            if (!packageExists)
            {
                return this.BadRequestErrorWithView("Invalid package ID");
            }

            var package = this.Context.Packages.First(p => p.Id == id);

            package.Status = PackageStatus.Delivered;
            
            this.Context.SaveChanges();

            return this.Redirect("/");
        }

        [Authorize]
        public IHttpResponse Acquire(int id)
        {
            var packageExists = this.Context.Packages.Any(p => p.Id == id);

            if (!packageExists)
            {
                return this.BadRequestErrorWithView("Invalid package ID");
            }

            var package = this.Context.Packages.First(p => p.Id == id);

            package.Status = PackageStatus.Acquired;

            this.Context.SaveChanges();

            var recipient = this.Context.Users.First(u => u.Username == this.User.Username);

            var receipt = new Receipt
            {
                Package = package,
                Recipient = recipient,
                IssuedOn = DateTime.UtcNow,
                Fee = (decimal)package.Weight * 2.67m
            };

            this.Context.Receipts.Add(receipt);
            this.Context.SaveChanges();

            return this.Redirect("/");
        }
    }
}

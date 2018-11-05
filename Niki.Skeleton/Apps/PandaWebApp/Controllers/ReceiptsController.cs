using PandaWebApp.ViewModels.Receipts;
using SIS.HTTP.Responses;
using SIS.MvcFramework;
using System.Linq;

namespace PandaWebApp.Controllers
{
    public class ReceiptsController : BaseController
    {
        [Authorize]
        public IHttpResponse Index()
        {
            var receipts = this.Context.Receipts
                .Where(r => r.Recipient.Username == this.User.Username)
                .Select(r => new ReceiptDetailsViewModel
                {
                    Id = r.Id,
                    IssuedOn = r.IssuedOn.ToString("dd/MM/yyyy"),
                    RecipientName = r.Recipient.Username,
                    Fee = r.Fee.ToString("F2")
                })
                .ToList();

            var receiptCollection = new ReceiptCollectionViewModel
            {
                Receipts = receipts
            };

            return this.View(receiptCollection);
        }

        [Authorize]
        public IHttpResponse Details(int id)
        {
            var receiptViewModel = this.Context.Receipts
                .Where(r => r.Id == id)
                .Select(r => new ReceiptDetailsViewModel
                {
                    Id = r.Id,
                    Address = r.Package.ShippingAddress,
                    Fee = r.Fee.ToString("F2"),
                    IssuedOn = r.IssuedOn.ToString("dd/MM/yyyy"),
                    PackageDescription = r.Package.Description,
                    PackageWeight = r.Package.Weight.ToString("F2"),
                    RecipientName = r.Recipient.Username
                })
                .First();
            
            return this.View(receiptViewModel);
        }
    }
}

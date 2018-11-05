namespace PandaWebApp.ViewModels.Receipts
{
    public class ReceiptDetailsViewModel
    {
        public int Id { get; set; }

        public string IssuedOn { get; set; }

        public string Address { get; set; }

        public string RecipientName { get; set; }

        public string PackageWeight { get; set; }

        public string Fee { get; set; }

        public string PackageDescription { get; set; }
    }
}

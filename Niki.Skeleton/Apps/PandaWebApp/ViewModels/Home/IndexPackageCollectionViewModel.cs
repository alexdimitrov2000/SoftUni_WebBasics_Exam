using System.Collections.Generic;

namespace PandaWebApp.ViewModels.Home
{
    public class IndexPackageCollectionViewModel
    {
        public List<IndexPackageViewModel> Pending { get; set; }
        public List<IndexPackageViewModel> Shipped { get; set; }
        public List<IndexPackageViewModel> Delivered { get; set; }
    }
}

using System.Web;
using System.Web.Optimization;

namespace NBAScoringBelt
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/css").Include("~/Content/bootstrap.yeti.min.css",
                                                                 "~/Content/site.css"));
        }
    }
}

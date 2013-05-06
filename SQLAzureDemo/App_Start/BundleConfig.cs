using System.Web.Optimization;

namespace SQLAzureDemo
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/footer").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/bootstrap.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-responsive.css"
            ));
        }
    }
}
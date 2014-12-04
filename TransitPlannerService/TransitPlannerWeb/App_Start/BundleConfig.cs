using System.Web;
using System.Web.Optimization;

namespace TransitPlannerWeb
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            // ====== SCRIPTS ====== //

            bundles.Add(new ScriptBundle("jquery.js").Include(
                        "~/Content/js/jquery-ui-1.11.2.custom/external/jquery/jquery.js",
                        "~/Content/js/jquery-ui-1.11.2.custom/jquery-ui.js",
                        "~/Content/js/jquery-ui-timepicker-addon.js"));

            bundles.Add(new ScriptBundle("fancytree.js").Include(
                        "~/Content/js/fancytree/src/jquery.fancytree.js",
                        "~/Content/js/fancytree/src/jquery.fancytree.filter.js"));

            bundles.Add(new ScriptBundle("menetrend.js").Include(
                        "~/Content/js/markerwithlabel.js",
                        "~/Content/js/menetrend.js"));

            // ====== STYLES ====== //

            bundles.Add(new StyleBundle("jquery.css").Include(
                "~/Content/js/jquery-ui-1.11.2.custom/jquery-ui.css",
                "~/Content/js/jquery-ui-timepicker-addon.css"));

            bundles.Add(new StyleBundle("fancytree.css").Include(
                "~/Content/js/fancytree/skin-win7/ui.fancytree.css"));

            bundles.Add(new StyleBundle("menetrend.css").Include(
                "~/Content/tps-web.css"));
        }
    }
}

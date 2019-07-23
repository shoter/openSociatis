using System.Web;
using System.Web.Optimization;

namespace Sociatis
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
          //  BundleTable.EnableOptimizations = true;


            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.blockUI.js",
                        "~/Scripts/jquery.postJson.js",
                        "~/Scripts/utils/shoter.paddle.js",
                        "~/Scripts/jquery.disabled.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.dialogOptions.js"));

            bundles.Add(new ScriptBundle("~/scripts/global").Include(
                        "~/Scripts/sociatis.global.js", //it must be first FILE
                        "~/Scripts/sociatis.utils.js",
                        "~/Scripts/utils/sociatis.debug.js",
                        "~/Scripts/enums/sociatis.enums.json_status.js",
                        "~/Scripts/jsrender.js",
                        "~/Scripts/utils/sociatis.blockUI.js",
                        "~/Scripts/store.everything.js",
                        "~/Scripts/Shared/dynamicTooltip.js"
                        ));

            bundles.Add(new ScriptBundle("~/scripts/scrolling").Include(
                "~/Scripts/simplebar.js"));

            bundles.Add(new ScriptBundle("~/scripts/notify", "https://cdnjs.cloudflare.com/ajax/libs/notify/0.4.2/notify.min.js").Include(
               "~/Scripts/utils/notify.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/bundles/uploader").Include(
                "~/Scripts/utils/fileuploader.js"));

            bundles.Add(new StyleBundle("~/Content/fileuploader").Include(
                "~/Content/fileuploader.css"));

            bundles.Add(new ScriptBundle("~/bundles/markdown").Include(
                        "~/Scripts/marked.js",
                        "~/Scripts/bootstrap-markdown-editor.js"));

            bundles.Add(new ScriptBundle("~/bundles/plot").Include(
                "~/Scripts/flot/jquery.flot.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/select2", "https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.4/js/select2.full.min.js").Include(
                "~/Scripts/select2.js",
                "~/Scripts/utils/sociatis.select2.js"));

            bundles.Add(new StyleBundle("~/Content/select2", "https://cdnjs.cloudflare.com/ajax/libs/select2/4.0.4/css/select2.min.css").Include
                ("~/Content/select2.css"));

            bundles.Add(new ScriptBundle("~/bundles/cropper").Include(
                    "~/Scripts/utils/cropper.js"
                ));

            bundles.Add(new StyleBundle("~/content/cropper").Include(
                "~/Content/cropper.css"
                ));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/foundation").Include(
                      "~/Scripts/foundation.js",
                      "~/Scripts/foundation-select.js",
                      "~/Scripts/what-input.min.js"));

            bundles.Add(new StyleBundle("~/Content/scrolling").Include(
                "~/Content/simplebar.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/foundation.css",
                      "~/Content/foundation-select.css",
                      "~/Content/themes/base/*.css",
                      "~/Content/site.css",
                      "~/Content/customFonts.css",
                      "~/Content/customStyles.css",
                      "~/Content/Styles/main.css",
                      "~/Content/font-awesome.css",
                      "~/Content/govicons.min.css",
                      "~/Content/militaryRankStyles.css",
                      "~/Content/colorpicker.css",
                      "~/Content/leaflet.css",
                      "~/Content/jquery.ui.css"
                      ));

            bundles.Add(new StyleBundle("~/Content/markdown").Include(
                      "~/Content/markdown_wyswig.css"));
        }
    }
}

using System.Web;
using System.Web.Mvc;
using FT.Utility.UEditor;
namespace FT.UI.Areas.PlatAdmin.Controllers
{
    public class UEditorController : Controller
    {
        
        // GET: UEditor
        public ContentResult Handle()
        {
            IUEditorHandle handle = null;
            string action = Request["action"];
            switch (action)
            {
                case "config":
                    handle = new ConfigHandler();
                    break;
                case "uploadimage":
                    var config = new UploadConfig()
                    {
                        AllowExtensions = Config.GetStringList("imageAllowFiles"),
                        PathFormat = Config.GetString("imagePathFormat"),
                        SizeLimit = Config.GetInt("imageMaxSize"),
                        UploadFieldName = Config.GetString("imageFieldName")
                    };
                    handle = new UploadHandle(config);
                    break;
                default:
                    handle = new NotSupportedHandler();
                    break;
            }

            var result = handle.Process();
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return Content(jsonString);
        }

        [HttpPost]
        // GET: UEditor
        public ContentResult Handle(string action)
        {
            IUEditorHandle handle = null;
            action = Request["action"];
            switch (action)
            {
                case "config":
                    handle = new ConfigHandler();
                    break;
                case "uploadimage":
                    var config = new UploadConfig()
                    {
                        AllowExtensions = Config.GetStringList("imageAllowFiles"),
                        PathFormat = Config.GetString("imagePathFormat"),
                        SizeLimit = Config.GetInt("imageMaxSize"),
                        UploadFieldName = Config.GetString("imageFieldName")
                    };
                    handle = new UploadHandle(config);
                    break;
                default:
                    handle = new NotSupportedHandler();
                    break;
            }

            var result = handle.Process();
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return Content(jsonString);
        }





        //  
        // GET: /Upload/  
        [HttpGet]
        public ActionResult Upload()
        {
            string url = Request.QueryString["url"];
            if (url == null)
            {
                url = "";
            }
            ViewData["url"] = url;
            return View();
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase filename)
        {
            //具体的保存代码  
            return View();
        }  

    }
}


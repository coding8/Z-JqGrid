﻿using Helper;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HelloJqGrid.Controllers
{
    public class FineUploaderController : Controller
    {
        //
        // GET: /FineUploader/

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase qqfile)//qqfile是个固定的名称！
        {
            string msg = "";
            if (qqfile != null && qqfile.ContentLength > 0)
            {
                var fileNameWithExtension = Path.GetFileName(qqfile.FileName);
                var fileName = Path.GetFileNameWithoutExtension(qqfile.FileName);
                var path = Path.Combine(Server.MapPath("~/Uploads"), fileNameWithExtension);

                try
                {
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                    qqfile.SaveAs(path);

                    //写入数据库
                    string tbName = "";
                    List<string> columnMapping = null;
                    ImportAndExport.MappingColumn(fileName, out tbName, out columnMapping);
                    ImportAndExport.ImportExcel(path, tbName, columnMapping);
                    System.IO.File.Delete(path);
                    msg = "<p style='background-color:#5DA30C'>写入成功: " + fileName + " --> 时间：" + DateTime.Now + "</p>";
                }
                catch (Exception ex)
                {
                    System.IO.File.Delete(path);
                    msg = "<p style='background-color:#D60000'>写入失败: " + fileName + " --> 时间：" + DateTime.Now + " --> [" + ex.Message + "]</p>";
                    var errorMsg = "写入失败: " + fileName + " --> 时间：" + DateTime.Now + " --> [" + ex.Message + "]";
                    return Json(new { success = false, error = errorMsg, msg = msg }, "text/html");
                }
            }
            //返回的json要有success字段：success = true 上传成功；success=false 上传失败。
            return Json(new { success = true, name = qqfile.FileName, msg = msg }, "text/html");
        }
    }
}
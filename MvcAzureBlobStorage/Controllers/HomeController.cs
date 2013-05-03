using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MvcAzureBlogStorage.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcAzureBlogStorage.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient storageClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer storageContainer = storageClient.GetContainerReference(
                ConfigurationManager.AppSettings.Get("CloudStorageContainerReference"));
            CloudFilesModel blobsList = new
                CloudFilesModel(storageContainer.ListBlobs(useFlatBlobListing: true));
            return View(blobsList);
        }

        public ActionResult UploadFile()
        {
            if (Request.Files.Count > 0)
            {
                CloudStorageAccount storageAccount =
                CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("StorageConnectionString"));
                var storageClient = storageAccount.CreateCloudBlobClient();
                var storageContainer = storageClient.GetContainerReference(
                    ConfigurationManager.AppSettings.Get("CloudStorageContainerReference"));
                storageContainer.CreateIfNotExists();
                
                for (int fileNum = 0; fileNum < Request.Files.Count; fileNum++)
                {
                    string fileName = Path.GetFileName(Request.Files[fileNum].FileName);
                    if (
                        Request.Files[fileNum] != null &&
                        Request.Files[fileNum].ContentLength > 0)
                    {
                        CloudBlockBlob azureBlockBlob = storageContainer.GetBlockBlobReference(fileName);
                        azureBlockBlob.UploadFromStream(Request.Files[fileNum].InputStream);
                    }
                }
                return RedirectToAction("Index");
            }
            return View("UploadFile");
        }
    }
}

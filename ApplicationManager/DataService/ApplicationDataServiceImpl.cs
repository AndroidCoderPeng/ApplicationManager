using System.Collections.Generic;
using System.IO;
using ApplicationManager.Models;
using Newtonsoft.Json;

namespace ApplicationManager.DataService
{
    public class ApplicationDataServiceImpl : IApplicationDataService
    {
        public List<PermissionModel> GetAndroidPermissions(string filePath)
        {
            var permissions = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<PermissionModel>>(permissions);
        }
    }
}
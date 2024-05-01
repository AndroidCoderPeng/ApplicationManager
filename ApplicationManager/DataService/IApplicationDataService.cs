using System.Collections.Generic;
using ApplicationManager.Models;

namespace ApplicationManager.DataService
{
    public interface IApplicationDataService
    {
        List<PermissionModel> GetAndroidPermissions(string filePath);
    }
}
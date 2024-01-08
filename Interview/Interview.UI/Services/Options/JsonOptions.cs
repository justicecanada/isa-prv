using Interview.UI.Models.Options;
using Newtonsoft.Json;

namespace Interview.UI.Services.Options
{
    
    public class JsonOptions : IOptions
    {

        private readonly string _wwwRootPath;

        public JsonOptions()
        {
            _wwwRootPath = Path.Combine(Path.GetFullPath("wwwroot"), "JsonOptions");
        }

        public List<DepartmentOption> GetDepartmentOptions()
        {

            List<DepartmentOption> result = null;
            string jsonFile = Path.Combine(_wwwRootPath, "Departments.json");
            string json = File.ReadAllText(jsonFile);

            result = JsonConvert.DeserializeObject<List<DepartmentOption>>(json);

            return result;

        }

    }

}

using Microsoft.AspNetCore.Mvc.Routing;

namespace payroll_api.Common
{
    public class ApiAreaRoute : Attribute, IRouteTemplateProvider
    {
        private const string TEMPLATE = "api/[area]/[controller]/[action]";
        public string Template => TEMPLATE;
        public int? Order { get; set; }
        public string? Name { get; set; }
    }
}

// Models/Helpers/Ctrl.cs
namespace FIT_Technology.Models.Helpers
{
    public static class Ctrl
    {
        public static string Get<T>() where T : Microsoft.AspNetCore.Mvc.Controller
            => typeof(T).Name.Replace("Controller", "");
    }
}
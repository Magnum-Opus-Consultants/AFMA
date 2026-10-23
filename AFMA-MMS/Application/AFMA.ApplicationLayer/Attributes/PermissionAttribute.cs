namespace AFMA.ApplicationLayer.Attributes
{
  using Helpers;
  using Interface;
  using Microsoft.AspNetCore.Mvc.Filters;
  using System;

  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
  public class ModulePermissionAttribute : ActionFilterAttribute
  {
    private readonly ModuleEnum _module;
    private readonly IApplicationLayer _applicationLayer;

    public ModulePermissionAttribute(IApplicationLayer applicationLayer, ModuleEnum module)
    {
      _module = module;
      _applicationLayer = applicationLayer;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      var userPermission = _applicationLayer.CheckMemberAccountModule(3404, _module).Result;

      if (userPermission)
      {
        return;
      }

      throw new UnauthorizedAccessException("You are not authorized to access this module");
      // Otherwise, respect the attribute and work your magic here!
      //
      //
      //
    }
  }
}

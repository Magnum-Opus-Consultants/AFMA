// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using AFMA;
using AFMA.ApplicationLayer.Interface;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AFMA.Stats.Admin.Areas.Identity.Pages.Account
{
  public class LogoutModel : PageModel
  {
    private readonly IIdentityLayer identityLayer;

    public LogoutModel(IIdentityLayer identityLayer)
    {
      this.identityLayer = identityLayer;
    }

    public async Task<IActionResult> OnGet()
    {
      await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
      await identityLayer.Identity.SignOutAsync().ConfigureAwait(false);
      return LocalRedirect(Url.Content("~/"));
    }
  }
}

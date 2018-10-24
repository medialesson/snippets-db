using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Snippets.Web.Views.Emails
{
    public class RegistrationModel : PageModel
    {
        public string DisplayName { get; set; }

        public string VerificationUrl { get; set; }

        public void OnGet()
        {

        }
    }
}

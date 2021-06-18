using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GSM.Models;
using GSM.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace GSM.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required, Display(Name ="Razón Social")]
            [MinLength(5), MaxLength(60)]
            public string RazonSocial { get; set; }

            [Required, Display(Name ="R.U.T.")]
            [MaxLength(10), MinLength(9)]
            public string Rut{ get; set; }

            [Required]
            [MinLength(5), MaxLength(60)]
            public string Direccion { get; set; }

            [Required]
            [Display(Name ="Teléfono")]
            [MaxLength(15), MinLength(5)]
            public string PhoneNumber { get; set; }

            [Required]
            [Display(Name = "Ciudad")]
            [MaxLength(15), MinLength(5)]
            public string Ciudad { get; set; }

            [Display(Name = "¿Es Admin?")]
            public bool IsAdmin { get; set; }
        }

        public void OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    RazonSocial = Input.RazonSocial,
                    Rut = Input.Rut,
                    Direccion = Input.Direccion,
                    PhoneNumber = Input.PhoneNumber,
                    Ciudad = Input.Ciudad
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {                   

                    if (!await _roleManager.RoleExistsAsync(SD.AdminEndUser))
                        await _roleManager.CreateAsync(new IdentityRole(SD.AdminEndUser));

                    if (!await _roleManager.RoleExistsAsync(SD.CustomerEndUser))
                        await _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser));

                    if (Input.IsAdmin)
                        await _userManager.AddToRoleAsync(user, SD.AdminEndUser);
                    else
                        await _userManager.AddToRoleAsync(user, SD.CustomerEndUser);



                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using AppIdea.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

namespace AppIdea.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppIdeaUser> _signInManager;
        private readonly UserManager<AppIdeaUser> _userManager;
        private readonly IUserStore<AppIdeaUser> _userStore;
        private readonly IUserEmailStore<AppIdeaUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly AppIdeaContext _context;

        // Các dịch vụ được Inject vào: UserManger, SignInManager, ILogger, IEmailSender
        public RegisterModel(
            UserManager<AppIdeaUser> userManager,
            IUserStore<AppIdeaUser> userStore,
            SignInManager<AppIdeaUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            AppIdeaContext context)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]

        //create a list add depart to show view

        public  Department Depart { get; set; }
        public SelectList DepartmentNameSL { get; set; }
        //public List<SelectListItem> Options { get; set; }

        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        ///  // Xác thực từ dịch vụ ngoài (Googe, Facebook ... bài này chứa thiết lập)
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        ///         // Lớp InputModel chứa thông tin Post tới dùng để tạo User
        public class InputModel
        {
            [Required]
            [StringLength(255, ErrorMessage = "the name filed shou have maximum of the 255 chracters")]
            [Display(Name = "Firstname")]
            public string Firstname { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

  

        }

       
        public async Task OnGetAsync(string returnUrl = null, object selectedDepartment = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            //add department into a list to show the viewpage
           var departmentsQuery = from d in _context.Departments
                                  orderby d.Name // Sort by name.
                                  select d;

            DepartmentNameSL = new SelectList(departmentsQuery.AsNoTracking(),
                nameof(Department.Id),// key
                nameof(Department.Name),//value
                selectedDepartment);// object selected
            


        }

        // Đăng ký tài khoản theo dữ liệu form post tới
        public async Task<IActionResult> OnPostAsync(string returnUrl = null, InputModel Input = null!)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();


            //var departmentsQuery = from d in _context.Departments
            //                       orderby d.Name // Sort by name.
            //                       select d;

            //DepartmentNameSL = new SelectList(departmentsQuery.AsNoTracking(),
            //    nameof(Department.Id),
            //    nameof(Department.Name),
            //    User.Id);

            /// // object selected
            
            string Departid = Depart.Id;

            if (ModelState.IsValid)
            {
        

                // Tạo AppIdeaUser sau đó tạo User mới (cập nhật vào db)
                var user = CreateUser();

                    user.Firstname = Input.Firstname;
                    
                    user.DepartmentsId =  Departid ;
                

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);


                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // phát sinh token theo thông tin user để xác nhận email
                    // mỗi user dựa vào thông tin sẽ có một mã riêng, mã này nhúng vào link
                    // trong email gửi đi để người dùng xác nhận
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    // callbackUrl = /Account/ConfirmEmail?userId=useridxx&code=codexxxx
                    // Link trong email người dùng bấm vào, nó sẽ gọi Page: /Acount/ConfirmEmail để xác nhận
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    // Gửi email  
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        // Nếu cấu hình phải xác thực email mới được đăng nhập thì chuyển hướng đến trang
                        // RegisterConfirmation - chỉ để hiện thông báo cho biết người dùng cần mở email xác nhận
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        // Không cần xác thực - đăng nhập luôn
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                // Có lỗi, đưa các lỗi thêm user vào ModelState để hiện thị ở html heleper: asp-validation-summary
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private AppIdeaUser CreateUser()
        {
            try
            {

                return Activator.CreateInstance<AppIdeaUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(AppIdeaUser)}'. " +
                    $"Ensure that '{nameof(AppIdeaUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<AppIdeaUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<AppIdeaUser>)_userStore;
        }
    }
}

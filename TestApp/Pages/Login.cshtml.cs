using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TestApp.Repositories;

public class LoginModel : PageModel
{
    [BindProperty] public string Email { get; set; }
    [BindProperty] public string Password { get; set; }

    private readonly IUserRepository userRepository;
    public LoginModel(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
        
    }

    public async Task<IActionResult> OnPost()
    {
        if (Email == "admin@example.com" && Password == "Admin@123")
        {
            return RedirectToPage("/UploadVideo");
        }

        var token = await userRepository.AuthenticateUser(Email, Password);

        if (!string.IsNullOrEmpty(token))
        {
            // Store token in local storage using JavaScript
            HttpContext.Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = false, // Allow access via JavaScript
                Secure = true, // Only for HTTPS
                SameSite = SameSiteMode.Strict
            });

            return RedirectToPage("/VideoSelection");
        }

        ModelState.AddModelError(string.Empty, "Invalid login credentials.");
        return Page();
    }

}

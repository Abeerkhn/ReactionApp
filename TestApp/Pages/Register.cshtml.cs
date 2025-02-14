using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using TestApp.Repositories;

namespace TestApp.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IUserRepository _userRepository;

        public RegisterModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [BindProperty]
        public string FullName { get; set; }
        [BindProperty]
        public string Email { get; set; }
        [BindProperty]
        public string Password { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var result = await _userRepository.RegisterUser(FullName, Email, Password);

            if (result == "Email already exists.")
            {
                ModelState.AddModelError(string.Empty, result);
                return Page();
            }

            return RedirectToPage("/Login");
        }
    }
}

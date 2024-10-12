using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SMS.Notification;
using SMS.Web.Services;
using SMS.AppCore.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SMS.Web.Models;
using System.Text;
using SMS.Domain.Entities;
using AutoMapper;
using SMS.AppCore.DTOs;

namespace SMS.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepo;
        private readonly IEmailSender _sender;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _backgroundTasks;
        private readonly IMapper _mapper;

        public UserController(IEmailService bgtasks, 
                              IUserRepository userRepository, 
                              UserManager<ApplicationUser> userManager, 
                              IEmailSender emailSender, 
                              IConfiguration configuration, 
                              RoleManager<IdentityRole> roleManager,
                              IMapper mapper)
        {
            _userRepo = userRepository;
            _sender = emailSender;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _backgroundTasks = bgtasks;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userRepo.GetAllUserAsync();
            var roles = await _userRepo.GetAllRolesAsync();

            UserViewModel VM = new UserViewModel();

            VM.Users = _mapper.Map<IEnumerable<UserDTO>>(users);
            VM.Roles = _mapper.Map<IEnumerable<RoleDTO>>(roles);

            return View(VM);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUser(UserViewModel model)
        {
            var defaultPassword = GenerateRandomPassword();
            model.User.Password = defaultPassword;

            var result = await _userRepo.SaveUserAsync(model.User, model.RoleId);

            if (result)
            {
                var userEmailMessage = $"Your account has been created. Your default password is: {defaultPassword}";
                await _backgroundTasks.SendEmailAsync(model.User.Email, "Your Account Details", userEmailMessage);

                // Send notification to admin
                var adminEmailMessage = $"A new user has been created:\n\nName: {model.User.FirstName} {model.User.LastName}\nEmail: {model.User.Email}\nDefault Password: {defaultPassword}";
                await _backgroundTasks.SendEmailAsync(_configuration["EmailSettings:AdminEmail"], "New User Created", adminEmailMessage);

                return RedirectToAction("Index", "User");
            }

            return View();
        }

        

        private string GenerateRandomPassword(int length = 12)
        {
            // Ensure minimum length of 12
            if (length < 12)
            {
                length = 12;
            }

            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "1234567890";
            const string special = "!@#$%^&*()_+";

            var random = new Random();
            var password = new StringBuilder();

            // Ensure at least one character from each category
            password.Append(lowercase[random.Next(lowercase.Length)]);
            password.Append(uppercase[random.Next(uppercase.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(special[random.Next(special.Length)]);

            // Fill the rest of the password length with random characters from all categories
            var allCharacters = lowercase + uppercase + digits + special;
            for (int i = 4; i < length; i++)
            {
                password.Append(allCharacters[random.Next(allCharacters.Length)]);
            }

            // Shuffle the password to ensure randomness
            var shuffledPassword = password.ToString().OrderBy(c => random.Next()).ToArray();

            return new string(shuffledPassword);
        }
    }
}

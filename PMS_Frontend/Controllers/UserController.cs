using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PMS_Frontend.Models.ViewModels.UserVM;

namespace PMS_Frontend.Controllers
{
    public class UserController : Controller
    {
        readonly HttpClient _httpClient;

        public UserController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(DtoRegister dto)
        {
            string apiUrl = "https://localhost:7202/api/user/create";
            var jsonContent = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return RedirectToAction("Login");
            }
            else
            {
                return View("Error");
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(DtoLogin dtoLogin)
        {
            string apiUrl = "https://localhost:7202/api/user/login";
            var jsonContent = new StringContent(JsonSerializer.Serialize(dtoLogin), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TokenResponse>();

                if (result != null)
                {
                    HttpContext.Session.SetString("JwtToken", result.Token);
                    HttpContext.Session.SetString("UserNickName", dtoLogin.NickName);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Token alınırken bir hata oluştu.";
                    return View(dtoLogin);
                }
            }
            else
            {
                ViewBag.Error = "Geçersiz kullanıcı adı veya şifre!";
                return View(dtoLogin);
            }
        }

        public async Task<IActionResult> Profile()
        {
            var userNickName = HttpContext.Session.GetString("UserNickName");
            if (string.IsNullOrEmpty(userNickName))
            {
                return RedirectToAction("Login");
            }
            
            string apiUrl = "https://localhost:7202/api/user/getuserbynickname";
            var userDto = new DtoUserNickName { NickName = userNickName };
            var jsonContent = new StringContent(JsonSerializer.Serialize(userDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, jsonContent);
            
            if (response.IsSuccessStatusCode)
            {
                string userIdStr = await response.Content.ReadAsStringAsync();
                var userId = JsonSerializer.Deserialize<Guid>(userIdStr);
                
                string profileUrl = "https://localhost:7202/api/user/getprofile";
                var profileRequest = new { UserId = userId };
                var profileContent = new StringContent(JsonSerializer.Serialize(profileRequest), Encoding.UTF8, "application/json");
                var profileResponse = await _httpClient.PostAsync(profileUrl, profileContent);
                
                if (profileResponse.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var profile = await profileResponse.Content.ReadFromJsonAsync<DtoProfileUpdate>(options);
                    
                    if (profile != null)
                    {
                        return View(profile);
                    }
                }
                
                var basicProfile = new DtoProfileUpdate
                {
                    UserId = userId,
                    NickName = userNickName
                };
                
                return View(basicProfile);
            }
            
            return View("Error");
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(DtoProfileUpdate model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }
            
            try
            {
                string apiUrl = "https://localhost:7202/api/user/updateprofile";
                var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiUrl, jsonContent);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Profil bilgileriniz başarıyla güncellendi.";
                    
                    if (!string.IsNullOrEmpty(model.NickName) && model.NickName != HttpContext.Session.GetString("UserNickName"))
                    {
                        HttpContext.Session.SetString("UserNickName", model.NickName);
                    }
                    
                    return RedirectToAction("Profile");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Profil güncellenirken bir hata oluştu: {errorContent}");
                    return View("Profile", model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Profil güncellenirken bir hata oluştu: {ex.Message}");
                return View("Profile", model);
            }
        }
    }
}
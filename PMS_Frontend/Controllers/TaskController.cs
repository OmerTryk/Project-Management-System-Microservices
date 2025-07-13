using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using PMS_Frontend.Models.ViewModels.TaskVM;
using PMS_Frontend.Models.ViewModels.UserVM;
using PMS_Frontend.Models.ViewModels.ProjectVM;
using Shared.ApiUri;

namespace PMS_Frontend.Controllers
{
    public class TaskController : Controller
    {
        readonly HttpClient _httpClient;

        public TaskController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> CreateTask()
        {
            await LoadUserProjects();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(DtoTaskUI dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadUserProjects();
                return View(dto);
            }

            var userId = await GetCurrentUserId();
            var assignedUserId = await GetUserIdByNickname(dto.AssignedUser);

            if (userId == Guid.Empty || assignedUserId == Guid.Empty)
            {
                ModelState.AddModelError("", "Kullanıcı bilgileri alınamadı.");
                await LoadUserProjects();
                return View(dto);
            }

            var taskDto = new
            {
                TaskName = dto.TaskName,
                Description = dto.Description,
                ProjectId = Guid.TryParse(dto.ProjectId, out var projectGuid) ? projectGuid : Guid.Empty,
                AssignedUserId = assignedUserId,
                AssignedByUserId = userId,
                DueDate = dto.DueDate,
                Priority = (int)dto.Priority,
                EstimatedMinutes = dto.EstimatedMinutes,
                Keywords = dto.Keywords ?? new List<string>()
            };

            var jsonContent = JsonSerializer.Serialize(taskDto);
            Console.WriteLine("Task API'ye gönderilen veri:");
            Console.WriteLine(jsonContent);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{ApiUrls.TaskUrl}/create", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Hatası: {response.StatusCode}");
                Console.WriteLine($"Hata Detayı: {errorContent}");

                ModelState.AddModelError("", $"Task oluşturulurken hata oluştu: {response.StatusCode} - {errorContent}");
                await LoadUserProjects();
                return View(dto);
            }

            return RedirectToAction("ProjectView", "Project");
        }

        public async Task<IActionResult> TaskList(string projectId = null)
        {
        if (string.IsNullOrEmpty(projectId))
        {
            await LoadUserProjects();
                ViewBag.PageTitle = "Tüm Görevlerim";
            var userId = await GetCurrentUserId();
            
            if (userId == Guid.Empty)
            {
            return RedirectToAction("Index", "Home");
        }
        var response = await _httpClient.GetAsync($"{ApiUrls.TaskUrl}/user/{userId}");
        
        if (response.IsSuccessStatusCode)
        {
            var tasks = await response.Content.ReadFromJsonAsync<List<DtoTaskItem>>();
            
                if (tasks != null && tasks.Count > 0)
                    {
                    await LoadUserNames(tasks);
                    }
                    
                    ViewBag.ShowAllTasks = true;
                    return View(tasks ?? new List<DtoTaskItem>());
                }
                ViewBag.ShowAllTasks = true;
                return View(new List<DtoTaskItem>());
            }
            var projectResponse = await _httpClient.GetAsync($"{ApiUrls.TaskUrl}/project/{projectId}");
            
            if (projectResponse.IsSuccessStatusCode)
            {
                var tasks = await projectResponse.Content.ReadFromJsonAsync<List<DtoTaskItem>>();
               
                if (tasks != null && tasks.Count > 0)
                {
                    await LoadUserNames(tasks);
                }
                
                ViewBag.ProjectId = projectId;
                ViewBag.PageTitle = $"{projectId} Projesi Görevleri";
                ViewBag.ShowAllTasks = false;
                return View(tasks ?? new List<DtoTaskItem>());
            }

            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> StartTask(Guid taskId, string projectId)
        {
            var userId = await GetCurrentUserId();

            var request = new
            {
                TaskId = taskId,
                UserId = userId,
                Status = 1,
                Comments = "Task başlatıldı"
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{ApiUrls.TaskUrl}/start", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(TaskList), new { projectId });
            }

            return RedirectToAction(nameof(TaskList), new { projectId });
        }

        [HttpPost]
        public async Task<IActionResult> CompleteTask(Guid taskId, string projectId, int actualMinutes)
        {
            var userId = await GetCurrentUserId();

            var request = new
            {
                TaskId = taskId,
                UserId = userId,
                Status = 2,
                CompletedAt = DateTime.UtcNow,
                ActualMinutes = actualMinutes,
                Comments = "Task tamamlandı"
            };

            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{ApiUrls.TaskUrl}/complete", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(TaskPerformance), new { taskId });
            }

            return RedirectToAction(nameof(TaskList), new { projectId });
        }

        public async Task<IActionResult> TaskPerformance(Guid taskId)
        {
            var response = await _httpClient.GetAsync($"{ApiUrls.TaskUrl}/performance/{taskId}");

            if (response.IsSuccessStatusCode)
            {
                var performance = await response.Content.ReadFromJsonAsync<DtoTaskPerformance>();
                return View(performance);
            }

            return View("Error");
        }

        #region Private Methods

        private async Task LoadUserProjects()
        {
            try
            {
                var userId = await GetCurrentUserId();
                var request = new DtoGetProject { Id = userId };

                var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiUrls.ProjectUrl}/getproject", content);

                if (response.IsSuccessStatusCode)
                {
                    var projects = await response.Content.ReadFromJsonAsync<List<DtoGetProjectContent>>();

                    ViewBag.UserProjects = projects?.Select(p => new ProjectSelectItem
                    {
                        Id = p.ProjectId.ToString(),
                        Name = p.ProjectName ?? "Proje Adı Yok"
                    }).ToList() ?? new List<ProjectSelectItem>();
                }
                else
                {
                    ViewBag.UserProjects = new List<ProjectSelectItem>();
                }
            }
            catch (Exception ex)
            {
                ViewBag.UserProjects = new List<ProjectSelectItem>();
            }
        }

        private async Task<Guid> GetCurrentUserId()
        {
            try
            {
                var userNickname = new DtoUserNickName
                {
                    NickName = HttpContext.Session.GetString("UserNickName") ?? string.Empty
                };

                var content = new StringContent(JsonSerializer.Serialize(userNickname), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiUrls.UserUrl}/getuserbynickname", content);

                if (response.IsSuccessStatusCode)
                {
                    var userResult = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Guid>(userResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch
            {
               
            }

            return Guid.Empty;
        }

        private async Task<Guid> GetUserIdByNickname(string nickname)
        {
            try
            {
                var userDto = new DtoUserNickName { NickName = nickname };
                var content = new StringContent(JsonSerializer.Serialize(userDto), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiUrls.UserUrl}/getuserbynickname", content);

                if (response.IsSuccessStatusCode)
                {
                    var userResult = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Guid>(userResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
            }
            catch
            {
                // Hata durumunda empty guid döndür
            }

            return Guid.Empty;
        }

        private async Task LoadUserNames(List<DtoTaskItem> tasks)
        {
            if (tasks == null || !tasks.Any()) return;

            try
            {
                var userIds = tasks.Select(t => t.AssignedUserId).Distinct().ToList();
                var content = new StringContent(JsonSerializer.Serialize(userIds), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{ApiUrls.UserUrl}/getusersnickname", content);

                if (response.IsSuccessStatusCode)
                {
                    var userNames = await response.Content.ReadFromJsonAsync<List<DtoGetNickNames>>();
                    var nicknameDictionary = userNames?.ToDictionary(u => u.Id, u => u.NickName) ?? new Dictionary<Guid, string>();

                    foreach (var task in tasks)
                    {
                        task.AssignedUserName = nicknameDictionary.TryGetValue(task.AssignedUserId, out var nickname) ? nickname : "Bilinmiyor";
                    }
                }
            }
            catch
            {
                foreach (var task in tasks)
                {
                    task.AssignedUserName = "Bilinmiyor";
                }
            }
        }

        #endregion
    }
}
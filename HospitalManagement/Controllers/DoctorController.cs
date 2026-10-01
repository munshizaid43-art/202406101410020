using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using HospitalManagement.Extensions;
using HospitalManagement.Models;
using System.Text.Json;

namespace HospitalManagement.Controllers
{
    // This controller manages doctor and appointment operations.
    public class DoctorController : Controller
    {
        // IMemoryCache stores doctor data temporarily in server memory.
        private readonly IMemoryCache _cache;

        // HttpClient is used for asynchronous web requests.
        private readonly HttpClient _httpClient;

        // This list contains sample doctors for the application.
        private static readonly List<Doctor> doctors = new List<Doctor>
        {
            // Doctor 1.
            new Doctor
            {
                DoctorId = 101,
                DoctorName = "Dr. Aarav Mehta",
                Specialization = "Cardiology",
                Experience = 12,
                ConsultationFee = 800
            },

            // Doctor 2.
            new Doctor
            {
                DoctorId = 102,
                DoctorName = "Dr. Sana Kapoor",
                Specialization = "Dermatology",
                Experience = 8,
                ConsultationFee = 600
            },

            // Doctor 3.
            new Doctor
            {
                DoctorId = 103,
                DoctorName = "Dr. Rohan Shah",
                Specialization = "Neurology",
                Experience = 15,
                ConsultationFee = 1000
            },

            // Doctor 4.
            new Doctor
            {
                DoctorId = 104,
                DoctorName = "Dr. Meera Joshi",
                Specialization = "Pediatrics",
                Experience = 10,
                ConsultationFee = 700
            }
        };

        // Constructor receives the memory cache and HTTP client.
        public DoctorController(IMemoryCache cache, IHttpClientFactory httpClientFactory)
        {
            // Store the memory cache object.
            _cache = cache;

            // Create an HttpClient object.
            _httpClient = httpClientFactory.CreateClient();
        }

        // This action displays all doctors.
        public IActionResult Index()
        {
            // Check whether doctor data already exists in memory cache.
            if (!_cache.TryGetValue("DoctorList", out List<Doctor>? doctorList))
            {
                // Store the doctor list in cache for 3 minutes.
                _cache.Set("DoctorList", doctors, TimeSpan.FromMinutes(3));

                // Assign the original doctor list to the local variable.
                doctorList = doctors;
            }

            // Read patient name from cookie.
            string patientName = Request.Cookies["PatientName"] ?? "";

            // Send patient name to the view.
            ViewBag.PatientName = patientName;

            // Display the doctor list.
            return View(doctorList);
        }

        // This action displays doctor details.
        [ResponseCache(Duration = 60)]
        public IActionResult Details(int id)
        {
            // Find a doctor using the ID received from query string.
            Doctor? doctor = doctors.FirstOrDefault(d => d.DoctorId == id);

            // If doctor is not found, show NotFound.
            if (doctor == null)
            {
                return NotFound();
            }

            // Calculate total consultation charge.
            ViewBag.TotalCharge = doctor.CalculateTotalCharge();

            // Display doctor details.
            return View(doctor);
        }

        // This action displays the appointment form.
        [HttpGet]
        public IActionResult Appointment(int id)
        {
            // Find the selected doctor.
            Doctor? doctor = doctors.FirstOrDefault(d => d.DoctorId == id);

            // If doctor is not found, show NotFound.
            if (doctor == null)
            {
                return NotFound();
            }

            // Store selected specialization in session.
            HttpContext.Session.SetString(
                "SelectedSpecialization",
                doctor.Specialization
            );

            // Display appointment form.
            return View(doctor);
        }

        // This action receives appointment form data.
        [HttpPost]
        public IActionResult Appointment(int DoctorId, string PatientName)
        {
            // Store patient name inside a cookie.
            Response.Cookies.Append(
                "PatientName",
                PatientName,
                new CookieOptions
                {
                    // Cookie remains available for one day.
                    Expires = DateTimeOffset.Now.AddDays(1)
                }
            );

            // Find the doctor using the hidden DoctorId.
            Doctor? doctor = doctors.FirstOrDefault(
                d => d.DoctorId == DoctorId
            );

            // If doctor is not found, show NotFound.
            if (doctor == null)
            {
                return NotFound();
            }

            // Store specialization in session.
            HttpContext.Session.SetString(
                "SelectedSpecialization",
                doctor.Specialization
            );

            // Send appointment information to confirmation page.
            ViewBag.PatientName = PatientName;
            ViewBag.Specialization =
                HttpContext.Session.GetString("SelectedSpecialization");

            // Calculate total consultation charge.
            ViewBag.TotalCharge = doctor.CalculateTotalCharge();

            // Show confirmation using the same doctor view.
            return View("Appointment", doctor);
        }

        // This action retrieves posts asynchronously.
        public async Task<IActionResult> Posts()
        {
            // Send asynchronous GET request to JSONPlaceholder.
            HttpResponseMessage response =
                await _httpClient.GetAsync(
                    "https://jsonplaceholder.typicode.com/posts"
                );

            // Check whether the request was successful.
            if (!response.IsSuccessStatusCode)
            {
                // Return an error message if request fails.
                return Content("Unable to load health feed.");
            }

            // Read response content as text asynchronously.
            string json = await response.Content.ReadAsStringAsync();

            // Convert JSON data into objects.
            List<Post>? posts =
                JsonSerializer.Deserialize<List<Post>>(
                    json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );

            // Display the posts in a table.
            return View(posts);
        }
    }

    // This class represents data received from JSONPlaceholder.
    public class Post
    {
        // Stores the post ID.
        public int Id { get; set; }

        // Stores the post title.
        public string Title { get; set; } = "";
    }
}
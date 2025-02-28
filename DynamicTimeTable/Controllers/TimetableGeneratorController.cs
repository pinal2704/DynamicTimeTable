using DynamicTimeTable.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DynamicTimeTable.Controllers
{
    public class TimetableGeneratorController : Controller
    {

        private readonly TimeTableContext _context;

        public TimetableGeneratorController(TimeTableContext context)
        {
            _context = context; 
        }

        public IActionResult Index()
        {
            return View(new TimetableGeneratorModels());
        }

        [HttpPost]
        public IActionResult SubjectHours(TimetableGeneratorModels model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var subjectHoursModel = new SubjectHoursModel
            {
                TotalHoursForWeek = model.TotalHoursForWeek
            };

            for (int i = 0; i < model.TotalSubjects; i++)
            {
                subjectHoursModel.Subjects.Add(new Subject());
            }

            // Store the input model in TempData for later use
            TempData["WorkingDays"] = model.WorkingDays;
            TempData["SubjectsPerDay"] = model.SubjectsPerDay;
            TempData["TotalSubjects"] = model.TotalSubjects;

            return View(subjectHoursModel);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateTimetable(SubjectHoursModel model, string timetableName = "My Timetable")
        {
            // Validate that the sum of hours equals TotalHoursForWeek
            int totalHours = model.Subjects.Sum(s => s.Hours);

            if (totalHours != model.TotalHoursForWeek)
            {
                ModelState.AddModelError("", $"The sum of subject hours ({totalHours}) must equal the total hours for the week ({model.TotalHoursForWeek})");
                return View("SubjectHours", model);
            }

            // Retrieve stored values
            int workingDays = (int)TempData["WorkingDays"];
            int subjectsPerDay = (int)TempData["SubjectsPerDay"];
            int totalSubjects = (int)TempData["TotalSubjects"];

            // Generate timetable
            var timetableModel = new TimetableModel
            {
                SubjectsPerDay = subjectsPerDay,
                WorkingDays = workingDays
            };

            // Set day names (Mon, Tue, etc.)
            string[] days = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            for (int i = 0; i < workingDays; i++)
            {
                timetableModel.DayNames.Add(days[i]);
            }

            // Generate timetable cells
            var random = new Random();
            var subjectsQueue = new Queue<string>();

            // Fill the queue with subjects according to their hours
            foreach (var subject in model.Subjects)
            {
                for (int i = 0; i < subject.Hours; i++)
                {
                    subjectsQueue.Enqueue(subject.Name);
                }
            }

            // Shuffle the queue to randomize subject allocation
            var subjectsList = subjectsQueue.ToList();
            for (int i = 0; i < subjectsList.Count; i++)
            {
                int j = random.Next(i, subjectsList.Count);
                var temp = subjectsList[i];
                subjectsList[i] = subjectsList[j];
                subjectsList[j] = temp;
            }

            // Convert back to queue
            subjectsQueue = new Queue<string>(subjectsList);

            // Fill the timetable
            for (int row = 0; row < subjectsPerDay; row++)
            {
                var rowData = new List<string>();
                for (int col = 0; col < workingDays; col++)
                {
                    if (subjectsQueue.Count > 0)
                    {
                        rowData.Add(subjectsQueue.Dequeue());
                    }
                    else
                    {
                        // This should not happen if validation is correct
                        rowData.Add("Empty");
                    }
                }
                timetableModel.Timetable.Add(rowData);
            }

            // Save timetable to database
            var timetableConfig = new TimetableConfiguration
            {
                Name = timetableName,
                WorkingDays = workingDays,
                SubjectsPerDay = subjectsPerDay,
                TotalSubjects = totalSubjects,
                Subjects = model.Subjects.Select(s => new Models.Subject
                {
                    Name = s.Name,
                    Hours = s.Hours
                }).ToList(),
                TimetableData = new TimetableData
                {
                    TimetableJson = JsonSerializer.Serialize(timetableModel)
                }
            };

            _context.TimetableConfigurations.Add(timetableConfig);
            await _context.SaveChangesAsync();

            // Store the saved timetable ID for reference
            TempData["SavedTimetableId"] = timetableConfig.Id;

            return View(timetableModel);
        }
        // GET: TimetableGenerator/List
        public async Task<IActionResult> List()
        {
            var timetables = await _context.TimetableConfigurations
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(timetables);
        }

        // GET: TimetableGenerator/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var timetableConfig = await _context.TimetableConfigurations
                .Include(t => t.Subjects)
                .Include(t => t.TimetableData)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (timetableConfig == null)
            {
                return NotFound();
            }

            var timetableModel = JsonSerializer.Deserialize<TimetableModel>(timetableConfig.TimetableData.TimetableJson);
            return View("GenerateTimetable", timetableModel);
        }

    }

}

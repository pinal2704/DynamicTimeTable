using System.ComponentModel.DataAnnotations;

namespace DynamicTimeTable.Models
{
    public class TimetableGeneratorModels
    {

        //[Key]
        //public int Id { get; set; }

        //[Required]
        //public string Name { get; set; }
        [Required]
        [Range(1, 7, ErrorMessage = "Number of working days must be between 1 and 7")]
        [Display(Name = "Number of Working Days")]
        public int WorkingDays { get; set; }

        [Required]
        [Range(1, 8, ErrorMessage = "Number of subjects per day must be between 1 and 8")]
        [Display(Name = "Number of Subjects per Day")]
        public int SubjectsPerDay { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Total subjects must be a positive number")]
        [Display(Name = "Total Subjects")]
        public int TotalSubjects { get; set; }

        public int TotalHoursForWeek => WorkingDays * SubjectsPerDay;


    }


    public class SubjectHoursModel
    {
        public List<Subject> Subjects { get; set; } = new List<Subject>();
        public int TotalHoursForWeek { get; set; }
    }

   

    public class TimetableModel
    {
        public List<string> DayNames { get; set; } = new List<string>();
        public List<List<string>> Timetable { get; set; } = new List<List<string>>();
        public int SubjectsPerDay { get; set; }
        public int WorkingDays { get; set; }
    }

}

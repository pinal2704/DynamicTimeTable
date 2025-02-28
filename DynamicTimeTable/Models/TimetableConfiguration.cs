using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DynamicTimeTable.Models
{
    public class TimetableConfiguration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int WorkingDays { get; set; }

        [Required]
        public int SubjectsPerDay { get; set; }

        [Required]
        public int TotalSubjects { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Subject> Subjects { get; set; }

        public virtual TimetableData TimetableData { get; set; }
    }

    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Hours { get; set; }

        [ForeignKey("TimetableConfiguration")]
        public int TimetableConfigurationId { get; set; }

        public virtual TimetableConfiguration TimetableConfiguration { get; set; }
    }

    public class TimetableData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TimetableJson { get; set; }

        [ForeignKey("TimetableConfiguration")]
        public int TimetableConfigurationId { get; set; }

        public virtual TimetableConfiguration TimetableConfiguration { get; set; }
    }
}

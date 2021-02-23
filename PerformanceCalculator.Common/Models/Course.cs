using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using PerformanceCalculator.Common.Constants;

namespace PerformanceCalculator.Common.Models
{
    public class Course : BaseModel
    {
        public string Title { get; set; }
        public string Code { get; set; }
        public float Credit { get; set; }
        public Year Year { get; set; }

        public SemesterEnum Semester { get; set; }
        [JsonIgnore]
        public Student Student { get; set; }
        public ICollection<Exam> Exams { get; set; }

        [JsonIgnore]
        public Teacher Teacher { get; set; }
    }
}
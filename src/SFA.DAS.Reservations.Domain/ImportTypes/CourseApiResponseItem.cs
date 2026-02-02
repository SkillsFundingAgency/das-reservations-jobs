using System;
using System.Collections.Generic;

namespace SFA.DAS.Reservations.Domain.ImportTypes
{
    public class CourseApiResponse
    {
        public List<CourseApiResponseItem> Courses { get; set; }
    }

    public class CourseApiResponseItem
    {
        public DateTime EffectiveTo { get; set; }

        public string Title { get; set; }

        public int Level { get; set; }

        public string Id { get; set; }

        public string ApprenticeshipType { get; set; }

        public string LearningType { get; set; }
    }
}
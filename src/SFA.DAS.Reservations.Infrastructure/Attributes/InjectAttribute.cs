﻿using System;
using Microsoft.Azure.WebJobs.Description;

namespace SFA.DAS.Reservations.Infrastructure.Attributes
{
    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
    }
}

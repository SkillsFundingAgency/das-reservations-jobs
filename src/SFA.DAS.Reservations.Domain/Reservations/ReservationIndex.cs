﻿using System;
using SFA.DAS.Reservations.Messages;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class ReservationIndex
    {
        public string Id => $"{IndexedProviderId}_{AccountLegalEntityId}_{ReservationId}";
        
        public Guid ReservationId { get; set; }
        public long AccountId { get; set; }
        public bool IsLevyAccount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public short Status { get; set; }
        public string CourseId { get; set; }
        public string CourseTitle { get; set; }
        public int? CourseLevel { get; set; }

        public string CourseName => $"{CourseTitle} {CourseLevel}";
        public long AccountLegalEntityId { get; set; }
        public uint? ProviderId { get; set; }
        public string AccountLegalEntityName { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public Guid? UserId { get; set; }
        public uint IndexedProviderId { get; set; }

        public static implicit operator ReservationIndex(ReservationCreatedEvent source)
        {
            return new ReservationIndex            
            {
                ReservationId = source.Id,
                AccountId = source.AccountId,
                AccountLegalEntityId = source.AccountLegalEntityId,
                AccountLegalEntityName = source.AccountLegalEntityName,
                StartDate = source.StartDate,
                ExpiryDate = source.EndDate,
                CreatedDate = source.CreatedDate,
                CourseId = source.CourseId,
                CourseTitle = source.CourseName,
                CourseLevel = int.Parse(source.CourseLevel),
                ProviderId = source.ProviderId
            };
        }
    }
}

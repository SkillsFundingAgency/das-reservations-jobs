using System;
using System.Globalization;
using Newtonsoft.Json;

namespace SFA.DAS.Reservations.Domain.Reservations
{
    public class IndexedReservation
    {
        [JsonProperty(PropertyName = "id")]
        public string Id => $"{IndexedProviderId}_{AccountLegalEntityId}_{ReservationId}";
        [JsonProperty(PropertyName = "reservationId")]
        public Guid ReservationId { get; set; }
        [JsonProperty(PropertyName = "accountId")]
        public long AccountId { get; set; }
        [JsonProperty(PropertyName = "isLevyAccount")]
        public bool IsLevyAccount { get; set; }
        
        [JsonProperty(PropertyName = "createdDate")]
        public DateTime CreatedDate { get; set; }
        [JsonProperty(PropertyName = "reservationPeriod")]
        public string ReservationPeriod
        {
            get
            {
                var period = string.Empty;

                if (StartDate.HasValue)
                {
                    period += $"{StartDate.Value.ToString("MMM yyyy", CultureInfo.InvariantCulture)}";
                }

                if (ExpiryDate.HasValue)
                {
                    period += $" to {ExpiryDate.Value.ToString("MMM yyyy", CultureInfo.InvariantCulture)}";
                }

                return period;
            }
        }
        [JsonProperty(PropertyName = "startDate")]
        public DateTime? StartDate { get; set; }
        [JsonProperty(PropertyName = "expiryDate")]
        public DateTime? ExpiryDate { get; set; }
        [JsonProperty(PropertyName = "status")]
        public short Status { get; set; }
        [JsonProperty(PropertyName = "courseId")]
        public string CourseId { get; set; }
        [JsonProperty(PropertyName = "courseTitle")]
        public string CourseTitle { get; set; }
        [JsonProperty(PropertyName = "courseLevel")]
        public int? CourseLevel { get; set; }
        [JsonProperty(PropertyName = "courseDescription")]
        public string CourseDescription => $"{CourseTitle} - Level {CourseLevel}";
        [JsonProperty(PropertyName = "accountLegalEntityId")]
        public long AccountLegalEntityId { get; set; }
        [JsonProperty(PropertyName = "providerId")]
        public uint? ProviderId { get; set; }
        [JsonProperty(PropertyName = "accountLegalEntityName")]
        public string AccountLegalEntityName { get; set; }
        [JsonProperty(PropertyName = "transferSenderAccountId")]
        public long? TransferSenderAccountId { get; set; }
        [JsonProperty(PropertyName = "userId")]
        public Guid? UserId { get; set; }
        [JsonProperty(PropertyName = "indexedProviderId")]
        public uint IndexedProviderId { get; set; }

        public static implicit operator IndexedReservation(Reservation source)
        {
            return new IndexedReservation            
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
                CourseLevel = source.CourseLevel,
                ProviderId = source.ProviderId,
                Status = (short)source.Status
            };
        }
    }
}

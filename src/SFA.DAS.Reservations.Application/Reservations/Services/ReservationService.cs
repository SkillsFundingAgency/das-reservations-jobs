using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Amqp.Serialization;
using SFA.DAS.Reservations.Domain.ProviderPermissions;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Application.Reservations.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _repository;
        private readonly IReservationIndexRepository _indexRepository;
        private readonly IProviderPermissionsRepository _permissionsRepository;

        public ReservationService(IReservationRepository repository, IReservationIndexRepository indexRepository,
            IProviderPermissionsRepository permissionsRepository)
        {
            _repository = repository;
            _indexRepository = indexRepository;
            _permissionsRepository = permissionsRepository;
        }

        public async Task UpdateReservationStatus(Guid reservationId, ReservationStatus status)
        {
            if (reservationId == null || reservationId.Equals(Guid.Empty))
            {
                throw new ArgumentException("Reservation ID must be set", nameof(reservationId));
            }

            await _repository.SaveStatus(reservationId, status);
        }

        public async Task RefreshReservationIndex()
        {
            var reservations = _repository.GetAll()?.ToList();

            var permissions = _permissionsRepository.GetAll()?.ToArray();
          
            if (reservations == null || !reservations.Any())
            {
                return;
            }

            if (permissions != null)
            {
                var copiedReservations = new List<Reservation>();

                foreach (var permission in permissions)
                {
                    if (reservations.All(r =>
                        r.AccountId.Equals(permission.AccountId) &&
                        r.AccountLegalEntityId.Equals(permission.AccountLegalEntityId) &&
                        r.ProviderId.HasValue && r.ProviderId.Value.Equals((uint) permission.ProviderId)))
                    {
                        continue;
                    }

                    var matchingReservations = reservations.Where(r =>
                        r.AccountId.Equals(permission.AccountId) &&
                        r.AccountLegalEntityId.Equals(permission.AccountLegalEntityId) &&
                        !r.ProviderId.Equals((uint)permission.ProviderId)).ToArray();

                    foreach (var match in matchingReservations)
                    {
                        var copy = match.Clone();
                        copy.ProviderId = (uint) permission.ProviderId;
                        copiedReservations.Add(copy);
                    }
                }

                reservations.AddRange(copiedReservations);
            }

            var reservationIndexes = reservations.Select(MapReservation);

            await _indexRepository.CreateIndex();

            await _indexRepository.Add(reservationIndexes);
        }

        private static ReservationIndex MapReservation(Reservation entity)
        {
            return new ReservationIndex
            {
                Id = entity.Id,
                Status = entity.Status,
                AccountId = entity.AccountId,
                AccountLegalEntityId = entity.AccountLegalEntityId,
                ProviderId = entity.ProviderId
            };
        }
    }
}

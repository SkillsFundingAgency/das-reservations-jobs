using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Domain.Reservations;
using Reservation = SFA.DAS.Reservations.Domain.Entities.Reservation;

namespace SFA.DAS.Reservations.Application.Reservations.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _repository;
        private readonly IReservationIndexRepository _indexRepository;

        public ReservationService(IReservationRepository repository, IReservationIndexRepository indexRepository)
        {
            _repository = repository;
            _indexRepository = indexRepository;
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
            var result = _repository.GetAll();

            var reservations = result?.ToArray();
          
            if (reservations == null || !reservations.Any())
            {
                return;
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
                Status = entity.Status
            };
        }
    }
}

using System;

namespace SFA.DAS.Reservations.Domain.Infrastructure
{
    public interface IServiceProviderBuilder
    {
        IServiceProvider Build();
    }
}

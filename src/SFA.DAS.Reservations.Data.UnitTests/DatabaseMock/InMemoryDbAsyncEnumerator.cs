using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Reservations.Data.UnitTests.DatabaseMock
{
    public class InMemoryDbAsyncEnumerator<T>(IEnumerator<T> enumerator) : IAsyncEnumerator<T>
    {
        private bool disposed = false;

        public T Current => enumerator.Current;

        protected virtual async ValueTask Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                    enumerator.Dispose();
                }

                this.disposed = true;
            }
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(enumerator.MoveNext());
        }

        public async ValueTask DisposeAsync()
        {
            await this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
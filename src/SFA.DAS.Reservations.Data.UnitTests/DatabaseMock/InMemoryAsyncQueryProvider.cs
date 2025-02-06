using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace SFA.DAS.Reservations.Data.UnitTests.DatabaseMock
{
    public class InMemoryAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider innerQueryProvider;

        public InMemoryAsyncQueryProvider(IQueryProvider innerQueryProvider)
        {
            this.innerQueryProvider = innerQueryProvider;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new InMemoryAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new InMemoryAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return this.innerQueryProvider.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return this.innerQueryProvider.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                .GetMethod(
                    name: nameof(IQueryProvider.Execute),
                    genericParameterCount: 1,
                    types: [typeof(Expression)])
                .MakeGenericMethod(expectedResultType)
                .Invoke(this, [expression]);

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                ?.MakeGenericMethod(expectedResultType)
                .Invoke(null, [executionResult]);
        }
    }
}
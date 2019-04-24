using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;
using NUnit.Framework;
using SFA.DAS.Reservations.Infrastructure.NServiceBus;

namespace SFA.DAS.Reservations.Infrastructure.UnitTests.NServiceBus
{
    public class WhenGettingTriggerBinder
    {
        [Test]
        public async Task ThenReturnsTriggerBinding()
        {
            //Arrange
            var paramInfo = TestClass.GetParamInfoWithTriggerAttrubute();
            var context = new TriggerBindingProviderContext(paramInfo, new CancellationToken(false));
            var provider = new NServiceBusTriggerBindingProvider();

            //Act
            var result = await provider.TryCreateAsync(context);

            //Assert
            var binding = result as NServiceBusTriggerBinding;

            Assert.IsNotNull(binding);
        }

        [Test]
        public async Task ThenReturnsNullIfNoAttributeFound()
        {
            //Arrange
            var paramInfo = TestClass.GetParamInfoWithoutTriggerAttrubute();
            var context = new TriggerBindingProviderContext(paramInfo, new CancellationToken(false));
            var provider = new NServiceBusTriggerBindingProvider();

            //Act
            var result = await provider.TryCreateAsync(context);

            //Assert
            Assert.IsNull(result);
        }

        private class TestClass
        {
            public static ParameterInfo GetParamInfoWithTriggerAttrubute()
            {
                return GetParamsInfo(nameof(PlaceholderMethod)).First();
            }

            public static ParameterInfo GetParamInfoWithoutTriggerAttrubute()
            {
                return GetParamsInfo(nameof(PlaceholderMethod)).Last();
            }

            private static IEnumerable<ParameterInfo> GetParamsInfo(string methodName)
            {
                var x =  typeof(TestClass).GetMethod(methodName).GetParameters();

                return x;
            }

            //This must be public for reflection to work
            public static void PlaceholderMethod([NServiceBusTrigger]string trigger, string notATrigger)
            {

            }
         }
    }
}

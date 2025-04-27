using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript
{
    public class TestTypes
    {
        public interface ITestService { }

        public class TestService : ITestService { }

        public class DummyService { }

        public class DummyServiceWithParams
        {
            public readonly ITestService TestService;
            public readonly string ConfigValue;

            public static bool ConstructorCalled = false;

            public DummyServiceWithParams(ITestService testService, string configValue)
            {
                if (testService == null) throw new ArgumentNullException(nameof(testService));
                if (configValue == null) throw new ArgumentNullException(nameof(configValue));

                ConstructorCalled = true;
                TestService = testService;
                ConfigValue = configValue;
            }
        }

        public class DummyServiceWithParamsAndContract : ITestService
        {
            public readonly ITestService Dependency;
            public readonly string ConfigSetting;

            public static bool ConstructorCalled = false;

            public DummyServiceWithParamsAndContract(ITestService dependency, string configSetting)
            {
                if (dependency == null) throw new ArgumentNullException(nameof(dependency));
                if (configSetting == null) throw new ArgumentNullException(nameof(configSetting));

                ConstructorCalled = true;
                Dependency = dependency;
                ConfigSetting = configSetting;
            }
        }
    }
}

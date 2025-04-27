using System;

namespace IngameScript
{
    class ContainerTests : BaseTestClass
    {
        private Container _container;
        private const string TestConfigValue = "Config123";

        public override void Setup()
        {
            _container = new Container();
        }

        public override Action[] Tests
        {
            get
            {
                return new Action[]
                {
                    Resolve_SingletonRegistration_WhenRegistered_ShouldReturnSameInstance,
                    Resolve_TransientRegistration_WhenRegistered_ShouldReturnDifferentInstances,
                    Resolve_RegisteredContract_WhenRegistered_ShouldReturnConcreteImplementation,
                    Resolve_UnregisteredType_WhenResolved_ShouldThrowInvalidOperationException,
                    Resolve_Singleton_WhenRegistered_ShouldCallFactoryOnlyOnce,
                    Resolve_Transient_WhenRegistered_ShouldCallFactoryEveryTime,
                    RegisterTransient_WhenOverridingSingleton_ShouldBehaveAsTransient,
                    Resolve_SingletonFactoryReturnsNull_WhenResolved_ShouldReturnNullWithoutException,
                    Resolve_SingletonFactoryThrowsException_WhenResolved_ShouldKeepContainerUsable,
                    RegisterTransientTwice_WhenRegistered_ShouldUseLastRegistration,
                    ResolveBeforeRegister_WhenResolved_ShouldThrowInvalidOperationException,
                    RegisterTransient_GenericContractAndService_WhenRegistered_ShouldResolveAsTransient,
                    Resolve_DummyServiceWithParams_WhenAllDependenciesRegistered_ShouldResolveSuccessfully,
                    Resolve_DummyServiceWithParams_WhenMissingDependency_ShouldThrowException,
                    Resolve_DummyServiceWithParams_WhenNoDependenciesRegistered_ShouldThrowException
                };
            }
        }

        private void ResetGlobalFlags()
        {
            TestTypes.DummyServiceWithParams.ConstructorCalled = false;
        }

        private void Resolve_DummyServiceWithParams_WhenMissingDependency_ShouldThrowException()
        {
            ResetGlobalFlags();
            _container.RegisterSingleton<TestTypes.ITestService, TestTypes.TestService>(c => new TestTypes.TestService());
            _container.RegisterTransient<TestTypes.DummyServiceWithParams>(c => new TestTypes.DummyServiceWithParams(
                c.Resolve<TestTypes.ITestService>(),
                c.Resolve<string>())
            );

            Assert.Throws<InvalidOperationException>(() => _container.Resolve<TestTypes.DummyServiceWithParams>());
        }

        private void Resolve_DummyServiceWithParams_WhenNoDependenciesRegistered_ShouldThrowException()
        {
            ResetGlobalFlags();
            _container.RegisterTransient<TestTypes.DummyServiceWithParams>(c => new TestTypes.DummyServiceWithParams(
                c.Resolve<TestTypes.ITestService>(),
                c.Resolve<string>())
            );

            Assert.Throws<InvalidOperationException>(() => _container.Resolve<TestTypes.DummyServiceWithParams>());
        }

        private void Resolve_DummyServiceWithParams_WhenAllDependenciesRegistered_ShouldResolveSuccessfully()
        {
            ResetGlobalFlags();
            _container.RegisterSingleton<TestTypes.ITestService, TestTypes.TestService>(c => new TestTypes.TestService());
            _container.RegisterSingleton<string>(c => TestConfigValue);
            _container.RegisterTransient<TestTypes.DummyServiceWithParams>(c => new TestTypes.DummyServiceWithParams(
                c.Resolve<TestTypes.ITestService>(),
                c.Resolve<string>())
            );

            var dummy = _container.Resolve<TestTypes.DummyServiceWithParams>();

            Assert.IsNotNull(dummy.TestService);
            Assert.AreEqual(TestConfigValue, dummy.ConfigValue);
            Assert.IsTrue(TestTypes.DummyServiceWithParams.ConstructorCalled);
        }

        private void RegisterTransient_GenericContractAndService_WhenRegistered_ShouldResolveAsTransient()
        {
            ResetGlobalFlags();
            _container.RegisterTransient<TestTypes.ITestService, TestTypes.TestService>(c => new TestTypes.TestService());

            var instance1 = _container.Resolve<TestTypes.ITestService>();
            var instance2 = _container.Resolve<TestTypes.ITestService>();

            Assert.IsTrue(instance1 is TestTypes.TestService);
            Assert.IsTrue(instance2 is TestTypes.TestService);
            Assert.IsFalse(object.ReferenceEquals(instance1, instance2));
        }

        private void ResolveBeforeRegister_WhenResolved_ShouldThrowInvalidOperationException()
        {
            ResetGlobalFlags();
            Assert.Throws<InvalidOperationException>(() => _container.Resolve<object>());
        }

        private void RegisterTransientTwice_WhenRegistered_ShouldUseLastRegistration()
        {
            ResetGlobalFlags();
            _container.RegisterTransient<object>(c => new object());
            var first = _container.Resolve<object>();

            _container.RegisterTransient<object>(c => new object());
            var second = _container.Resolve<object>();

            Assert.IsFalse(object.ReferenceEquals(first, second));
        }

        private void Resolve_SingletonFactoryThrowsException_WhenResolved_ShouldKeepContainerUsable()
        {
            ResetGlobalFlags();

            _container.RegisterSingleton<object>(c =>
            {
                throw new Exception("Factory failed");
            });

            try
            {
                _container.Resolve<object>();
            }
            catch (Exception)
            {
                // Expected failure
            }

            _container.RegisterSingleton<string>(c => TestConfigValue);
            var strInstance = _container.Resolve<string>();

            Assert.AreEqual(TestConfigValue, strInstance);
        }

        private void Resolve_SingletonRegistration_WhenRegistered_ShouldReturnSameInstance()
        {
            ResetGlobalFlags();
            _container.RegisterSingleton(c => new object());

            var instance1 = _container.Resolve<object>();
            var instance2 = _container.Resolve<object>();

            Assert.IsTrue(object.ReferenceEquals(instance1, instance2));
        }

        private void Resolve_TransientRegistration_WhenRegistered_ShouldReturnDifferentInstances()
        {
            ResetGlobalFlags();
            _container.RegisterTransient(c => new object());

            var instance1 = _container.Resolve<object>();
            var instance2 = _container.Resolve<object>();

            Assert.IsFalse(object.ReferenceEquals(instance1, instance2));
        }

        private void Resolve_RegisteredContract_WhenRegistered_ShouldReturnConcreteImplementation()
        {
            ResetGlobalFlags();
            _container.RegisterSingleton<TestTypes.ITestService, TestTypes.TestService>(c => new TestTypes.TestService());

            var service = _container.Resolve<TestTypes.ITestService>();

            Assert.IsTrue(service is TestTypes.TestService);
        }

        private void Resolve_UnregisteredType_WhenResolved_ShouldThrowInvalidOperationException()
        {
            ResetGlobalFlags();
            Assert.Throws<InvalidOperationException>(() => _container.Resolve<TestTypes.DummyService>());
        }

        private void Resolve_Singleton_WhenRegistered_ShouldCallFactoryOnlyOnce()
        {
            ResetGlobalFlags();
            int counter = 0;
            _container.RegisterSingleton(c =>
            {
                counter++;
                return new object();
            });

            var instance1 = _container.Resolve<object>();
            var instance2 = _container.Resolve<object>();

            Assert.AreEqual(1, counter);
        }

        private void Resolve_Transient_WhenRegistered_ShouldCallFactoryEveryTime()
        {
            ResetGlobalFlags();
            int counter = 0;
            _container.RegisterTransient(c =>
            {
                counter++;
                return new object();
            });

            var instance1 = _container.Resolve<object>();
            var instance2 = _container.Resolve<object>();

            Assert.AreEqual(2, counter);
        }

        private void RegisterTransient_WhenOverridingSingleton_ShouldBehaveAsTransient()
        {
            ResetGlobalFlags();
            int counter = 0;

            _container.RegisterSingleton(c =>
            {
                counter++;
                return new object();
            });


            // Re-register as Transient: expect new behavior
            _container.RegisterTransient(c =>
            {
                counter++;
                return new object();
            });

            var instance1 = _container.Resolve<object>();
            var instance2 = _container.Resolve<object>();

            Assert.IsFalse(object.ReferenceEquals(instance1, instance2));
            Assert.AreEqual(2, counter);
        }

        private void Resolve_SingletonFactoryReturnsNull_WhenResolved_ShouldReturnNullWithoutException()
        {
            ResetGlobalFlags();
            _container.RegisterSingleton<object>(c => null);

            var instance = _container.Resolve<object>();

            Assert.IsNull(instance);
        }
    }
}

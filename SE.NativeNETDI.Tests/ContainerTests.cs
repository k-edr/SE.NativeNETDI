using System;

namespace IngameScript
{
    class ContainerTests : BaseTestClass
    {
        private Container _container;

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
                    Resolve_SingletonRegistration_ReturnsSameInstance,
                    Resolve_TransientRegistration_ReturnsDifferentInstances,
                    Resolve_RegisteredContract_ReturnsConcreteImplementation,
                    Resolve_UnregisteredType_ThrowsInvalidOperationException,
                    Resolve_Singleton_CallsFactoryOnlyOnce,
                    Resolve_Transient_CallsFactoryEveryTime,
                    RegisterTransient_OverridesSingletonBehavior,

                    Resolve_SingletonFactoryReturnsNull_ReturnsNullWithoutException,
                    Resolve_SingletonFactoryThrowsException_ContainerRemainsUsable,
                    RegisterTransientTwice_LastRegistrationWins,
                    ResolveBeforeRegister_ThrowsInvalidOperationException,
                    RegisterTransient_GenericContractAndService_ResolvesAsTransient,

                    Resolve_DummyServiceWithParamsWithDependencies_ResolvesSuccessfully,                 
                    Resolve_DummyServiceWithParamsWithMissingDependency_ThrowsException,
                    Resolve_DummyServiceWithParamsWithoutDependencies_ThrowsException
                };
            }
        }

        // Actual tests

        static void Resolve_DummyServiceWithParamsWithMissingDependency_ThrowsException()
        {
            var container = new Container();

            container.RegisterSingleton<TestTypes.ITestService, TestTypes.TestService>(c => new TestTypes.TestService());
            // string intentionally not registered

            container.RegisterTransient<TestTypes.DummyServiceWithParams>(c => new TestTypes.DummyServiceWithParams(
                c.Resolve<TestTypes.ITestService>(),
                c.Resolve<string>())
            );

            try
            {
                container.Resolve<TestTypes.DummyServiceWithParams>();
                throw new Exception("Expected InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                // Expected: string dependency missing
            }
        }
        static void Resolve_DummyServiceWithParamsWithoutDependencies_ThrowsException()
        {
            var container = new Container();

            container.RegisterTransient<TestTypes.DummyServiceWithParams>(c => new TestTypes.DummyServiceWithParams(
                c.Resolve<TestTypes.ITestService>(),
                c.Resolve<string>())
            );

            try
            {
                container.Resolve<TestTypes.DummyServiceWithParams>();
                throw new Exception("Expected InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                // Expected: no ITestService and no string registered
            }
        }

        static void Resolve_DummyServiceWithParamsWithDependencies_ResolvesSuccessfully()
        {
            var container = new Container();

            container.RegisterSingleton<TestTypes.ITestService, TestTypes.TestService>(c => new TestTypes.TestService());
            container.RegisterSingleton<string>(c => "Config123");

            container.RegisterTransient<TestTypes.DummyServiceWithParams>(c => new TestTypes.DummyServiceWithParams(
                c.Resolve<TestTypes.ITestService>(),
                c.Resolve<string>())
            );

            var dummy = container.Resolve<TestTypes.DummyServiceWithParams>();

            Assert.IsNotNull(dummy.TestService);
            Assert.AreEqual("Config123", dummy.ConfigValue);
            Assert.IsTrue(TestTypes.DummyServiceWithParams.ConstructorCalled);
        }     

        static void RegisterTransient_GenericContractAndService_ResolvesAsTransient()
        {
            var container = new Container();

            container.RegisterTransient<TestTypes.ITestService, TestTypes.TestService>(c => new TestTypes.TestService());

            var instance1 = container.Resolve<TestTypes.ITestService>();
            var instance2 = container.Resolve<TestTypes.ITestService>();

            Assert.IsTrue(instance1 is TestTypes.TestService);
            Assert.IsTrue(instance2 is TestTypes.TestService);
            Assert.IsFalse(object.ReferenceEquals(instance1, instance2));
        }


        static void ResolveBeforeRegister_ThrowsInvalidOperationException()
        {
            var container = new Container();

            try
            {
                var instance = container.Resolve<object>();
                throw new Exception("Expected exception was not thrown");
            }
            catch (InvalidOperationException ex)
            {
                Assert.IsTrue(ex.Message.Contains("No registration for type"));
            }
        }

        static void RegisterTransientTwice_LastRegistrationWins()
        {
            var container = new Container();

            container.RegisterTransient<object>(c => new object());
            var first = container.Resolve<object>();

            container.RegisterTransient<object>(c => new object());
            var second = container.Resolve<object>();

            Assert.IsFalse(object.ReferenceEquals(first, second));
        }


        static void Resolve_SingletonFactoryThrowsException_ContainerRemainsUsable()
        {
            var container = new Container();

            container.RegisterSingleton<object>(c => { throw new Exception("Factory failed"); });

            try
            {
                container.Resolve<object>();
            }
            catch (Exception)
            {
                // Expected
            }

            container.RegisterSingleton<string>(c => "test");
            var strInstance = container.Resolve<string>();

            Assert.AreEqual("test", strInstance);
        }


        static void Resolve_SingletonRegistration_ReturnsSameInstance()
        {
            var container = new Container();
            container.RegisterSingleton(c => new object());

            var instance1 = container.Resolve<object>();
            var instance2 = container.Resolve<object>();

            Assert.IsTrue(object.ReferenceEquals(instance1, instance2));
        }

        static void Resolve_TransientRegistration_ReturnsDifferentInstances()
        {
            var container = new Container();
            container.RegisterTransient(c => new object());

            var instance1 = container.Resolve<object>();
            var instance2 = container.Resolve<object>();

            Assert.IsFalse(object.ReferenceEquals(instance1, instance2));
        }

        static void Resolve_RegisteredContract_ReturnsConcreteImplementation()
        {
            var container = new Container();
            container.RegisterSingleton<TestTypes.ITestService, TestTypes.TestService>(c => new TestTypes.TestService());

            var service = container.Resolve<TestTypes.ITestService>();

            Assert.IsTrue(service is TestTypes.TestService);
        }

        static void Resolve_UnregisteredType_ThrowsInvalidOperationException()
        {
            var container = new Container();

            try
            {
                container.Resolve<TestTypes.DummyService>();
                throw new Exception("Expected exception was not thrown");
            }
            catch (InvalidOperationException ex)
            {
                Assert.IsTrue(ex.Message.Contains("No registration for type"));
            }
        }

        static void Resolve_Singleton_CallsFactoryOnlyOnce()
        {
            var container = new Container();
            int counter = 0;
            container.RegisterSingleton(c =>
            {
                counter++;
                return new object();
            });

            var instance1 = container.Resolve<object>();
            var instance2 = container.Resolve<object>();

            Assert.AreEqual(1, counter);
        }

        static void Resolve_Transient_CallsFactoryEveryTime()
        {
            var container = new Container();
            int counter = 0;
            container.RegisterTransient(c =>
            {
                counter++;
                return new object();
            });

            var instance1 = container.Resolve<object>();
            var instance2 = container.Resolve<object>();

            Assert.AreEqual(2, counter);
        }

        static void RegisterTransient_OverridesSingletonBehavior()
        {
            var container = new Container();
            int counter = 0;

            container.RegisterSingleton(c =>
            {
                counter++;
                return new object();
            });

            container.RegisterTransient(c =>
            {
                counter++;
                return new object();
            });

            var instance1 = container.Resolve<object>();
            var instance2 = container.Resolve<object>();

            Assert.IsFalse(object.ReferenceEquals(instance1, instance2));
            Assert.AreEqual(2, counter);
        }

        static void Resolve_SingletonFactoryReturnsNull_ReturnsNullWithoutException()
        {
            var container = new Container();
            container.RegisterSingleton<object>(c => null);

            var instance = container.Resolve<object>();

            Assert.Equals(null, instance);
        }    
    }
}
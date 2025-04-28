using System;
using System.Collections.Generic;

namespace IngameScript
{
    public class Container
    {
        private readonly Dictionary<Type, Func<Container, object>> _factories = new Dictionary<Type, Func<Container, object>>();
        private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        private readonly HashSet<Type> _singletonTypes = new HashSet<Type>();

        public void RegisterSingleton<TService>(Func<Container, TService> factory)
            => RegisterSingletonInternal(typeof(TService), c => factory(c));

        public void RegisterSingleton<TContract, TService>(Func<Container, TService> factory) where TService : TContract
            => RegisterSingletonInternal(typeof(TContract), c => factory(c));

        public void RegisterTransient<TService>(Func<Container, TService> factory)
            => RegisterTransientInternal(typeof(TService), c => factory(c));

        public void RegisterTransient<TContract, TService>(Func<Container, TService> factory) where TService : TContract
            => RegisterTransientInternal(typeof(TContract), c => factory(c));

        private void RegisterSingletonInternal(Type type, Func<Container, object> factory)
        {
            _factories[type] = factory;
            _singletonTypes.Add(type);
        }

        private void RegisterTransientInternal(Type type, Func<Container, object> factory)
        {
            _factories[type] = factory;
            _singletonTypes.Remove(type);
        }

        public void Resolve<TService>(TService service)
        {
            service = Resolve<TService>();
        }

        public TService Resolve<TService>()
        {
            var type = typeof(TService);

            object singletonInstance;
            if (_singletons.TryGetValue(type, out singletonInstance))
            {
                return (TService)singletonInstance;
            }

            Func<Container, object> factory;
            if (!_factories.TryGetValue(type, out factory))
            {
                throw new InvalidOperationException("No registration for type " + type.FullName);
            }

            var instance = factory(this);

            if (_singletonTypes.Contains(type))
            {
                _singletons[type] = instance;
            }

            return (TService)instance;
        }
    }
}

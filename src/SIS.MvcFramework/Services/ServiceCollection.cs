using System;
using System.Collections.Generic;
using System.Linq;

namespace SIS.MvcFramework.Services
{
    public class ServiceCollection : IServiceCollection
    {
        private IDictionary<Type, Type> dependencyContainer;

        public ServiceCollection()
        {
            this.dependencyContainer = new Dictionary<Type, Type>();
        }

        public void AddService<TSource, TDestination>()
        {
            this.dependencyContainer[typeof(TSource)] = typeof(TDestination);
        }

        public T CreateInstance<T>()
        {
            return (T) this.CreateInstance(typeof(T));
        }

        public object CreateInstance(Type type)
        {
            if (this.dependencyContainer.ContainsKey(type))
            {
                type = this.dependencyContainer[type];
            }

            if (type.IsInterface || type.IsAbstract)
            {
                throw new Exception($"Type {type.FullName} cannot be instantiated.");
            }

            // TODO: if empty -> use it 
            var constructor = type.GetConstructors().OrderBy(x => x.GetParameters().Length).First();
            var constructorParameters = constructor.GetParameters();
            var constructorParameterObjects = new List<object>();
            foreach (var constructorParameter in constructorParameters)
            {
                var parameterObject = this.CreateInstance(
                    constructorParameter.ParameterType);
                constructorParameterObjects.Add(parameterObject);
            }

            var obj = constructor.Invoke(constructorParameterObjects.ToArray());
            return obj;
        }
    }
}
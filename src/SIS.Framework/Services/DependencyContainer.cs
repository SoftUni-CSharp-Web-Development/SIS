using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SIS.Framework.Services.Contracts;

namespace SIS.Framework.Services
{
    public class DependencyContainer : IDependencyContainer
    {
        private readonly IDictionary<Type, Type> dependencyMap;

        public DependencyContainer(IDictionary<Type, Type> dependencyMap)
        {
            this.dependencyMap = dependencyMap;
        }

        private Type this[Type key] => 
            this.dependencyMap.ContainsKey(key) 
                ? this.dependencyMap[key] 
                : null;

        public void RegisterDependency<TSource, TDestination>()
        {
            this.dependencyMap[typeof(TSource)] = typeof(TDestination);
        }

        public T CreateInstance<T>() =>
            (T) CreateInstance(typeof(T));

        public object CreateInstance(Type type)
        {
            Type typeInstance = this[type] ?? type;

            if (typeInstance.IsInterface || typeInstance.IsAbstract)
            {
                throw new InvalidOperationException(
                    $"Type {typeInstance.FullName} cannot be instantiated. Abstract type and interfaces cannot be instantiated");
            }

            ConstructorInfo constructor = typeInstance
                .GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .First();

            ParameterInfo[] constructorParameters = constructor.GetParameters();

            object[] constructorParametersObjects = new object[constructorParameters.Length];

            for (int index = 0; index < constructorParameters.Length; index++)
            {
                constructorParametersObjects[index] = this.CreateInstance(
                    constructorParameters[index].ParameterType);
            }

            return constructor.Invoke(constructorParametersObjects);
        }
    }
}

using System;

namespace SIS.MvcFramework.Services
{
    public interface IServiceCollection
    {
        void AddService<TSource, TDestination>();

        T CreateInstance<T>();

        object CreateInstance(Type type);

        void AddService<T>(Func<T> p);
    }
}

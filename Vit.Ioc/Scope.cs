using Microsoft.Extensions.DependencyInjection;

using System;
using System.Runtime.CompilerServices;

namespace Vit.Ioc
{
    public class Scope : IDisposable
    {

        public IServiceProvider serviceProvider { get; private set; } = null;

        public IServiceScope scope { get; private set; } = null;

        public Scope(IServiceProvider rootServiceProvider)
        {
            scope = rootServiceProvider.CreateScope();
            serviceProvider = scope.ServiceProvider;
        }
        public Scope(Scope parentScope) : this(parentScope.serviceProvider)
        {
        }

        /// <summary>
        /// create root scope
        /// </summary>
        public Scope() : this(IocHelp.Instance.rootServiceProvider)
        {
        }
        public Scope(Ioc ioc) : this(ioc.rootServiceProvider)
        {
        }



        public void Dispose()
        {
            serviceProvider = null;
            scope?.Dispose();
            scope = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Scope CreateScope()
        {
            return new Scope(this);
        }


        #region Create


        /// <summary>
        /// Get service of type <typeparamref name="T" /> from the <see cref="T:System.IServiceProvider" />.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <returns>A service object of type <typeparamref name="T" /> or null if there is no such service.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Create<T>()
        {
            return serviceProvider.GetService<T>();
        }

        /// <summary>Gets the service object of the specified type.</summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType">serviceType</paramref>.  
        ///  -or-  
        ///  null if there is no service object of type <paramref name="serviceType">serviceType</paramref>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Create(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }
        #endregion


        #region AutoCreate

        /// <summary>
        /// Get service of type <typeparamref name="T" /> from the <see cref="T:System.IServiceProvider" />.
        /// if have not yet registered Type T in Ioc,then return new T();
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <returns>A service object of type <typeparamref name="T" /> or null if there is no such service.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T AutoCreate<T>() where T : class, new()
        {
            return Create<T>() ?? new T();
        }


        /// <summary>
        /// Get service of type <typeparamref name="TService" /> from the <see cref="TService:System.IServiceProvider" />.
        /// if have not yet registered Type TService in Ioc,then return new TImplementation();
        /// </summary>
        /// <typeparam name="TService">The type of the service to add.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation to use.</typeparam>      
        /// <returns>A reference to this instance after the operation has completed.</returns>
        /// <seealso cref="F:Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient" />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TService AutoCreate<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService, new()
        {
            return Create<TService>() ?? new TImplementation();
        }
        #endregion
    }
}

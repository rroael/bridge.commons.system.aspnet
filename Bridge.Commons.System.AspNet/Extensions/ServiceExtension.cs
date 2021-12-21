using System;
using System.Collections.Generic;
using Bridge.Commons.System.AspNet.Filters;
using Bridge.Commons.System.AspNet.Transformations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;

namespace Bridge.Commons.System.AspNet.Extensions
{
    /// <summary>
    ///     Extensão de serviços
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        ///     Adiciona roteamento
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddCustomRouting(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddRouting(options =>
            {
                options.ConstraintMap = new Dictionary<string, Type>
                    { { "slugify", typeof(SlugifyParameterTransformer) } };
                options.LowercaseUrls = true;
            });

            return services;
        }

        /// <summary>
        ///     Adiciona MVC
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IMvcBuilder AddCustomMvc(this IServiceCollection services)
        {
            return services.AddCustomMvc(options =>
            {
                options.Filters.Add(new ValidationFilter());
                options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
            });
        }

        /// <summary>
        ///     Adiciona MVC
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IMvcBuilder AddCustomMvc(this IServiceCollection services, Action<MvcOptions> setupAction)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));
            if (setupAction == null)
                throw new ArgumentNullException(nameof(setupAction));
            var mvcBuilder = services.AddMvc();
            mvcBuilder.Services.Configure(setupAction);
            return mvcBuilder;
        }
    }
}
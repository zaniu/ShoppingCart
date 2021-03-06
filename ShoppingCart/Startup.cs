﻿using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using ShoppingCart.Repository;
using ShoppingCart.Shared;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Mappers;
using ShoppingCart.Shared.Model;
using Swashbuckle.AspNetCore.Swagger;

namespace ShoppingCart
{
    public class Startup
    {
        IHostingEnvironment envoirment;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddTransient(typeof(IDataProvider<Cart>), typeof(StaticCartProvider));
            services.AddSingleton(typeof(ICartRepository), typeof(InMemoryCartRepository));
            services.AddTransient(typeof(IDataProvider<Product>), x=> new ProductDataProvider(x.GetService<IFileProvider>(), Configuration.GetValue<string>("ProductsSourceFile")));
            services.AddSingleton(typeof(IRepository<Product>), typeof(InMemoryProductReposiotry));
            services.AddTransient(typeof(IFileProvider), x => envoirment.ContentRootFileProvider);

            services.AddTransient(typeof(IValueResolver<CartItem, CartItemDto, CartProductDto>), typeof(ProductDtoResolver));
            services.AddTransient(typeof(IMapperProvider<Product, CartProductDto>), typeof(ProductMapperProvider));
            services.AddTransient(typeof(IMapperProvider<Cart, CartDto>), typeof(CartMapperProvider));
            services.AddTransient(typeof(IMapperProvider<AddCartItemDto, CartItem>), typeof(AddCartItemMapperProvider));
            

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "ShoppingCart", Version = "0.0.0.1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseStaticFiles();
            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ShoppingCart 0.0.0.1");
            });

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            envoirment = env;
        }
    }

    public class Config
    {
        public string Products { get; set; }
    }
}

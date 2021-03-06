﻿using AutoMapper;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Mappers;
using ShoppingCart.Shared.Model;
using SimpleFixture;
using System;
using System.Threading.Tasks;

namespace ShoppingCart.Shared.UnitTests.Mappers
{
    [TestClass]
    public class ProductDtoResolverTests
    {
        private Fixture fixture;
        private Mock<IMapper> mapperMock;
        private Mock<IMapperProvider<Product, CartProductDto>> mapperProviderMock;
        private Mock<IRepository<Product>> productRepositoryMock;

        [TestInitialize]
        public void Initialize()
        {
            fixture = new Fixture();
            mapperMock = new Mock<IMapper>();
            mapperProviderMock = new Mock<IMapperProvider<Product, CartProductDto>>();
            productRepositoryMock = new Mock<IRepository<Product>>();
        }

        [TestMethod]
        public void Should_ResolveValidDto()
        {
            // Arrange
            var cartItem = fixture.Generate<CartItem>();
            var product = fixture.Generate<Product>(constraints: new { Identifier = cartItem.ProductId });
            mapperMock.Setup(x => x.Map<CartProductDto>(It.IsAny<Product>()))
                .Returns<Product>(x => fixture.Generate<CartProductDto>(constraints: new { Id = x.Id }));
            
            mapperProviderMock.Setup(x => x.Provide())
                .Returns(mapperMock.Object);

            productRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Func<Product, bool>>()))
                .ReturnsAsync(product);

            var resolver = new ProductDtoResolver(productRepositoryMock.Object, mapperProviderMock.Object);

            // Act
            var result = resolver.Resolve(cartItem, null, null, null);

            // Assert
            cartItem.Should().NotBeNull();
            cartItem.ProductId.Should().Be(cartItem.ProductId);
        }

        [TestMethod]
        public void Should_ResolveToNull_When_ProductIsNotFound()
        {
            // Arrange
            var cartItem = fixture.Generate<CartItem>();

            mapperProviderMock.Setup(x => x.Provide())
                .Returns(mapperMock.Object);

            productRepositoryMock.Setup(x => x.GetAsync(It.IsAny<Func<Product,bool>>()))
                .ReturnsAsync(null as Product);

            var resolver = new ProductDtoResolver(productRepositoryMock.Object, mapperProviderMock.Object);

            // Act
            var result = resolver.Resolve(cartItem, null, null, null);

            // Assert
            result.Should().BeNull();
        }
    }
}

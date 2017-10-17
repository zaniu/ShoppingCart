﻿using AutoMapper;
using ShoppingCart.Shared.Dto;
using ShoppingCart.Shared.Model;
using System.Threading.Tasks;

namespace ShoppingCart.Shared.Mappers
{
    public class ProductDtoResolver : IValueResolver<CartItem, CartItemDto, CartProductDto>
    {
        private readonly IQueryableByIdRepository<Product> _productsRepository;
        private readonly IMapper _mapper;

        public ProductDtoResolver(IQueryableByIdRepository<Product> productsRepository, IMapperProvider<Product, CartProductDto> mapper)
        {
            this._productsRepository = productsRepository;
            this._mapper = mapper.Provide();
        }

        public CartProductDto Resolve(CartItem source, CartItemDto destination, CartProductDto destMember, ResolutionContext context)
        {
            var model = Task
                .Run(async () => await _productsRepository.GetByIdAsync(source.ID))
                .Result;

            return _mapper.Map<CartProductDto>(model);
        }
    }
}

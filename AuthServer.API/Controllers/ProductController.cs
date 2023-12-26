using AuthServer.Core.DTOs;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.API.Controllers
{
    //product apisine erişecek kişinin üye olması lazım token yollamalı
    [Authorize]
    public class ProductController : CustomBaseController
    {
        private readonly IGenericService<Product, ProductDTO> _productService;

        public ProductController(IGenericService<Product, ProductDTO> productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return CreateActionResult(await _productService.GetAllAsync());
        }

        [HttpPost]
        public async Task<IActionResult> SaveProduct(ProductDTO productDTO)
        {
            return CreateActionResult(await _productService.AddAsync(productDTO));
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductDTO productDTO)
        {
            return CreateActionResult(await _productService.UpdateAsync(productDTO, productDTO.Id));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveProduct(int id)
        {
            return CreateActionResult(await _productService.RemoveAsync(id));
        }
    }
}
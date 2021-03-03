using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    // https://localhost:5001/products
    // http://localhost:5000/products
    [Route("products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> Get(
            [FromServices] DataContext context
        )
        {
            var products = await context
                        .Products
                        .Include(x => x.Category)
                        .AsNoTracking()
                        .ToListAsync();
            return Ok(products);
        }

        [HttpGet]
        [Route("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Product>> GetById(
            int id,
            [FromServices] DataContext context
        )
        {
            var Product = await context
                        .Products
                        .Include(x => x.Category)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == id);
            return Ok(Product);
        }

        [HttpGet]
        [Route("categories/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Product>>> GetByCategory(
            [FromServices] DataContext context,
            int id
        )
        {
            var products = await context
                        .Products
                        .Include(x => x.Category)
                        .AsNoTracking()
                        .Where(x => x.CategoryId == id)
                        .ToListAsync();

            return Ok(products);
        }

        [HttpPost]
        [Route("")]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult<List<Product>>> Post(
            [FromBody] Product model,
            [FromServices] DataContext context

        )
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Adiciona os dados
                context.Products.Add(model);

                // Persiste os dados no banco
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível criar o produto" });
            }
        }
    }
}
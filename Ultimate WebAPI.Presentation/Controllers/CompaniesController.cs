﻿using Asp.Versioning;
using CompanyEmployees.Presentation.ActionFilters;
using CompanyEmployees.Presentation.ModelBinders;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    //[Route("api/{v:apiversion}/companies")] // instead of query param and header
    [Route("api/companies")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]

    public class CompaniesController : ControllerBase
    {


        private readonly IServiceManager _service;
        public CompaniesController(IServiceManager service) => _service = service;

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok();
        }
        /// <summary>
        /// Get list of companies
        /// </summary>
        /// <returns>The Company List</returns>
        [HttpGet]
        [Authorize(Roles = "Manager")] // zsaiyn01 with same password
        //[Authorize]
        public async Task<IActionResult> GetCompanies()
        {
            //throw new Exception("Exception");
            var companies = await
            _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
            return Ok(companies);
        }
        [HttpGet("{id:guid}", Name = "CompanyById")]
        [ResponseCache(CacheProfileName = "120SecondsDuration")]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
            return Ok(company);
        }
        /// <summary> 
        /// Creates a newly created company 
        /// </summary> 
        /// <param name="company"></param> 
        /// <returns>A newly created company</returns> 
        /// <response code="201">Returns the newly created item</response> 
        /// <response code="400">If the item is null</response> 
        /// <response code="422">If the model is invalid</response>
        [ProducesResponseType(201)] 
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {

            var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);
            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection
            ([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false);

            return Ok(companies);
        }
        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection
            ([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {

            var result = await
                _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);
            return CreatedAtRoute("CompanyCollection", new { result.ids },
            result.companies);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);
            return NoContent();
        }
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {

            await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);
            return NoContent();
        }
    }
}


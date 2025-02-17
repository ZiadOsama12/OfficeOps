using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Shared.RequestFeatures;
using Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    internal class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }
        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId,
            EmployeeParameters employeeParameters, bool trackChanges)
        {

            var employees = await FindByCondition(e => e.CompanyId.Equals(companyId),
                trackChanges)
            .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
            //.Search(employeeParameters.SearchTerm) // Note the position of the Search is important
            .OrderBy(e => e.Name)
            .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
            .Take(employeeParameters.PageSize)
            .Search(employeeParameters.SearchTerm) // Note the position of the Search is important
            .Sort(employeeParameters.OrderBy)

            .ToListAsync();

            var count = await FindByCondition(e => e.CompanyId.Equals(companyId), 
                trackChanges).CountAsync();
            return new PagedList<Employee>(employees, count,
                employeeParameters.PageNumber, employeeParameters.PageSize);
        }

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
            await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
                .SingleOrDefaultAsync();
        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }
        public void DeleteEmployee(Employee employee) => Delete(employee);

    }
}

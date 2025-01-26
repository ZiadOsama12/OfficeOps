using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    //[Serializable]
    //public record CompanyDto(Guid Id, string Name, string FullAddress); // this is ctor..so to be able to use it => use methd "ForConstructor"
    public record CompanyDto 
    {
        public Guid Id { get; init; }
        public string? Name { get; init; }
        public string? FullAddress { get; init; }
    }

}

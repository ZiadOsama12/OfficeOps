using Entities.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IDataShaper<T>
    {
        IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fieldsString); // for list
        ExpandoObject ShapeData(T entity, string fieldsString); // for only one

        //IEnumerable<Entity> ShapeData(IEnumerable<T> entities, string fieldsString);
        //Entity ShapeData(T entity, string fieldsString); // Remember it's just a wrapper for ExpandoObject + Root.

    }
}

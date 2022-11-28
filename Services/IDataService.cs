using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tp_final.Services
{
    interface IDataService<T>
    {
        IEnumerable<T> GetAll();

        int Insert(T value);
        int Update(T value);
        int Delete(T value);
        T GetById(int id);

        /// <summary>
        /// Returns the items that a true to the predicate
        /// </summary>
        /// <param name="predicate">Eg : person => person.age >= 18</param>
        /// <returns>IEnumerable of T</returns>
        // IEnumerable<T> Filter(Func<T, bool> predicate);
    }
}

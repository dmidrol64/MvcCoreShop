using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStoreMVC.Extensions
{
    // Урок 6
    public static class IEnumerableExtension
    {
        // 1. Создаём метод, который по умолчанию будет возвращать тип IEnumerable<SelectListItem>
        // А принемать в качестве параметров дженерик коллекцию IEnumerable и int selectedValue
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue)
        {
            // 2. Рассмотрим другой синтаксис запросов LINQ
            return from item in items
                   select new SelectListItem
                   {
                       // 3. !!! После этого шага добавить ещё один класс !!!
                       Text = item.GetPropertyValue("Name"),
                       Value = item.GetPropertyValue("Id"),
                       Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                   };
        }
    }
}

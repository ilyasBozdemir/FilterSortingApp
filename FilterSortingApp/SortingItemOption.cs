namespace FilterSortingApp;

public class SortingItemOption
{
    public string PropertyName { get; set; } // Sıralama yapılacak özellik adı
    public SortingDirection Direction { get; set; } // Sıralama yöntemi (Artan veya Azalan)

    public IQueryable<T> ApplySorting<T>(IQueryable<T> query)
    {

        if (Direction == SortingDirection.Ascending)

            return query.OrderBy(item => item.GetType().GetProperty(PropertyName).GetValue(item, null));

        else
            return query.OrderByDescending(item => item.GetType().GetProperty(PropertyName).GetValue(item, null));

    }
}
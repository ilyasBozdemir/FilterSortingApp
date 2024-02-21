using FilterSortingApp;

var people = new List<Person>
        {
            new Person { Name = "Ali", Age = 30 },
            new Person { Name = "Arif", Age = 45 },
            new Person { Name = "Ahmet", Age = 40 },
            new Person { Name = "Berk", Age = 25 },
            new Person { Name = "Cem", Age = 35 },
            new Person { Name = "Cemile", Age = 32 },
            new Person { Name = "Deniz", Age = 40 }
        }.AsQueryable();

var filters = new List<FilterItem>
{
    new FilterItem { ColumnName = "Name", Operator = FilterOperator.StartsWith, Value = "A" },
    new FilterItem { ColumnName = "Age", Operator = FilterOperator.IsBetween, Value = "30,45" },
    new FilterItem { ColumnName = "Age", Operator = FilterOperator.IsNotEqualTo, Value = 40 },
};

IQueryable<Person> filteredQuery = people;

if (filters.Any())
{
    foreach (var filter in filters)
    {
        filteredQuery = FilterItem.ApplyFilter(filteredQuery, filter);
    }
}
foreach (var filter in filters)
{
    filteredQuery = FilterItem.ApplyFilter(filteredQuery, filter);
}

var sortingItem = new SortingItemOption { PropertyName = "Age", Direction = SortingDirection.Ascending };
filteredQuery = sortingItem.ApplySorting(filteredQuery);

Console.WriteLine("Query:");
Console.WriteLine(filteredQuery.ToString());
Console.WriteLine();

Console.WriteLine("Filtered and Sorted People:");
foreach (var person in filteredQuery)
{
    Console.WriteLine($"Name: {person.Name}, Age: {person.Age}");
}
Console.ReadLine();
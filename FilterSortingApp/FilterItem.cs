using System.Linq.Expressions;

namespace FilterSortingApp;

public class FilterItem
{
    public string ColumnName { get; set; }
    public FilterOperator Operator { get; set; }
    public object Value { get; set; }

    public static IQueryable<T> ApplyFilter<T>(IQueryable<T> query, FilterItem filter)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, filter.ColumnName);
        var constant = Expression.Constant(filter.Value);
        Expression expression;

        switch (filter.Operator)
        {
            case FilterOperator.IsLessThan:
                expression = Expression.LessThan(property, constant);
                break;
            case FilterOperator.IsLessThanOrEqualTo:
                expression = Expression.LessThanOrEqual(property, constant);
                break;
            case FilterOperator.IsEqualTo:
                expression = Expression.Equal(property, constant);
                break;
            case FilterOperator.IsNotEqualTo:
                expression = Expression.NotEqual(property, constant);
                break;
            case FilterOperator.IsGreaterThanOrEqualTo:
                expression = Expression.GreaterThanOrEqual(property, constant);
                break;
            case FilterOperator.IsGreaterThan:
                expression = Expression.GreaterThan(property, constant);
                break;
            case FilterOperator.StartsWith:
                expression = Expression.Call(property, typeof(string).GetMethod("StartsWith", new[] { typeof(string) }), constant);
                break;
            case FilterOperator.EndsWith:
                expression = Expression.Call(property, typeof(string).GetMethod("EndsWith", new[] { typeof(string) }), constant);
                break;
            case FilterOperator.Contains:
                expression = Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant);
                break;
            case FilterOperator.DoesNotContain:
                expression = Expression.Not(Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant));
                break;
            case FilterOperator.IsNull:
                expression = Expression.Equal(property, Expression.Constant(null, property.Type));
                break;
            case FilterOperator.IsNotNull:
                expression = Expression.NotEqual(property, Expression.Constant(null, property.Type));
                break;
            case FilterOperator.IsEmpty:
                expression = Expression.Equal(property, Expression.Constant("", property.Type));
                break;
            case FilterOperator.IsNotEmpty:
                expression = Expression.NotEqual(property, Expression.Constant("", property.Type));
                break;
            case FilterOperator.TextSearch:
                expression = Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant);
                break;
            case FilterOperator.IsBetween:
                var values = filter.Value.ToString().Split(',').Select(x => Convert.ChangeType(x.Trim(), property.Type)).ToArray();
                if (values.Length != 2)
                {
                    throw new ArgumentException("IsBetween operatörü için iki değer gereklidir.");
                }
                expression = Expression.AndAlso(
                    Expression.GreaterThanOrEqual(property, Expression.Constant(values[0])),
                    Expression.LessThanOrEqual(property, Expression.Constant(values[1]))
                );
                break;
            case FilterOperator.In:
                var valueList = filter.Value.ToString().Split(',').Select(x => Convert.ChangeType(x.Trim(), property.Type));
                expression = Expression.Call(
                    typeof(Enumerable),
                    "Contains",
                    new[] { typeof(object) },
                    Expression.Constant(valueList),
                    property
                );
                break;
            case FilterOperator.NotIn:
                var notInValueList = filter.Value.ToString().Split(',').Select(x => Convert.ChangeType(x.Trim(), property.Type));
                expression = Expression.Not(Expression.Call(
                    typeof(Enumerable),
                    "Contains",
                    new[] { typeof(object) },
                    Expression.Constant(notInValueList),
                    property
                ));
                break;

            default:
                throw new NotSupportedException($"{filter.Operator} desteklenmiyor.");
        }

        var lambda = Expression.Lambda<Func<T, bool>>(expression, parameter);
        return query.Where(lambda);
    }
}

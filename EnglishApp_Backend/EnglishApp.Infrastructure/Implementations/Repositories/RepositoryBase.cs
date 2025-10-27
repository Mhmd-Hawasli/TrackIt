using EnglishApp.Domain.Common;
using EnglishApp.Domain.Repositories;
using EnglishApp.Infrastructure.Implementations.Persistence;

using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public RepositoryBase(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    // ==========================================================
    // Get All Operations  
    // ==========================================================

    #region GetQueryable
    public virtual IQueryable<T> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }
    #endregion

    #region GetAllWithFilterAsync
    public virtual async Task<(List<T>, int count)> GetAllWithFilterAndCountAsync(QueryParameters filterModel, params string[] includes)
    {
        IQueryable<T> query = GetQueryable();

        // Add includes to the query
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await ApplyFilterAsync(filterModel, query);
    }
    public virtual async Task<List<T>> GetAllWithFilterAsync(QueryParameters filterModel, params string[] includes)
    {
        IQueryable<T> query = GetQueryable();

        // Add includes to the query
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        (List<T> responseList, int count) = await ApplyFilterAsync(filterModel, query);
        return responseList;
    }
    #endregion

    #region ApplyFilterAsync
    public virtual async Task<(List<T>, int count)> ApplyFilterAsync(QueryParameters filterModel, IQueryable<T> query)
    {
        // Apply Global Filter
        if (!string.IsNullOrEmpty(filterModel.GlobalFilter))
        {
            var filterExpressions = new List<string>();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var propName = prop.Name;
                var propType = prop.PropertyType;
                var isNullable = Nullable.GetUnderlyingType(propType) != null;
                var nonNullablePropType = isNullable ? Nullable.GetUnderlyingType(propType) : propType;

                // Handle different data types for global filter
                if (nonNullablePropType == typeof(string))
                {
                    filterExpressions.Add($"({propName} != null && {propName}.ToLower().Contains(@0))");
                }
                else if (nonNullablePropType.IsPrimitive || nonNullablePropType == typeof(decimal) || nonNullablePropType == typeof(DateTime))
                {
                    filterExpressions.Add($"({propName} != null && {propName}.ToString().ToLower().Contains(@0))");
                }
            }

            if (filterExpressions.Any())
            {
                query = query.Where(string.Join(" || ", filterExpressions), filterModel.GlobalFilter.ToLower());
            }
        }

        // example of use (in userTable for example)
        // Just property => UserId 
        // Relation + property => UserType.UserTypeName
        // ManyRelation + property => Products.Any(ProductName)
        // Apply Specific Filters
        if (filterModel.Filters != null && filterModel.Filters.Any())
        {
            var filterStrings = new List<string>();
            var filterValues = new List<object>();

            foreach (var filter in filterModel.Filters)
            {

                object? filterValue = filter.Value.Value;
                if (filterValue == null || filter.Value.MatchMode == null) continue;
                string matchMode = filter.Value.MatchMode!.ToLower();

                var propertyPath = GetPropertyPathFromRelations(filter.Key); //Private helper function
                if (propertyPath == null)
                {
                    throw new Exception($"{filter.Key} Not found in {typeof(T).Name}");
                }

                string filterExpression = string.Empty;

                try
                {
                    // Detect if this is a relation filter using Any()
                    bool isRelationFilter = propertyPath.Contains(".Any(");

                    // Get the property type dynamically
                    var propertyType = GetPropertyTypeFromPath(typeof(T), propertyPath); // Private helper
                    if (propertyType == null) continue;

                    // Handle Nullable types
                    var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
                    bool isNullable = Nullable.GetUnderlyingType(propertyType) != null;

                    // Path used in the expression
                    string path = isNullable ? $"{propertyPath}.Value" : propertyPath;

                    // Set up subProperty in case of a relation
                    string subProperty = string.Empty;
                    if (isRelationFilter)
                    {
                        // Example: ProductCollections.Any(CollectionID)
                        var beforeAny = propertyPath.Split(".Any(")[0];
                        var insideAny = propertyPath.Split(".Any(")[1].TrimEnd(')');
                        subProperty = isNullable ? $"{insideAny}.Value" : insideAny; // CollectionID
                        path = subProperty; // path will be used inside Any
                    }
                    if (underlyingType == typeof(DateTime))
                    {
                        filterValue = ParseDateToUtc(filterValue.ToString()!);
                    }

                    // Convert the value coming from JSON
                    if (filterValue is JsonElement jsonElement)
                    {
                        if (underlyingType == typeof(string))
                            filterValue = jsonElement.GetString();
                        else if (underlyingType == typeof(int))
                            filterValue = jsonElement.GetInt32();
                        else if (underlyingType == typeof(long))
                            filterValue = jsonElement.GetInt64();
                        else if (underlyingType == typeof(decimal))
                            filterValue = jsonElement.GetDecimal();
                        else if (underlyingType == typeof(bool))
                            filterValue = jsonElement.GetBoolean();
                        else if (underlyingType == typeof(DateTime))
                            filterValue = jsonElement.GetDateTime();
                        else
                            filterValue = jsonElement.ToString(); // fallback
                    }

                    var convertedValue = Convert.ChangeType(filterValue, underlyingType);
                    filterValues.Add(convertedValue!);
                    int valueIndex = filterValues.Count - 1;

                    // Build the conditional expression
                    string condition = BuildFilterExpression(
                        path,
                        underlyingType,
                        matchMode,
                        valueIndex
                    );

                    if (!string.IsNullOrEmpty(condition))
                    {
                        if (isRelationFilter)
                        {
                            // Wrap inside Any
                            filterExpression = $"{propertyPath.Split(".Any(")[0]}.Any(x => {condition.Replace(path, "x." + subProperty)})";
                        }
                        else
                        {
                            filterExpression = condition;
                        }

                        filterStrings.Add(filterExpression);
                    }
                }
                catch (InvalidCastException)
                {
                    continue;
                }


            }

            if (filterStrings.Any())
            {
                query = query.Where(string.Join(" and ", filterStrings), filterValues.ToArray());
            }
        }


        // Apply Sorting
        if (filterModel.MultiSortMeta != null && filterModel.MultiSortMeta.Any())
        {
            var sortExpressions = new List<string>();
            foreach (var sort in filterModel.MultiSortMeta)
            {
                if (string.IsNullOrEmpty(sort.Field)) continue;
                // The dynamic sorting expression builder handles nested properties automatically
                // no need to check with GetProperty here

                var sortDirection = sort.Order.HasValue && sort.Order.Value == -1 ? "descending" : "ascending";
                var sortField = GetPropertyPathFromRelations(sort.Field); //private helper method
                sortExpressions.Add($"{sortField} {sortDirection}");
            }

            if (sortExpressions.Any())
            {
                query = query.OrderBy(string.Join(", ", sortExpressions));
            }
        }

        // Apply Pagination
        int responseCount = await query.CountAsync();
        List<T> responseList = await query.Skip(filterModel.First).Take(filterModel.Rows).ToListAsync();
        return (responseList, responseCount);
    }

    // Helper method to get the property type from a nested path
    private DateTime ParseDateToUtc(string dateString)
    {
        // If the string ends with 'Z', it's already in UTC (ISO8601 format)
        if (dateString.EndsWith("Z", StringComparison.OrdinalIgnoreCase))
        {
            return DateTime.Parse(dateString, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }

        // Try to parse as DateTime
        if (DateTime.TryParse(dateString, out var dateTime))
        {
            return dateTime.Kind switch
            {
                DateTimeKind.Utc => dateTime,
                DateTimeKind.Local => dateTime.ToUniversalTime(),
                _ => DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)
            };
        }

        // Try to parse as DateOnly (yyyy-MM-dd)
        if (DateOnly.TryParse(dateString, out var dateOnly))
        {
            return DateTime.SpecifyKind(
                dateOnly.ToDateTime(TimeOnly.MinValue),
                DateTimeKind.Utc
            );
        }

        // Throw if none of the formats matched
        throw new FormatException("Invalid date format: " + dateString);
    }
    private string? GetPropertyPathFromRelations(string filterKey)
    {
        if (filterKey.Contains("."))
        {
            return filterKey;
        }
        var properties = typeof(T).GetProperties().Select(p => p.Name.ToLower()).ToList();
        if (properties.Contains(filterKey.ToLower()))
        {
            return filterKey;
        }
        var relations = new PropertyFromRelations();

        if (relations.GetPropertyFromRelations.TryGetValue(typeof(T).Name.ToLower(), out var relationList))
        {
            var match = relationList.FirstOrDefault(x =>
                x.Property.Equals(filterKey, StringComparison.OrdinalIgnoreCase));

            if (match != null)
            {
                return match.PropertyWithRelations;
            }
        }
        return null;
    }
    private Type? GetPropertyTypeFromPath(Type type, string propertyPath)
    {
        while (!string.IsNullOrEmpty(propertyPath))
        {
            if (propertyPath.StartsWith("Any(", StringComparison.OrdinalIgnoreCase))
            {
                int startIndex = 4; // بعد "Any("
                int endIndex = propertyPath.IndexOf(')', startIndex);
                if (endIndex == -1) return null;

                string innerPath = propertyPath.Substring(startIndex, endIndex - startIndex); // المسار الداخلي داخل الـ Any
                if (!type.IsGenericType) return null;

                var elementType = type.GetGenericArguments()[0];

                // تابع recursive على المسار الداخلي
                type = GetPropertyTypeFromPath(elementType, innerPath);

                // اكمل بعد ) إذا فيه مسار اضافي
                propertyPath = propertyPath.Length > endIndex + 1 ? propertyPath.Substring(endIndex + 1).TrimStart('.') : "";
            }
            else
            {
                int dotIndex = propertyPath.IndexOf('.');
                string currentPart = dotIndex == -1 ? propertyPath : propertyPath.Substring(0, dotIndex);

                var prop = type.GetProperty(currentPart, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (prop == null) return null;

                type = prop.PropertyType;
                propertyPath = dotIndex == -1 ? "" : propertyPath.Substring(dotIndex + 1);
            }

            // إذا property هي Collection
            if (type.IsGenericType && typeof(IEnumerable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                type = type.GetGenericArguments()[0];
            }
        }

        return type;
    }
    private string BuildFilterExpression(string propertyPath, Type propType, string matchMode, int valueIndex)
    {
        if (propType == typeof(string))
        {
            switch (matchMode)
            {
                case "equals":
                case "eq":
                    return $"{propertyPath}.ToLower() == @{valueIndex}.ToLower()";
                case "notequals":
                case "ne":
                    return $"{propertyPath}.ToLower() != @{valueIndex}.ToLower()";
                case "contains":
                    return $"{propertyPath}.ToLower().Contains(@{valueIndex}.ToLower())";
                case "notcontains":
                    return $"!{propertyPath}.ToLower().Contains(@{valueIndex}.ToLower())";
                case "startswith":
                    return $"{propertyPath}.ToLower().StartsWith(@{valueIndex}.ToLower())";
                case "endswith":
                    return $"{propertyPath}.ToLower().EndsWith(@{valueIndex}.ToLower())";
            }
        }
        else if (propType == typeof(bool))
        {
            return $"{propertyPath} == @{valueIndex}";
        }
        else if (propType.IsPrimitive || propType == typeof(decimal))
        {
            switch (matchMode)
            {
                case "equals":
                case "eq":
                    return $"{propertyPath} == @{valueIndex}";
                case "notequals":
                case "ne":
                    return $"{propertyPath} != @{valueIndex}";
                case "lessthan":
                case "lt":
                    return $"{propertyPath} < @{valueIndex}";
                case "lte":
                case "less_than_or_equal_to":
                    return $"{propertyPath} <= @{valueIndex}";
                case "greaterthan":
                case "gt":
                    return $"{propertyPath} > @{valueIndex}";
                case "greaterthanorequalto":
                case "gte":
                    return $"{propertyPath} >= @{valueIndex}";
            }
        }
        else if (propType == typeof(DateTime))
        {
            switch (matchMode)
            {
                case "dateis":
                    return $"{propertyPath}.Date == @{valueIndex}.Date";
                case "dateisnot":
                    return $"{propertyPath}.Date != @{valueIndex}.Date";
                case "datebefore":
                    return $"{propertyPath}.Date < @{valueIndex}.Date";
                case "dateafter":
                    return $"{propertyPath}.Date > @{valueIndex}.Date";
            }
        }

        return string.Empty;
    }
    #endregion

    #region GetAllAsync
    public virtual async Task<IEnumerable<T>> GetAllAsync(params string[] includes)
    {
        IQueryable<T> query = GetQueryable();

        // Add includes to the query
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.ToListAsync();
    }
    #endregion

    #region GetAllAsync -softDelete
    public virtual async Task<IEnumerable<T>> GetAllWithSoftDeleteAsync(params string[] includes)
    {
        IQueryable<T> query = GetQueryable();

        // Add includes to the query
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.ToListAsync();
    }
    #endregion

    #region GetAllAsync WithWhere
    public virtual async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, params string[] includes)
    {
        IQueryable<T> query = GetQueryable();

        // Add includes to the query
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.Where(predicate).ToListAsync();
    }
    #endregion

    #region GetAllAsync WithWhere -softDelete
    public virtual async Task<IEnumerable<T>> GetAllWithSoftDeleteAsync(Expression<Func<T, bool>> predicate, params string[] includes)
    {
        IQueryable<T> query = GetQueryable();

        // Add includes to the query
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.Where(predicate).ToListAsync();
    }
    #endregion

    #region FindAllWithCancellationTokenAsync
    public virtual async Task<IEnumerable<T>> FindAllWithCancellationTokenAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }
    #endregion

    // ==========================================================
    // Special Get Operations  
    // ==========================================================

    #region CountAsync
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.CountAsync(predicate);
    }
    #endregion

    #region AnyAsync
    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }
    #endregion

    // ==========================================================
    // Get One Operations  
    // ==========================================================

    #region GetByIdAsync
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    #endregion

    #region FirstOrDefaultAsync
    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, params string[] includes)
    {
        IQueryable<T> query = GetQueryable();

        // Add includes to the query
        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return await query.FirstOrDefaultAsync(predicate);
    }
    #endregion

    #region FirstOrDefaultWithoutSoftDeleteAsync
    public virtual async Task<T?> FirstOrDefaultWithSoftDeleteAsync(Expression<Func<T, bool>> predicate, params string[] includes)
    {
        IQueryable<T> query = _dbSet.IgnoreQueryFilters();
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        return await query.FirstOrDefaultAsync(predicate);
    }
    #endregion  

    #region FirstOrDefaultWithSelectAsync
    public virtual async Task<TResult?> FirstOrDefaultWithSelectAsync<TResult>(Expression<Func<T, bool>> predicate,
        Expression<Func<T, TResult>> selectExpression) where TResult : class
    {
        return await _dbSet
            .Where(predicate)
            .Select(selectExpression)
            .FirstOrDefaultAsync();
    }
    #endregion

    // ==========================================================
    // Add Operations
    // ==========================================================

    #region AddAsync
    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }
    #endregion

    #region AddRangeAsync
    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }
    #endregion

    // ==========================================================
    // Update Operations
    // ==========================================================

    #region Update
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }
    #endregion

    // ==========================================================
    // Delete Operations
    // ==========================================================


    #region Remove
    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
    #endregion

    #region HardDelete
    public virtual void HardDelete(T entity)
    {
        _dbSet.Remove(entity);
    }
    #endregion

    #region RemoveRange
    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
    #endregion

    #region RemoveAllAsync
    public virtual async Task RemoveAllAsync(Expression<Func<T, bool>> predicate)
    {
        var entitiesToRemove = await _dbSet.Where(predicate).ToListAsync();
        if (entitiesToRemove.Any())
        {
            _dbSet.RemoveRange(entitiesToRemove);
        }
    }
    #endregion

}
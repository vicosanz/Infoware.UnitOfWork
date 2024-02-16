using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Reflection;

namespace Infoware.Specification.EfCore.Evaluators;

public class IncludeEvaluator : IEvaluator
{
	private static readonly MethodInfo _includeMethodInfo = typeof(EntityFrameworkQueryableExtensions)
		.GetTypeInfo().GetDeclaredMethods(nameof(EntityFrameworkQueryableExtensions.Include))
		.Single(mi => mi.GetGenericArguments().Length == 2
			&& mi.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
			&& mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));

	private static readonly MethodInfo _thenIncludeAfterReferenceMethodInfo
		= typeof(EntityFrameworkQueryableExtensions)
			.GetTypeInfo().GetDeclaredMethods(nameof(EntityFrameworkQueryableExtensions.ThenInclude))
			.Single(mi => mi.GetGenericArguments().Length == 3
				&& mi.GetParameters()[0].ParameterType.GenericTypeArguments[1].IsGenericParameter
				&& mi.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IIncludableQueryable<,>)
				&& mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>));

	private static readonly MethodInfo _thenIncludeAfterEnumerableMethodInfo
		= typeof(EntityFrameworkQueryableExtensions)
			.GetTypeInfo().GetDeclaredMethods(nameof(EntityFrameworkQueryableExtensions.ThenInclude))
			.Where(mi => mi.GetGenericArguments().Length == 3)
			.Single(
				mi =>
				{
					var typeInfo = mi.GetParameters()[0].ParameterType.GenericTypeArguments[1];

					return typeInfo.IsGenericType
						  && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>)
						  && mi.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IIncludableQueryable<,>)
						  && mi.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>);
				});

	private IncludeEvaluator()
	{
	}

	/// <summary>
	/// <see cref="IncludeEvaluator"/> instance without any additional features.
	/// </summary>
	public static IncludeEvaluator Default { get; } = new IncludeEvaluator();

	public bool IsCriteriaEvaluator => false;

	public IQueryable<T> GetQuery<T>(IQueryable<T> query, ISpecification<T> specification) where T : class
	{
		foreach (var includeString in specification.IncludeStrings)
		{
			query = query.Include(includeString);
		}

		foreach (var includeInfo in specification.IncludeExpressions)
		{
			if (includeInfo.Type == IncludeTypeEnum.Include)
			{
				query = BuildInclude<T>(query, includeInfo);
			}
			else if (includeInfo.Type == IncludeTypeEnum.ThenInclude)
			{
				query = BuildThenInclude<T>(query, includeInfo);
			}
		}

		return query;
	}

	private IQueryable<T> BuildInclude<T>(IQueryable query, IncludeExpressionInfo includeInfo)
	{
		_ = includeInfo ?? throw new ArgumentNullException(nameof(includeInfo));

		var result = _includeMethodInfo.MakeGenericMethod(includeInfo.EntityType, includeInfo.PropertyType).Invoke(null, [query, includeInfo.LambdaExpression]);

		_ = result ?? throw new TargetException();

		return (IQueryable<T>)result;
	}

	private IQueryable<T> BuildThenInclude<T>(IQueryable query, IncludeExpressionInfo includeInfo)
	{
		_ = includeInfo ?? throw new ArgumentNullException(nameof(includeInfo));
		_ = includeInfo.PreviousPropertyType ?? throw new ArgumentNullException(nameof(includeInfo.PreviousPropertyType));

		var result = (IsGenericEnumerable(includeInfo.PreviousPropertyType, out var previousPropertyType)
				? _thenIncludeAfterEnumerableMethodInfo
				: _thenIncludeAfterReferenceMethodInfo).MakeGenericMethod(includeInfo.EntityType, previousPropertyType, includeInfo.PropertyType)
			.Invoke(null, [query, includeInfo.LambdaExpression,]);

		_ = result ?? throw new TargetException();

		return (IQueryable<T>)result;
	}

	private static bool IsGenericEnumerable(Type type, out Type propertyType)
	{
		if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
		{
			propertyType = type.GenericTypeArguments[0];

			return true;
		}

		propertyType = type;

		return false;
	}
}

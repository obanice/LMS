using Core.Db;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Logic.Helpers
{
	public interface IBaseHelper
	{
		bool Create<TViewModel, TEntity>(TViewModel details)
			where TViewModel : class
			where TEntity : class, new();
		TEntity CreateAndReturnEntity<TViewModel, TEntity>(TViewModel details)
			where TViewModel : class
			where TEntity : class, new();
		IQueryable<TEntity> GetAll<TEntity>() where TEntity : class;
		IQueryable<TEntity> GetById<TEntity, T>(T id) where TEntity : class;
		IQueryable<TEntity> GetByPredicate<TEntity>(Expression<Func<TEntity, bool>>? predicate = null) where TEntity : class;
		bool HardDelete<TEntity, T>(T id) where TEntity : class;
		bool SoftDelete<TEntity, T>(T id) where TEntity : class;
		bool Update<TViewModel, TEntity, T>(TViewModel details, T primaryKey)
			where TViewModel : class
			where TEntity : class, new();
		string GetEnumDescription(Enum value);
		TEntity? Update<TEntity, T>(string propertyName, object propertyValue, T primaryKey) where TEntity : class, new();
		TEntity? Update<TEntity, T>(Dictionary<string, object> propertyValues, T primaryKey) where TEntity : class, new();
	}

	public class BaseHelper : IBaseHelper
	{
		private readonly AppDbContext _context;

		public BaseHelper(AppDbContext context)
		{
			_context = context;
		}
		/// <summary>
		/// Create any record and save into your desired class by passing the model and view-model as parameter.
		/// Make sure the name of the property is the same both in view-model and model.
		/// </summary>
		/// <typeparam name="TViewModel"></typeparam>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="details"></param>
		/// <returns><see cref="bool"/></returns>
		public bool Create<TViewModel, TEntity>(TViewModel details)
		   where TViewModel : class
		   where TEntity : class, new()
		{
			if (details == null)
			{
				return false;
			}

			var viewModelType = typeof(TViewModel);
			var entityType = typeof(TEntity);

			var properties = viewModelType.GetProperties();

			if (properties.Length == 0)
			{
				return false;
			}

			var instance = Activator.CreateInstance(entityType);

			foreach (var property in properties)
			{
				var sourceProperty = viewModelType.GetProperty(property.Name);
				var targetProperty = entityType.GetProperty(property.Name);

				if (sourceProperty != null && targetProperty != null)
				{
					var value = sourceProperty.GetValue(details);
					targetProperty.SetValue(instance, value);
				}
			}
			_context.Set<TEntity>().Add((TEntity)instance);
			_context.SaveChanges();
			return true;
		}
		/// <summary>
		/// Create any record and save into your desired class by passing the model and view-model as parameter.
		/// Make sure the name of the property is the same both in view-model and model.
		/// </summary>
		/// <typeparam name="TViewModel"></typeparam>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="details"></param>
		/// <returns><see cref="TEntity"/></returns>
		public TEntity CreateAndReturnEntity<TViewModel, TEntity>(TViewModel details)
		   where TViewModel : class
		   where TEntity : class, new()
		{
			if (details == null)
			{
				throw new ArgumentNullException(nameof(details));
			}

			var viewModelType = typeof(TViewModel);
			var entityType = typeof(TEntity);

			var properties = viewModelType.GetProperties();

			if (properties.Length == 0)
			{
				throw new ArgumentNullException(nameof(details));
			}

			var instance = Activator.CreateInstance(entityType);

			foreach (var property in properties)
			{
				var sourceProperty = viewModelType.GetProperty(property.Name);
				var targetProperty = entityType.GetProperty(property.Name);

				if (sourceProperty != null && targetProperty != null)
				{
					var value = sourceProperty.GetValue(details);
					targetProperty.SetValue(instance, value);
				}
			}
			_context.Set<TEntity>().Add((TEntity)instance);
			_context.SaveChanges();
			return (TEntity)instance;
		}
		/// <summary>
		/// Update the table with the parameter and data passed from the view-model
		/// </summary>
		/// <typeparam name="TViewModel"></typeparam>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="details"></param>
		/// <param name="primaryKey"></param>
		/// <returns>bool</returns>
		public bool Update<TViewModel, TEntity, T>(TViewModel details, T primaryKey)
			where TViewModel : class
			where TEntity : class, new()
		{
			if (details == null || primaryKey == null)
			{
				return false;
			}

			var viewModelType = typeof(TViewModel);
			var entityType = typeof(TEntity);

			var properties = viewModelType.GetProperties();

			if (properties.Length == 0)
			{
				return false;
			}

			var instance = _context.Set<TEntity>().Find(primaryKey);

			if (instance == null)
			{
				return false;
			}

			foreach (var property in properties)
			{
				var sourceProperty = viewModelType.GetProperty(property.Name);
				var targetProperty = entityType.GetProperty(property.Name);

				if (sourceProperty != null && targetProperty != null)
				{
					var value = sourceProperty.GetValue(details);
					targetProperty.SetValue(instance, value);
				}
			}
			_context.SaveChanges();
			return true;
		}
		public TEntity? Update<TEntity, T>(string propertyName, object propertyValue, T primaryKey)
			where TEntity : class, new()
		{
			if (propertyName == null || primaryKey == null)
			{
				return null;
			}

			var entityType = typeof(TEntity);

			var instance = _context.Set<TEntity>().Find(primaryKey);

			if (instance == null)
			{
				return null;
			}

			var targetProperty = entityType.GetProperty(propertyName);

			if (targetProperty == null || !targetProperty.CanWrite)
			{
				return null;
			}

			targetProperty.SetValue(instance, propertyValue);

			_context.SaveChanges();
			return instance;
		}
		public TEntity? Update<TEntity, T>(Dictionary<string, object> propertyValues, T primaryKey) where TEntity : class, new()
		{
			var entity = GetByPredicate<TEntity>(e => EF.Property<T>(e, "Id").Equals(primaryKey)).FirstOrDefault();
			if (entity != null)
			{
				foreach (var entry in propertyValues)
				{
					var property = entity.GetType().GetProperty(entry.Key);

					if (property != null)
					{
						property.SetValue(entity, entry.Value);
					}
				}
				_context.SaveChanges();
				return entity;
			}
			return null;
		}

		/// <summary>
		/// Delete record from db using the id of the class. 
		/// These set the active property to false and the deleted property to true
		/// </summary>
		/// <typeparam name="TViewModel"></typeparam>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="details"></param>
		/// <param name="primaryKey"></param>
		/// <returns>bool</returns>
		public bool SoftDelete<TEntity, T>(T id) where TEntity : class
		{
			var entity = _context.Set<TEntity>().Find(id);

			if (entity == null)
			{
				return false;
			}
			var activeProperty = typeof(TEntity).GetProperty("Active");
			var deletedProperty = typeof(TEntity).GetProperty("Deleted");

			if (activeProperty != null)
			{
				activeProperty.SetValue(entity, false);
			}
			if (deletedProperty != null)
			{
				deletedProperty.SetValue(entity, true);
			}
			_context.Set<TEntity>().Update(entity);
			_context.SaveChanges();
			return true;
		}
		/// <summary>
		/// Delete record from db using the id of the class. 
		/// These method remove the entry from db totally 
		/// </summary>
		/// <typeparam name="TViewModel"></typeparam>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="details"></param>
		/// <param name="primaryKey"></param>
		/// <returns>bool</returns>
		public bool HardDelete<TEntity, T>(T id) where TEntity : class
		{
			var entity = _context.Set<TEntity>().Find(id);

			if (entity == null)
			{
				return false;
			}
			_context.Set<TEntity>().Remove(entity);
			_context.SaveChanges();
			return true;
		}
		/// <summary>
		/// Get all entry from a class as IQuaryable using the Id
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns><see cref="IQueryable{T}"/></returns>
		public IQueryable<TEntity> GetById<TEntity, T>(T id) where TEntity : class
		{
			var query = _context.Set<TEntity>().AsQueryable();
			var idProperty = typeof(TEntity).GetProperty("Id");
			if (idProperty != null)
			{
				var parameter = Expression.Parameter(typeof(TEntity), "x");
				var predicate = Expression.Lambda<Func<TEntity, bool>>(
					Expression.Equal(
						Expression.Property(parameter, idProperty),
						Expression.Constant(id)
					),
					parameter
				);
				query = query.Where(predicate);
			}
			return query;
		}
		/// <summary>
		/// Get all entry from a class as IQuaryable.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <returns><see cref="IQueryable{T}"/></returns>
		public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class
		{
			return _context.Set<TEntity>().AsQueryable();
		}
		/// <summary>
		/// Get all entries of a class using a lambda query as predicate
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <param name="predicate"></param>
		/// <returns><see cref="IQueryable{T}"/></returns>
		public IQueryable<TEntity> GetByPredicate<TEntity>(Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
		{
			IQueryable<TEntity> query = _context.Set<TEntity>().AsQueryable();

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			return query;
		}
		public string GetEnumDescription(Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());

			DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

			if (attributes != null && attributes.Any())
			{
				var des = attributes.First().Description;
				return des;
			}

			return value.ToString();
		}
	}
}

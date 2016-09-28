using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using FT.Model;
namespace FT.Entities
{
    /// <summary>
    /// 扩展Entity Framework 中DbSet<T>类型的方法
    /// </summary>
    public static class DbSetExtend
    {
        #region 查询

        /// <summary>
        /// 扩展Entity Framework 中DbSet<T>类型的方法，主要用于查找实体对象的全部记录，不适用任何Where谓词
        /// </summary>
        /// <returns></returns>
        public static IQueryable<T> FindAll<T>(this DbSet<T> dbSet) where T : class
        {
            return dbSet.Where(item => true);
        }
        /// <summary>
        /// 扩展Entity Framework 中DbSet<T>类型的方法，主要用于查找实体对象的全部记录,支持分页、排序项等功能
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="dbSet">DbSet实体类型</param>
        /// <param name="pageNumber">第几页</param>
        /// <param name="pageSize">一页记录数量</param>
        /// <param name="total">总数量</param>
        /// <param name="orderBy">排序对象</param>
        /// <param name="isAsc">排序方式</param>
        /// <returns></returns>
        public static IQueryable<T> FindAll<T, TKey>(
            this DbSet<T> dbSet, int pageNumber, int pageSize, out int total, Expression<Func<T, TKey>> orderBy, string order = "true"
            ) where T : class
        {
            total = dbSet.Count();
            IQueryable<T> entities = dbSet.Where(item => true);

            if (order == "true")
                entities = entities.OrderBy(orderBy);
            else
                entities = entities.OrderByDescending(orderBy);

            entities = entities.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return entities;
        }

        /// <summary>
        /// 根据编号查询实体
        /// </summary>
        /// <param name="id">实体编号</param>
        /// <returns></returns>
        public static T FindById<T>(this DbSet<T> dbSet, object id) where T : class
        {
            T entity = dbSet.Find(id);
            return entity;
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public static IQueryable<T> FindBy<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> where) where T : class
        {
            return dbSet.Where(where);
        }
        /// <summary>
        /// 根据条件查询
        /// 
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <returns></returns>
        public static IQueryable<T> FindBy<T>(this IQueryable<T> entities, Expression<Func<T, bool>> where) where T : class
        {
            return entities.Where(where);
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="pageNumber">页码(起始为1)</param>
        /// <param name="pageSize">每页数据行数</param>
        /// <param name="total">符合条件总数据数量</param>
        /// <returns></returns>
        public static IQueryable<T> FindBy<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> where, int pageNumber, int pageSize, out int total) where T : class
        {
            total = dbSet.Count(where);
            return dbSet.Where(where).Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="pageNumber">页码(起始为1)</param>
        /// <param name="pageSize">每页数据行数</param>
        /// <param name="total">符合条件总数据数量</param>
        /// <returns></returns>
        public static IQueryable<T> FindBy<T>(this IQueryable<T> entities, Expression<Func<T, bool>> where, int pageNumber, int pageSize, out int total) where T : class
        {
            total = entities.Count(where);
            return entities.Where(where).Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="pageNumber">页码(起始为1)</param>
        /// <param name="pageSize">每页数据行数</param>
        /// <param name="total">符合条件总数据数量</param>
        /// <returns></returns>
        public static IQueryable<T> FindBy<T, TKey>(this DbSet<T> dbSet, Expression<Func<T, bool>> where, int pageNumber, int pageSize, out int total, Expression<Func<T, TKey>> sort, bool order = true) where T : class
        {
            total = dbSet.Count(where);
            IQueryable<T> entities;
            if (order)
                entities = dbSet.Where(where).OrderBy(sort).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            else
                entities = dbSet.Where(where).OrderByDescending(sort).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return entities;
        }
        /// <summary>
        /// 根据条件查询
        /// </summary>
        /// <param name="where">查询条件</param>
        /// <param name="pageNumber">页码(起始为1)</param>
        /// <param name="pageSize">每页数据行数</param>
        /// <param name="total">符合条件总数据数量</param>
        /// <returns></returns>
        public static IQueryable<T> FindBy<T, TKey>(this IQueryable<T> entities, Expression<Func<T, bool>> where, int pageNumber, int pageSize, out int total, Expression<Func<T, TKey>> sort, bool order = true) where T : class
        {
            IQueryable<T> newEntities;
            total = entities.Count(where);
            if (order)
                newEntities = entities.Where(where).OrderBy(sort).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            else
                newEntities = entities.Where(where).OrderByDescending(sort).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return newEntities;
        }
        public static IQueryable<T> FindBy<T>(this IQueryable<T> entities, Expression<Func<T, bool>> where, int pageNumber, int pageSize, out int total, string sort, bool isdesc = true) where T : class
        {
            IQueryable<T> newEntities;
            try
            {
                total = entities.Count(where);
                if (!isdesc)
                    newEntities = entities.Where(where).OrderBy(sort).Skip((pageNumber - 1) * pageSize).Take(pageSize);
                else
                    newEntities = entities.Where(where).OrderByDescending(sort).Skip((pageNumber - 1) * pageSize).Take(pageSize);
            }
            catch (Exception ex)
            {
                total = 0;
                newEntities = null;
            }
            return newEntities;
        }
        #endregion

        #region 排序扩展
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, String propertyName)
        {
            return InternalOrder<T>(source, propertyName, "OrderBy");
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, String propertyName)
        {
            return InternalOrder<T>(source, propertyName, "OrderByDescending");
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, String propertyName)
        {
            return InternalOrder<T>(source, propertyName, "ThenBy");
        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, String propertyName)
        {
            return InternalOrder<T>(source, propertyName, "ThenByDescending");
        }
        private static IOrderedQueryable<T> InternalOrder<T>(IQueryable<T> source, String propertyName, String methodName)
        {
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "p");
            PropertyInfo property = type.GetProperty(propertyName);
            Expression expr = Expression.Property(arg, property);
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), property.PropertyType);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            return ((IOrderedQueryable<T>)(typeof(Queryable).GetMethods().Single(
                p => String.Equals(p.Name, methodName, StringComparison.Ordinal)
                    && p.IsGenericMethodDefinition
                    && p.GetGenericArguments().Length == 2
                    && p.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.PropertyType)
                .Invoke(null, new Object[] { source, lambda })));
        }
        #endregion

        #region 查询条件扩展
        /// <summary>
        /// 查询条件判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, bool condition)
        {
            return condition ? source.Where(predicate) : source;
        }
        /// <summary>
        /// 查询条件判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool condition)
        {
            return condition ? source.Where(predicate) : source;
        }
        #endregion

        #region 删除

        /// <summary>
        /// 根据编号删除实体
        /// </summary>
        /// <param name="ids">待删除的实体编号集合</param>
        public static void Remove<T>(this DbSet<T> dbSet, params object[] ids) where T : class
        {
            List<T> entities = new List<T>();
            foreach (var id in ids)
                entities.Add(dbSet.FindById(id));
            dbSet.RemoveRange(entities);
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="where">待删除实体需要符合的条件</param>
        public static void Remove<T>(this DbSet<T> dbSet, Expression<Func<T, bool>> where) where T : class
        {
            IQueryable<T> entities = dbSet.FindBy(where);
            dbSet.RemoveRange(entities);
        }

        #endregion
    }
}

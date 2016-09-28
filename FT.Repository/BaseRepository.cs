using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EntityFramework.Caching;
using FT.IRepository;
using EntityFramework.Extensions;
using FT.Model;
using FT.Utility.Helper;
using FT.Utility.Linq;
namespace FT.Repository
{
    public abstract class BaseRepository<T> : FtContext, IBaseRepository<T> where T : class
    {
        private static readonly object Obj = new object();
        public int Total = 0;
        public bool Insert(T model)
        {
            return QueryDb((context) =>
            {
                context.Set<T>().Add(model);
                return context.SaveChanges() > 0;
            },false);
        }
        public bool Update(Expression<Func<T, bool>> predi, Expression<Func<T, T>> filed)
        {
            return QueryDb((context) => context.Set<T>().Where(predi).Update(filed) > 0);
        }
        public bool Delete(T model)
        {
            return QueryDb((context) =>
            {
                context.Set<T>().Remove(model);
                return context.SaveChanges() > 0;
            });
        }
        public bool Delete(Expression<Func<T, bool>> predi)
        {
            return QueryDb((context) => context.Set<T>().Where(predi).Delete() > 0,false);
        }
        public T Find(Expression<Func<T, bool>> predi)
        {
            return QueryDb((context) => context.Set<T>().FirstOrDefault(predi));
        }

        public List<T> GetList(Expression<Func<T, bool>> predi)
        {
            return QueryDb((context) => context.Set<T>().Where(predi).ToList(), new List<T>());
        }

        public List<T> GetListWithCache(Expression<Func<T, bool>> predi, int cachetimes)
        {
            return QueryDb((context) => context.Set<T>()
                .Where(predi)
                .FromCache(CachePolicy.WithSlidingExpiration(TimeSpan.FromSeconds(cachetimes))).ToList(), new List<T>());
        }
        public EasyDataGrid<T> DataGrid(Expression<Func<T, bool>> predi, int page, int size)
        {
            var grid=new EasyDataGrid<T>();
            return QueryDb((context) =>
            {
                var q = context.Set<T>().Where(predi);
                var q1 = q.FutureCount();
                var q2 = q.Skip((page - 1)*size).Take(size).Future();
                grid.rows = q2.ToList();
                grid.total = q1.Value;
                return grid;
            }, grid);
        }
        #region method
        public long GetOrderId()
        {
            lock (Obj)
            {
                var orderId = string.Empty;
                var random = new Random(BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0));
                for (var i = 0; i < 5; i++)
                {
                    var rand = random.Next();
                    var code = (char)('0' + (char)(rand % 10));
                    orderId += code.ToString();
                }
                return long.Parse(DateTime.Now.ToString("yyyyMMddfff") + orderId);
            }
        }
        #endregion
        /// <summary>
        /// 批量删除执行SQL
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public int DeleteSql(Expression<Func<T, bool>> predicate)
        {
            //获取表名
            var tableName = typeof (T).Name;
            //查询条件表达式转换成SQL的条件语句
            var builder = new ConditionBuilder();
            builder.Build(predicate.Body);
            string sqlCondition = builder.Condition;
            //SQL命令
            string commandText = string.Format("DELETE FROM {0} WHERE {1}", tableName, sqlCondition);
            //获取SQL参数数组 
            var args = builder.Arguments;
            return QueryDb((context) => context.Database.ExecuteSqlCommand(commandText, args), 0);
        }

        /// <summary>
        /// 批量更新执行SQL [不支持数据库可为空字段]
        /// </summary>
        /// <param name="predi"></param>
        /// <param name="filed"></param>
        /// <returns></returns>
        public int UpdateSql(Expression<Func<T, bool>> predi, Expression<Func<T, T>> filed)
        {
            //获取表名
            var tableName = typeof(T).Name;
            //查询条件表达式转换成SQL的条件语句
            var builder = new ConditionBuilder();
            builder.Build(predi.Body);
            var sqlCondition = builder.Condition;
            //获取Update的赋值语句
            var updateMemberExpr = (MemberInitExpression)filed.Body;
            var updateMemberCollection = updateMemberExpr.Bindings.Cast<MemberAssignment>().Select(c => new
            {
                Name = c.Member.Name,
                Value = ((ConstantExpression)c.Expression).Value
            });
            var i = builder.Arguments.Length;
            var sqlUpdateBlock = string.Join(", ", updateMemberCollection.Select(c => string.Format("[{0}]={1}", c.Name, "{" + (i++) + "}")).ToArray());
            //SQL命令
            var commandText = string.Format("UPDATE {0} SET {1} WHERE {2}", tableName, sqlUpdateBlock, sqlCondition);
            //获取SQL参数数组 (包括查询参数和赋值参数)
            var args = builder.Arguments.Union(updateMemberCollection.Select(c => c.Value)).ToArray();
            //执行
            return QueryDb((context) => 
                context.Database.ExecuteSqlCommand(commandText, args)
                , 0);
        }
        /// <summary>
        /// 执行SQL 
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <returns>int</returns>
        public int ExecuteSql(string sql)
        {
            return QueryDb((context) =>
               context.Database.ExecuteSqlCommand(TransactionalBehavior.EnsureTransaction,sql)
               , 0);
        }
    }
}

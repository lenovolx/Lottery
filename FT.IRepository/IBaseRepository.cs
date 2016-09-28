using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using FT.Model;

namespace FT.IRepository
{
    public interface IBaseRepository<T> where T : class
    {
        bool Insert(T model);
        bool Update(Expression<Func<T, bool>> predi, Expression<Func<T, T>> filed);
        bool Delete(T model);
        bool Delete(Expression<Func<T, bool>> predi);
        T Find(Expression<Func<T, bool>> predi);
        List<T> GetList(Expression<Func<T, bool>> predi);
        List<T> GetListWithCache(Expression<Func<T, bool>> predi, int cachetimes);
        EasyDataGrid<T> DataGrid(Expression<Func<T, bool>> predi, int page, int size);
        //SQL操作
        int DeleteSql(Expression<Func<T, bool>> predicate);
        int UpdateSql(Expression<Func<T, bool>> predi, Expression<Func<T, T>> filed);
        int ExecuteSql(string sql);
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FT.Entities;
using FT.Utility.Helper;
using System.Transactions;
using FT.Utility.CacheHelper;

namespace FT.Repository
{
    public class FtContext
    {
        public double TimeZonePlat
        {
            get
            {
                try
                {
                    return string.IsNullOrEmpty(System.Configuration.ConfigurationSettings.AppSettings["TimeZone"])
                        ? 8
                        : int.Parse(System.Configuration.ConfigurationSettings.AppSettings["TimeZone"]);
                }
                catch
                {
                    return 8;
                }
            }
        }
        /// <summary>
        /// 数据库操作 返回dynamic类型
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        protected static dynamic QueryDb(Func<Entity, dynamic> func,dynamic def=null)
        {
            try
            {
                using (var context = new Entity())
                {
                    var result = func(context);
                    return result;
                }
            }
            catch (DbEntityValidationException dbex)
            {
                var error = string.Empty;
                foreach (var item2 in dbex.EntityValidationErrors.SelectMany(item => item.ValidationErrors))
                {
                    error = string.Format("{0}:{1}\r\n", item2.PropertyName, item2.ErrorMessage);
                }
                Log.Error(error);
                return def ?? default(dynamic);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
                return def ?? default(dynamic);
            }
        }
        /// <summary>
        /// 数据库操作 无返回值
        /// </summary>
        /// <param name="action"></param>
        protected static void QueryDb(Action<Entity> action)
        {
            try
            {
                using (var context = new Entity())
                {
                    action(context);
                }
            }
            catch (DbEntityValidationException dbex)
            {
                var error = string.Empty;
                foreach (var item2 in dbex.EntityValidationErrors.SelectMany(item => item.ValidationErrors))
                {
                    error = string.Format("{0}:{1}\r\n", item2.PropertyName, item2.ErrorMessage);
                }
                Log.Error(error);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        /// <summary>
        /// 数据库操作 使用事务
        /// </summary>
        /// <param name="func">操作方法</param>
        /// <param name="def">默认值传入</param>
        /// <returns></returns>
        protected static dynamic QueryDbUseTran(Func<Entity, dynamic> func, dynamic def = null)
        {
            using (var tran = new TransactionScope())
            {
                try
                {
                    using (var context = new Entity())
                    {
                        var result = func(context);
                        context.SaveChanges();
                        tran.Complete();
                        return result;
                    }
                }
                catch (DbEntityValidationException dbex)
                {
                    var error = string.Empty;
                    foreach (var item2 in dbex.EntityValidationErrors.SelectMany(item => item.ValidationErrors))
                    {
                        error = string.Format("{0}:{1}\r\n", item2.PropertyName, item2.ErrorMessage);
                    }
                    Log.Error(error);
                    return def ?? default(dynamic);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    return def ?? default(dynamic);
                }
                finally
                {
                    tran.Dispose();
                }
            }
        }
    }
}

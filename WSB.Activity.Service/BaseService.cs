using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WSB.Activity;
using WSB.Activity.Interface;
using WSB.Activity.Common;

namespace WSB.Activity.Service
{
    public abstract class BaseService : IBaseService
    {
        #region identity
        protected DbContext Context { get; private set; }

        public BaseService(DbContext context)
        {
            this.Context = context;
        }
        #endregion

        public IQueryable<T> Set<T>() where T : class
        {
            return this.Context.Set<T>();
        }

        public void Commit()
        {
            this.Context.SaveChanges();
        }

        #region insert操作
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public T Insert<T>(T t) where T : class
        {
            this.Context.Set<T>().Add(t);
            this.Commit();
            return t;
        }

        /// <summary>
        /// 插入多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public IEnumerable<T> Insert<T>(IEnumerable<T> list) where T : class
        {
            this.Context.Set<T>().AddRange(list);
            this.Commit();
            return list;
        }
        #endregion insert

        #region delete操作
        /// <summary>
        /// 删除一条数据（根据编号）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void Delete<T>(T t) where T : class
        {
            if (t == null)
            {
                throw new Exception("t is null");
            }
            this.Context.Set<T>().Remove(t);
            this.Commit();
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        public void Delete<T>(int Id) where T : class
        {
            T t = this.Find<T>(Id);
            if (t == null)
            {
                throw new Exception("t is null");
            }
            this.Context.Set<T>().Remove(t);
            this.Commit();
        }

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList"></param>
        public void Delete<T>(IEnumerable<T> tList) where T : class
        {
            foreach (var t in tList)
            {
                this.Context.Set<T>().Attach(t);
            }
            this.Context.Set<T>().RemoveRange(tList);
            this.Commit();
        }
        #endregion

        #region update操作
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void Update<T>(T t) where T : class
        {
            if (t == null)
            {
                throw new Exception("t is null");
            }
            this.Context.Set<T>().Attach(t);
            this.Context.Entry<T>(t).State = EntityState.Modified;
            this.Commit();
        }

        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tList"></param>
        public void Update<T>(IEnumerable<T> tList) where T : class
        {
            foreach (var t in tList)
            {
                this.Context.Set<T>().Attach(t);
                this.Context.Entry<T>(t).State = EntityState.Modified;
            }
            this.Commit();
        }
        #endregion

        #region query操作
        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Find<T>(int id) where T : class
        {
            return this.Context.Set<T>().Find(id);
        }

        public IQueryable<T> Query<T>(Expression<Func<T, bool>> conditions) where T : class
        {
            return this.Context.Set<T>().Where<T>(conditions);
        }

        public IQueryable<T> ExcuteQuery<T>(string sql, SqlParameter[] parameters) where T : class
        {
            return this.Context.Database.SqlQuery<T>(sql, parameters).AsQueryable();
        }

        public void Excute<T>(string sql, SqlParameter[] parameters) where T : class
        {
            DbContextTransaction trans = null;
            try
            {
                trans = this.Context.Database.BeginTransaction();
                this.Context.Database.ExecuteSqlCommand(sql, parameters);
                trans.Commit();
            }
            catch (Exception ex)
            {
                if (trans != null)
                {
                    trans.Rollback();
                }
                throw ex;
            }
        }

        public virtual void Dispose()
        {
            if (this.Context != null)
            {
                this.Context.Dispose();
            }
        }
        #endregion

        #region 分页查询
        public PageResult<T> QueryPage<T, S>(Expression<Func<T, bool>> conditions, int pageIndex, int pageSize,
            string sort, bool isAsc = true) where T : class
        {
            var list = this.Set<T>();
            if (conditions != null)
            {
                list = list.Where<T>(conditions);
            }
            if (!string.IsNullOrEmpty(sort))
            {
                list = list.OrderBy(sort, isAsc);
            }
            PageResult<T> result = new PageResult<T>()
            {
                DataList = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = conditions != null ? this.Set<T>().Count(conditions) : this.Set<T>().Count()
            };
            return result;
        }
        #endregion
    }
}

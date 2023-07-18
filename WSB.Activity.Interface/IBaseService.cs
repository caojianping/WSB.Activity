using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WSB.Activity.Interface
{
    public interface IBaseService
    {
        IQueryable<T> Set<T>() where T : class;

        void Commit();

        #region insert操作
        /// <summary>
        /// 插入一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        T Insert<T>(T t) where T : class;

        /// <summary>
        /// 插入多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        IEnumerable<T> Insert<T>(IEnumerable<T> list) where T : class;
        #endregion

        #region delete操作
        /// <summary>
        /// 删除一条数据（根据编号）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        void Delete<T>(int id) where T : class;

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        void Delete<T>(T t) where T : class;

        /// <summary>
        /// 删除多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        void Delete<T>(IEnumerable<T> list) where T : class;
        #endregion

        #region update操作
        /// <summary>
        /// 更新一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        void Update<T>(T t) where T : class;

        /// <summary>
        /// 更新多条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        void Update<T>(IEnumerable<T> list) where T : class;
        #endregion

        #region query操作
        /// <summary>
        /// 查询一条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        T Find<T>(int id) where T : class;

        IQueryable<T> Query<T>(Expression<Func<T, bool>> conditions) where T : class;

        IQueryable<T> ExcuteQuery<T>(string sql, SqlParameter[] parameters) where T : class;

        void Excute<T>(string sql, SqlParameter[] parameters) where T : class;
        #endregion

        #region 分页查询
        PageResult<T> QueryPage<T, S>(Expression<Func<T, bool>> conditions, int pageIndex, int pageSize,
            string sort, bool isAsc = true) where T : class;
        #endregion
    }
}

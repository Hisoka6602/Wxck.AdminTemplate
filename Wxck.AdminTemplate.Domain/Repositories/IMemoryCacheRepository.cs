namespace Wxck.AdminTemplate.Domain.Repositories {

    public interface IMemoryCacheRepository<T> : IRepository<T> where T : class {

        /// <summary>
        /// 获取缓存内容
        /// </summary>
        /// <returns></returns>
        Task<List<T>?> MemoryCacheData();

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <returns>操作是否成功</returns>
        Task<bool> ClearCache();
    }
}
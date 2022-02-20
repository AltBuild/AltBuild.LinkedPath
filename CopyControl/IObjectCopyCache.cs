using System;

namespace AltBuild.LinkedPath
{
    public interface IObjectCopyCache
    {
        /// <summary>
        ///  Get cache with create
        /// </summary>
        /// <param name="type">Create type</param>
        /// <param name="id">Create ID</param>
        /// <param name="value">Create instance</param>
        /// <returns>false: get cache, true: get create</returns>
        bool TryGetOrCreate(Type type, object id, out object value);

        /// <summary>
        ///  Get cache with create
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="value">Destine object</param>
        /// <returns>false: get cache, true: get create</returns>
        bool TryGetOrCreate(object source, out object value);

        /// <summary>
        ///  Get cache
        /// </summary>
        /// <param name="source">Source object</param>
        /// <param name="func">Where</param>
        /// <param name="value">Destine object</param>
        /// <returns></returns>
        bool TryGetOrCreate(object source, Func<object> func, out object value);
    }
}

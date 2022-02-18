using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    public interface ICopyCache
    {
        /// <summary>
        ///  �w��̌^��ID�����C���X�^���X���擾����
        /// </summary>
        /// <param name="type">�����^�C�v</param>
        /// <param name="id">����ID</param>
        /// <param name="value">�����C���X�^���X</param>
        /// <returns>false: get cache, true: get create</returns>
        bool TryGetOrCreate(Type type, object id, out object value);

        /// <summary>
        ///  �C���X�^���X���擾����
        /// </summary>
        /// <param name="source">������</param>
        /// <param name="value">������</param>
        /// <returns>false: get cache, true: get create</returns>
        bool TryGetOrCreate(object source, out object value);

        /// <summary>
        ///  �C���X�^���X���擾����
        /// </summary>
        /// <param name="source">������</param>
        /// <param name="func">  </param>
        /// <param name="value">������</param>
        /// <returns></returns>
        bool TryGetOrCreate(object source, Func<object> func, out object value);
    }
}

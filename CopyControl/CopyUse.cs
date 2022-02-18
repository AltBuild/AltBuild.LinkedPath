using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// Object copy use.
    /// </summary>
    public enum CopyUse
    {
        /// <summary>
        /// ��������
        /// </summary>
        Auto = 0,

        /// <summary>
        /// �V��������Ƃ��ĕ�������
        ///   Summary:
        ///     clone = source.Clone();
        ///     clone.Original = null;
        ///     clone.Rule = Writable;
        ///     //source.Original   ���ύX�Ȃ�
        /// </summary>
        NewGeneration,

        /// <summary>
        /// ����
        ///   Summary:
        ///     clone = source.Clone();
        ///     clone.Original = source.Original;
        /// </summary>
        Duplicate,

        /// <summary>
        /// �I���W�i���̃N���[�����쐬����
        ///   Summary:
        ///     clone = source.Clone();
        ///     clone.Original = source;
        ///     source.Original = clone;
        /// </summary>
        Original,

        /// <summary>
        /// �L���b�V���p�̃N���[�����쐬����i�V�K�L���b�V���p�j
        ///   Summary:
        ///     clone = source.Clone();
        ///     clone.Original = null;
        ///     clone.Rule &= ~DObjectRule.Writable;
        /////     source.Original = clone;
        /// </summary>
        Cache,

        /// <summary>
        /// �L���b�V���p�̃R�s�[������i�L���b�V���X�V�p�j
        ///   Summary:
        /// </summary>
        CacheUpdate,
    }
}

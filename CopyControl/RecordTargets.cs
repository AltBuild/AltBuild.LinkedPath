using System;
using System.Collections.Generic;
using System.Linq;

namespace AltBuild.LinkedPath
{
    [Flags]
    public enum RecordTargets
    {
        /// <summary>
        /// Not edit (Default)
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// �ꎟ�e�[�u���܂�
        /// </summary>
        First = 0x0001,

        /// <summary>
        /// �񎟃e�[�u���܂�
        /// </summary>
        Second = 0x0002,

        /// <summary>
        /// �f�[�^�e�[�u�����Ώ�
        /// </summary>
        Data = 0x1000,

        /// <summary>
        /// �}�X�^�[�e�[�u�����Ώ�
        /// </summary>
        Master = 0x2000,

        /// <summary>
        /// �ꎟ�f�[�^�e�[�u���܂�
        /// </summary>
        FirstData = First | Data,

        /// <summary>
        /// �ꎟ�}�X�^�[�e�[�u���܂�
        /// </summary>
        FirstMaster = First | Master,

        /// <summary>
        /// �񎟃f�[�^�e�[�u���܂�
        /// </summary>
        SecondData = Second | Data,

        /// <summary>
        /// �񎟃}�X�^�[�e�[�u���܂�
        /// </summary>
        SecondMaster = Second | Master,

        /// <summary>
        /// �S�đΏ�
        /// </summary>
        All = Data | Master,

        /// <summary>
        /// Auto mode
        /// </summary>
        Auto = 0x800000,
    }
}

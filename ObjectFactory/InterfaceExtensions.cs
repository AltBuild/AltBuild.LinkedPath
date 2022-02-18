using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AltBuild.LinkedPath
{
    public static class InterfaceExtensions
    {
        /// <summary>
        /// �L���b�V����ێ�
        /// </summary>
        static Dictionary<Type, object> KeyValuePairs = new();

        /// <summary>
        /// �Ώۃh���C���̑S�^�C�v����w��̃C���^�[�t�F�[�X���p�����Ă���S�N���X���擾����
        /// </summary>
        /// <param name="targetObject"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static T[] GetInterfaceInstance<T>()
        {
            // �Y�����\�b�h
            T[] results;

            // �^�C�v���擾
            Type typeClass = typeof(T);

            // �L���b�V�����擾
            if (KeyValuePairs.TryGetValue(typeClass, out object classes) == false)
            {
                // ���X�g��ێ�
                var list = new List<T>();

                // �A�Z���u�����Ŏw��̃C���^�[�t�F�[�X���p������S�N���X���擾
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                    foreach (var type in asm.GetTypes().Where(t => t.GetInterfaces().Contains(typeClass)))
                        list.Add((T)Activator.CreateInstance(type));

                // �C���^�[�t�F�[�X�ꗗ��ێ�
                KeyValuePairs[typeClass] = results = list.ToArray();

                // �C���^�[�t�F�[�X�ꗗ��Ԃ�
                return results;
            }
            
            else
            {
                return (T[])classes;
            }
        }
    }
}

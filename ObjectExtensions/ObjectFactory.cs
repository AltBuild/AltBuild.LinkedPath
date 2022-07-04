using System;
using System.Collections.Concurrent;

namespace AltBuild.LinkedPath
{
    public static class ObjectFactory
    {
        static ConcurrentDictionary<Type, IObjectFactory[]> factoryList = new ();

        /// <summary>
        /// Create the Object special member.
        /// </summary>
        /// <param name="obj">Member initialize target object.</param>
        /// <param name="rule">Rule</param>
        public static void Initialize(object obj, InductiveRule rule = InductiveRule.Writable)
        {
            // �����o�[�̏�����
            Initialize(new ObjectInitializeParameters { Object = obj, CreateEntities = true, Rule = rule });
        }

        /// <summary>
        /// �I�u�W�F�N�g�̃v���p�e�B������������
        /// </summary>
        /// <param name="parameters">Member initialize parameters.</param>
        public static void Initialize(ObjectInitializeParameters parameters)
        {
            // �C���^�[�t�F�[�X�C���X�^���X���擾�i�����j
            IObjectFactory[] factories =
                factoryList.GetOrAdd(typeof(IObjectFactory), t => InterfaceExtensions.GetInterfaceInstance<IObjectFactory>());

            // All initialize
            foreach (var factory in factories)
                factory.InitializeObject(parameters);
        }
    }
}

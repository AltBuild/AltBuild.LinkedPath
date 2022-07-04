namespace AltBuild.LinkedPath.Parser
{
    /// <summary>
    /// �����N�p�X�����
    ///
    ///   �P�j�w�Z.�w��('xxxx','yyyy').�w��[Gakka.ID='xxxx']:readonly,class='text-primary'
    ///       ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ///       ��(A)LinkedName                                ��(B)                         �� ':' �ŋ�؂�iDLinkedPath:PathOption�j
    /// 
    ///       ~~~~ ~~~~~~~~~~~~~~~~~~~ ~~~~~~~~~~~~~~~~~~~~~                               �� '.' �ŋ�؂�iDLinkedPathCollection�Ń��X�g���j
    ///       ��(1)    ��(2)Method       ��(3)Property
    /// </summary>
    public interface IPathFrameInner
    {
        /// <summary>
        ///  First path
        /// </summary>
        PathMember First { get; }

        /// <summary>
        ///  Parsed attribute information
        /// </summary>
        PathAttributeCollection Attributes { get; }

        /// <summary>
        ///  Does the attribute information exist?
        /// </summary>
        bool AttributesIsValid { get; }

        /// <summary>
        ///  Set attributes.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        void SetAttributes(string attributes, string prefix = null, string suffix = null);

        void SetAttributes(ChunkPartAttributes attributes);
    }
}

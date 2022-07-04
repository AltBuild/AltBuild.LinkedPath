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
    public partial class PathFrame : IPathFrameInner
    {
        /// <summary>
        ///  First path
        /// </summary>
        public PathMember First { get; init; }

        /// <summary>
        ///  Constractor
        /// </summary>
        internal PathFrame() : base() { }

        /// <summary>
        ///  Parsed attribute information
        /// </summary>
        public PathAttributeCollection Attributes => _attributes ??= new PathAttributeCollection();
        PathAttributeCollection _attributes;

        /// <summary>
        ///  Does the attribute information exist?
        /// </summary>
        public bool AttributesIsValid => _attributes != null && _attributes.Count > 0;

        /// <summary>
        ///  Set attributes.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        void IPathFrameInner.SetAttributes(string attributes, string prefix, string suffix) =>
            _attributes = new PathAttributeCollection(attributes, prefix, suffix);

        void IPathFrameInner.SetAttributes(ChunkPartAttributes attributes) =>
            _attributes = new PathAttributeCollection(attributes);
    }
}

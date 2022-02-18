using AltBuild.BaseExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AltBuild.LinkedPath
{
    /// <summary>
    /// リンクパスを解析
    ///
    ///   １）学校.学級('xxxx','yyyy').学科[Gakka.ID='xxxx']:readonly,class='text-primary'
    ///       ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ///       ↑(A)LinkedName                                ↑(B)                         → ':' で区切る（DLinkedPath:PathOption）
    /// 
    ///       ~~~~ ~~~~~~~~~~~~~~~~~~~ ~~~~~~~~~~~~~~~~~~~~~                               → '.' で区切る（DLinkedPathCollectionでリスト化）
    ///       ↑(1)    ↑(2)Method       ↑(3)Property
    /// </summary>
    public partial class PathFrame : IPathFrameInner
    {
        /// <summary>
        ///  First path
        /// </summary>
        public PathMember First { get; init; }

        /// <summary>
        /// Control flags.
        /// </summary>
        public DiveControlFlags Flags { get; private set; }

        /// <summary>
        ///  Log
        /// </summary>
        public ILogBase Log
        {
            get
            {
                if (_log == null)
                    _log = new LogBase<LogItem>();

                return _log;
            }
        }
        ILogBase _log;

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
        /// Set flags.
        /// </summary>
        void IPathFrameInner.SetFlags(DiveControlFlags flags) => Flags = flags;

        /// <summary>
        ///  Set log.
        /// </summary>
        /// <param name="log"></param>
        void IPathFrameInner.SetLog(ILogBase log) => _log = log ?? new LogBase<LogItem>();

        /// <summary>
        ///  Set attributes.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        void IPathFrameInner.SetAttributes(string attributes, string prefix = null, string suffix = null) =>
            _attributes = new PathAttributeCollection(attributes, prefix, suffix);
    }
}

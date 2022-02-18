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
    public interface IPathFrameInner
    {
        /// <summary>
        ///  First path
        /// </summary>
        PathMember First { get; }

        /// <summary>
        /// Control flags.
        /// </summary>
        DiveControlFlags Flags { get; }

        /// <summary>
        ///  Log
        /// </summary>
        ILogBase Log { get; }

        /// <summary>
        ///  Set log.
        /// </summary>
        /// <param name="log"></param>
        void SetLog(ILogBase log);

        /// <summary>
        /// Set DiveControlFlags.
        /// </summary>
        /// <param name="controlFlags"></param>
        void SetFlags(DiveControlFlags controlFlags);

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
    }
}

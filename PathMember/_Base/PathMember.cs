using AltBuild.BaseExtensions;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AltBuild.LinkedPath
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
    public partial class PathMember
    {
        /// <summary>
        /// Object Member Name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Object Member Type.
        /// </summary>
        public MemberTypes Type { get; set; }

        /// <summary>
        /// Root Object.
        /// </summary>
        public PathFrame Frame { get; private set; }

        /// <summary>
        /// Object Sub member
        /// </summary>
        public PathMember SubMember { get; set; }

        /// <summary>
        /// Chain of parent.
        /// </summary>
        public PathMember Parent { get; private set; }

        /// <summary>
        /// Chain of child.
        /// </summary>
        public PathMember Child { get; private set; }

        /// <summary>
        /// Chain of Last.
        /// </summary>
        public PathMember Last => Child != null ? Child.Last : this;

        /// <summary>
        /// Chain of First.
        /// </summary>
        public PathMember First => Parent != null ? Parent.First : this;

        /// <summary>
        /// Depth Count.
        /// </summary>
        public int Depth
        {
            get
            {
                if (_depth == -1)
                {
                    if (Parent != null)
                        _depth = Parent.Depth + 1;
                    else
                        _depth = 0;
                }
                return _depth;
            }
        }
        int _depth = -1;

        /// <summary>
        /// Get static content
        /// </summary>
        /// <param name="content">content</param>
        /// <returns>true: success, false: failed</returns>
        public bool TryGetContent(out string content)
        {
            if (Type == MemberTypes.Custom)
            {
                content = Name;
                return true;
            }

            content = null;
            return false;
        }

        ///// <summary>
        ///// �p�X�\�����V���v���ł��邩�i�V���v���ɒl���擾�\�ȃp�X�ł��邩�H�j
        ///// �@�e���̂���I�v�V������
        ///// �@�� Indexers: �C���f�N�T
        ///// �@�� Arguments: ����
        ///// �@
        ///// </summary>
        //public bool IsSimple => (_indexerCollection == null && _argumentCollection == null);

        /// <summary>
        /// Get arguments
        /// </summary>
        public PathArgumentCollection Arguments
        {
            get => _argumentCollection ??= new PathArgumentCollection { Path = this };
            private set => _argumentCollection = value;
        }
        PathArgumentCollection _argumentCollection;

        /// <summary>
        /// Get indexers
        /// </summary>
        public PathIndexerCollection Indexers
        {
            get => _indexerCollection ??= new PathIndexerCollection { Path = this };
            private set => _indexerCollection = value;
        }
        PathIndexerCollection _indexerCollection;

        /// <summary>
        /// Get options
        /// </summary>
        public PathOptionCollection Options
        {
            get => _optionCollection ??= new PathOptionCollection { Path = this };
            private set => _optionCollection = value;
        }
        PathOptionCollection _optionCollection;

        /// <summary>
        /// �A�[�M�������g�i������j���Z�b�g
        /// </summary>
        /// <param name="line"></param>
        internal void SetArguments(string line) => Arguments = new PathArgumentCollection(line) { Path = this };

        /// <summary>
        /// �C���f�N�T�i������j���Z�b�g
        /// </summary>
        /// <param name="line"></param>
        internal void SetIndexers(string line) => Indexers = new PathIndexerCollection(line) { Path = this };

        internal void SetOptions(string line) => Options = new PathOptionCollection(line) { Path = this };

        internal PathMember(string name, PathFrame frame, PathMember parent, PathMember child)
        {
            // �t���[���𐶐����ێ�
            Frame = frame != null ? frame : new PathFrame { First = this };

            // �e��ێ�
            Parent = parent;

            // �q��ێ�
            Child = child;

            // �����o�[����ێ�
            string[] names = name == null ? new string[] { null } : name.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (names.Length >= 1)
            {
                // �����o�[����ێ�
                Name = names[0];

                // �����o�[�����X�^�e�B�b�N���Ɠ���Ȃ�A�X�^�e�B�b�N�^�O�Ƃ��ď���
                if (PathFactory.StaticMarkups.Find(t => t == names[0]) != null)
                    Type = MemberTypes.Custom;
            }

            // �T�u�����o�[���\��
            if (names.Length >= 2)
            {
                PathMember face = this;
                for (int i = 1; i < names.Length; i++)
                    face = face.SubMember = new PathMember(names[i]);
            }
        }

        internal PathMember()
        {
            Frame = new PathFrame();
        }

        internal PathMember(string name) : this()
        {
            Name = name;
        }

        public string FullName => (Child != null ? $"{Name}.{Child.FullName}" : Name);

        public void SetChild(PathMember child) => Child = child;

        public T GetAttribute<T>(object obj)
        {
            foreach (var memberInfo in obj.GetType().GetMember(Name))
                if (memberInfo.TryGetAttribute(out T attribute))
                    return attribute;

            return default(T);
        }

        public bool TryGetAttribute<T>(object obj, out T attribute)
        {
            attribute = GetAttribute<T>(obj);
            return attribute != null;
        }

        public MemberInfo GetMemberInfo(object source)
        {
            if (source != null && string.IsNullOrWhiteSpace(Name) == false)
            {
                var memberInfos = source.GetType().GetMember(Name, PathHelper.Flags);
                if (memberInfos.Length > 0)
                    return memberInfos[0];
            }

            return null;
        }

        /// <summary>
        /// �I�u�W�F�N�g���̃v���p�e�B�l
        /// �i�v���p�e�B�̂݃T�|�[�g�j
        /// </summary>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetPropertyValue<T>(object source, out T value)
        {
            if (string.IsNullOrWhiteSpace(Name) == false)
            {
                var propertyInfo = source.GetType().GetProperty(Name);
                if (propertyInfo != null && propertyInfo.GetValue(source) is T tValue)
                {
                    value = tValue;
                    return true;
                }
            }

            // ���s
            value = default;
            return false;
        }

        /// <summary>
        /// Create DObjectMember.
        /// </summary>
        /// <param name="source">Definition object</param>
        /// <param name="dObjectMember">Created DMemberInfo</param>
        /// <returns>Successful: true, Failed: false</returns>
        public bool TryGetValue<T>(object source, out T value)
        {
            var propertyInfo = source.GetType().GetProperty(Name, PathHelper.Flags);
            if (propertyInfo != null)
            {
                // Index ���l�������l���擾
                var propertyValue = propertyInfo.GetValue(source);
                if (propertyValue != null && Indexers.Index.HasValue)
                {
                    // �C���f�b�N�X����
                    if (propertyValue is IObjectIndexer objectIndexer)
                        propertyValue = objectIndexer[Indexers.Index.Value];
                }

                if (Child != null && string.IsNullOrWhiteSpace(Child.Name) == false)
                {
                    return Child.TryGetValue<T>(propertyValue, out value);
                }

                // �w��̃^�C�v�ւ̃L���X�g�����݂�
                else if (propertyValue is T atValue)
                {
                    value = atValue;
                    return true;
                }
            }

            value = default(T);
            return false;
        }

        /// <summary>
        /// Create DObjectMember.
        /// </summary>
        /// <param name="source">Definition object</param>
        /// <param name="dObjectMember">Created DMemberInfo</param>
        /// <returns>Successful: true, Failed: false</returns>
        public bool TrySetPropertyValue<T>(object source, T value)
        {
            var propertyInfo = source.GetType().GetProperty(Name, PathHelper.Flags);
            if (propertyInfo != null)
            {
                // Index ���l�������l���擾
                var propertyValue = propertyInfo.GetValue(source);
                if (propertyValue != null)
                {
                    if (Child != null && string.IsNullOrWhiteSpace(Child.Name) == false)
                    {
                        // �C���f�b�N�X����
                        if (Indexers.Index.HasValue && propertyValue is IObjectIndexer objectIndexer)
                            propertyValue = objectIndexer[Indexers.Index.Value];

                        return Child.TrySetPropertyValue<T>(propertyValue, value);
                    }
                    else
                    {
                        if (Indexers.Index.HasValue && propertyValue is IObjectIndexer objectIndexer)
                        {
                            objectIndexer[Indexers.Index.Value] = value;
                            return true;
                        }
                        else
                        {
                            return PathHelper.TrySetValue<T>(value, source, Name);
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Create DiveValue.
        /// </summary>
        /// <param name="source">source object</param>
        /// <returns>DiveValue</returns>
        public DiveValue FromDiveInner(object source, DiveControlFlags flags)
        {
            ((IPathFrameInner)Frame).SetFlags(flags);
            return FromDiveInner(source);
        }

        /// <summary>
        /// Create DiveValue.
        /// </summary>
        /// <param name="source">source object</param>
        /// <returns>DiveValue</returns>
        public DiveValue FromDiveInner(object source)
        {
            object resultValue = null;

            if (source == null)
                Frame.Log.Add(new LogItem { Level = LogLevel.Warning, Name = "FromDiveInner", Message = "source is null." });

            if (Name == null)
                Frame.Log.Add(new LogItem { Level = LogLevel.Error, Name = "FromDiveInner", Message = "Name is null." });

            // Escalation to parent object.
            if (Name == ".")
            {
                if (Child != null && source is IObjectBase iObjectBase && iObjectBase.DefinitionObject != null)
                {
                    resultValue = iObjectBase.DefinitionObject;
                }
            }

            // Get member value.
            else if (PathHelper.TryGetPathValue(out DiveValue pathValue, source, Name, this))
            {
                Type = pathValue.MemberInfo.MemberType;
                resultValue = pathValue.Value;
            }

            // other error.
            else
            {
                Frame.Log.Add(new LogItem { Level = LogLevel.Error, Name = "FromDiveInner", Message = $"Name[{Name}] is not found." });
            }

            return new DiveValue { Source = source, PathMember = this, Value = resultValue };
        }

        /// <summary>
        /// �t���p�X���i�I�v�V�����������j���擾
        /// </summary>
        /// <returns></returns>
        public string ToStringWithoutOptions() => _toStringWithoutOptions ??= _createString(true);
        string _toStringWithoutOptions;

        /// <summary>
        /// �t���p�X�����擾����
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _toString ??= _createString();
        string _toString;

        string _createString(bool isDisableOptions = false)
        {
            // ����
            var bild = new StringBuilder();

            // �����Ώ�
            var atMember = this;

            // �q���𑖍�
            for (; ; )
            {
                // �p�X���̒ǉ�
                atMember.ToStringBuilder(bild, isDisableOptions);

                // �p��
                if ((atMember = atMember.Child) == null)
                    break;

                // �p�����鎞�� '.' ��ǉ�
                bild.Append('.');
            }

            // �������擾
            if (isDisableOptions == false)
                bild.Append(Frame.Attributes.ToString());

            // ����
            return bild.ToString();
        }

        public StringBuilder ToStringBuilder(StringBuilder builder = null, bool isDisableOptions = false)
        {
            if (builder == null)
                builder = new StringBuilder();

            // Name
            builder.Append(Name);

            // Arguments
            if (_argumentCollection != null && _argumentCollection.IsEmpty == false)
                builder.Append(_argumentCollection.ToString());

            // Indexers
            if (_indexerCollection != null && _indexerCollection.IsEmpty == false)
                builder.Append(_indexerCollection.ToString());

            // Options
            if (isDisableOptions == false)
                if (_optionCollection != null && _optionCollection.IsEmpty == false)
                    builder.Append(_optionCollection.ToString());

            return builder;
        }

        public static explicit operator PathElement(PathMember pathMember) =>
            new PathElement(PathElementType.Value, pathMember);
    }
}

namespace System.Instant
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Series;
    using System.Uniques;

    public class RubricBuilder : IUnique
    {
        public RubricBuilder(MemberRubric member)
        {
            SetMember(member);
        }

        public MemberInfo SetMember(MemberRubric member)
        {
            if (member.MemberType == MemberTypes.Field)
            {
                SetField((FieldRubric)member.RubricInfo);
                Member ??= member;
                Member.FigureField = member.FigureField;
            }
            else if (member.MemberType == MemberTypes.Property)
            {
                SetProperty((PropertyRubric)member.RubricInfo);
                Member = member;
            }
            return null;
        }

        public PropertyRubric SetProperty(PropertyRubric property)
        {
            Property = property;
            Name ??= property.RubricName;
            Type = property.PropertyType;
            Getter = property.GetGetMethod();
            Setter = property.GetSetMethod();
            return property;
        }

        public FieldRubric SetField(FieldRubric field)
        {
            Field = field;
            Type ??= field.FieldType;
            FieldType = field.FieldType;
            FieldName = field.FieldName;
            Name ??= field.RubricName;
            return field;
        }

        public string Name { get; set; }
        public string FieldName { get; set; }
        public Type Type { get; set; }
        public MemberRubric Member { get; set; }
        public PropertyRubric Property { get; set; }
        public FieldRubric Field { get; set; }
        public MethodInfo Getter { get; set; }
        public MethodInfo Setter { get; set; }
        public Type FieldType { get; set; }

        public IUnique Empty => throw new NotImplementedException();
        public ulong UniqueKey
        {
            get => Name.UniqueKey64();
            set => throw new NotImplementedException();
        }
        public ulong UniqueType
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public int CompareTo(IUnique other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IUnique other)
        {
            throw new NotImplementedException();
        }

        public byte[] GetBytes()
        {
            throw new NotImplementedException();
        }

        public byte[] GetUniqueBytes()
        {
            throw new NotImplementedException();
        }
    }

    public class InstantBuilder
    {
        public IDeck<RubricBuilder> CreateBuilders(Type modelType)
        {
            Catalog<RubricBuilder> rubricBuilders = new Catalog<RubricBuilder>();

            var memberRubrics = PrepareMembers(GetMembers(modelType));

            return CreateBuilders(memberRubrics);
        }

        public IDeck<RubricBuilder> CreateBuilders(MemberRubric[] memberRubrics)
        {
            Catalog<RubricBuilder> rubricBuilders = new Catalog<RubricBuilder>();

            memberRubrics.ForEach(
                (member) =>
                {
                    if (!rubricBuilders.TryGet(member.RubricName, out RubricBuilder fieldProperty))
                        rubricBuilders.Put(new RubricBuilder(member));
                    else
                        fieldProperty.SetMember(member);
                }
            );
            int order = 0;
            rubricBuilders.ForEach(
                (fp) =>
                {
                    order++;
                    fp.Member.RubricId = order;
                    fp.Member.FieldId = order;
                }
            );

            return rubricBuilders;
        }

        public MemberRubric[] PrepareMembers(IEnumerable<MemberInfo> membersInfo)
        {
            return membersInfo
                .Select(
                    m =>
                        !(m is MemberRubric rubric)
                            ? m.MemberType == MemberTypes.Field
                                ? new MemberRubric((FieldInfo)m)
                                : m.MemberType == MemberTypes.Property
                                    ? new MemberRubric((PropertyInfo)m)
                                    : null
                            : rubric
                )
                .Where(p => p != null)
                .ToArray();
        }

        public MemberInfo[] GetMembers(Type modelType)
        {
            return modelType
                .GetMembers(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(
                    m =>
                        m.Name != "Item"
                        && m.Name != "ValueArray"
                        && (
                            m.MemberType == MemberTypes.Field
                            || (
                                m.MemberType == MemberTypes.Property
                                && ((PropertyInfo)m).CanRead
                                && ((PropertyInfo)m).CanWrite
                            )
                        )
                )
                .ToArray();
        }
    }
}

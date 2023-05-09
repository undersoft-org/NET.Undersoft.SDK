namespace System.Instant
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Uniques;

    public class FiguresCompiler
    {
        private readonly ConstructorInfo marshalAsCtor = typeof(MarshalAsAttribute).GetConstructor(
            new Type[] { typeof(UnmanagedType) }
        );
        private FieldBuilder count;
        private Type DeckType = typeof(FigureAlbum);
        private Figures figures;
        private FieldBuilder rubricsField;
        private FieldBuilder serialCodeField;
        private FieldBuilder structSizeField;
        private FieldBuilder structTypeField;
        private FieldBuilder tableField;

        public FiguresCompiler(Figures instantFigures, bool safeThread)
        {
            figures = instantFigures;
            if (safeThread)
                DeckType = typeof(FigureCatalog);
            figures.BaseType = DeckType;
        }

        public Type CompileFigureType(string typeName)
        {
            TypeBuilder tb = GetTypeBuilder(typeName);

            CreateSerialCodeProperty(tb, typeof(Uscn), "UniqueCode");

            CreateUniqueKeyProperty(tb);

            CreateUniqueSeedProperty(tb);

            CreateIsPrimeField(tb, typeof(bool), "Prime");

            CreateRubricsField(tb, typeof(MemberRubrics), "Rubrics");

            CreateKeyRubricsField(tb, typeof(MemberRubrics), "KeyRubrics");

            CreateFigureTypeField(tb, typeof(Type), "FigureType");

            CreateFigureSizeField(tb, typeof(int), "FigureSize");

            CreateNewFigureObject(tb, "NewFigure");

            CreateNewFigureObject(tb, "NewSleeve", typeof(ISleeve));

            CreateItemByIntProperty(tb);

            CreateItemByStringProperty(tb);

            return tb.CreateTypeInfo();
        }

        public void CreateUniqueKeyProperty(TypeBuilder tb)
        {
            PropertyBuilder prop = tb.DefineProperty(
                "UniqueKey",
                PropertyAttributes.HasDefault,
                typeof(ulong),
                new Type[] { typeof(ulong) }
            );

            PropertyInfo iprop = DeckType.GetProperty("UniqueKey");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(
                accessor.Name,
                accessor.Attributes & ~MethodAttributes.Abstract,
                accessor.CallingConvention,
                accessor.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldflda, serialCodeField);
            il.EmitCall(OpCodes.Call, typeof(Uscn).GetProperty("UniqueKey").GetGetMethod(), null);
            il.Emit(OpCodes.Ret);

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(
                mutator.Name,
                mutator.Attributes & ~MethodAttributes.Abstract,
                mutator.CallingConvention,
                mutator.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(setter, mutator);

            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldflda, serialCodeField);
            il.Emit(OpCodes.Ldarg_1);
            il.EmitCall(OpCodes.Call, typeof(Uscn).GetProperty("UniqueKey").GetSetMethod(), null);
            il.Emit(OpCodes.Ret);
        }

        public void CreateUniqueSeedProperty(TypeBuilder tb)
        {
            PropertyBuilder prop = tb.DefineProperty(
                "UniqueType",
                PropertyAttributes.HasDefault,
                typeof(ulong),
                new Type[] { typeof(ulong) }
            );

            PropertyInfo iprop = DeckType.GetProperty("UniqueType");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(
                accessor.Name,
                accessor.Attributes & ~MethodAttributes.Abstract,
                accessor.CallingConvention,
                accessor.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldflda, serialCodeField);
            il.EmitCall(OpCodes.Call, typeof(Uscn).GetProperty("UniqueType").GetGetMethod(), null);
            il.Emit(OpCodes.Ret);

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(
                mutator.Name,
                mutator.Attributes & ~MethodAttributes.Abstract,
                mutator.CallingConvention,
                mutator.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(setter, mutator);

            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldflda, serialCodeField);
            il.Emit(OpCodes.Ldarg_1);
            il.EmitCall(OpCodes.Call, typeof(Uscn).GetProperty("UniqueType").GetSetMethod(), null);
            il.Emit(OpCodes.Ret);
        }

        private PropertyBuilder CreateArrayCountField(TypeBuilder tb)
        {
            count = tb.DefineField(
                "_" + "count",
                typeof(IFigure).MakeArrayType(),
                FieldAttributes.Public
            );
            PropertyBuilder prop = tb.DefineProperty(
                "Count",
                PropertyAttributes.HasDefault,
                typeof(int),
                Type.EmptyTypes
            );

            PropertyInfo iprop = DeckType.GetProperty("Count");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(
                accessor.Name,
                accessor.Attributes & ~MethodAttributes.Abstract,
                accessor.CallingConvention,
                accessor.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, count);
            il.Emit(OpCodes.Ret);

            return prop;
        }

        private PropertyBuilder CreateArrayLengthField(TypeBuilder tb)
        {
            PropertyBuilder prop = tb.DefineProperty(
                "Length",
                PropertyAttributes.HasDefault,
                typeof(int),
                Type.EmptyTypes
            );

            PropertyInfo iprop = DeckType.GetProperty("Length");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(
                accessor.Name,
                accessor.Attributes & ~MethodAttributes.Abstract,
                accessor.CallingConvention,
                accessor.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(
                OpCodes.Ldflda,
                typeof(FigureAlbum).GetField(
                    "cards",
                    BindingFlags.NonPublic | BindingFlags.Instance
                )
            );
            il.Emit(OpCodes.Ldlen);
            il.Emit(OpCodes.Ret);

            return prop;
        }

        private void CreateDeckField(TypeBuilder tb, Type type, string name)
        {
            FieldBuilder fb = CreateField(tb, type, name);
            tableField = fb;

            PropertyBuilder prop = tb.DefineProperty(
                name,
                PropertyAttributes.HasDefault,
                type,
                new Type[] { type }
            );

            PropertyInfo iprop = DeckType.GetProperty("Figures");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(
                accessor.Name,
                accessor.Attributes & ~MethodAttributes.Abstract,
                accessor.CallingConvention,
                accessor.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fb);
            il.Emit(OpCodes.Ret);

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(
                mutator.Name,
                mutator.Attributes & ~MethodAttributes.Abstract,
                mutator.CallingConvention,
                mutator.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(setter, mutator);

            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fb);
            il.Emit(OpCodes.Ret);
        }

        private void CreateDeckObject(TypeBuilder tb)
        {
            MethodInfo createArray = DeckType.GetMethod("NewDeck");

            ParameterInfo[] args = createArray.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder method = tb.DefineMethod(
                createArray.Name,
                createArray.Attributes & ~MethodAttributes.Abstract,
                createArray.CallingConvention,
                createArray.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(method, createArray);

            ILGenerator il = method.GetILGenerator();
            il.DeclareLocal(typeof(IFigure).MakeArrayType());

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Newarr, typeof(IFigure));
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Stfld, tableField);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
        }

        private void CreateDeckTypeField(TypeBuilder tb, Type type, string name)
        {
            FieldBuilder fb = CreateField(tb, type, name);
            structTypeField = fb;

            PropertyBuilder prop = tb.DefineProperty(
                name,
                PropertyAttributes.HasDefault,
                type,
                new Type[] { type }
            );

            PropertyInfo iprop = DeckType.GetProperty("Figures");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(
                accessor.Name,
                accessor.Attributes & ~MethodAttributes.Abstract,
                accessor.CallingConvention,
                accessor.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fb);
            il.Emit(OpCodes.Ret);

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(
                mutator.Name,
                mutator.Attributes & ~MethodAttributes.Abstract,
                mutator.CallingConvention,
                mutator.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(setter, mutator);

            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fb);
            il.Emit(OpCodes.Ret);
        }

        private void CreateElementByIntProperty(TypeBuilder tb)
        {
            PropertyInfo prop = typeof(IFigures).GetProperty("Item", new Type[] { typeof(int) });

            MethodInfo accessor = prop.GetGetMethod();
            if (accessor != null)
            {
                ParameterInfo[] args = accessor.GetParameters();
                Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                if (args.Length == 1 && argTypes[0] == typeof(int))
                {
                    MethodBuilder method = tb.DefineMethod(
                        accessor.Name,
                        accessor.Attributes & ~MethodAttributes.Abstract,
                        accessor.CallingConvention,
                        accessor.ReturnType,
                        argTypes
                    );
                    tb.DefineMethodOverride(method, accessor);
                    ILGenerator il = method.GetILGenerator();

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldarg_1);
                    il.EmitCall(
                        OpCodes.Callvirt,
                        DeckType.GetMethod("get_Item", new Type[] { typeof(int) }),
                        null
                    );
                    il.Emit(OpCodes.Ret);
                }
            }

            MethodInfo mutator = prop.GetSetMethod();
            if (mutator != null)
            {
                ParameterInfo[] args = mutator.GetParameters();
                Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                MethodBuilder method = tb.DefineMethod(
                    mutator.Name,
                    mutator.Attributes & ~MethodAttributes.Abstract,
                    mutator.CallingConvention,
                    mutator.ReturnType,
                    argTypes
                );
                tb.DefineMethodOverride(method, mutator);
                ILGenerator il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_2);
                il.EmitCall(
                    OpCodes.Callvirt,
                    DeckType.GetMethod("set_Item", new Type[] { typeof(int), typeof(object) }),
                    null
                );
                il.Emit(OpCodes.Ret);
            }
        }

        private FieldBuilder CreateField(TypeBuilder tb, Type type, string name)
        {
            return tb.DefineField("_" + name, type, FieldAttributes.Public);
        }

        private void CreateFigureSizeField(TypeBuilder tb, Type type, string name)
        {
            FieldBuilder fb = CreateField(tb, type, name);
            structSizeField = fb;

            PropertyBuilder prop = tb.DefineProperty(
                name,
                PropertyAttributes.HasDefault,
                type,
                new Type[] { type }
            );

            PropertyInfo iprop = DeckType.GetProperty("FigureSize");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(
                accessor.Name,
                accessor.Attributes & ~MethodAttributes.Abstract,
                accessor.CallingConvention,
                accessor.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fb);
            il.Emit(OpCodes.Ret);

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(
                mutator.Name,
                mutator.Attributes & ~MethodAttributes.Abstract,
                mutator.CallingConvention,
                mutator.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(setter, mutator);

            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fb);
            il.Emit(OpCodes.Ret);
        }

        private void CreateFigureTypeField(TypeBuilder tb, Type type, string name)
        {
            FieldBuilder fb = CreateField(tb, type, name);
            structTypeField = fb;

            PropertyBuilder prop = tb.DefineProperty(
                name,
                PropertyAttributes.HasDefault,
                type,
                new Type[] { type }
            );

            PropertyInfo iprop = DeckType.GetProperty("FigureType");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(
                accessor.Name,
                accessor.Attributes & ~MethodAttributes.Abstract,
                accessor.CallingConvention,
                accessor.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fb);
            il.Emit(OpCodes.Ret);

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(
                mutator.Name,
                mutator.Attributes & ~MethodAttributes.Abstract,
                mutator.CallingConvention,
                mutator.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(setter, mutator);

            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fb);
            il.Emit(OpCodes.Ret);
        }

        private void CreateIsPrimeField(TypeBuilder tb, Type type, string name)
        {
            FieldBuilder fb = CreateField(tb, type, name);
            structSizeField = fb;

            PropertyBuilder prop = tb.DefineProperty(
                name,
                PropertyAttributes.HasDefault,
                type,
                new Type[] { type }
            );

            PropertyInfo iprop = DeckType.GetProperty("Prime");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(
                accessor.Name,
                accessor.Attributes & ~MethodAttributes.Abstract,
                accessor.CallingConvention,
                accessor.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fb);
            il.Emit(OpCodes.Ret);

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(
                mutator.Name,
                mutator.Attributes & ~MethodAttributes.Abstract,
                mutator.CallingConvention,
                mutator.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(setter, mutator);

            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fb);
            il.Emit(OpCodes.Ret);
        }

        private void CreateItemByIntProperty(TypeBuilder tb)
        {
            PropertyInfo prop = DeckType.GetProperty(
                "Item",
                new Type[] { typeof(int), typeof(int) }
            );

            MethodInfo accessor = prop.GetGetMethod();
            if (accessor != null)
            {
                ParameterInfo[] args = accessor.GetParameters();
                Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                MethodBuilder method = tb.DefineMethod(
                    accessor.Name,
                    accessor.Attributes & ~MethodAttributes.Abstract,
                    accessor.CallingConvention,
                    accessor.ReturnType,
                    argTypes
                );
                tb.DefineMethodOverride(method, accessor);
                ILGenerator il = method.GetILGenerator();
                il.DeclareLocal(typeof(IFigure).MakeArrayType());

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.EmitCall(
                    OpCodes.Callvirt,
                    DeckType.GetMethod("get_Item", new Type[] { typeof(int) }),
                    null
                );
                il.Emit(OpCodes.Ldarg_2);
                il.EmitCall(
                    OpCodes.Callvirt,
                    typeof(IFigure).GetMethod("get_Item", new Type[] { typeof(int) }),
                    null
                );
                il.Emit(OpCodes.Ret);
            }

            MethodInfo mutator = prop.GetSetMethod();
            if (mutator != null)
            {
                ParameterInfo[] args = mutator.GetParameters();
                Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                MethodBuilder method = tb.DefineMethod(
                    mutator.Name,
                    mutator.Attributes & ~MethodAttributes.Abstract,
                    mutator.CallingConvention,
                    mutator.ReturnType,
                    argTypes
                );
                tb.DefineMethodOverride(method, mutator);
                ILGenerator il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.EmitCall(
                    OpCodes.Callvirt,
                    DeckType.GetMethod("get_Item", new Type[] { typeof(int) }),
                    null
                );
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldarg_3);
                il.EmitCall(
                    OpCodes.Callvirt,
                    typeof(IFigure).GetMethod(
                        "set_Item",
                        new Type[] { typeof(int), typeof(object) }
                    ),
                    null
                );
                il.Emit(OpCodes.Ret);
            }
        }

        private void CreateItemByStringProperty(TypeBuilder tb)
        {
            PropertyInfo prop = DeckType.GetProperty(
                "Item",
                new Type[] { typeof(int), typeof(string) }
            );

            MethodInfo accessor = prop.GetGetMethod();
            if (accessor != null)
            {
                ParameterInfo[] args = accessor.GetParameters();
                Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                MethodBuilder method = tb.DefineMethod(
                    accessor.Name,
                    accessor.Attributes & ~MethodAttributes.Abstract,
                    accessor.CallingConvention,
                    accessor.ReturnType,
                    argTypes
                );
                tb.DefineMethodOverride(method, accessor);
                ILGenerator il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.EmitCall(
                    OpCodes.Callvirt,
                    DeckType.GetMethod("get_Item", new Type[] { typeof(int) }),
                    null
                );
                il.Emit(OpCodes.Ldarg_2);
                il.EmitCall(
                    OpCodes.Callvirt,
                    typeof(IFigure).GetMethod("get_Item", new Type[] { typeof(string) }),
                    null
                );
                il.Emit(OpCodes.Ret);
            }

            MethodInfo mutator = prop.GetSetMethod();
            if (mutator != null)
            {
                ParameterInfo[] args = mutator.GetParameters();
                Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

                MethodBuilder method = tb.DefineMethod(
                    mutator.Name,
                    mutator.Attributes & ~MethodAttributes.Abstract,
                    mutator.CallingConvention,
                    mutator.ReturnType,
                    argTypes
                );
                tb.DefineMethodOverride(method, mutator);
                ILGenerator il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.EmitCall(
                    OpCodes.Callvirt,
                    DeckType.GetMethod("get_Item", new Type[] { typeof(int) }),
                    null
                );
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldarg_3);
                il.EmitCall(
                    OpCodes.Callvirt,
                    typeof(IFigure).GetMethod(
                        "set_Item",
                        new Type[] { typeof(string), typeof(object) }
                    ),
                    null
                );
                il.Emit(OpCodes.Ret);
            }
        }

        private void CreateKeyRubricsField(TypeBuilder tb, Type type, string name)
        {
            FieldBuilder fb = CreateField(tb, type, name);
            rubricsField = fb;
            PropertyBuilder prop = tb.DefineProperty(
                name,
                PropertyAttributes.HasDefault,
                type,
                new Type[] { type }
            );

            PropertyInfo iprop = DeckType.GetProperty("KeyRubrics");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(
                accessor.Name,
                accessor.Attributes & ~MethodAttributes.Abstract,
                accessor.CallingConvention,
                accessor.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fb);
            il.Emit(OpCodes.Ret);

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(
                mutator.Name,
                mutator.Attributes & ~MethodAttributes.Abstract,
                mutator.CallingConvention,
                mutator.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(setter, mutator);

            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(
                OpCodes.Call,
                typeof(MemberRubrics).GetMethod("set_Figures", new Type[] { DeckType }),
                null
            );
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fb);
            il.Emit(OpCodes.Ret);
        }

        private void CreateMarshalAttribue(FieldBuilder field, MarshalAsAttribute attrib)
        {
            List<object> attribValues = new List<object>(1);
            List<FieldInfo> attribFields = new List<FieldInfo>(1);
            attribValues.Add(attrib.SizeConst);
            attribFields.Add(attrib.GetType().GetField("SizeConst"));
            field.SetCustomAttribute(
                new CustomAttributeBuilder(
                    marshalAsCtor,
                    new object[] { attrib.Value },
                    attribFields.ToArray(),
                    attribValues.ToArray()
                )
            );
        }

        private void CreateNewFigureObject(TypeBuilder tb, string name, Type castType = null)
        {
            MethodInfo createArray = DeckType.GetMethod(name);

            ParameterInfo[] args = createArray.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder method = tb.DefineMethod(
                createArray.Name,
                createArray.Attributes & ~MethodAttributes.Abstract,
                createArray.CallingConvention,
                createArray.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(method, createArray);

            ILGenerator il = method.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, structTypeField);
            il.EmitCall(
                OpCodes.Call,
                typeof(Activator).GetMethod("CreateInstance", new Type[] { typeof(Type) }),
                null
            );
            il.Emit(OpCodes.Castclass, castType ??= typeof(IFigure));
            il.Emit(OpCodes.Ret);
        }

        private void CreateNewObject(TypeBuilder tb)
        {
            MethodInfo createArray = DeckType.GetMethod("NewObject");

            ParameterInfo[] args = createArray.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder method = tb.DefineMethod(
                createArray.Name,
                createArray.Attributes & ~MethodAttributes.Abstract,
                createArray.CallingConvention,
                createArray.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(method, createArray);

            ILGenerator il = method.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, structTypeField);
            il.EmitCall(
                OpCodes.Call,
                typeof(Activator).GetMethod("CreateInstance", new Type[] { typeof(Type) }),
                null
            );
            il.Emit(OpCodes.Ret);
        }

        private PropertyBuilder CreateProperty(
            TypeBuilder tb,
            FieldBuilder field,
            Type type,
            string name
        )
        {
            PropertyBuilder prop = tb.DefineProperty(
                name,
                PropertyAttributes.HasDefault,
                type,
                new Type[] { type }
            );

            MethodBuilder getter = tb.DefineMethod(
                "get_" + name,
                MethodAttributes.Public | MethodAttributes.HideBySig,
                type,
                Type.EmptyTypes
            );
            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, field);
            il.Emit(OpCodes.Ret);

            MethodBuilder setter = tb.DefineMethod(
                "set_" + name,
                MethodAttributes.Public | MethodAttributes.HideBySig,
                typeof(void),
                new Type[] { type }
            );
            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, field);
            il.Emit(OpCodes.Ret);

            return prop;
        }

        private void CreateRubricsField(TypeBuilder tb, Type type, string name)
        {
            FieldBuilder fb = CreateField(tb, type, name);
            rubricsField = fb;
            PropertyBuilder prop = tb.DefineProperty(
                name,
                PropertyAttributes.HasDefault,
                type,
                new Type[] { type }
            );

            PropertyInfo iprop = DeckType.GetProperty("Rubrics");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(
                accessor.Name,
                accessor.Attributes & ~MethodAttributes.Abstract,
                accessor.CallingConvention,
                accessor.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fb);
            il.Emit(OpCodes.Ret);

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(
                mutator.Name,
                mutator.Attributes & ~MethodAttributes.Abstract,
                mutator.CallingConvention,
                mutator.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(setter, mutator);

            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Ldarg_0);
            il.EmitCall(
                OpCodes.Call,
                typeof(MemberRubrics).GetMethod("set_Figures", new Type[] { DeckType }),
                null
            );
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fb);
            il.Emit(OpCodes.Ret);
        }

        private void CreateSerialCodeProperty(TypeBuilder tb, Type type, string name)
        {
            FieldBuilder fb = CreateField(tb, type, name);
            serialCodeField = fb;
            PropertyBuilder prop = tb.DefineProperty(
                name,
                PropertyAttributes.HasDefault,
                type,
                new Type[] { type }
            );

            PropertyInfo iprop = DeckType.GetProperty("UniqueCode");

            MethodInfo accessor = iprop.GetGetMethod();

            ParameterInfo[] args = accessor.GetParameters();
            Type[] argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder getter = tb.DefineMethod(
                accessor.Name,
                accessor.Attributes & ~MethodAttributes.Abstract,
                accessor.CallingConvention,
                accessor.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(getter, accessor);

            prop.SetGetMethod(getter);
            ILGenerator il = getter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, fb);
            il.Emit(OpCodes.Ret);

            MethodInfo mutator = iprop.GetSetMethod();

            args = mutator.GetParameters();
            argTypes = Array.ConvertAll(args, a => a.ParameterType);

            MethodBuilder setter = tb.DefineMethod(
                mutator.Name,
                mutator.Attributes & ~MethodAttributes.Abstract,
                mutator.CallingConvention,
                mutator.ReturnType,
                argTypes
            );
            tb.DefineMethodOverride(setter, mutator);

            prop.SetSetMethod(setter);
            il = setter.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, fb);
            il.Emit(OpCodes.Ret);
        }

        private TypeBuilder GetTypeBuilder(string typeName)
        {
            string typeSignature = typeName;
            figures.Name = typeName;
            AssemblyName an = new AssemblyName(typeSignature);

            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                an,
                AssemblyBuilderAccess.RunAndCollect
            );
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(
                typeSignature + "Module"
            );
            TypeBuilder tb = null;

            tb = moduleBuilder.DefineType(
                typeSignature,
                TypeAttributes.Class
                    | TypeAttributes.Public
                    | TypeAttributes.Serializable
                    | TypeAttributes.AnsiClass
            );

            tb.SetCustomAttribute(
                new CustomAttributeBuilder(
                    typeof(DataContractAttribute).GetConstructor(Type.EmptyTypes),
                    new object[0]
                )
            );
            tb.SetParent(DeckType);
            return tb;
        }
    }
}

Imports System.Runtime.CompilerServices
Imports System.Reflection
Imports System.Linq.Expressions
Imports System.Reflection.Emit

Namespace Extension
    Public Module TypeHelper
        <Extension()> _
        Public Function GetNamespace(ByVal m_type As Type) As String
            Dim ns = m_type.Namespace
            Return IIf(ns IsNot Nothing, ns, String.Empty)
        End Function

        Private Function SetDelegate(Of T)(m As MethodInfo, type As Type) As Func(Of T, Object, Object)
            Dim param_obj As ParameterExpression = Expression.Parameter(GetType(T), "obj")
            Dim param_val As ParameterExpression = Expression.Parameter(GetType(Object), "val")
            Dim body_val As UnaryExpression = Expression.Convert(param_val, type)
            Dim body As MethodCallExpression = Expression.Call(param_obj, m, body_val)
            Dim setl As Action(Of T, Object) = Expression.Lambda(Of Action(Of T, Object))(body, param_obj, param_val).Compile()

            Return Function(instance, v)
                       setl(instance, v)
                       Return Nothing
                   End Function
        End Function

        <Extension()>
        Public Function GetEntity(Of T)(dt As DataTable) As List(Of T)
            Dim lst As New List(Of T)()
            Dim dic As New Dictionary(Of String, Func(Of T, Object, Object))()
            For Each dc As DataColumn In dt.Columns
                Dim pi As PropertyInfo = GetType(T).GetProperty(dc.ColumnName)
                If Not dic.ContainsKey(dc.ColumnName) Then
                    Dim fc As Func(Of T, Object, Object) = SetDelegate(Of T)(pi.GetSetMethod(), pi.PropertyType)
                    dic.Add(dc.ColumnName, fc)
                End If
            Next

            For Each dr As DataRow In dt.Rows
                Dim model As T = DirectCast(Activator.CreateInstance(GetType(T)), T)

                For Each dc As DataColumn In dt.Columns
                    Dim fc As Func(Of T, Object, Object) = dic(dc.ColumnName)
                    fc(model, dr(dc.ColumnName))
                Next
                lst.Add(model)
            Next

            Return lst
        End Function

        <Extension()>
        Public Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function

        Public Function CreateTypeBuilder(assemblyName As String, moduleName As String, typeName As String) As TypeBuilder
            Dim tb = AppDomain.CurrentDomain.DefineDynamicAssembly(New AssemblyName(assemblyName), AssemblyBuilderAccess.Run).DefineDynamicModule(moduleName).DefineType(typeName, TypeAttributes.Public)
            tb.DefineDefaultConstructor(MethodAttributes.Public)
            Return tb
        End Function

        <Extension>
        Public Sub CreateAutoImplementedProperty(builder As TypeBuilder, propertyName As String, propertyType As Type)
            Const PrivateFieldPrefix = "m_"
            Const GetterPrefix = "get_"
            Const SetterPrefix = "set_"
            Dim fieldBuilder = builder.DefineField(String.Concat(PrivateFieldPrefix, propertyName), propertyType, FieldAttributes.Private)
            Dim propertyBuilder = builder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, Nothing)
            Dim propertyMethodAttributes = MethodAttributes.Public Or MethodAttributes.SpecialName Or MethodAttributes.HideBySig
            Dim getterMethod = builder.DefineMethod(String.Concat(GetterPrefix, propertyName), propertyMethodAttributes, propertyType, Type.EmptyTypes)
            Dim getterILCode = getterMethod.GetILGenerator()
            getterILCode.Emit(OpCodes.Ldarg_0)
            getterILCode.Emit(OpCodes.Ldfld, fieldBuilder)
            getterILCode.Emit(OpCodes.Ret)
            Dim setterMethod = builder.DefineMethod(String.Concat(SetterPrefix, propertyName), propertyMethodAttributes, Nothing, New Type() {propertyType})
            Dim setterILCode = setterMethod.GetILGenerator()
            setterILCode.Emit(OpCodes.Ldarg_0)
            setterILCode.Emit(OpCodes.Ldarg_1)
            setterILCode.Emit(OpCodes.Stfld, fieldBuilder)
            setterILCode.Emit(OpCodes.Ret)
            propertyBuilder.SetGetMethod(getterMethod)
            propertyBuilder.SetSetMethod(setterMethod)
        End Sub

        <Extension>
        Public Function CreateMethodBuilder(builder As TypeBuilder, functionname As String, returnType As Type, paramTypes() As Type) As MethodBuilder
            Dim mMethodAttributes = MethodAttributes.Public Or MethodAttributes.SpecialName Or MethodAttributes.HideBySig
            Return builder.DefineMethod(functionname, mMethodAttributes, returnType, paramTypes)
        End Function

        Public Delegate Sub SetILCode(il As ILGenerator)

        <Extension>
        Public Sub SetMethod(builder As MethodBuilder, setil As SetILCode)
            Dim il = builder.GetILGenerator
            setil(il)
        End Sub

        <Extension>
        Public Function GetStaticFieldValue(Of T)(type As Type, fieldName As String) As T
            Dim fieldInfo = type.GetField(fieldName, BindingFlags.NonPublic Or BindingFlags.Static)
            If fieldInfo.IsStatic Then
                Return CType(fieldInfo.GetValue(type), T)
            End If
            Return Nothing
        End Function

        <Extension>
        Public Function IsBetween(Of T As {IComparable, IComparable(Of T)})(item As T, start As T, endt As T) As Boolean
            Return Comparer(Of T).Default.Compare(item, start) >= 0 AndAlso Comparer(Of T).Default.Compare(item, endt) <= 0
        End Function
    End Module
End Namespace

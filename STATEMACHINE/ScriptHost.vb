Option Explicit On 
Option Strict On

Imports System.Reflection
Imports System.IO
Imports System.Text
Imports System.Diagnostics
Imports MSScriptControl
Imports System.CodeDom
Imports System.CodeDom.Compiler
Imports Microsoft.CSharp
Imports Microsoft.VisualBasic
Imports Microsoft.JScript
Imports Microsoft.VJSharp


Namespace Diacom.APCStates

    Friend Structure ScriptErrorInfo
        Dim Description As String
        Dim Line As Integer
        Dim Column As Integer
        Dim Source As String
        Dim Text As String
        Dim CompilerError As Boolean
        Public Sub Clear()
            Description = ""
            Line = 0
            Column = 0
            Source = ""
            Text = ""
            CompilerError = False
        End Sub
    End Structure

    Friend MustInherit Class ScriptHost

        Public Enum EngineTypes
            NONE = 0
            VBSCRIPT
            JSCRIPT
            VBSCRIPT_NET
            JSCRIPT_NET
            CSHARP_NET
            JSHARP_NET
            COMPILED
        End Enum

        Protected m_Language As String
        Protected m_Type As EngineTypes
        Public [Error] As ScriptErrorInfo

        Public MustOverride Property ScriptSource() As String
        Public MustOverride Function Procedures() As String()
        Public MustOverride Function Functions() As String()
        Public MustOverride Function Modules() As String()
        Public MustOverride Sub AddModuleCode(ByVal Code As String)
        Public MustOverride Sub AddClassCode(ByVal Code As String)
        Public MustOverride Sub AddGlobalObject(ByVal Name As String, ByVal Type As String, ByVal Instance As System.Object, Optional ByVal GlobalVisibility As Boolean = True)
        Public MustOverride Sub AddEventObject(ByVal Name As String, ByVal Type As String, ByVal Instance As System.Object)
        Public MustOverride Sub AddReference(ByVal AssemblyName As String)
        Public MustOverride Sub Compile()
        Public MustOverride Sub Run()
        Public MustOverride Function Invoke(ByVal MethodName As String, Optional ByVal Arguments As Object() = Nothing) As Object
        Public Overridable Function GetMethodInfo(ByVal MethodName As String) As MethodInfo
            Return Nothing
        End Function
        Public Overridable Function GetInstanceInfo(ByVal MethodName As String) As Object
            Return Nothing
        End Function

        Public ReadOnly Property Language() As String
            Get
                Return m_Language
            End Get
        End Property
        Public ReadOnly Property EngineType() As EngineTypes
            Get
                Return m_Type
            End Get
        End Property

        Public Shared Function CreateInstance(ByVal Language As String) As ScriptHost
            Select Case Language.Trim.ToUpper
                Case "VB", "VISUAL BASIC", "JSCRIPT.NET", "VBSCRIPT.NET", "VB.NET", "J.NET", "J#.NET", "J#", "C#.NET", "CSHARP.NET", "C_SHARP.NET", "C#", "CSHARP", "C_SHARP"
                    Return New SrcScriptHost(Language)
                Case "JSCRIPT", "VBSCRIPT"
                    Return New MSActiveScriptHost(Language)
                Case Else
                    Return New CompiledScriptHost(Language)
            End Select
        End Function
    End Class

    Friend Class MSActiveScriptHost : Inherits ScriptHost
        Private myScriptControl As MSScriptControl.ScriptControl
        Private m_ScriptSource As String = String.Empty

        Public Sub New(ByVal Language As String)
            m_Type = EngineTypes.NONE
            Select Case Language.Trim.ToUpper
                Case "VBSCRIPT"
                    m_Type = EngineTypes.VBSCRIPT
                Case "JSCRIPT"
                    m_Type = EngineTypes.JSCRIPT
            End Select
            m_Language = Language
            myScriptControl = New MSScriptControl.ScriptControl
            myScriptControl.Language = ""
            myScriptControl.AllowUI = False
            myScriptControl.Language = Language
            CType(myScriptControl, MSScriptControl.IScriptControl).Timeout = ScriptControlConstants.NoTimeout
            myScriptControl.State = ScriptControlStates.Initialized
            [Error].Clear()
        End Sub

        Public Overrides Property ScriptSource() As String
            Get
                Return m_ScriptSource
            End Get
            Set(ByVal Value As String)
                m_ScriptSource &= Value
                '  Add the code to the script engine
                AddModuleCode(Value)
            End Set
        End Property

        ' Collection of Procedures in the engine
        Public Overrides Function Procedures() As String()
            Dim _procNames As New Collections.ArrayList
            Dim _Procedures As MSScriptControl.Procedures
            Dim _Procedure As MSScriptControl.Procedure

            Dim intProcCount, _NumberOfProcedures As Integer

            _Procedures = myScriptControl.Modules("GLOBAL").Procedures
            _NumberOfProcedures = _Procedures.Count()

            ' Return the ScriptControl Procedures
            For intProcCount = 1 To _NumberOfProcedures
                _Procedure = _Procedures(intProcCount)
                _procNames.Add(_Procedure.Name)
            Next
            Return CType(_procNames.ToArray(GetType(String)), String())
        End Function

        Public Overrides Function Functions() As String()
            Dim _procNames As New Collections.ArrayList
            Return CType(_procNames.ToArray(GetType(String)), String())
        End Function

        Public Overrides Function Modules() As String()
            Dim _modNames As New Collections.ArrayList
            Dim intModcount As Integer

            ' Return the ScriptControl Modules
            For intModcount = 1 To myScriptControl.Modules.Count()
                _modNames.Add(myScriptControl.Modules(intModcount).Name)
            Next
            Return CType(_modNames.ToArray(GetType(String)), String())
        End Function

        Public Overrides Sub AddGlobalObject(ByVal Name As String, ByVal Type As String, ByVal Instance As System.Object, Optional ByVal GlobalVisibility As Boolean = True)
            myScriptControl.AddObject(Name, Instance, GlobalVisibility)
        End Sub

        Public Overrides Sub AddEventObject(ByVal Name As String, ByVal Type As String, ByVal Instance As System.Object)
            myScriptControl.AddObject(Name, Instance, True)
        End Sub

        Public Overrides Sub AddModuleCode(ByVal Code As String)
            [Error].Clear()
            Try
                myScriptControl.Modules("GLOBAL").AddCode(Code)
            Catch
                [Error].Line = CType(myScriptControl, MSScriptControl.IScriptControl).Error.Line
                [Error].Description = CType(myScriptControl, MSScriptControl.IScriptControl).Error.Description
                [Error].Column = CType(myScriptControl, MSScriptControl.IScriptControl).Error.Column
                [Error].Text = CType(myScriptControl, MSScriptControl.IScriptControl).Error.Text
                [Error].Source = CType(myScriptControl, MSScriptControl.IScriptControl).Error.Source
                [Error].CompilerError = True
                Throw New System.ApplicationException(Language & " : Compilation Error")
            End Try
        End Sub

        Public Overrides Sub AddClassCode(ByVal Code As String)
            AddModuleCode(Code)
        End Sub

        Public Overrides Function Invoke(ByVal MethodName As String, Optional ByVal Arguments As Object() = Nothing) As Object
            ' Unfortunately COM Paramarrays are not supported in Beta 2
            ' so can't call methods in COM script when early bound :(
            ' Return myScriptControl.Run(MethodName, Nothing)
            ' Need to use late bound access to work around this for the moment
            Dim _ret As Object

            ' Create an arguments collection for the invokemember call
            Try
                ' Call the Run method on the script control - it's different for VB and JScript
                If (m_Type = EngineTypes.JSCRIPT) Then
                    Dim args() As Object = {Nothing}
                    If Not Arguments Is Nothing Then ' Check to see if there are any arguments specified
                        ReDim args(Arguments.Length - 1)
                        Arguments.CopyTo(args, 0)           ' We've got some so add them to the args array
                    End If
                    _ret = (myScriptControl.Modules("GLOBAL").Run(MethodName, args))
                Else
                    If Not Arguments Is Nothing Then ' Check to see if there are any arguments specified
                        _ret = myScriptControl.Modules("GLOBAL").Run(MethodName, Arguments)
                    Else
                        _ret = myScriptControl.Modules("GLOBAL").Run(MethodName)
                    End If
                End If
            Catch e As Exception
                DBGEX(e)
                DBG(Language & " : Cannot Invoke Method : " & MethodName)
                Throw New System.ApplicationException(Language & " : Cannot find Method : " & MethodName)
            End Try

            GC.KeepAlive(myScriptControl)
            Return _ret
        End Function

        Public Overrides Sub Run()
            ' ActiveScript doesn't have a notion of running so no need to do anything here
            If (myScriptControl.State = MSScriptControl.ScriptControlStates.Initialized) Then
                Me.Compile()
            End If
        End Sub

        Public Overrides Sub Compile()
            ' ActiveScript doesn't have a notion of compiling so no need to do anything here
            ' To make things a little more consistent between VSA and ActiveScript
            ' set the state of the engine to connected
            [Error].Clear()
            Try
                myScriptControl.State = MSScriptControl.ScriptControlStates.Connected
            Catch
                DBG(ScriptSource)
                [Error].Line = CType(myScriptControl, MSScriptControl.IScriptControl).Error.Line
                [Error].Description = CType(myScriptControl, MSScriptControl.IScriptControl).Error.Description
                [Error].Column = CType(myScriptControl, MSScriptControl.IScriptControl).Error.Column
                [Error].Text = CType(myScriptControl, MSScriptControl.IScriptControl).Error.Text
                [Error].Source = CType(myScriptControl, MSScriptControl.IScriptControl).Error.Source
                [Error].CompilerError = True
                Throw New System.ApplicationException(Language & " : Compilation Error")
            End Try
        End Sub

        Public Overrides Sub AddReference(ByVal AssemblyName As String)
        End Sub
    End Class


    Friend Class SrcScriptHost : Inherits ScriptHost
        Private m_Compiled As CompiledScriptHost
        Private myEngine As CodeDomProvider ' Engine to be used in the class
        Private cp As New CompilerParameters()
        Private m_ScriptSource As String = String.Empty

        Public Sub New(ByVal Language As String)
            Try
                m_Language = Language       ' Store the langauge
                m_Compiled = New CompiledScriptHost(Language)
                m_Type = EngineTypes.NONE
                Select Case Language.Trim.ToUpper
                    Case "VBSCRIPT.NET", "VB", "VISUAL BASIC", "VB.NET"
                        m_Type = EngineTypes.VBSCRIPT_NET
                        myEngine = New VBCodeProvider
                    Case "JSCRIPT.NET", "J.NET"
                        m_Type = EngineTypes.JSCRIPT_NET
                        myEngine = New JScriptCodeProvider
                    Case "C#.NET", "CSHARP.NET", "C_SHARP.NET", "C#", "CSHARP", "C_SHARP"
                        m_Type = EngineTypes.CSHARP_NET
                        myEngine = New CSharpCodeProvider
                    Case "J#.NET", "J#", "JSHARP", "J_SHARP", "JSHARP.NET", "J_SHARP.NET"
                        myEngine = New VJSharpCodeProvider
                        m_Type = EngineTypes.JSHARP_NET
                    Case Else
                        Throw New System.ApplicationException(Language & " : Unknown Engine")
                End Select
                cp.GenerateExecutable = False
                cp.GenerateInMemory = True
                cp.TreatWarningsAsErrors = False
                cp.WarningLevel = 3
                cp.IncludeDebugInformation = False

                [Error].Clear()
                If m_Type = ScriptHost.EngineTypes.VBSCRIPT_NET Then
                    cp.CompilerOptions = "/optionstrict- /optionexplicit-"
                End If
                AddReference("System.dll")  ' Create a reference to system
            Catch e As Exception
                DBGEX(e)
            End Try
        End Sub

        ' The script code
        Public Overrides Property ScriptSource() As String
            Get
                If m_Type = ScriptHost.EngineTypes.VBSCRIPT_NET Then
                End If
                Return m_ScriptSource
            End Get
            Set(ByVal Value As String)
                m_ScriptSource = Value
            End Set
        End Property

        ' Collection of Procedures in the engine
        Public Overrides Function Procedures() As String()
            Return m_Compiled.Procedures()
        End Function

        ' Collection of Functions in the engine
        Public Overrides Function Functions() As String()
            Return m_Compiled.Functions()
        End Function

        Public Overrides Function Modules() As String()
            Return m_Compiled.Modules()
        End Function

        Public Overrides Sub AddModuleCode(ByVal Code As String)
            Dim tempSrc As New StringBuilder(ScriptSource) ' temporary Src string 
            ' Need to get the source text to put the code inside the module/class
            If m_Type = ScriptHost.EngineTypes.VBSCRIPT_NET Then
                Dim _idx As Integer = tempSrc.ToString.ToUpper.LastIndexOf("END MODULE")
                If (_idx >= 0) Then
                    tempSrc.Insert(_idx, Code & vbCrLf)
                Else
                    tempSrc.Append(Code & vbCrLf)
                End If
            Else
            End If
            ScriptSource = tempSrc.ToString
        End Sub

        Public Overrides Sub AddClassCode(ByVal Code As String)
            ScriptSource &= Code & vbCrLf
        End Sub

        Public Overrides Sub AddGlobalObject(ByVal Name As String, ByVal Type As String, ByVal Instance As System.Object, Optional ByVal GlobalVisibility As Boolean = True)
            m_Compiled.AddGlobalObject(Name, Type, Instance, GlobalVisibility)
        End Sub

        Public Overrides Sub AddEventObject(ByVal Name As String, ByVal Type As String, ByVal Instance As System.Object)
            m_Compiled.AddEventObject(Name, Type, Instance)
        End Sub

        Public Overrides Sub AddReference(ByVal AssemblyName As String)
            Try
                If String.IsNullOrEmpty(AssemblyName) Then Exit Sub

                AssemblyName = AssemblyName.Trim.ToUpper
                If Not AssemblyName.EndsWith(".DLL") OrElse AssemblyName.EndsWith(".EXE") Then
                    AssemblyName = AssemblyName & ".DLL"
                End If
                ' Create a reference to the object
                ' Dim ExePath As String = System.IO.Path.GetDirectoryName(AssemblyName)
                ' If (Not ExePath Is Nothing) AndAlso ExePath.Equals(String.Empty) Then
                '                    AssemblyName = System.IO.Path.GetDirectoryName([Assembly].GetExecutingAssembly.Location()) & _
                '                       System.IO.Path.DirectorySeparatorChar & AssemblyName
                ' End If
                cp.ReferencedAssemblies.Add(AssemblyName)
            Catch e As Exception
                DBGEX(e)
                Throw New System.ApplicationException(Language & " : Couldn't add Assembly")
            End Try
        End Sub

        Public Overrides Sub Compile()
            Try
                [Error].Clear()
                Dim cr As CompilerResults = myEngine.CompileAssemblyFromSource(cp, m_ScriptSource)
                If (cr.Errors.HasErrors) Then
                    Dim _out As New StringBuilder
                    For Each err As CompilerError In cr.Errors
                        _out.Append(String.Format("On Line {0} in Col {1} Error ""{2}"" ", err.Line, err.Column, err.ErrorText))
                        _out.Append(vbCrLf)
                    Next
                    DBG(_out.ToString)
                    [Error].Line = cr.Errors(0).Line
                    [Error].Column = cr.Errors(0).Column
                    [Error].Text = cr.Errors(0).ErrorNumber
                    [Error].Description = cr.Errors(0).ErrorText
                    [Error].Source = cr.Errors(0).ToString
                    [Error].CompilerError = True
                Else
                    m_Compiled.AddAssembly(cr.CompiledAssembly)
                    DBG("File was compiled successfully :" + vbCrLf)
                    DBG(Me.ScriptSource)
                End If

            Catch e As Exception
                DBGEX(e)
                [Error].CompilerError = True
            Finally
                If ([Error].CompilerError) Then
                    Throw New System.ApplicationException(Language & " : Compilation Error")
                End If
            End Try
        End Sub

        Public Overrides Function Invoke(ByVal MethodName As String, Optional ByVal Arguments As Object() = Nothing) As Object
            Return m_Compiled.Invoke(MethodName, Arguments)
        End Function

        Public Overrides Sub Run()
            m_Compiled.Run()
        End Sub

        Public Overrides Function GetMethodInfo(ByVal MethodName As String) As MethodInfo
            Return m_Compiled.GetMethodInfo(MethodName)
        End Function

        Public Overrides Function GetInstanceInfo(ByVal MethodName As String) As Object
            Return m_Compiled.GetInstanceInfo(MethodName)
        End Function
    End Class


    '
    '
    ' Implementation of the compiled Script Host
    '
    Friend Class CompiledScriptHost : Inherits ScriptHost
        Private CompiledModules As New Collections.ArrayList
        Private InitializedClasses As New Collections.Hashtable
        Private m_GlobalInstances As New Collections.ArrayList

        Public Sub New(ByVal Language As String)
            m_Type = EngineTypes.COMPILED
        End Sub

        Public Overrides Property ScriptSource() As String
            Get
                Return ""
            End Get
            Set(ByVal Value As String)
            End Set
        End Property

        ' Collection of Procedures in the engine
        Public Overrides Function Procedures() As String()
            Dim _module As System.Reflection.[Module]
            Dim _procNames As New Collections.ArrayList
            Dim _method As MethodInfo
            Dim _type As Type

            Dim _APCCtlName As String = GetType(Diacom.APCLineControl).Name
            Dim _APCCtlNameSpace As String = GetType(Diacom.APCLineControl).Namespace

            For Each _asm As [Assembly] In CompiledModules
                For Each _module In _asm.GetModules()
                    For Each _type In _module.GetTypes()    ' Go thru every type
                        If (_type.IsClass AndAlso _type.IsPublic AndAlso _type.BaseType.Name.Equals(_APCCtlName) AndAlso _type.BaseType.Namespace.Equals(_APCCtlNameSpace)) Then
                            For Each _method In _type.GetMethods(BindingFlags.DeclaredOnly Or BindingFlags.IgnoreCase Or BindingFlags.Public Or BindingFlags.Static Or BindingFlags.Instance)
                                If (_method.GetParameters().Length = 0) Then    ' Takes no parameters
                                    _procNames.Add(_type.Name & "." & _method.Name)
                                End If
                            Next _method
                        End If
                    Next _type
                Next _module
            Next _asm
            Return CType(_procNames.ToArray(GetType(String)), String())
        End Function

        ' Collection of Functions in the engine
        Public Overrides Function Functions() As String()
            Dim _module As System.Reflection.[Module]
            Dim _procNames As New Collections.ArrayList
            Dim _method As MethodInfo
            Dim _type As Type
            Dim _APCLineName As String = GetType(Diacom.APCLine).Name
            Dim _APCLineNameSpace As String = GetType(Diacom.APCLine).Namespace

            For Each _asm As [Assembly] In CompiledModules
                For Each _module In _asm.GetModules()
                    For Each _type In _module.GetTypes()    ' Go thru every type
                        If (_type.IsClass AndAlso _type.IsPublic) Then   ' Should be a class 
                            For Each _method In _type.GetMethods(BindingFlags.DeclaredOnly Or BindingFlags.IgnoreCase Or BindingFlags.Public Or BindingFlags.Static Or BindingFlags.Instance)
                                ' Check the number of return arguments, the type of the arguments
                                If (_method.GetParameters().Length >= 1 AndAlso _
                                        _method.GetParameters(0).ParameterType.Name.Equals(_APCLineName) AndAlso _method.GetParameters(0).ParameterType.Namespace.Equals(_APCLineNameSpace)) Then     ' Takes 1 or more parameters
                                    _procNames.Add(_type.Name & "." & _method.Name)
                                End If
                            Next _method
                        End If
                    Next _type
                Next _module
            Next _asm
            Return CType(_procNames.ToArray(GetType(String)), String())
        End Function

        Public Overrides Function Modules() As String()
            Dim _module As System.Reflection.[Module]
            Dim _modNames As New Collections.ArrayList

            For Each _asm As [Assembly] In CompiledModules
                For Each _module In _asm.GetModules()
                    _modNames.Add(_module.ScopeName)
                Next _module
            Next _asm
            Return CType(_modNames.ToArray(GetType(String)), String())
        End Function

        Public Overrides Sub AddGlobalObject(ByVal Name As String, ByVal Type As String, ByVal Instance As System.Object, Optional ByVal GlobalVisibility As Boolean = True)
            Try
                m_GlobalInstances.Add(Instance)
            Catch e As Exception
                DBG(Language & " : Couldn't add global object to the hashtable : " & Name)
            End Try
        End Sub

        Public Overrides Sub AddEventObject(ByVal Name As String, ByVal Type As String, ByVal Instance As System.Object)
        End Sub

        Public Overrides Sub AddModuleCode(ByVal Code As String)
            Try
                AddAssembly([Assembly].LoadFrom(Code))
            Catch e As Exception
                DBG(Language & " : Couldn't load the specified Compiled Script : " & Code)
            End Try
        End Sub

        Public Overrides Sub AddClassCode(ByVal Code As String)
            Try
                AddAssembly([Assembly].LoadFrom(Code))
            Catch e As Exception
                DBG(Language & " : Couldn't load the specified Compiled Script : " & Code)
            End Try
        End Sub

        Friend Sub AddAssembly(ByVal Code As [Assembly])
            CompiledModules.Add(Code)
        End Sub

        Public Overrides Function Invoke(ByVal MethodName As String, Optional ByVal Arguments As Object() = Nothing) As Object

            Dim _module As System.Reflection.[Module]
            Dim _method As MethodInfo
            Dim _type As Type
            Dim ClassName As String = Nothing

            Try
                MethodName = MethodName.Trim.ToUpper
                Dim _split() As String = MethodName.Split("."c)
                If (_split.Length > 1) Then
                    ClassName = _split(0)
                    MethodName = _split(1)
                End If
                For Each _asm As [Assembly] In CompiledModules
                    For Each _module In _asm.GetModules
                        For Each _type In _module.GetTypes()
                            If (_type.Name.Trim.ToUpper.Equals(ClassName)) Then
                                For Each _method In _type.GetMethods(BindingFlags.DeclaredOnly Or BindingFlags.IgnoreCase Or BindingFlags.Public Or BindingFlags.Static Or BindingFlags.Instance)
                                    If (_method.Name.Trim.ToUpper.Equals(MethodName)) Then
                                        If _method.IsStatic Then
                                            Return _method.Invoke(Nothing, Arguments)
                                        Else
                                            Return _method.Invoke(InitializedClasses(ClassName), Arguments)
                                        End If
                                    End If
                                Next _method
                            End If
                        Next _type
                    Next _module
                Next _asm
            Catch _e As Exception
                DBGEX(_e)
                DBG(Language & " : Error invoking Method : " & MethodName)
                Throw _e
            End Try
            Return Nothing
        End Function

        Public Overrides Sub Run()
            Dim _module As System.Reflection.[Module]
            Dim _field As FieldInfo
            Dim _type As Type
            Dim _globalObj As Object
            Try
                ' Now we have to initialize the pointers
                ' Go thru every module, if there are fields - check if they are of the type marked as 
                ' global - initialize them
                For Each _asm As [Assembly] In CompiledModules
                    For Each _module In _asm.GetModules
                        For Each _type In _module.GetTypes
                            If _type.IsClass AndAlso _type.IsPublic _
                                AndAlso _type.GetConstructors().Length > 0 _
                                    AndAlso _type.GetConstructors()(0).GetParameters.Length = 0 Then
                                Try
                                    Dim _createdObject As Object = Activator.CreateInstance(_type, True)
                                    InitializedClasses(_type.Name.Trim.ToUpper) = _createdObject
                                    For Each _globalObj In m_GlobalInstances
                                        For Each _field In _type.GetFields
                                            If (_field.IsPublic AndAlso _field.FieldType.Equals(_globalObj.GetType)) Then
                                                _field.SetValue(_createdObject, _globalObj)
                                            End If
                                        Next _field
                                    Next _globalObj
                                Catch ex As Exception
                                    DBGEX(ex)
                                End Try
                            End If
                        Next _type
                    Next _module
                Next _asm
            Catch _e As Exception
                DBGEX(_e)
                DBG(Language & " : Error on Running")
            End Try
        End Sub

        Public Overrides Sub Compile()
        End Sub

        Public Overrides Sub AddReference(ByVal AssemblyName As String)
        End Sub

        Public Overrides Function GetMethodInfo(ByVal MethodName As String) As MethodInfo
            Dim _module As System.Reflection.[Module]
            Dim _method As MethodInfo
            Dim _type As Type
            Dim ClassName As String = Nothing
            Try
                MethodName = MethodName.Trim.ToUpper
                Dim _split() As String = MethodName.Split("."c)
                If (_split.Length > 1) Then
                    ClassName = _split(0)
                    MethodName = _split(1)
                Else
                    ClassName = "GLOBAL"
                End If
                For Each _asm As [Assembly] In CompiledModules
                    For Each _module In _asm.GetModules
                        For Each _type In _module.GetTypes()
                            If ClassName.Equals(_type.Name.Trim.ToUpper) Then
                                For Each _method In _type.GetMethods(BindingFlags.DeclaredOnly Or BindingFlags.IgnoreCase Or BindingFlags.Public Or BindingFlags.Static Or BindingFlags.Instance)
                                    If (_method.Name.Trim.ToUpper.Equals(MethodName)) Then
                                        Return _method
                                    End If
                                Next _method
                            End If
                        Next _type
                    Next _module
                Next _asm
            Catch _e As Exception
                DBGEX(_e)
                DBG(Language & " : Cannot GetMethodInfo : " & MethodName)
            End Try
            Return Nothing
        End Function

        Public Overrides Function GetInstanceInfo(ByVal MethodName As String) As Object
            Dim ClassName As String = Nothing
            Try
                Dim _split() As String = MethodName.Trim.ToUpper.Split("."c)
                If (_split.Length > 1) Then
                    ClassName = _split(0)
                Else
                    ClassName = "GLOBAL"
                End If
                Return InitializedClasses(ClassName)
            Catch _e As Exception
                DBGEX(_e)
                DBG(Language & " : Cannot GetInstanceInfo : " & MethodName)
            End Try
            Return Nothing
        End Function
    End Class

End Namespace
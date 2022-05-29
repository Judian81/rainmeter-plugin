Imports System
Imports System.Runtime.InteropServices

Namespace Rainmeter
    Public Class API
        Private Shared m_Rm As IntPtr

        Public Shared Sub API(rm As IntPtr)
            m_Rm = rm
        End Sub

        Public Sub New(rm As IntPtr)
            m_Rm = rm
        End Sub

        Public Shared Widening Operator CType(ByVal rm As IntPtr) As API
            Return New Rainmeter.API(rm)
        End Operator

        <DllImport("Rainmeter.dll", CharSet:=CharSet.Unicode)>
        Public Shared Function RmReadString(rm As IntPtr, PluginOption As String, defValue As String, replaceMeasures As Boolean) As IntPtr
        End Function

        <DllImport("Rainmeter.dll", CharSet:=CharSet.Unicode)>
        Public Shared Function RmReadFormula(rm As IntPtr, PluginOption As String, defValue As Double) As Double
        End Function

        <DllImport("Rainmeter.dll", CharSet:=CharSet.Unicode)>
        Public Shared Function RmReplaceVariables(rm As IntPtr, str As String) As IntPtr
        End Function

        <DllImport("Rainmeter.dll", CharSet:=CharSet.Unicode)>
        Public Shared Function RmPathToAbsolute(rm As IntPtr, relativePath As String) As IntPtr
        End Function

        <DllImport("Rainmeter.dll", EntryPoint:="RmExecute", CharSet:=CharSet.Unicode)>
        Public Shared Sub Execute(skin As IntPtr, command As String)
        End Sub

        <DllImport("Rainmeter.dll")>
        Public Shared Function RmGet(rm As IntPtr, type As RmGetType) As IntPtr
        End Function

        <DllImport("Rainmeter.dll", CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.Cdecl)>
        Public Shared Function LSLog(type As Integer, unused As String, message As String) As Integer
        End Function

        <DllImport("Rainmeter.dll", CharSet:=CharSet.Unicode, CallingConvention:=CallingConvention.StdCall)>
        Public Shared Function RmLog(rm As IntPtr, type As LogType, message As String) As Integer
        End Function

        Enum RmGetType
            MeasureName = 0
            Skin = 1
            SettingsFile = 2
            SkinName = 3
            SkinWindowHandle = 4
        End Enum

        Enum LogType
            Errors = 1
            Warning = 2
            Notice = 3
            Debug = 4
        End Enum

        Public Shared Function ReadString(PluginOption As String, defValue As String, Optional replaceMeasures As Boolean = True) As String
            Return Marshal.PtrToStringUni(RmReadString(m_Rm, PluginOption, defValue, replaceMeasures))
        End Function

        Public Shared Function ReadPath(PluginOption As String, defValue As String) As String
            Return Marshal.PtrToStringUni(RmPathToAbsolute(m_Rm, ReadString(PluginOption, defValue)))
        End Function

        Public Shared Function ReadDouble(PluginOption As String, defValue As Double) As Double
            Return RmReadFormula(m_Rm, PluginOption, defValue)
        End Function

        Public Shared Function ReadInt(PluginOption As String, defValue As Integer) As Integer
            Return RmReadFormula(m_Rm, PluginOption, defValue)
        End Function

        Public Shared Function ReplaceVariables(str As String) As String
            Return Marshal.PtrToStringUni(RmReplaceVariables(m_Rm, str))
        End Function

        Public Shared Function GetMeasureName() As String
            Return Marshal.PtrToStringUni(RmGet(m_Rm, RmGetType.MeasureName))
        End Function

        Public Shared Function GetSkin() As IntPtr
            Return RmGet(m_Rm, RmGetType.Skin)
        End Function

        Public Shared Function GetSettingsFile() As String
            Return Marshal.PtrToStringUni(RmGet(IntPtr.Zero, RmGetType.SettingsFile))
        End Function

        Public Shared Function GetSkinName() As String
            Return Marshal.PtrToStringUni(RmGet(m_Rm, RmGetType.SkinName))
        End Function

        Public Shared Sub Execute(command As String)
            Execute(GetSkin(), command)
        End Sub

        Public Shared Function GetSkinWindow() As IntPtr
            Return RmGet(m_Rm, RmGetType.SkinWindowHandle)
        End Function

        Public Shared Sub Log(type As Integer, message As String)
            LSLog(type, 0, message)
        End Sub

        Public Shared Sub Log(rm As IntPtr, type As LogType, message As String)
            RmLog(rm, type, message)
        End Sub

        Public Shared Sub LogF(rm As IntPtr, type As LogType, format As String, args() As Object)
            RmLog(rm, type, String.Format(format, args))
        End Sub

        Public Shared Sub Log(type As LogType, message As String)
            RmLog(m_Rm, type, message)
        End Sub

        Public Shared Sub LogF(type As LogType, format As String, args() As Object)
            RmLog(m_Rm, type, String.Format(format, args))
        End Sub
    End Class

    <AttributeUsage(AttributeTargets.Method)>
    Public Class DllExport
        Inherits System.Attribute
        Public DllExport()
    End Class
End Namespace

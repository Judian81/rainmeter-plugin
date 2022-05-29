Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports PluginEmpty.Rainmeter

Namespace PluginEmpty
    Class Measure
        Public Shared Function Implicit(ByVal data As IntPtr) As Measure
            Return GCHandle.FromIntPtr(data).Target
        End Function
        Public buffer As IntPtr = IntPtr.Zero
    End Class

    Public Class Plugin
        <DllExport>
        Public Sub Initialize(ByRef data As IntPtr, rm As IntPtr)
            data = GCHandle.ToIntPtr(GCHandle.Alloc(New Measure()))
            'Rainmeter.API api = (Rainmeter.API)rm
        End Sub

        <DllExport>
        Public Shadows Sub Finalize(data As IntPtr)
            'Dim Measure As Measure = Measure(data)
            Dim Measure As Measure = Measure.Implicit(data)
            If (Measure.buffer <> IntPtr.Zero) Then
                Marshal.FreeHGlobal(Measure.buffer)
            End If
            GCHandle.FromIntPtr(data).Free()
        End Sub

        <DllExport>
        Public Sub Reload(data As IntPtr, rm As IntPtr, ByRef maxValue As Double)
            Dim Measure As Measure = Measure.Implicit(data)
        End Sub

        <DllExport>
        Public Function Update(data As IntPtr) As Double
            Dim Measure As Measure = Measure.Implicit(data)
            Return 0.0
        End Function
    End Class
End Namespace
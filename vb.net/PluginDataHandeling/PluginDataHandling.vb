'/*
'Copyright(C) 2017 Trevor Hamilton

'This program Is free software you can redistribute it And/Or
'modify it under the terms Of the GNU General Public License
'as published by the Free Software Foundation either version 2
'of the License, Or (at your option) any later version.

'This program Is distributed In the hope that it will be useful,
'but WITHOUT ANY WARRANTY without even the implied warranty Of
'MERCHANTABILITY Or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License For more details.

'You should have received a copy Of the GNU General Public License
'along with this program if Not, write to the Free Software
'Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
'*/

Imports System
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports PluginDataHandling.Rainmeter

' Overview: This example demonstrates Imports both the data argument To keep data across
' functions calls And also storing data in the Rainmeter.Data file for presistance across
' skin reloads And even measures. In this example we will make a counter that counts the
' number of updates that happens And saves them on Finalize to the Rainmeter.data
' Note: You should never rely On Update happening at the exact time you specify

' Sample skin

'[Rainmeter]
'Update = 1000
'DynamicWindowSize = 1
'BackgroundMode = 2
'SolidColor = 255,255,255

'[mCount]
'Measure = Plugin
'Plugin = DataHandling.dll
'StoreData = 1

'[TextCount]
'Meter = String
'MeasureName = mCount
'X = 0
'Y = 0R
'FontSize = 20
'Text =%1

'[mCountAt0]
'Measure = Plugin
'Plugin = DataHandling.dll
'StartingValue = 0

'[TextCountAt0]
'Meter = String
'MeasureName = mCountAt0
'X = 0
'Y = 0R
'FontSize = 20
'Text =%1

Namespace PluginDataHandling
    Public Class Measure
        Public Shared Widening Operator CType(data As IntPtr) As Measure
            Return GCHandle.FromIntPtr(data).Target
        End Operator
        Public Shared myCounter As Integer = 0
        Public Shared storeData As Boolean = False
    End Class

    Public Class Plugin
        <DllImport("kernel32.dll", CharSet:=CharSet.Unicode)>
        Public Shared Function GetPrivateProfileString(section As String, key As String, defaultValue As String, value() As Char, size As Integer, filePath As String) As Integer
        End Function

        <DllImport("kernel32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)>
        Public Shared Function WritePrivateProfileString(section As String, key As String, value As String, filePath As String) As Boolean
        End Function

        Public Shared PluginName As String = "PluginDataHandling"
        Public Shared KeyName As String = "StoredCount"
        Public Const MAXSIZE As Integer = 256

        <DllExport>
        Public Shared Sub Initialize(ByRef data As IntPtr, rm As IntPtr)
            data = GCHandle.ToIntPtr(GCHandle.Alloc(New Measure()))
        End Sub

        <DllExport>
        Public Shared Sub Reload(data As IntPtr, rm As IntPtr, ByRef maxValue As Double)
            Dim Measure As Measure = GCHandle.FromIntPtr(data).Target
            Dim api As New Rainmeter.API(rm)
            Dim startValue As Integer = API.ReadInt("StartingValue", -1)
            If (startValue = -1) Then
                Dim outString(MAXSIZE) As Char
                GetPrivateProfileString(PluginName, KeyName, "0", outString, MAXSIZE, API.GetSettingsFile())
                Try
                    Dim intString As String = outString
                    Measure.myCounter = Convert.ToInt32(intString)
                Catch
                    API.Log(API.LogType.Errors, "Error converting value stored in Rainmeter.data to integer")
                End Try
            Else
                Measure.myCounter = startValue
            End If
            If API.ReadInt("StoreData", 0) = 1 Then
                Measure.storeData = True
            Else
                Measure.storeData = False
            End If
        End Sub

        <DllExport>
        Public Shared Function Update(Data As IntPtr) As Double
            Dim Measure As Measure = GCHandle.FromIntPtr(Data).Target
            Return ++Measure.myCounter
        End Function

        <DllExport>
        Public Shared Shadows Sub Finalize(Data As IntPtr)
            Dim Measure As Measure = GCHandle.FromIntPtr(Data).Target
            Dim API As New Rainmeter.API(Data)
            If (Measure.storeData) Then
                WritePrivateProfileString(PluginName, KeyName, Measure.myCounter.ToString(), API.GetSettingsFile())
            End If
            GCHandle.FromIntPtr(Data).Free()
        End Sub
    End Class
End Namespace

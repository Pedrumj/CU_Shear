
''' <summary>
''' The values of a certain force type at a certain station. Due to the fact that there may be different
''' models and each model may have different laod cases, there will be multiple values. 
''' </summary>
''' <remarks></remarks>
Public Class ForceType


    Public Cases As List(Of Force_LoadCase)
    Friend Sub New()
        Cases = New List(Of Force_LoadCase)

    End Sub

    ''' <summary>
    ''' Finds the index of the input load case. If a match is not found -1 is returned
    ''' </summary>
    ''' <param name="__LoadCase"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetCaseIndex(ByVal __LoadCase As String) As Long
        For i As Integer = 0 To Cases.Count - 1
            If Cases.Item(i).LoadCase = __LoadCase Then
                Return i
            End If
        Next
        Return -1
    End Function

    ''' <summary>
    ''' Adds the input force to the list of forces
    ''' </summary>
    ''' <param name="__Value">The force value</param>
    ''' <param name="__Case">The force load case</param>
    ''' <param name="__Model">The force model</param>
    ''' <remarks></remarks>
    Friend Sub AddForce(ByVal __Value As Double, ByVal __Case As String, ByVal __Model As String)
        Dim _tmpForce As Force_LoadCase
        Dim _CaseIndex As Long

        _CaseIndex = GetCaseIndex(__Case)
        If _CaseIndex <> -1 Then
            Cases.Item(_CaseIndex).Add_Force(__Value, __Model)
        Else
            _tmpForce = New Force_LoadCase(__Case)
            Cases.Add(_tmpForce)
            Cases.Item(Cases.Count - 1).Add_Force(__Value, __Model)
        End If

    End Sub


End Class



''' <summary>
''' represents the forces at a station. Due to the fact that there may be different models and each
''' model will have different load cases, there will be multiple values for each force type
''' at a single station.
''' </summary>
''' <remarks></remarks>
Public Class Force
    'V2 shear
    Public V2 As ForceType
    'V3 shear
    Public V3 As ForceType
    'axial
    Public P As ForceType
    'station associated with force object
    Public Station As Double
    'the ID of the elemenet
    Public ElementID As Long

    Friend Sub New(ByVal __Station As String, ByVal __ElementID As Long)
        V2 = New ForceType()
        V3 = New ForceType()
        P = New ForceType()
        Station = __Station
        ElementID = __ElementID
    End Sub

    ''' <summary>
    ''' Add V2 to the force
    ''' </summary>
    ''' <param name="__Value">The value of V2</param>
    ''' <param name="__Model">The model assocaited with V2</param>
    ''' <param name="__LoadCase">The load case assocaited with V2</param>
    ''' <remarks></remarks>
    Friend Sub AddV2(ByVal __Value As Double, ByVal __Model As String, ByVal __LoadCase As String)
        V2.AddForce(__Value, __LoadCase, __Model)
    End Sub

    ''' <summary>
    ''' Add V3 to the force
    ''' </summary>
    ''' <param name="__Value">The value of V3</param>
    ''' <param name="__Model">The model assocaited with V3</param>
    ''' <param name="__LoadCase">The load case assocaited with V3</param>
    ''' <remarks></remarks>
    Friend Sub AddV3(ByVal __Value As Double, ByVal __Model As String, ByVal __LoadCase As String)
        V3.AddForce(__Value, __LoadCase, __Model)
    End Sub

    ''' <summary>
    ''' Add P to the force
    ''' </summary>
    ''' <param name="__Value">The value of P</param>
    ''' <param name="__Model">The model assocaited with P</param>
    ''' <param name="__LoadCase">The load case associated with P</param>
    ''' <remarks></remarks>
    Friend Sub AddP(ByVal __Value As Double, ByVal __Model As String, ByVal __LoadCase As String)
        P.AddForce(__Value, __LoadCase, __Model)
    End Sub
End Class

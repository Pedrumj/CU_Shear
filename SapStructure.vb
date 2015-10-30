''' <summary>
''' Represents the structure
''' </summary>
''' <remarks></remarks>
Public Class SapStructure
    'reference to the sap application
    Private SapObject As SAP2000v16.SapObject
    'reference to the sap model
    Private SapModel As SAP2000v16.cSapModel
    'list of joints along the cast unit
    Private lstJoints As List(Of Joint)
    'list of elements along the cast unit
    Private lstElements As List(Of Element)
    'X coordinate of a point along the cast unit
    Private X As Double
    'Y coordinate of a point along the cast unit
    Private Y As Double
    'Z coordinate of a point along the cast unit
    Private Z As Double
    'The element type, Beam or column
    Private Type As ELEMENT_TYPE
    'the element direction X or Y
    Private Direction As ELEMENT_DIRECTION
    'reference to the cast unit object
    Private CastUnitObj As CastUnit

    Public Enum ELEMENT_TYPE
        BEAM
        COLUMN
    End Enum
    Public Enum ELEMENT_DIRECTION
        X
        Y
        Z
    End Enum


    ''' <summary>
    ''' Class constructor
    ''' </summary>
    ''' <param name="__strPath">Path to the sap model</param>
    ''' <param name="__X">X coordinate of a point along the cast unit</param>
    ''' <param name="__Y">Y coordinate of a point along the cast unit</param>
    ''' <param name="__Z">Z coordinate of a point along the cast unit</param>
    ''' <param name="__type">Beam or column</param>
    ''' <param name="__dir">X or Y</param>
    ''' <remarks></remarks>
    Public Sub New(ByVal __strPath As String, ByVal __X As Double, ByVal __Y As Double, ByVal __Z As Double, _
                    ByVal __type As ELEMENT_TYPE, ByVal __dir As ELEMENT_DIRECTION)
        Dim ret As Long



        'create Sap2000 object
        SapObject = New SAP2000v16.SapObject

        'start Sap2000 application
        SapObject.ApplicationStart()

        'create SapModel object
        SapModel = SapObject.SapModel

        'initialize model
        ret = SapModel.InitializeNewModel

        ret = SapModel.SetPresentUnits(8)

        X = __X
        Y = __Y
        Z = __Z
        Type = __type
        Direction = __dir
        SetModelDetails(__strPath)
    End Sub

    ''' <summary>
    ''' Sets the forces associated with the cast unit for all load cases. The reason this was seperated from the constructor
    ''' is because there might be multiple models with identical structure that might need to be analyzed under different load
    ''' cases
    ''' </summary>
    ''' <param name="__strPath">The path of the sap model with the forces</param>
    ''' <remarks></remarks>
    Public Sub GetResults_LoadCases(ByVal __strPath As String)
        Dim _strName As String

        _strName = GetName(__strPath)
        OpenModel(__strPath)
        Element.SetResults_LoadCases(SapModel, _strName, lstElements)
        CastUnitObj.AddForces(lstElements, _strName)
    End Sub

    ''' <summary>
    ''' Returns the name of the sap file from its path
    ''' </summary>
    ''' <param name="__strPath">Sap file path</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetName(ByVal __strPath As String) As String
        Dim _strOuput As String
        Dim _lngIndex As Long
        _lngIndex = InStrRev(__strPath, "\")
        _strOuput = Strings.Right(__strPath, Strings.Len(__strPath) - _lngIndex)
        Return _strOuput
    End Function

    ''' <summary>
    ''' Sets the details of the cast unit. This includes all details except forces.
    ''' </summary>
    ''' <param name="__strPath"></param>
    ''' <remarks></remarks>
    Private Sub SetModelDetails(ByVal __strPath As String)
        OpenModel(__strPath)
        lstJoints = Joint.GetSpecJoints(SapModel, X, Y, Z, Type, Direction)
        lstElements = Element.GetEles(lstJoints, Type, Direction, SapModel)
        CastUnitObj = New CastUnit(lstElements)
    End Sub

    ''' <summary>
    ''' Opens the sap model
    ''' </summary>
    ''' <param name="__strPath">Path to sap model</param>
    ''' <remarks></remarks>
    Private Sub OpenModel(ByVal __strPath As String)

        Dim ret As Long
        'create new blank model
        ret = SapModel.File.OpenFile(__strPath)
    End Sub

    ''' <summary>
    ''' closes the sap application
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CloseApplication()
        SapObject.ApplicationExit(False)
        SapObject = Nothing
        SapModel = Nothing
    End Sub

    ''' <summary>
    ''' returns the list of forces along the cast unit
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Forces() As List(Of Force)
        Return CastUnitObj.Forces
    End Function

    ''' <summary>
    ''' returns the list of t2 dimensions along the cast unit. Each t2 is at a station
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function t2() As List(Of Double)
        Return CastUnitObj.t2
    End Function

    ''' <summary>
    ''' returns the list of t3 dimensions along the cast unit. Each t3 is at a station.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function t3() As List(Of Double)
        Return CastUnitObj.t3
    End Function
End Class

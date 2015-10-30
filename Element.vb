''' <summary>
''' Represents elements in the structure. Could be columns or beams
''' </summary>
''' <remarks></remarks>
Friend Class Element
    'First joint of the element
    Public Joint1 As Joint
    'second joint of the element
    Public Joint2 As Joint
    'element ID as in SAP
    Public ID As Long '
    'the direction of the element
    Public FDirection As SapStructure.ELEMENT_DIRECTION
    'each force object defines the forces at a station
    Public Forces As List(Of Force)
    'the t2 dimension of the element
    Public t2 As Double
    'the t3 dimension of the element
    Public t3 As Double

    ''' <summary>
    ''' class constructor
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        Forces = New List(Of Force)
    End Sub

    ''' <summary>
    ''' Determines the ID of the element associated with the input string. The input string
    ''' could be in the format "3-13". This function will parse the string and return "3"
    ''' </summary>
    ''' <param name="__strName"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetElId(ByVal __strName As String) As String
        Dim _strOutput As String
        If InStr(__strName, "-") <> 0 Then
            _strOutput = Left(__strName, InStr(__strName, "-") - 1)
        Else
            _strOutput = __strName
        End If
        Return _strOutput
    End Function

    ''' <summary>
    ''' Determines if the input frame is found in the input list of frames. IDs are compared
    ''' </summary>
    ''' <param name="__Frames">List of frames</param>
    ''' <param name="__frame">Frame to find in list</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function ChkFrame(ByRef __Frames As List(Of Element), ByRef __frame As Element)
        For i As Integer = 0 To __Frames.Count - 1
            If __Frames.Item(i).ID = __frame.ID Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' The function recieves as input a list of frames and a joint. It looks through all the connected
    ''' frames to the joint and tries to find a match in the list of frames. If a match is NOT found then that
    ''' frame is added to the list of frames. 
    ''' </summary>
    ''' <param name="__Joint"></param>
    ''' <param name="__frames"></param>
    ''' <remarks></remarks>
    Private Shared Sub AddJFrames(ByRef __Joint As Joint, ByRef __frames As List(Of Element))
        For i As Integer = 0 To __Joint.Connectivity.Count - 1
            If ChkFrame(__frames, __Joint.Connectivity.Item(i)) = False Then
                __frames.Add(__Joint.Connectivity.Item(i))
            End If
        Next
    End Sub


    ''' <summary>
    ''' The functions recieves as input a list of joints. These joints are assumed to be on the same
    ''' line. It will return a list of elements that is on the same line as the joints. 
    ''' </summary>
    ''' <param name="__Joints"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetEleList(ByRef __Joints As List(Of Joint)) As List(Of Element)
        Dim _lstOutput As List(Of Element)
        _lstOutput = New List(Of Element)
        For i As Integer = 0 To __Joints.Count - 1
            AddJFrames(__Joints.Item(i), _lstOutput)
        Next
        Return _lstOutput
    End Function

    ''' <summary>
    ''' Recieves as input a frame ID and a list of joints. Returns a subset of the joints that are connected
    ''' to the frame
    ''' </summary>
    ''' <param name="__Joints">a list of joints</param>
    ''' <param name="__FID">A frame ID</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetConJoints(ByRef __Joints As List(Of Joint), ByVal __FID As Long) As List(Of Joint)
        Dim _lstOutput As List(Of Joint)
        _lstOutput = New List(Of Joint)

        For i As Integer = 0 To __Joints.Count - 1
            For j As Integer = 0 To __Joints.Item(i).Connectivity.Count - 1
                If __Joints.Item(i).Connectivity.Item(j).ID = __FID Then
                    _lstOutput.Add(__Joints.Item(i))
                    j = __Joints.Item(i).Connectivity.Count
                End If
            Next
        Next
        Return _lstOutput
    End Function

    ''' <summary>
    ''' Recieves as input a list of joints. It finds the 2 joints that are connected to the ends
    ''' of the frame and sets the Joint1 and Joint2 members.
    ''' </summary>
    ''' <param name="__Joints"></param>
    ''' <remarks></remarks>
    Private Sub SetJoints(ByRef __Joints As List(Of Joint))
        Dim _lstConJoints As List(Of Joint)
        Dim _flag1Smaller As Boolean

        _lstConJoints = GetConJoints(__Joints, ID)
        _flag1Smaller = False
        If FDirection = SapStructure.ELEMENT_DIRECTION.X Then
            If _lstConJoints.Item(0).X < _lstConJoints.Item(1).X Then
                _flag1Smaller = True
            End If
        ElseIf FDirection = SapStructure.ELEMENT_DIRECTION.Y Then
            If _lstConJoints.Item(0).Y < _lstConJoints.Item(1).Y Then
                _flag1Smaller = True
            End If
        Else
            If _lstConJoints.Item(0).Z < _lstConJoints.Item(1).Z Then
                _flag1Smaller = True
            End If
        End If

        If _flag1Smaller = True Then
            Joint1 = _lstConJoints.Item(0)
            Joint2 = _lstConJoints.Item(1)
        Else
            Joint1 = _lstConJoints.Item(1)
            Joint2 = _lstConJoints.Item(0)
        End If
    End Sub

    ''' <summary>
    ''' Sets Joint1 and Joint2 for all the input frames
    ''' </summary>
    ''' <param name="__Joints"></param>
    ''' <param name="__frames"></param>
    ''' <remarks></remarks>
    Private Shared Sub SetEleJoints(ByRef __Joints As List(Of Joint), ByRef __frames As List(Of Element))
        For i As Integer = 0 To __frames.Count - 1
            __frames.Item(i).SetJoints(__Joints)
        Next
    End Sub

    ''' <summary>
    ''' Sets the elements direction
    ''' </summary>
    ''' <param name="__frames">The element to set its direction</param>
    ''' <param name="__type">Beam or column</param>
    ''' <param name="__dir">X or Y for beams only</param>
    ''' <remarks></remarks>
    Private Shared Sub SetElesDir(ByRef __frames As List(Of Element), ByVal __type As SapStructure.ELEMENT_TYPE, _
                                  ByVal __dir As SapStructure.ELEMENT_DIRECTION)
        Dim _dir As SapStructure.ELEMENT_DIRECTION
        If __type = SapStructure.ELEMENT_TYPE.COLUMN Then
            _dir = SapStructure.ELEMENT_DIRECTION.Z
        Else
            If __dir = SapStructure.ELEMENT_DIRECTION.X Then
                _dir = SapStructure.ELEMENT_DIRECTION.X
            Else
                _dir = SapStructure.ELEMENT_DIRECTION.Y
            End If
        End If
        For i As Integer = 0 To __frames.Count - 1
            __frames.Item(i).FDirection = _dir
        Next
    End Sub

    ''' <summary>
    ''' Gets the forces associated with the input frame and the input loadcase or combination
    ''' </summary>
    ''' <param name="__Frame">The frame for which the shear will be attained</param>
    ''' <param name="__Combo_LC">The combo/LoadCase for which the shear will be attained</param>
    ''' <param name="__SapModel">The sap model from which the shear will  be attained</param>
    Private Shared Sub GetForces_LC(ByRef __Frame As Element, ByVal __Combo_LC As String, _
                                         ByRef __SapModel As SAP2000v16.cSapModel, ByRef __P() As Double, _
                                         ByRef __V2() As Double, ByRef __V3() As Double, ByRef __M2() As Double, _
                                         ByRef __M3() As Double, ByRef __T() As Double)
        Dim ret As Long

        Dim NumberResults As Long
        Dim Obj() As String = Nothing
        Dim ObjSta() As Double = Nothing
        Dim Elm() As String = Nothing
        Dim ElmSta() As Double = Nothing
        Dim LoadCase() As String = Nothing
        Dim StepType() As String = Nothing
        Dim StepNum() As Double = Nothing



        ret = __SapModel.Results.Setup.DeselectAllCasesAndCombosForOutput

        'set case selected for output
        ret = __SapModel.Results.Setup.SetComboSelectedForOutput(__Combo_LC)
        'if this is a load case
        If ret = 1 Then
            ret = __SapModel.Results.Setup.SetCaseSelectedForOutput(__Combo_LC)
        End If
        ret = __SapModel.Results.FrameForce(Strings.Trim(Str(__Frame.ID)), 0, _
                                                  NumberResults, Obj, ObjSta, Elm, ElmSta, LoadCase, StepType, StepNum, _
                                                  __P, __V2, __V3, __T, __M2, __M3)
    End Sub


    ''' <summary>
    ''' Adds the input list of forces to the elements force list
    ''' </summary>
    ''' <param name="__V2"></param>
    ''' <param name="__V3"></param>
    ''' <param name="__P"></param>
    ''' <param name="__strModel"></param>
    ''' <param name="__strCase"></param>
    ''' <remarks></remarks>
    Private Sub AddForces(ByRef __V2 As List(Of Double), ByRef __V3 As List(Of Double), _
                          ByRef __P As List(Of Double), ByVal __strModel As String, _
                           ByVal __strCase As String)
        For i As Integer = 0 To __V2.Count - 1
            Forces.Item(i).AddV2(__V2.Item(i), __strModel, __strCase)
            Forces.Item(i).AddV3(__V3.Item(i), __strModel, __strCase)
            Forces.Item(i).AddP(__P.Item(i), __strModel, __strCase)
        Next
    End Sub

    ''' <summary>
    ''' converts the input array to a list
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="__arrInput"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function Arr2List(Of T)(ByRef __arrInput() As T) As List(Of T)
        Dim _lstOutput As List(Of T)
        _lstOutput = New List(Of T)
        For i As Integer = LBound(__arrInput) To UBound(__arrInput)
            _lstOutput.Add(__arrInput(i))
        Next

        Return _lstOutput
    End Function

    ''' <summary>
    ''' Sets the force values of the elements force list for each load case
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <param name="__StrModel"></param>
    ''' <remarks></remarks>
    Private Sub SetForces_LoadCases(ByRef __SapModel As SAP2000v16.cSapModel, ByVal __StrModel As String)
        Dim i As Long
        Dim _lstV2 As List(Of Double)
        Dim _lstP As List(Of Double)
        Dim _lstV3 As List(Of Double)

        Dim _flagNew As Boolean
        Dim _lstLoadCases As List(Of String)
        Dim _P() As Double = Nothing
        Dim _T() As Double = Nothing
        Dim _V2() As Double = Nothing
        Dim _V3() As Double = Nothing
        Dim _M2() As Double = Nothing
        Dim _M3() As Double = Nothing

        _flagNew = False

        _lstLoadCases = Cmb_LC.GetLoadCases(__SapModel)
        For i = 0 To _lstLoadCases.Count - 1
            GetForces_LC(Me, _lstLoadCases.Item(i), __SapModel, _P, _V2, _V3, _M2, _M3, _T)
            _lstV2 = Arr2List(_V2)
            _lstP = Arr2List(_P)
            _lstV3 = Arr2List(_V3)
            ' _lstShears = GetV2Abs(_lstShears)
            AddForces(_lstV2, _lstV3, _lstP, __StrModel, _lstLoadCases.Item(i))
        Next

    End Sub


    ''' <summary>
    ''' Returns the list of stations assocaited with the element
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetStations(ByRef __SapModel As SAP2000v16.cSapModel) As List(Of Double)
        Dim ret As Long

        Dim NumberResults As Long
        Dim Obj() As String = Nothing
        Dim ObjSta() As Double = Nothing
        Dim Elm() As String = Nothing
        Dim ElmSta() As Double = Nothing
        Dim LoadCase() As String = Nothing
        Dim StepType() As String = Nothing
        Dim StepNum() As Double = Nothing
        Dim P() As Double = Nothing
        Dim V2() As Double = Nothing
        Dim V3() As Double = Nothing
        Dim T() As Double = Nothing
        Dim M2() As Double = Nothing
        Dim M3() As Double = Nothing
        Dim _lstOutput As List(Of Double)

        Dim _strLoadCase As String

        _strLoadCase = Cmb_LC.GetLoadCases(__SapModel).Item(0)

        ret = __SapModel.Results.Setup.DeselectAllCasesAndCombosForOutput

        'set case selected for output
        ret = __SapModel.Results.Setup.SetCaseSelectedForOutput(_strLoadCase)
        ret = __SapModel.Results.FrameForce(Strings.Trim(Str(ID)), 0, _
                                                  NumberResults, Obj, ObjSta, Elm, ElmSta, LoadCase, StepType, StepNum, P, V2, V3, T, M2, M3)

        _lstOutput = New List(Of Double)
        For i = LBound(ElmSta) To UBound(ElmSta)
            _lstOutput.Add(ElmSta(i))
        Next
        Return _lstOutput
    End Function


    ''' <summary>
    ''' Generates the Force list of the element. The function only sets the force objects station
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <remarks></remarks>
    Private Sub SetStations(ByRef __SapModel As SAP2000v16.cSapModel)
        Dim _lstStations As List(Of Double)
        Dim i As Integer
        Dim _tmpForce As Force

        _lstStations = GetStations(__SapModel)
        For i = 0 To _lstStations.Count - 1
            _tmpForce = New Force(_lstStations.Item(i), ID)
            Forces.Add(_tmpForce)
        Next

    End Sub
   
    ''' <summary>
    ''' returns the name of the elements section
    ''' </summary>
    ''' <param name="__Element"></param>
    ''' <param name="__SapModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetSection(ByRef __Element As Element, _
                                       ByRef __SapModel As SAP2000v16.cSapModel) As String


        Dim ret As Long
        Dim PropName As String = ""
        Dim SAuto As String = ""


        ret = __SapModel.FrameObj.GetSection(Strings.Trim(Str(__Element.ID)), PropName, SAuto)
        Return PropName
    End Function


    ''' <summary>
    ''' Sets the t3 and t2 values of the element
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <remarks></remarks>
    Private Sub SetDims(ByRef __SapModel As SAP2000v16.cSapModel)


        Dim _ret As Long
        Dim _FileName As String = ""
        Dim _MatProp As String = ""
        Dim _t3 As Double
        Dim _t2 As Double
        Dim _Color As Long
        Dim _Notes As String = ""
        Dim _GUID As String = ""
        Dim _strSection As String

        _strSection = GetSection(Me, __SapModel)

        _ret = __SapModel.PropFrame.GetRectangle(_strSection, _FileName, _MatProp, _t3, _t2, _Color, _Notes, _GUID)
        t3 = _t3
        t2 = _t2
    End Sub

    ''' <summary>
    ''' Returns the list of elements associated with the input list of joints. It is assumed the input joints are
    ''' all in the same line
    ''' </summary>
    ''' <param name="__Joints"></param>
    ''' <param name="__type"></param>
    ''' <param name="__dir"></param>
    ''' <param name="__SapModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetEles(ByRef __Joints As List(Of Joint),
                                   ByVal __type As SapStructure.ELEMENT_TYPE, ByVal __dir As SapStructure.ELEMENT_DIRECTION, _
                                ByRef __SapModel As SAP2000v16.cSapModel) As List(Of Element)

        Dim _lstOutput As List(Of Element)

        _lstOutput = GetEleList(__Joints)
        SetElesDir(_lstOutput, __type, __dir)
        For i As Integer = 0 To _lstOutput.Count - 1
            _lstOutput.Item(i).SetJoints(__Joints)
            _lstOutput.Item(i).SetStations(__SapModel)
            _lstOutput.Item(i).SetDims(__SapModel)
        Next

        Return _lstOutput
    End Function


    ''' <summary>
    ''' Sets the elements force objects force values for every load case
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <param name="__strModelName"></param>
    ''' <param name="__lstFrames"></param>
    ''' <remarks></remarks>
    Public Shared Sub SetResults_LoadCases(ByRef __SapModel As SAP2000v16.cSapModel, ByRef __strModelName As String, _
                             ByRef __lstFrames As List(Of Element))

        For i As Integer = 0 To __lstFrames.Count - 1
            __lstFrames.Item(i).SetForces_LoadCases(__SapModel, __strModelName)
        Next
    End Sub

End Class

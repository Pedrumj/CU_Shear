''' <summary>
''' Represents a joint in the model
''' </summary>
''' <remarks></remarks>
Friend Class Joint
    'the x coorindate
    Public X As Double
    'the y coordinate
    Public Y As Double
    'the z coordinate
    Public Z As Double
    'the element ID as in SAP
    Public ID As Integer
    'a list of elements connceted to the joint
    Private ConAll As List(Of Element)
    'a list of elements connected to the joint in the direction and line we are targeting (along the cast unit)
    Public Connectivity As List(Of Element)

    Private Const TOLERANCE = 0.001
    Public Sub New(ByVal __X As Double, ByVal __Y As Double, ByVal __Z As Double, _
                   ByVal __ID As Double, ByRef __SapModel As SAP2000v16.cSapModel)
        X = __X
        Y = __Y
        Z = __Z
        ID = __ID

        SetPEle(__SapModel)

    End Sub

    ''' <summary>
    ''' Check if the frame is connected to the input joint
    ''' </summary>
    Private Function CheckCon(ByRef __Joint As Joint, ByVal __frame As Element)
        For i = 0 To __Joint.ConAll.Count - 1
            If __Joint.ConAll.Item(i).ID = __frame.ID Then
                Return True
            End If
        Next
        Return False
    End Function

    ''' <summary>
    ''' return the number of joints that are connected to the input frame
    ''' </summary>
    Private Function ConCount(ByRef __lstJoints As List(Of Joint), ByVal __frame As Element) As Long
        Dim _lngCount As Long

        _lngCount = 0
        For i = 0 To __lstJoints.Count - 1
            If CheckCon(__lstJoints.Item(i), __frame) = True Then
                _lngCount += 1
            End If
        Next
        Return _lngCount
    End Function

    ''' <summary>
    ''' Sets the Connectivity memeber
    ''' </summary>
    ''' <param name="__lstJoints">The list of joints along the cast unit</param>
    ''' <remarks></remarks>
    Private Sub SetJointCon(ByRef __lstJoints As List(Of Joint))
        Dim _frame As Element
        Dim i As Integer

        Connectivity = New List(Of Element)
        For i = 0 To ConAll.Count - 1
            'if more than 2 of the joints in the list are connected to the frame
            If ConCount(__lstJoints, ConAll.Item(i)) > 1 Then
                _frame = New Element()
                _frame.ID = ConAll.Item(i).ID
                Connectivity.Add(_frame)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Sets the connectivity member of all the joints
    ''' </summary>
    ''' <param name="__lstJoints">The list of joints along the cast unit</param>
    ''' <remarks></remarks>
    Private Shared Sub SetCon(ByRef __lstJoints As List(Of Joint))
        For i = 0 To __lstJoints.Count - 1
            __lstJoints.Item(i).SetJointCon(__lstJoints)
        Next
    End Sub

    ''' <summary>
    ''' Sets the ConAll member of the joint
    ''' </summary>
    ''' <param name="__SapModel"></param>
    ''' <remarks></remarks>
    Private Sub SetPEle(ByRef __SapModel As SAP2000v16.cSapModel)
        Dim ret As Long
        Dim NumberItems As Long
        Dim ObjectType() As Integer
        Dim ObjectName() As String
        Dim PointNumber() As Integer
        Dim i As Integer

        Dim _frame As Element


        ReDim ObjectType(0 To 0)
        ReDim ObjectName(0 To 0)
        ReDim PointNumber(0 To 0)

        ConAll = New List(Of Element)

        ret = __SapModel.PointElm.GetConnectivity(Strings.Trim(Str(ID)), NumberItems, _
                                                  ObjectType, ObjectName, PointNumber)
        For i = LBound(ObjectType) To UBound(ObjectType)
            'frame element
            If ObjectType(i) = 2 Then
                _frame = New Element()
                _frame.ID = Element.GetElId(ObjectName(i))
                ConAll.Add(_frame)
            End If
        Next

    End Sub

    ''' <summary>
    ''' returns the distance of 2 joints
    ''' </summary>
    ''' <param name="__Joint1"></param>
    ''' <param name="__Joint2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function Distance(ByRef __Joint1 As Joint, ByRef __Joint2 As Joint) As Double
        Dim _distance As Double
        _distance = ((__Joint1.X - __Joint2.X) ^ 2 + (__Joint1.Y - __Joint2.Y) ^ 2 + (__Joint1.Z - __Joint2.Z) ^ 2) ^ 0.5
        Return _distance
    End Function


    ''' <summary>
    ''' Returns all the joints in the sap model
    ''' </summary>
    ''' <param name="__sapModel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Shared Function GetAllJoints(ByRef __sapModel As SAP2000v16.cSapModel) As List(Of Joint)
        Dim _intCount As Integer
        Dim x As Double
        Dim y As Double
        Dim z As Double
        Dim ret As Long
        Dim _Joint
        Dim _lstOutput As List(Of Joint)

        _lstOutput = New List(Of Joint)
        _intCount = __sapModel.PointElm.Count
        For i As Integer = 1 To _intCount
            ret = __sapModel.PointObj.GetCoordCartesian(Strings.Trim(Str(i)), x, y, z)
            _Joint = New Joint(x, y, z, i, __sapModel)
            _lstOutput.Add(_Joint)
        Next
        Return _lstOutput
    End Function


    ''' <summary>
    ''' Returns the set of joints in the sap model that are along the cast unit
    ''' </summary>
    ''' <param name="__sapModel">A refernce to the sap model</param>
    ''' <param name="__xTar">An x coordinate along the cast unit</param>
    ''' <param name="__yTar">A Y coordinate along the cast unit</param>
    ''' <param name="__zTar">A Z coorindate along the cast unit</param>
    ''' <param name="__type">Beam or column</param>
    ''' <param name="__direction">X or Y for beams only</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetSpecJoints(ByRef __sapModel As SAP2000v16.cSapModel, ByVal __xTar As Double, _
                                          ByVal __yTar As Double, ByVal __zTar As Double, ByVal __type As SapStructure.ELEMENT_TYPE, _
                                          ByVal __direction As SapStructure.ELEMENT_DIRECTION) As List(Of Joint)
        Dim x As Double
        Dim y As Double
        Dim z As Double

        Dim _lstAllJoints As List(Of Joint)

        Dim _lstOutput As List(Of Joint)
        Dim _Joint As Joint

        _lstAllJoints = GetAllJoints(__sapModel)
        _lstOutput = New List(Of Joint)

        'loop through all joints in the model
        For i As Integer = 0 To _lstAllJoints.Count - 1
            x = _lstAllJoints.Item(i).X
            y = _lstAllJoints.Item(i).Y
            z = _lstAllJoints.Item(i).Z

            If __type = SapStructure.ELEMENT_TYPE.BEAM Then
                If __direction = SapStructure.ELEMENT_DIRECTION.X Then
                    If PointEq(z, __zTar, y, __yTar, TOLERANCE) = True Then
                        _Joint = New Joint(x, y, z, _lstAllJoints.Item(i).ID, __sapModel)
                        _lstOutput.Add(_Joint)
                    End If
                Else
                    If PointEq(z, __zTar, x, __yTar, TOLERANCE) = True Then
                        _Joint = New Joint(x, y, z, _lstAllJoints.Item(i).ID, __sapModel)
                        _lstOutput.Add(_Joint)
                    End If
                End If
            Else
                If PointEq(y, __yTar, x, __xTar, TOLERANCE) = True Then
                    _Joint = New Joint(x, y, z, _lstAllJoints.Item(i).ID, __sapModel)
                    _lstOutput.Add(_Joint)
                End If
            End If
        Next
        SetCon(_lstOutput)
        Return _lstOutput
    End Function

    'determines if the input points are the same. 
    Private Shared Function PointEq(ByVal X1 As Double, ByVal X2 As Double, ByVal Y1 As Double, ByVal Y2 As Double, _
                          ByVal Tolerance As Double) As Boolean
        If ((X1 - X2) ^ 2 - (Y1 - Y2) ^ 2) ^ 0.5 < Tolerance Then
            Return True
        Else
            Return False
        End If
    End Function

End Class

Public Class clsDepartmentEvent
    Inherits System.EventArgs
    Public mDepartmentID As Integer

    Public Sub New(ByVal vDepartmentID As Integer)
        MyBase.New()
        mDepartmentID = vDepartmentID
    End Sub

End Class

Public Class clsSchoolEvent
    Inherits System.EventArgs
    Public mSchoolD As Integer

    Public Sub New(ByVal vSchoolID As Integer)
        MyBase.New()
        mSchoolD = vSchoolID
    End Sub

End Class

Public Class clsDepartmentEvent
    Inherits System.EventArgs
    Public mDepartmentID As Integer

    Public Sub New(ByVal vDepartmentID As Integer)
        MyBase.New()
        mDepartmentID = vDepartmentID
    End Sub

End Class


Public Class clsSubjectEvent
    Inherits System.EventArgs
    Public mSubjectID As Integer

    Public Sub New(ByVal vSubjectID As Integer)
        MyBase.New()
        mSubjectID = vSubjectID
    End Sub

End Class

Public Class clsQualEvent
    Inherits System.EventArgs
    Public mQualID As Integer

    Public Sub New(ByVal vQualID As Integer)
        MyBase.New()
        mQualID = vQualID
    End Sub

End Class
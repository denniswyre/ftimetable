Public Class summary
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        getSubjectSummary()
    End Sub


    Sub getSubjectSummary()
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vSubj = (From p In vContext.subjects Select p).ToList
        Dim Vqu = (From p In vContext.qualifications Select p).ToList
        Dim DummyFacultyID = CType(ConfigurationManager.AppSettings("dummyfaculty"), Integer)
        'per faculty
        For Each x In (From p In vContext.faculties).ToList
            If x.ID = DummyFacultyID Then
                lstReports.Items.Add("Faculty:" + "Unassigned")
            Else
                lstReports.Items.Add("Faculty:" + x.code)
            End If
            Dim FacSubj = From p In vSubj Where p.department.school.facultyID = x.ID Select p
            Dim FacQua = From p In Vqu Where p.department.school.facultyID = x.ID Select p

            lstReports.Items.Add(" Total Subjects:" + CStr(FacSubj.Count))
            lstReports.Items.Add(" Total Qualifications:" + CStr(FacQua.Count))

        Next

    End Sub

End Class
Public Class createResources
    Inherits System.Web.UI.UserControl



    Private Sub btnCreate_Click(sender As Object, e As System.EventArgs) Handles btnCreate.Click
        Dim vContext As timetableEntities = New timetableEntities()
        Try
            Dim i As Integer = 0
            Dim vClassgrp = (From p In vContext.classgroups Select p).ToList
            For Each x In vClassgrp
                If x.resources.Count = 0 Then
                    createFirstResource(x.ID)
                    i = i + 1
                End If
            Next
            ErrorMessage.Text = clsGeneral.displaymessage(CStr(i) + " resources created", False)
        Catch ex As Exception
            ErrorMessage.Text = clsGeneral.displaymessage(ex.Message, True)
        End Try
    End Sub


    'auto generate first resource for newly created classgroup
    Public Function createFirstResource(ByVal ClassID As Integer) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vResourceType = (From p In vContext.resourcetypes Where p.isClassRoom = True Select p).FirstOrDefault
        If IsNothing(vResourceType) Then
            Return 0
        End If
        Dim DefaultResourceTypeID = vResourceType.ID
        Dim classgroup = (From p In vContext.classgroups Where p.ID = ClassID Select p).First
        Dim vRes As New sResource With {
            .Name = getResourceName(ClassID),
            .AmtParticipants = classgroup.classSize,
            .AmtTimeSlots = classgroup.TimeSlotTotal,
            .MaxMergedTimeSlots = CInt(ConfigurationManager.AppSettings("defaultClassMaxMergedSlots")),
            .ResourceTypeID = DefaultResourceTypeID,
            .year = getResourceYear(classgroup.academicblock.startWeek, classgroup.academicblock.endWeek),
            .startWeek = classgroup.academicblock.startWeek,
            .endWeek = classgroup.academicblock.endWeek,
            .classgrouplinked = True,
            .TimeslotsArrangement = ""}
        Return CreateResource(vRes, ClassID)
    End Function

    Function getResourceYear(ByVal startweek As Integer, ByVal endweek As Integer) As Integer
        Dim vCurWeek = DatePart(DateInterval.WeekOfYear, Now)
        If vCurWeek > endweek Then
            Return Year(Now) + 1
        Else
            Return Year(Now)
        End If
    End Function

    Structure sResource
        Dim ID As Integer
        Dim Name As String
        Dim year As Integer
        Dim startweek As Integer
        Dim endweek As Integer
        Dim ResourceTypeID As Integer
        Dim classgrouplinked As Boolean
        Dim AmtParticipants As Integer
        Dim AmtTimeSlots As Integer
        Dim MaxMergedTimeSlots As Integer
        Dim TimeslotsArrangement As String
    End Structure

    Protected Function CreateResource(ByVal vRes As sResource, ByVal ClassID As Integer) As Integer
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vclassgroup = (From p In vContext.classgroups Where p.ID = ClassID Select p).First
        Dim vResource = New resource With {
          .Name = vRes.Name,
          .AmtParticipants = vRes.AmtParticipants,
          .AmtTimeSlots = vRes.AmtTimeSlots,
          .MaxMergedTimeSlots = vRes.MaxMergedTimeSlots,
          .year = vRes.year,
          .startWeek = vRes.startweek,
          .endWeek = vRes.endweek,
          .classgrouplinked = vRes.classgrouplinked,
          .ResourceTypeID = vRes.ResourceTypeID,
          .TimeslotsArrangement = vRes.TimeslotsArrangement}
        vContext.resources.AddObject(vResource)
        vContext.SaveChanges()
        vResource.classgroups.Add(vclassgroup)
        vContext.SaveChanges()
        Return vResource.ID
    End Function

    Function getResourceName(ByVal ClassID As Integer) As String
        Dim vContext As timetableEntities = New timetableEntities()
        Dim vclassgroup = (From p In vContext.classgroups Where p.ID = ClassID Select p).First
        Dim i = 0
        Do
            Dim vName = vclassgroup.siteclustersubject.subject.Code + "(" + vclassgroup.code + ")-" + CStr(i)
            Dim vResource =
                   (From p In vclassgroup.resources
                       Where p.Name = vName
                           Select p).FirstOrDefault
            If IsNothing(vResource) Then
                Return vName
            End If
            i = i + 1
        Loop While True
        Return ""
    End Function

    
End Class
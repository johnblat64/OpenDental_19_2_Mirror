Imports System
Imports EnvDTE
Imports EnvDTE80
Imports System.Diagnostics
Imports System.Windows.Forms

Public Module ConvertDataObject
    Public Sub Convert()
        Convert(DTE.ActiveDocument)
    End Sub

    Public Sub Convert(ByVal document As Document)
        Dim projectItem As ProjectItem = Document.ProjectItem
        Dim fileCodeModel As FileCodeModel = ProjectItem.FileCodeModel
        Dim namespaceElement As CodeElement
        Dim classElement As CodeElement

        Dim element As CodeElement
        For Each element In fileCodeModel.CodeElements
            ' Enter the namespace
            If element.Kind = vsCMElement.vsCMElementNamespace Then
                namespaceElement = element
                Exit For
            End If
        Next

        For Each element In namespaceElement.Children
            If element.Kind = vsCMElement.vsCMElementClass Then
                classElement = element
                Exit For
            End If
        Next

        ' get the variable's parent
        Dim codeClass As CodeClass = CType(classElement, CodeClass)

        ' Make the class inherit from "DataObjectBase"
        codeClass.AddBase("DataObjectBase")
        codeClass.AddAttribute("DataObject", """" + codeClass.Name.ToLower() + """")

        For Each element In classElement.Children
            If element.Kind = vsCMElement.vsCMElementVariable Then
                ' we've tested so we know its a variable
                Dim codeVariable As CodeVariable = CType(element, CodeVariable)
                Dim fieldName As String = codeVariable.Name
                Dim propertyName As String = fieldName
                Dim fieldType As String = codeVariable.Type.AsString

                If Not Char.IsUpper(fieldName(0)) Then
                    MessageBox.Show(String.Format("The field {0} does not start with an upper case letter. Can not continue refactoring!", fieldName))
                    Continue For
                End If

                fieldName = String.Concat(Char.ToLower(fieldName(0)), fieldName.Substring(1))

                ' add the DataField attribute
                codeVariable.AddAttribute("DataField", """" + propertyName + """")

                ' rename the field so we don't have a collision with property
                codeVariable.Name = fieldName

                ' make sure field is private
                codeVariable.Access = vsCMAccess.vsCMAccessPrivate

                ' add a new property
                Dim codeProperty As CodeProperty = codeClass.AddProperty("dummy", _
                    "dummy", fieldType, element)

                codeProperty.Name = propertyName

                ' implement the getter
                Dim getExpression As String = "get { return " + fieldName + "; }"
                Dim getter As EditPoint = codeProperty.Getter.StartPoint.CreateEditPoint()
                getter.ReplaceText(codeProperty.Getter.EndPoint, getExpression, vsEPReplaceTextOptions.vsEPReplaceTextAutoformat)

                ' implement the setter
                Dim setExpression As String = "set { " + fieldName + "= value;MarkDirty(); " + fieldName + "Changed = true; }"
                Dim setter As EditPoint = codeProperty.Setter.StartPoint.CreateEditPoint()
                setter.ReplaceText(codeProperty.Setter.EndPoint, setExpression, vsEPReplaceTextOptions.vsEPReplaceTextAutoformat)


                fieldName = String.Concat(Char.ToLower(fieldName(0)), fieldName.Substring(1))

                ' Mark the item as public
                codeProperty.Access = vsCMAccess.vsCMAccessPublic

                ' Add an extra propertyChanged field
                Dim changedVariable As CodeVariable = codeClass.AddVariable(fieldName + "Changed", GetType(System.Boolean).ToString(), element)

                ' Add a PropertyChanged property
                Dim changedProperty As CodeProperty = codeClass.AddProperty("dummy", _
                    Nothing, GetType(System.Boolean).ToString(), codeProperty)

                changedProperty.Name = propertyName + "Changed"
                changedProperty.Access = vsCMAccess.vsCMAccessPublic

                ' implement the getter
                getExpression = "get { return " + fieldName + "Changed; }"
                getter = changedProperty.Getter.StartPoint.CreateEditPoint()
                getter.ReplaceText(changedProperty.Getter.EndPoint, getExpression, vsEPReplaceTextOptions.vsEPReplaceTextAutoformat)

                ' move the Doc comments
                codeProperty.DocComment = codeVariable.DocComment
                Dim codeCommentStart As EditPoint = codeVariable.StartPoint.CreateEditPoint()
                codeCommentStart.LineUp()
                Dim codeCommentEnd As EditPoint = codeVariable.StartPoint.CreateEditPoint()
                codeCommentStart.Delete(codeCommentEnd)

                ' Fix white space
                changedProperty.EndPoint.CreateEditPoint().Insert(Environment.NewLine)
                codeProperty.GetEndPoint.CreateEditPoint().Delete(changedProperty.StartPoint)
                codeProperty.GetEndPoint.CreateEditPoint().Insert(Environment.NewLine)
                changedProperty.GetStartPoint().CreateEditPoint().Indent(Nothing, 2)

                Dim codePropertyStart As EditPoint = codeProperty.StartPoint.CreateEditPoint()
                codePropertyStart.LineUp()
                changedVariable.GetEndPoint.CreateEditPoint().Delete(codePropertyStart)
                changedVariable.GetEndPoint.CreateEditPoint().Insert(Environment.NewLine)
                codePropertyStart.LineDown()
                codePropertyStart.Indent(Nothing, 2)

            ElseIf element.Kind = vsCMElement.vsCMElementFunction AndAlso element.Name = "Copy" Then
                ' Do nothing (for now)
            End If
        Next
    End Sub

    Public Sub AutoConvert()
        Dim document As Document
        For Each document In DTE.Documents
            document.Activate()
            Convert(document)
            document.Save()
            document.Close()
        Next
    End Sub

    Public Sub FixWhiteSpace()
        'Looks like I'm going to have to learn how to do this Macro thing before this can be implemented.
        'The plan is to make the file more human readable.  See Mount.cs.  I only had time to do the first three fields.
        'I have a very specific way that I want the whitespace to show, but it will take time to create a macro
        'which can do it. -Jordan
    End Sub
End Module



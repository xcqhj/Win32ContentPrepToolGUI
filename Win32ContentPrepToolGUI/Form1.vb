﻿'
' Author: Connor Hudson
'
' Date: 15-Sept-2022
'

Imports System.Diagnostics.Eventing.Reader
Imports System.IO



'
' TODO:
'
'   Implement a fix for installerPath to include quotes around the filename if it includes spaces
'
'   The installerPath MUST have quotes around the installer file if the file has spaces in the name
'
'   For example, if the installer is named "installme-now.msi", then there is no issue.
'   But if the file is named "Install me now.msi" with spaces between the words, the file MUST be quote wrapped
'   otherwise IntuneWinAppUtil.exe will complain about not being able to access the setup file.
'


Public Class Form1

    Dim rootOfDrive As String = System.Environment.GetEnvironmentVariable("HOMEDRIVE")
    Dim installerPath As String
    Dim sourceFolder As String
    Dim outputFolder As String
    Dim prepToolExe As String
    Dim installerFileName As String
    Dim catalogChoice As Boolean = False

#Region " Buttons "

    Private Sub btnSelectInstaller_Click(sender As Object, e As EventArgs) Handles btnSelectInstaller.Click

        Dim ofd As OpenFileDialog = opnfilediagSelectInstaller

        ofd.Title = "Select Installer File (.exe or .msi)"
        ofd.InitialDirectory = rootOfDrive
        ofd.ShowDialog()

    End Sub
    Private Sub btnStartPackaging_Click(sender As Object, e As EventArgs) Handles btnStartPackaging.Click

        Dim properties As New ProcessStartInfo

        Dim param_setupFolder As String = "-c"
        Dim param_setupFile As String = "-s"
        Dim param_outputFolder As String = "-o"
        Dim param_catalogFolder As String = "-a"
        Dim param_quietMode As String = "-q"
        Dim sp As String = " "

        Dim args As String = ""

        args = param_setupFolder + sp + sourceFolder + sp + param_setupFile + sp + installerPath + sp + param_outputFolder + sp + outputFolder + param_quietMode

        debugMessages("Debug args: ", lblDebug_args, args)

        txtArguments.Text = args

        properties.FileName = prepToolExe
        properties.Arguments = args
        properties.WindowStyle = ProcessWindowStyle.Normal

    End Sub

    Private Sub btnSelectInstallerFolder_Click(sender As Object, e As EventArgs) Handles btnSelectInstallerFolder.Click

        SelectFolder(fbdiagSelectSourceFolder, txtPathOfInstallerFolder)
        sourceFolder = txtPathOfInstallerFolder.Text
        debugMessages("Debug installerFolder: ", lblDebug_installerFolder, sourceFolder)

    End Sub
    Private Sub btnSelectOutputFolder_Click(sender As Object, e As EventArgs) Handles btnSelectOutputFolder.Click

        SelectFolder(fbdiagSelectOutputFolder, txtOutputFolderPath)
        outputFolder = txtOutputFolderPath.Text
        debugMessages("Debug outputFolder: ", lblDebug_outputFolder, outputFolder)

    End Sub
    Private Sub btnSelectPrepToolExe_Click(sender As Object, e As EventArgs) Handles btnSelectPrepToolExe.Click

        Dim ofd As OpenFileDialog = opnfilediagSelectPrepToolExe

        ofd.Title = "Select IntuneWinAppUtil.exe"
        ofd.InitialDirectory = rootOfDrive
        ofd.ShowDialog()

    End Sub

#End Region

#Region " Open File Dialogs "

    Private Sub opnfilediagSelectInstaller_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles opnfilediagSelectInstaller.FileOk

        Dim sb As New StringBuilder()

        Dim reversedFilePath As String

        Dim index As Integer

        SelectFile(opnfilediagSelectInstaller, txtPathOfInstaller)
        installerPath = txtPathOfInstaller.Text
        reversedFilePath = StrReverse(installerPath)

        debugMessages("Debug installerPath: ", lblDebug_installerPath, installerPath)


        ' This loop is so we can capture the filename of the executable we want to pack.
        ' Unfortunately, if there are spaces in the filename, IntuneWinAppUtil.exe throws
        ' some errors unless it is wrapped in quotes.

        For Each c As Char In reversedFilePath
            index += 1
            If c = "\" Then
                installerFileName = reversedFilePath.Substring(0, index - 1)
                installerFileName = StrReverse(installerFileName)

                installerFileName = sb.Append(Chr(34)).Append(installerFileName).Append(Chr(34)).ToString() ' Add double quotation marks to the beginning and ending of filename

                debugMessages("Installer filename is: ", lblDebug_installerFileName, installerFileName)
                Exit For
            End If
        Next



    End Sub
    Private Sub opnfilediagSelectPrepToolExe_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles opnfilediagSelectPrepToolExe.FileOk

        SelectFile(opnfilediagSelectPrepToolExe, txtPathOfPrepToolExe)
        prepToolExe = txtPathOfPrepToolExe.Text
        debugMessages("Debug prepToolExe: ", lblDebug_prepToolExe, prepToolExe)

    End Sub

#End Region

#Region " Sub Procedures and Functions "
    Sub SelectFolder(fbdiag As FolderBrowserDialog, txtbox As TextBox)

        If (fbdiag.ShowDialog() = DialogResult.OK) Then
            txtbox.Text = fbdiag.SelectedPath()
        End If

    End Sub

    Sub SelectFile(openFileDiag As OpenFileDialog, txtBox As TextBox)

        Dim stream As System.IO.Stream
        stream = openFileDiag.OpenFile()
        txtBox.Text = openFileDiag.FileName.ToString()

    End Sub


    Sub debugMessages(debugMsg As String, debugLabel As Label, Optional debugValue As String = "")
        debugLabel.Text = debugMsg + debugValue
    End Sub
#End Region

#Region " Catalog Choice Group Box "
    Private Sub GetGroupBoxCheckedRadioButton(grpBox As GroupBox)

        Dim rbtn As RadioButton = grpBox.Controls.OfType(Of RadioButton).Where(Function(r) r.Checked = True).FirstOrDefault()

        If rbtn.Name = "radNo" Then
            debugMessages("Debug catalogChoice: ", lblDebug_catalogChoice, "No")
        Else
            debugMessages("Debug catalogChoice: ", lblDebug_catalogChoice, "Yes")
        End If

    End Sub

    Private Sub radYes_CheckedChanged(sender As Object, e As EventArgs) Handles radYes.CheckedChanged
        GetGroupBoxCheckedRadioButton(grpCatalogFolder)
    End Sub

    Private Sub radNo_CheckedChanged(sender As Object, e As EventArgs) Handles radNo.CheckedChanged
        GetGroupBoxCheckedRadioButton(grpCatalogFolder)
    End Sub

#End Region


End Class

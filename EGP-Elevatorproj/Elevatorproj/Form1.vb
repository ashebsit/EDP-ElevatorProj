Imports System.Windows.Forms
Imports System.Drawing

Public Class Form1

    Private currentFloor As Integer = 1
    Private targetFloor As Integer = 1
    Private isMoving As Boolean = False
    Private isEmergency As Boolean = False

    Private pnlShaft As New Panel
    Private pnlElevator As New Panel
    Private pnlDoorLeft As New Panel
    Private pnlDoorRight As New Panel

    Private pnlHead As New Panel
    Private pnlBody As New Panel
    Private pnlArmL As New Panel
    Private pnlArmR As New Panel
    Private pnlLegL As New Panel
    Private pnlLegR As New Panel

    Private lblFloorIndicator(3) As Label

    Private lblDisplay As New Label
    Private lblStatus As New Label
    Private lblEmergency As New Label
    Private btnFloor(3) As Button
    Private btnOpen As New Button
    Private btnClose As New Button
    Private btnEmergency As New Button

    Private btnCallUp(3) As Button
    Private btnCallDown(3) As Button

    Private lblDirection As New Label

    Private moveTimer As New Timer
    Private doorTimer As New Timer

    Private floorY As New List(Of Integer)
    Private doorState As Integer

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Elevator"
        Me.Size = New Size(950, 700)
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.BackColor = Color.FromArgb(40, 40, 40)

        pnlShaft.Size = New Size(150, 520)
        pnlShaft.Location = New Point(200, 50)
        pnlShaft.BackColor = Color.FromArgb(20, 20, 20)
        pnlShaft.BorderStyle = BorderStyle.FixedSingle
        AddHandler pnlShaft.Paint, AddressOf DrawFloorLines
        Me.Controls.Add(pnlShaft)

        Dim elevHeight As Integer = 90
        floorY.Add(pnlShaft.Height - elevHeight - 20)
        floorY.Add(floorY(0) - 110)
        floorY.Add(floorY(1) - 110)
        floorY.Add(floorY(2) - 110)

        pnlElevator.Size = New Size(100, elevHeight)
        pnlElevator.Location = New Point(25, floorY(0))
        pnlElevator.BackColor = Color.FromArgb(180, 180, 180)
        pnlShaft.Controls.Add(pnlElevator)

        pnlDoorLeft.Size = New Size(50, elevHeight)
        pnlDoorLeft.Location = New Point(0, 0)
        pnlDoorLeft.BackColor = Color.FromArgb(70, 70, 80)
        pnlElevator.Controls.Add(pnlDoorLeft)

        pnlDoorRight.Size = New Size(50, elevHeight)
        pnlDoorRight.Location = New Point(50, 0)
        pnlDoorRight.BackColor = Color.FromArgb(70, 70, 80)
        pnlElevator.Controls.Add(pnlDoorRight)

        pnlHead.Size = New Size(14, 14)
        pnlHead.Location = New Point(43, 8)
        pnlHead.BackColor = Color.FromArgb(255, 210, 180)
        pnlElevator.Controls.Add(pnlHead)

        pnlBody.Size = New Size(22, 30)
        pnlBody.Location = New Point(39, 24)
        pnlBody.BackColor = Color.FromArgb(0, 100, 200)
        pnlElevator.Controls.Add(pnlBody)

        pnlArmL.Size = New Size(8, 24)
        pnlArmL.Location = New Point(29, 24)
        pnlArmL.BackColor = Color.FromArgb(255, 210, 180)
        pnlElevator.Controls.Add(pnlArmL)

        pnlArmR.Size = New Size(8, 24)
        pnlArmR.Location = New Point(63, 24)
        pnlArmR.BackColor = Color.FromArgb(255, 210, 180)
        pnlElevator.Controls.Add(pnlArmR)

        pnlLegL.Size = New Size(8, 28)
        pnlLegL.Location = New Point(35, 52)
        pnlLegL.BackColor = Color.FromArgb(50, 50, 150)
        pnlElevator.Controls.Add(pnlLegL)

        pnlLegR.Size = New Size(8, 28)
        pnlLegR.Location = New Point(57, 52)
        pnlLegR.BackColor = Color.FromArgb(50, 50, 150)
        pnlElevator.Controls.Add(pnlLegR)

        For i As Integer = 0 To 3
            Dim fy As Integer = floorY(i)
            Dim floorNum As Integer = i + 1
            Dim floorPanel As New Panel
            floorPanel.Size = New Size(85, 80)
            floorPanel.Location = New Point(20, fy - 5)
            floorPanel.BackColor = Color.FromArgb(60, 60, 60)
            floorPanel.BorderStyle = BorderStyle.FixedSingle
            Me.Controls.Add(floorPanel)

            Dim lblFl As New Label
            lblFl.Size = New Size(85, 14)
            lblFl.Location = New Point(0, 2)
            lblFl.Text = "FL " & floorNum
            lblFl.TextAlign = ContentAlignment.MiddleCenter
            lblFl.BackColor = Color.Black
            lblFl.ForeColor = Color.Lime
            floorPanel.Controls.Add(lblFl)

            btnCallUp(i) = New Button
            btnCallUp(i).Size = New Size(38, 26)
            btnCallUp(i).Text = "▲"
            btnCallUp(i).Font = New Font("Arial", 10, FontStyle.Bold)
            btnCallUp(i).Location = New Point(3, 18)

            If i = 3 Then
                btnCallUp(i).BackColor = Color.DarkGray
                btnCallUp(i).Enabled = False
            Else
                btnCallUp(i).BackColor = Color.Gray
                btnCallUp(i).ForeColor = Color.White
            End If
            AddHandler btnCallUp(i).Click, Sub(s, ev) CallButton_Click(floorNum)
            floorPanel.Controls.Add(btnCallUp(i))

            btnCallDown(i) = New Button
            btnCallDown(i).Size = New Size(38, 26)
            btnCallDown(i).Text = "▼"
            btnCallDown(i).Font = New Font("Arial", 10, FontStyle.Bold)
            btnCallDown(i).Location = New Point(44, 18)

            If i = 0 Then
                btnCallDown(i).BackColor = Color.DarkGray
                btnCallDown(i).Enabled = False
            Else
                btnCallDown(i).BackColor = Color.Gray
                btnCallDown(i).ForeColor = Color.White
            End If
            AddHandler btnCallDown(i).Click, Sub(s, ev) CallButton_Click(floorNum)
            floorPanel.Controls.Add(btnCallDown(i))

            lblFloorIndicator(i) = New Label
            lblFloorIndicator(i).Size = New Size(85, 16)
            lblFloorIndicator(i).Location = New Point(0, 58)
            lblFloorIndicator(i).Text = "FLR: " & floorNum
            lblFloorIndicator(i).TextAlign = ContentAlignment.MiddleCenter
            lblFloorIndicator(i).BackColor = Color.Black
            lblFloorIndicator(i).ForeColor = Color.Red
            floorPanel.Controls.Add(lblFloorIndicator(i))
        Next

        lblDisplay.Size = New Size(140, 60)
        lblDisplay.Location = New Point(600, 50)
        lblDisplay.TextAlign = ContentAlignment.MiddleCenter
        lblDisplay.Font = New Font("Consolas", 32, FontStyle.Bold)
        lblDisplay.BackColor = Color.Black
        lblDisplay.ForeColor = Color.Lime
        lblDisplay.Text = "1"
        Me.Controls.Add(lblDisplay)

        lblDirection.Size = New Size(140, 35)
        lblDirection.Location = New Point(600, 120)
        lblDirection.TextAlign = ContentAlignment.MiddleCenter
        lblDirection.Font = New Font("Arial", 16, FontStyle.Bold)
        lblDirection.BackColor = Color.FromArgb(50, 50, 50)
        lblDirection.ForeColor = Color.Yellow
        Me.Controls.Add(lblDirection)

        lblEmergency.Size = New Size(140, 35)
        lblEmergency.Location = New Point(600, 165)
        lblEmergency.TextAlign = ContentAlignment.MiddleCenter
        lblEmergency.BackColor = Color.Black
        lblEmergency.ForeColor = Color.Red
        Me.Controls.Add(lblEmergency)

        lblStatus.Size = New Size(140, 35)
        lblStatus.Location = New Point(600, 210)
        lblStatus.TextAlign = ContentAlignment.MiddleCenter
        lblStatus.Font = New Font("Arial", 14, FontStyle.Bold)
        lblStatus.BackColor = Color.FromArgb(30, 30, 30)
        lblStatus.ForeColor = Color.White
        lblStatus.Text = "CLOSED"
        Me.Controls.Add(lblStatus)

        For i As Integer = 0 To 3
            Dim floorNum As Integer = i + 1
            btnFloor(i) = New Button
            btnFloor(i).Size = New Size(55, 55)
            btnFloor(i).Text = floorNum.ToString
            btnFloor(i).Location = New Point(600, 260 + i * 65)
            btnFloor(i).BackColor = Color.FromArgb(60, 60, 60)
            btnFloor(i).ForeColor = Color.White
            btnFloor(i).Font = New Font("Arial", 16, FontStyle.Bold)
            AddHandler btnFloor(i).Click, Sub(s, ev) CallInside(floorNum)
            Me.Controls.Add(btnFloor(i))
        Next

        btnOpen.Size = New Size(60, 45)
        btnOpen.Location = New Point(600, 530)
        btnOpen.Text = "OPEN"
        btnOpen.BackColor = Color.Green
        btnOpen.ForeColor = Color.White
        AddHandler btnOpen.Click, AddressOf OpenDoors
        Me.Controls.Add(btnOpen)

        btnClose.Size = New Size(60, 45)
        btnClose.Location = New Point(680, 530)
        btnClose.Text = "CLOSE"
        btnClose.BackColor = Color.Crimson
        btnClose.ForeColor = Color.White
        AddHandler btnClose.Click, AddressOf CloseDoors
        Me.Controls.Add(btnClose)

        btnEmergency.Size = New Size(140, 50)
        btnEmergency.Location = New Point(600, 585)
        btnEmergency.Text = "⚠ EMERGENCY"
        btnEmergency.Font = New Font("Arial", 13, FontStyle.Bold)
        btnEmergency.BackColor = Color.Red
        btnEmergency.ForeColor = Color.White
        AddHandler btnEmergency.Click, AddressOf ToggleEmergency
        Me.Controls.Add(btnEmergency)

        moveTimer.Interval = 20
        AddHandler moveTimer.Tick, AddressOf UpdateMovement

        doorTimer.Interval = 10
        AddHandler doorTimer.Tick, AddressOf UpdateDoors

        UpdateFloorDisplay()
    End Sub

    Private Sub DrawFloorLines(sender As Object, e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        Dim pen As New Pen(Color.Gray, 2)
        For i As Integer = 0 To 3
            g.DrawLine(pen, 0, floorY(i), 150, floorY(i))
        Next
    End Sub

    Private Sub CallButton_Click(floorNum As Integer)
        If isMoving Or isEmergency Then Exit Sub
        targetFloor = floorNum

        If targetFloor > currentFloor Then
            lblDirection.Text = "▲ UP"
        ElseIf targetFloor < currentFloor Then
            lblDirection.Text = "▼ DOWN"
        Else
            lblDirection.Text = ""
        End If

        If currentFloor = targetFloor Then
            OpenDoors(Nothing, Nothing)
            lblDirection.Text = ""
            Exit Sub
        End If

        CloseDoors(Nothing, Nothing)
        isMoving = True
        moveTimer.Start()
    End Sub

    Private Sub CallInside(f As Integer)
        If f < 1 Or f > 4 Then Exit Sub
        If isMoving Or isEmergency Then Exit Sub
        targetFloor = f

        If targetFloor > currentFloor Then
            lblDirection.Text = "▲ UP"
        ElseIf targetFloor < currentFloor Then
            lblDirection.Text = "▼ DOWN"
        Else
            lblDirection.Text = ""
        End If

        If currentFloor = targetFloor Then
            OpenDoors(Nothing, Nothing)
            lblDirection.Text = ""
            Exit Sub
        End If

        CloseDoors(Nothing, Nothing)
        isMoving = True
        moveTimer.Start()
    End Sub

    Private Sub UpdateMovement(sender As Object, e As EventArgs)
        Dim targetY As Integer = floorY(targetFloor - 1)
        Dim speed As Integer = 4
        Dim diff As Integer = pnlElevator.Top - targetY

        If Math.Abs(diff) <= speed Then
            pnlElevator.Top = targetY
            moveTimer.Stop()
            isMoving = False
            currentFloor = targetFloor
            UpdateFloorDisplay()
            lblDirection.Text = ""
            lblStatus.Text = "ARRIVED"
            OpenDoors(Nothing, Nothing)
        Else
            If diff > 0 Then
                pnlElevator.Top -= speed
            Else
                pnlElevator.Top += speed
            End If
        End If
    End Sub

    Private Sub UpdateFloorDisplay()
        lblDisplay.Text = currentFloor.ToString()
        For i As Integer = 0 To 3
            lblFloorIndicator(i).ForeColor = Color.Red
        Next
        lblFloorIndicator(currentFloor - 1).ForeColor = Color.Lime
    End Sub

    Private Sub OpenDoors(sender As Object, e As EventArgs)
        If isEmergency Or doorState = 2 Then Exit Sub
        doorState = 1
        lblStatus.Text = "OPENING..."
        If Not doorTimer.Enabled Then doorTimer.Start()
    End Sub

    Private Sub CloseDoors(sender As Object, e As EventArgs)
        If isEmergency Or doorState = 0 Then Exit Sub
        doorState = 3
        lblStatus.Text = "CLOSING..."
        If Not doorTimer.Enabled Then doorTimer.Start()
    End Sub

    Private Sub ToggleEmergency(sender As Object, e As EventArgs)
        isEmergency = Not isEmergency

        If isEmergency Then
            lblEmergency.Text = "⚠ EMERGENCY"
            lblEmergency.ForeColor = Color.Red
            btnEmergency.BackColor = Color.DarkRed
            btnEmergency.Text = "🔓 RESET"
            doorState = 1
            lblStatus.Text = "EMERGENCY"
            If Not doorTimer.Enabled Then doorTimer.Start()
            For i As Integer = 0 To 3
                btnFloor(i).Enabled = False
            Next
        Else
            lblEmergency.Text = ""
            btnEmergency.BackColor = Color.Red
            btnEmergency.Text = "⚠ EMERGENCY"
            For i As Integer = 0 To 3
                btnFloor(i).Enabled = True
            Next
            CloseDoors(Nothing, Nothing)
        End If
    End Sub

    Private Sub UpdateDoors(sender As Object, e As EventArgs)
        If doorState = 1 Then
            If pnlDoorLeft.Left > -40 Then pnlDoorLeft.Left -= 2
            If pnlDoorRight.Left < 90 Then pnlDoorRight.Left += 2
            If pnlDoorLeft.Left <= -40 And pnlDoorRight.Left >= 90 Then
                doorState = 2
                doorTimer.Stop()
                lblStatus.Text = "DOOR OPEN"
            End If
        ElseIf doorState = 3 Then
            If pnlDoorLeft.Left < 0 Then pnlDoorLeft.Left += 2
            If pnlDoorRight.Left > 50 Then pnlDoorRight.Left -= 2
            If pnlDoorLeft.Left >= 0 And pnlDoorRight.Left <= 50 Then
                doorState = 0
                doorTimer.Stop()
                lblStatus.Text = "CLOSED"
            End If
        End If
    End Sub

End Class
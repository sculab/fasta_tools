Imports System.IO
Public Class Form1
    Dim local_path As String = ""
    Dim timer_id As Integer = 0

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If TextBox6.Text <> "" Then
            format_fasta(TextBox6.Text, local_path + "temp_file.tmp")
            timer_id = 1
        Else
            MsgBox("请先选择源文件。")
        End If
        
    End Sub
    Public Function get_new_line() As String
        If CheckBox1.Checked Then
            Return vbLf
        Else
            Return vbCrLf
        End If
    End Function
	Public Function format_fasta(ByVal input As String, ByVal output As String) As Integer
		Dim sw As New StreamWriter(output, False)
		Dim sr As New StreamReader(input, False)
		Dim line As String = sr.ReadLine
		Dim count As Integer = 0
		Do
			If line <> "" Then
				If line.StartsWith(">") Then
					If count > 0 Then
						sw.Write(get_new_line)
					End If
					If TextBox3.Text <> "" Then
						line = ">" + line.Substring(0).Split(TextBox3.Text)(NumericUpDown1.Value)
					End If
					If CheckBox4.Checked Then
						sw.Write(">" + (count + 1).ToString)
					Else
						sw.Write(line)
					End If

					RichTextBox1.AppendText(line.Replace(">", "") + Chr(13))
					sw.Write(get_new_line)
					count += 1
				Else
					line = line.ToUpper
					'If CheckBox2.Checked Then
					'	Dim mixed_AA() As String = {"R", "Y", "M", "K", "S", "W", "H", "B", "V", "D", "N"}
					'	For Each i As String In mixed_AA
					'		line = line.Replace(i, "-")
					'	Next
					'End If
					sw.Write(line)
				End If
			End If
			line = SR.ReadLine
		Loop Until line Is Nothing
		sw.Write(get_new_line)
		SR.Close()
		sw.Close()
		TextBox1.Text = "共计" + count.ToString + "条序列"
		Return count
	End Function
	Private Sub Button1_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button1.MouseHover
        TextBox1.Text = "将Fasta文件的每条序列归并成一行，转换成一行名称一行序列的标准格式"
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        If TextBox6.Text <> "" Then
			clean_fasta(TextBox6.Text, local_path + "temp_file.tmp")
			timer_id = 1
		Else
            MsgBox("请先选择源文件。")
        End If
    End Sub
    Public Function clean_fasta(ByVal input As String, ByVal output As String) As Integer
        Dim sr As New StreamReader(input)
        Dim sw As New StreamWriter(output, False)
        Dim line As String = sr.ReadLine
        Dim count As Integer = 0
        Dim name As String = ""
        Dim seq As String = ""
        Dim passed As Boolean = True
        Do
            If line <> "" Then
                If line.StartsWith(">") Then
                    name = line
                    seq = sr.ReadLine
                End If
                If CheckBox2.Checked Then
                    passed = Check_Mixed_AA(seq.ToUpper)
                End If
                If TextBox9.Text <> "" Then
                    If seq.ToUpper.Replace("-", "").Length >= CInt(TextBox9.Text) Then
                        passed = True
                    Else
                        passed = False
                    End If
                End If
                If passed Then
                    count += 1
                    RichTextBox1.AppendText(name.Replace(">", "") + Chr(13))
                    sw.Write(name)
                    sw.Write(get_new_line)
                    sw.Write(seq.ToUpper)
                    sw.Write(get_new_line)
                Else
                    RichTextBox1.AppendText(name.Replace(">", "") + Chr(9) + "X" + Chr(13))
                End If

            End If
            line = sr.ReadLine
        Loop Until line Is Nothing
        sr.Close()
        sw.Close()
        TextBox1.Text = "共计" + count.ToString + "条序列"
        Return count
    End Function
    Public Function Check_Mixed_AA(ByVal seq As String) As Boolean
        Dim mixed_AA() As String = {"R", "Y", "M", "K", "S", "W", "H", "B", "V", "D", "N"}
        For Each i As String In mixed_AA
            If seq.Contains(i) Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Sub Button2_MouseHover(ByVal sender As Object, ByVal e As System.EventArgs) Handles Button2.MouseHover
        TextBox1.Text = "移除包含兼并碱基的序列（其中R=A/G，Y=C/T，M=A/C，K=G/T，S=C/G，W=A/T，H=A/C/T，B=C/G/T，V=A/C/G， D=A/G/T, N=A/C/G/T）"
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

        If TextBox6.Text <> "" Then
            hap_fasta(TextBox6.Text, local_path + "temp_file.tmp")
            timer_id = 1
        Else
            MsgBox("请先选择源文件。")
        End If
    End Sub
    Public Function hap_fasta(ByVal input As String, ByVal output As String) As Integer


		Dim count As Integer = 0
        Dim hap_seq() As String
        Dim name() As String
        Dim org_seq() As String
		Dim hap_name() As Integer
		Dim gap_list() As Integer
		ReDim hap_seq(0)
        ReDim org_seq(0)
        ReDim hap_name(0)
		ReDim name(0)
		ReDim gap_list(0)
		Dim sr As New StreamReader(input)
		Dim line As String = sr.ReadLine

		Do
            If line <> "" Then
                If line.StartsWith(">") Then
					ReDim Preserve name(UBound(name) + 1)
					name(UBound(name)) = line.Replace(">", "")
                Else
					ReDim Preserve org_seq(UBound(name))
					org_seq(UBound(org_seq)) += line
				End If
            End If
            line = sr.ReadLine
        Loop Until line Is Nothing
		sr.Close()
		gap_list(0) = -1
		For Each seq_line As String In org_seq
			If seq_line <> "" Then
				Dim mixed() As String = {"R", "Y", "M", "K", "S", "W", "H", "B", "V", "D", "N", "-"}
				For Each i As String In mixed
					If seq_line.Contains(i) Then
						Dim temp_index As Integer = -1
						Do
							temp_index = seq_line.IndexOf(i, temp_index + 1)
							If Array.IndexOf(gap_list, temp_index) < 0 Then
								ReDim Preserve gap_list(UBound(gap_list) + 1)
								gap_list(UBound(gap_list)) = temp_index
							End If
						Loop Until seq_line.IndexOf(i, temp_index + 1) < 0
					End If

				Next
			End If
		Next
		Array.Sort(gap_list)
		For i As Integer = 1 To UBound(org_seq)
			For j As Integer = 0 To UBound(gap_list) - 1
				org_seq(i) = org_seq(i).Remove(gap_list(UBound(gap_list) - j), 1)
			Next
		Next
		For i As Integer = 1 To UBound(org_seq)
			line = org_seq(i)
			ReDim Preserve hap_name(UBound(hap_name) + 1)
			Dim hap_index As Integer = Array.IndexOf(hap_seq, line)
			If hap_index > 0 Then
				hap_name(UBound(hap_name)) = hap_index
			Else
				ReDim Preserve hap_seq(UBound(hap_seq) + 1)
				hap_seq(UBound(hap_seq)) = line
				hap_name(UBound(hap_name)) = UBound(hap_seq)
				count += 1
			End If
		Next

		Dim trait_list() As String
		ReDim trait_list(0)
		For i As Integer = 1 To UBound(name)
            Dim s As String = name(i).ToUpper.Split(TextBox8.Text)(NumericUpDown4.Value)
            If TextBox10.Text <> "" Then
                s = name(i).ToUpper.Split(TextBox8.Text)(NumericUpDown4.Value).Substring(0, CInt(TextBox10.Text))
            End If
            If Array.IndexOf(trait_list, s) < 0 Then
				ReDim Preserve trait_list(UBound(trait_list) + 1)
				trait_list(UBound(trait_list)) = s
			End If
		Next
		Array.Sort(trait_list)

		Dim hap_trait_table(,) As String
		ReDim hap_trait_table(UBound(hap_seq), UBound(trait_list))

		For i As Integer = 0 To UBound(trait_list)
			For j As Integer = 0 To UBound(hap_seq)
				hap_trait_table(j, i) = 0
			Next
		Next
		For i As Integer = 1 To UBound(trait_list)
			hap_trait_table(0, i) = trait_list(i)
		Next
		For i As Integer = 1 To UBound(hap_seq)
			hap_trait_table(i, 0) = "Hap_" + i.ToString
		Next
		hap_trait_table(0, 0) = ""
		For i As Integer = 1 To UBound(name)
            Dim s As String = name(i).ToUpper.Split(TextBox8.Text)(NumericUpDown4.Value)
            If TextBox10.Text <> "" Then
                s = name(i).ToUpper.Split(TextBox8.Text)(NumericUpDown4.Value).Substring(0, CInt(TextBox10.Text))
            End If
            hap_trait_table(hap_name(i), Array.IndexOf(trait_list, s)) = CInt(hap_trait_table(hap_name(i), Array.IndexOf(trait_list, s))) + 1
		Next
		Dim hap_trait_sw As New StreamWriter(output + ".hap_trait.txt", False)
		For j As Integer = 0 To UBound(hap_seq)
			For i As Integer = 0 To (UBound(trait_list) - 1)
				hap_trait_sw.Write(hap_trait_table(j, i) + ",")
			Next
			hap_trait_sw.Write(hap_trait_table(j, UBound(trait_list)) + vbCrLf)
		Next
		hap_trait_sw.Close()

		Dim seq_trait_sw As New StreamWriter(output + ".seq_trait.txt", False)
		For i As Integer = 1 To UBound(trait_list)
			seq_trait_sw.Write("," + trait_list(i))
		Next
		seq_trait_sw.Write(vbCrLf)
		For j As Integer = 1 To UBound(name)
			seq_trait_sw.Write(name(j).ToUpper.Split(TextBox3.Text)(NumericUpDown1.Value))
			For i As Integer = 1 To (UBound(trait_list))
				If name(j).ToUpper.Contains(trait_list(i)) Then
					seq_trait_sw.Write(",1")
				Else
					seq_trait_sw.Write(",0")
				End If
			Next
			seq_trait_sw.Write(vbCrLf)
		Next
		seq_trait_sw.Close()

		Dim reduced_seq() As String
        ReDim reduced_seq(UBound(hap_seq))
        For j As Integer = 1 To hap_seq(1).Length
            Select Case hap_seq(1)(j - 1).ToString.ToUpper
                Case "A", "G", "C", "T"
                    Dim var_site As Boolean = False
                    For i As Integer = 1 To UBound(hap_seq)
                        Select Case hap_seq(i)(j - 1).ToString.ToUpper
                            Case "A", "G", "C", "T"
                            Case Else
                                var_site = False
                                Exit For
                        End Select
                        If CheckBox8.Checked = True Then
                            For k As Integer = i + 1 To UBound(hap_seq) - NumericUpDown5.Value
                                If hap_seq(i)(j - 1) <> hap_seq(k)(j - 1) Then
                                    var_site = True
                                End If
                            Next
                        Else
                            For k As Integer = i + 1 To UBound(hap_seq)
                                If hap_seq(i)(j - 1) <> hap_seq(k)(j - 1) Then
                                    var_site = True
                                End If
                            Next
                        End If


                    Next

                    If var_site Then
                        For i As Integer = 1 To UBound(hap_seq)
                            reduced_seq(i) += hap_seq(i)(j - 1)
                        Next
                        RichTextBox1.AppendText(j.ToString + Chr(9) + "SNP_" + (reduced_seq(1).Length).ToString + Chr(13))
                    End If
                Case Else
            End Select
        Next
        Dim sw As New StreamWriter(output + ".fas", False)
        For i As Integer = 1 To UBound(reduced_seq)
            sw.Write(">Hap_" + i.ToString)
            sw.Write(get_new_line)
            sw.Write(reduced_seq(i))
            sw.Write(get_new_line)
        Next
        sw.Close()
		Dim sw1 As New StreamWriter(output + ".phy", False)
		sw1.Write(UBound(reduced_seq).ToString + " " + reduced_seq(1).Length.ToString + vbCrLf)
		For i As Integer = 1 To UBound(reduced_seq)
			sw1.Write("Hap_" + i.ToString + " " + reduced_seq(i))
			sw1.Write(get_new_line)
		Next
		sw1.Close()



        Dim reduced_seq1() As String
        ReDim reduced_seq1(UBound(org_seq))
		For j As Integer = 1 To org_seq(1).Length
			Select Case org_seq(1)(j - 1).ToString.ToUpper
				Case "A", "G", "C", "T"
                    Dim var_site As Boolean = False
                    For i As Integer = 1 To UBound(org_seq)
                        Select Case org_seq(i)(j - 1).ToString.ToUpper
                            Case "A", "G", "C", "T"
                            Case Else
                                var_site = False
                                Exit For
                        End Select
                        If CheckBox8.Checked = True Then
                            For k As Integer = i + 1 To UBound(org_seq) - NumericUpDown5.Value
                                If org_seq(i)(j - 1) <> org_seq(k)(j - 1) Then
                                    var_site = True
                                End If
                            Next
                        Else
                            For k As Integer = i + 1 To UBound(org_seq)
                                If org_seq(i)(j - 1) <> org_seq(k)(j - 1) Then
                                    var_site = True
                                End If
                            Next
                        End If


                    Next

                    If var_site Then
						For i As Integer = 1 To UBound(org_seq)
							reduced_seq1(i) += org_seq(i)(j - 1)
						Next
					End If
				Case Else
			End Select
		Next

		Dim sw2 As New StreamWriter(output + ".seq.phy", False)
		sw2.Write(UBound(reduced_seq1).ToString + " " + reduced_seq1(1).Length.ToString + vbCrLf)
		For i As Integer = 1 To UBound(reduced_seq1)
			sw2.Write(name(i).ToUpper.Split(TextBox3.Text)(NumericUpDown1.Value) + " " + reduced_seq1(i))
			sw2.Write(get_new_line)
        Next
        sw2.Close()

		Dim sw3 As New StreamWriter(output + ".seq.fas", False)
		For i As Integer = 1 To UBound(reduced_seq1)
			sw3.Write(">" + name(i).ToUpper.Split(TextBox3.Text)(NumericUpDown1.Value) + Chr(13) + reduced_seq1(i))
			sw3.Write(get_new_line)
		Next
		sw3.Close()

		Return count
    End Function

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim Savedialog As New SaveFileDialog
        Savedialog.Filter = "CSV 文件(*.*)|*.csv"
        Savedialog.FileName = ""
        Savedialog.DefaultExt = ".csv"
        Dim resultdialog As DialogResult = Savedialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            RichTextBox1.Text = ""
            snp_fasta(TextBox6.Text, Savedialog.FileName)
        End If
    End Sub
    Public Function snp_fasta(ByVal input As String, ByVal output As String) As Integer
        Dim sr As New StreamReader(input)
        Dim line As String = sr.ReadLine
        Dim count As Integer = 0
        Dim snp_seq() As String
        Dim name() As String
        ReDim snp_seq(0)
        ReDim name(0)
        Dim append_list() As String = TextBox5.Text.Split(vbLf)
        Dim append_id As Integer = 0
        Dim sp_sign As String = ""
		Select Case ComboBox2.SelectedIndex
			Case 0
				sp_sign = "	"
			Case 1
				sp_sign = ","
			Case 2
				sp_sign = " "
		End Select

		Do
            If line <> "" Then
                If line.StartsWith(">") Then
                    ReDim Preserve name(UBound(name) + 1)
                    name(UBound(name)) = line.Replace(">", "")
                Else

                    ReDim Preserve snp_seq(UBound(snp_seq) + 1)
                    snp_seq(UBound(snp_seq)) = line
                    count += 1
                End If
            End If
            line = sr.ReadLine
        Loop Until line Is Nothing
        sr.Close()

		Dim trait_list() As String
		ReDim trait_list(0)
		For i As Integer = 1 To UBound(name)
			Dim s As String = name(i).ToUpper.Split(TextBox7.Text)(NumericUpDown3.Value)
			If Array.IndexOf(trait_list, s) < 0 Then
				ReDim Preserve trait_list(UBound(trait_list) + 1)
				trait_list(UBound(trait_list)) = s

			End If
		Next
		Array.Sort(trait_list)
		For i As Integer = 1 To UBound(trait_list)
			RichTextBox1.AppendText(i.ToString + vbTab + trait_list(i) + Chr(13))
		Next
		Dim sw As New StreamWriter(output, False)
		If CheckBox6.Checked Then
			sw.Write(sp_sign)
			If CheckBox9.Checked Then
				sw.Write(sp_sign)
			End If
		End If
        sw.Write("SNP_1")
        For j As Integer = 2 To snp_seq(1).Length
            sw.Write(sp_sign + "SNP_" + j.ToString)
        Next
        If CheckBox5.Checked Then
            sw.Write(sp_sign + append_list(append_id))
            append_id += 1
        End If
		sw.Write(get_new_line)
		For i As Integer = 1 To UBound(snp_seq)
			If CheckBox6.Checked Then
				Dim temp_name As String = ""
				If TextBox3.Text <> "" Then
					temp_name = name(i).Split(TextBox3.Text)(NumericUpDown1.Value)
				Else
					temp_name = name(i)
				End If
				sw.Write(temp_name)
				If CheckBox9.Checked Then
					Dim temp_trait As String = ""
					temp_trait = name(i).Split(TextBox7.Text)(NumericUpDown3.Value).ToUpper
					sw.Write(sp_sign)
					sw.Write(Array.IndexOf(trait_list, temp_trait))
				End If
			End If

			For j As Integer = 1 To 1
                Dim temp_str As String
                Select Case ComboBox1.SelectedIndex
                    Case 1
                        temp_str = snp_seq(i)(j - 1).ToString.Replace("A", "A/A").Replace("G", "G/G").Replace("C", "C/C").Replace("T", "T/T")
                    Case 2
                        temp_str = snp_seq(i)(j - 1).ToString.Replace("A", "0").Replace("G", "1").Replace("C", "2").Replace("T", "3")
                    Case Else
                        temp_str = snp_seq(i)(j - 1)
                End Select
				If CheckBox6.Checked Then
					sw.Write(sp_sign + temp_str)
				Else
					sw.Write(temp_str)
                End If
            Next
            For j As Integer = 2 To snp_seq(1).Length
                Dim temp_str As String
				Select Case ComboBox1.SelectedIndex
					Case 1
						temp_str = snp_seq(i)(j - 1).ToString.Replace("A", "A/A").Replace("G", "G/G").Replace("C", "C/C").Replace("T", "T/T")
					Case 2
						temp_str = snp_seq(i)(j - 1).ToString.Replace("A", "0").Replace("G", "1").Replace("C", "2").Replace("T", "3")
					Case Else
						temp_str = snp_seq(i)(j - 1)
				End Select
				sw.Write(sp_sign + temp_str)
				'If CheckBox8.Checked Then
				'	sw.Write(get_new_line() + sp_sign + temp_str)
				'End If
			Next
            If CheckBox5.Checked Then
                sw.Write(sp_sign + append_list(append_id))
                append_id += 1
            End If
			sw.Write(get_new_line)
		Next
        sw.Close()

        TextBox1.Text = "共计" + snp_seq(1).Length.ToString + "个SNP"
        Return count
    End Function
    Public Function fasta_nex(ByVal input As String, ByVal output As String) As Integer
        Dim sr As New StreamReader(input)
        Dim line As String = sr.ReadLine
        Dim count As Integer = 0
        Dim fasta_seq() As String
        Dim name() As String
        ReDim fasta_seq(0)
        ReDim name(0)
        Do
            If line <> "" Then
                If line.StartsWith(">") Then
                    ReDim Preserve name(UBound(name) + 1)
                    name(UBound(name)) = line.Replace(">", "")
                Else

                    ReDim Preserve fasta_seq(UBound(fasta_seq) + 1)
                    fasta_seq(UBound(fasta_seq)) = line
                    count += 1
                End If
            End If
            line = sr.ReadLine
        Loop Until line Is Nothing
        sr.Close()

        Dim sw As New StreamWriter(output + ".nex", False)
        Dim template As String = TextBox2.Text
        template = template.Replace("$ntax$", UBound(fasta_seq).ToString)
        template = template.Replace("$nchar$", fasta_seq(1).Length)
        If CheckBox3.Checked Then
            template = template.Replace("$type$", "Protein")
        Else
            template = template.Replace("$type$", "DNA")
        End If
        If TextBox4.Text <> "" Then
            template = template.Replace("[Outgroup]", "Outgroup " + TextBox4.Text + ";")
        End If
        Dim matrix As String = ""

        For i As Integer = 1 To UBound(fasta_seq)
            If TextBox3.Text <> "" Then
                name(i) = name(i).Split(TextBox3.Text)(NumericUpDown1.Value)
            End If
            matrix += Chr(9) + name(i) + Chr(9) + fasta_seq(i) + Chr(13)
        Next

        template = template.Replace("$matrix$", matrix)
        sw.Write(template)
        sw.Close()
        Return count
    End Function

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Fasta文件(*.*)|*.fas;*.fasta"
        opendialog.FileName = ""
        opendialog.Multiselect = False
        opendialog.DefaultExt = ".fas"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            RichTextBox1.Text = ""
            fasta_nex(opendialog.FileName, opendialog.FileName.Substring(0, opendialog.FileName.LastIndexOf(".")))
        End If
    End Sub
   
    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Fasta文件(*.*)|*.fas;*.fasta"
        opendialog.FileName = ""
        opendialog.Multiselect = False
        opendialog.DefaultExt = ".fas"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            RichTextBox1.Text = ""
            clean_align(opendialog.FileName, opendialog.FileName.Substring(0, opendialog.FileName.LastIndexOf(".")) + "_alg.fas")
        End If
    End Sub
    Public Function clean_align(ByVal input As String, ByVal output As String) As Integer
        Dim sr As New StreamReader(input)
        Dim line As String = sr.ReadLine
        Dim count As Integer = 0
        Dim fasta_seq() As String
        Dim name() As String
        ReDim fasta_seq(0)
        ReDim name(0)
        Do
            If line <> "" Then
                If line.StartsWith(">") Then
                    ReDim Preserve name(UBound(name) + 1)
                    name(UBound(name)) = line.Replace(">", "")
                Else

                    ReDim Preserve fasta_seq(UBound(fasta_seq) + 1)
                    fasta_seq(UBound(fasta_seq)) = line
                    count += 1
                End If
            End If
            line = sr.ReadLine
        Loop Until line Is Nothing
        sr.Close()

        Dim sw As New StreamWriter(output, False)

        Dim start_point As Integer = 0
        Dim end_point As Integer = 0
        For i As Integer = 1 To fasta_seq(1).Length
      
            Dim get_point As Boolean = True
            For k As Integer = i - 1 To i + 1
                Dim gap_number As Integer = 0
                Dim temp_aa As String = ""
                For j As Integer = 1 To UBound(fasta_seq)

                    If fasta_seq(j)(k) = "-" Then
                        gap_number += 1
                    Else
                        If temp_aa = "" Then
                            temp_aa = fasta_seq(j)(k)
                        ElseIf temp_aa <> fasta_seq(j)(k) Then
                            temp_aa += fasta_seq(j)(k)
                        End If
                    End If
                    If temp_aa.Length > 1 Or gap_number > NumericUpDown2.Value Then
                        get_point = False
                    End If
                Next
            Next
            If get_point Then
                start_point = i - 1
                i = fasta_seq(1).Length + 1
            End If
        Next
        For i As Integer = 1 To fasta_seq(1).Length
            
            Dim get_point As Boolean = True
            For k As Integer = fasta_seq(1).Length - i - 2 To fasta_seq(1).Length - i
                Dim gap_number As Integer = 0
                Dim temp_aa As String = ""
                For j As Integer = 1 To UBound(fasta_seq)
                    If fasta_seq(j)(k) = "-" Then
                        gap_number += 1
                    Else
                        If temp_aa = "" Then
                            temp_aa = fasta_seq(j)(k)
                        ElseIf temp_aa <> fasta_seq(j)(k) Then
                            temp_aa += fasta_seq(j)(k)
                        End If
                    End If
                    If temp_aa.Length > 1 Or gap_number > NumericUpDown2.Value Then
                        get_point = False
                    End If
                Next
            Next
            If get_point Then
                end_point = fasta_seq(1).Length - i
                i = fasta_seq(1).Length + 1
            End If
        Next

        For i As Integer = 1 To UBound(fasta_seq)
            sw.Write(">" + name(i))
            sw.Write(get_new_line)
            sw.Write(fasta_seq(i).Substring(start_point, end_point - start_point + 1))
            sw.Write(get_new_line)
        Next
        sw.Close()
        Return count
    End Function
    Private Sub ListFiles(ByVal From_dir As String, ByVal ext As String, ByVal run_id As Integer)

        If Not (From_dir Is Nothing) Then
            Dim mFileInfo As System.IO.FileInfo
            Dim mDirInfo As New System.IO.DirectoryInfo(From_dir)
            For Each mFileInfo In mDirInfo.GetFiles()
                If mFileInfo.Extension.ToUpper = ext Then
                    Dim new_name As String = mFileInfo.FullName.Substring(0, mFileInfo.FullName.LastIndexOf("."))
                    Try
                        Select Case run_id
                            Case 0
                                format_fasta(mFileInfo.FullName, new_name + ".txt")
                                clean_align(new_name + ".txt", new_name + ".txt")
                                fasta_nex(new_name + ".txt", new_name)
                            Case 1
                                combine_fasta(new_name + ".txt", From_dir + "\")

                            Case Else

                        End Select

                    Catch ex As Exception
                        MsgBox(mFileInfo.FullName)
                    End Try
                End If
            Next
            Select Case run_id
                Case 1
                    Dim sw As New StreamWriter(From_dir + "\combine.fas")
                    For Each mFileInfo In mDirInfo.GetFiles()
                        Try
                            If mFileInfo.Extension.ToUpper = ".SEQ" Then
                                Dim seq_name As String = mFileInfo.FullName.Substring(mFileInfo.FullName.LastIndexOf("\") + 1).ToUpper.Replace(".SEQ", "")
                                sw.WriteLine(">" + seq_name + vbCrLf)
                                Dim sr As New StreamReader(mFileInfo.FullName)
                                sw.WriteLine(sr.ReadToEnd)
                                sr.Close()
                            End If
                        Catch ex As Exception
                            MsgBox(mFileInfo.FullName)
                        End Try
                    Next
                    sw.Close()
                Case Else

            End Select
        End If
    End Sub
    Public Function combine_fasta(ByVal input As String, ByVal output As String) As Integer
        Dim sr As New StreamReader(input)
        Dim line As String = sr.ReadLine
        Dim count As Integer = 0
        Dim fasta_seq() As String
        Dim name() As String
        ReDim fasta_seq(0)
        ReDim name(0)
        Do
            If line <> "" Then
                If line.StartsWith(">") Then
                    ReDim Preserve name(UBound(name) + 1)
                    name(UBound(name)) = line.Replace(">", "")
                Else

                    ReDim Preserve fasta_seq(UBound(fasta_seq) + 1)
                    fasta_seq(UBound(fasta_seq)) = line
                    count += 1
                End If
            End If
            line = sr.ReadLine
        Loop Until line Is Nothing
        sr.Close()
        For i As Integer = 1 To UBound(name)
            Dim sw As New StreamWriter(output + name(i).Split(TextBox3.Text)(NumericUpDown1.Value) + ".seq", True)
            sw.Write(fasta_seq(i))
            sw.Write(get_new_line)
            sw.Close()
        Next
        Return UBound(name)
    End Function
    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Dim se As DialogResult
        se = Me.FolderBrowserDialog1.ShowDialog()
        If se = DialogResult.OK Then
            ListFiles(FolderBrowserDialog1.SelectedPath, ".FAS", 0)
        End If
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        Dim se As DialogResult
        se = Me.FolderBrowserDialog1.ShowDialog()
        If se = DialogResult.OK Then
            ListFiles(FolderBrowserDialog1.SelectedPath, ".TXT", 1)
        End If
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Fasta文件(*.*)|*.fas;*.fasta"
        opendialog.FileName = ""
        opendialog.Multiselect = False
        opendialog.DefaultExt = ".fas"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            TextBox6.Text = opendialog.FileName
        End If
    End Sub
	Public Sub safe_copy(source, target)
		If File.Exists(source) Then
			File.Copy(source, target, True)
			File.Delete(source)
		End If
	End Sub
	Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Select Case timer_id
            Case 0
            Case 1
                Timer1.Enabled = False
                Dim savedialog As New SaveFileDialog
				savedialog.Filter = "Fasta文件(*.*)|*.fas"
				savedialog.FileName = ""
				savedialog.DefaultExt = ".fas"
				Dim resultdialog As DialogResult = savedialog.ShowDialog()
				If resultdialog = DialogResult.OK Then
					safe_copy(local_path + "temp_file.tmp", savedialog.FileName)
					safe_copy(local_path + "temp_file.tmp.seq.fas", savedialog.FileName)
					safe_copy(local_path + "temp_file.tmp.seq.phy", savedialog.FileName.Remove(savedialog.FileName.LastIndexOf(".")) + ".phy")
					safe_copy(local_path + "temp_file.tmp.fas", savedialog.FileName + ".hap.fas")
					safe_copy(local_path + "temp_file.tmp.phy", savedialog.FileName + ".hap.phy")
					safe_copy(local_path + "temp_file.tmp.hap_trait.txt", savedialog.FileName + ".hap_trait.txt")
					safe_copy(local_path + "temp_file.tmp.seq_trait.txt", savedialog.FileName + ".seq_trait.txt")
				End If
				If CheckBox7.Checked Then
                    TextBox6.Text = savedialog.FileName
                End If
                timer_id = 0
                Timer1.Enabled = True
            Case Else

        End Select
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox1.SelectedIndex = 0
        ComboBox2.SelectedIndex = 1
        local_path = Application.ExecutablePath.Remove(Application.ExecutablePath.LastIndexOf("\") + 1, Application.ExecutablePath.Length - Application.ExecutablePath.LastIndexOf("\") - 1)
    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter

    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Dim opendialog As New OpenFileDialog
        opendialog.Filter = "Text文件(*.*)|*.txt;*.txt"
        opendialog.FileName = ""
        opendialog.Multiselect = False
        opendialog.DefaultExt = ".txt"
        opendialog.CheckFileExists = True
        opendialog.CheckPathExists = True
        Dim resultdialog As DialogResult = opendialog.ShowDialog()
        If resultdialog = DialogResult.OK Then
            Dim sr As New StreamReader(opendialog.FileName)
            Dim row(0) As String
            Dim column(0) As String
            Dim contain(0) As String
            Dim matrix(5500, 5500) As String
            Dim i As Integer = 0
            Dim j As Integer = 0
            Dim line As String = sr.ReadLine
            Do
                Dim temp() As String = line.Split("	")
                If temp.Length = 3 Then
                    If Array.IndexOf(column, temp(1)) < 0 Then
                        i += 1
                        ReDim Preserve column(i)
                        column(i) = temp(1)
                    End If
                    If Array.IndexOf(row, temp(2)) < 0 Then
                        j += 1
                        ReDim Preserve row(j)
                        row(j) = temp(2)
                    End If
                    matrix(i, j) = temp(0)
                End If
                line = sr.ReadLine
            Loop Until line = ""
            sr.Close()
            Dim sw As New StreamWriter(opendialog.FileName + ".csv")
            For n As Integer = 1 To j
                sw.Write(row(n) + ",")
            Next
            sw.Write(Chr(13))
            For m As Integer = 1 To i
                sw.Write(column(m) + ",")
                For n As Integer = 1 To j
                    sw.Write(matrix(m, n) + ",")
                Next
                sw.Write(Chr(13))
            Next
            sw.Close()
        End If
    End Sub

	Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

	End Sub

	Private Sub NumericUpDown2_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown2.ValueChanged

	End Sub
End Class

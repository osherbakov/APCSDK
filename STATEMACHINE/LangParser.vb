Option Explicit On 
Option Strict On

Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Reflection

Namespace Diacom.APCStates

    Friend Delegate Function Conversion(ByVal RuleSetName As String, ByVal Parameter As Double, ByVal ConversionFunction As Conversion) As String


    ' We need to implement this structure for the following reason:
    ' All substitutions should be done from left-to-right, i.e. they should be sorted
    ' We must replace the exact text of the <...< >...> or =...= with the
    ' results of the substitution
    Friend Structure Substitution : Implements IComparable
        Dim Index As Integer            ' Index of the substitution string
        Dim Text As String              ' The Substitution text
        Dim FunctionName As String      ' The name of the function to call
        Dim NumberBase As Double        ' The Base of the Number
        Dim NumberDivisor As Double     ' The Divisor of the Number

        Private FileNumberAdd As Double  ' The Number to be added
        Private FileNumberDivide As Double  ' The Divisor of the result
        Private FormatString As String      ' Formatting string for the FileName.

        Public Sub New(ByVal StringIndex As Integer, ByVal StringText As String, _
            Optional ByVal FuncName As String = Nothing, Optional ByVal Base As Double = 1, _
                Optional ByVal Divisor As Double = 1)
            Index = StringIndex
            Text = StringText
            NumberBase = Base
            NumberDivisor = Divisor
            FunctionName = FuncName

            FileNumberAdd = 0
            FileNumberDivide = 1
            FormatString = String.Empty

            ' Check here for special cases - formatting and the filename
            If StringText.StartsWith("#") AndAlso FuncName = String.Empty Then
                FunctionName = Nothing
            ElseIf StringText.Length > 2 Then
                Dim _str As String = Text.Substring(1, Text.Length - 2).Trim
                Select Case _str.Chars(0)
                    Case "0"c
                        FormatString = "{0:" & _str & "}"
                        FunctionName = Nothing

                    Case "#"c
                        FormatString = "{0:" & _str & "}"
                        FunctionName = Nothing

                    Case "("c
                        Dim _FileNameBase As String = String.Empty
                        Dim _MinDigits As Integer = 0

                        If _str.Length > 2 Then
                            _str = _str.Substring(1, _str.Length - 2).Trim
                            Dim _params() As String = _str.Split(",".ToCharArray)
                            Select Case _params.Length
                                Case 1
                                    _FileNameBase = _params(0).Trim
                                Case 2
                                    _FileNameBase = _params(0).Trim
                                    FileNumberAdd = GetNumber(_params(1))
                                Case 3
                                    _FileNameBase = _params(0).Trim
                                    FileNumberAdd = GetNumber(_params(1))
                                    FileNumberDivide = GetNumber(_params(2))
                                Case 4
                                    _FileNameBase = _params(0).Trim
                                    FileNumberAdd = GetNumber(_params(1))
                                    FileNumberDivide = GetNumber(_params(2))
                                    _MinDigits = CInt(GetNumber(_params(3)))
                            End Select
                        End If
                        If (_MinDigits = 0) Then
                            FormatString = _FileNameBase & "{0}"
                        Else
                            FormatString = _FileNameBase & "{0:" & New String("0"c, _MinDigits) & "}"
                        End If
                        FunctionName = Nothing
                End Select
            End If
        End Sub

        Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
            Return Index.CompareTo(CType(obj, Substitution).Index)
        End Function

        Public Function Convert(ByVal Parameter As Double) As String
            Return String.Format(Me.FormatString, (Parameter + Me.FileNumberAdd) / Me.FileNumberDivide)
        End Function

        Public Shared Function GetNumber(ByVal Data As String) As Double
            Dim _num As Double = 0
            If Data Is Nothing Then Return 0
            For Each _char As Char In Data
                If Char.IsDigit(_char) Then
                    _num = _num * 10 + (Char.GetNumericValue(_char) - Char.GetNumericValue("0"c))
                End If
            Next
            Return _num
        End Function
    End Structure


    Friend Class ConversionRule : Implements IComparable
        Private Base As Double
        Private Divisor As Double
        Private Text As New StringBuilder
        Private SubType As SubstitutionType
        Private Substitutions As New System.Collections.ArrayList
        Private Shared rxMinor As Regex = New Regex(">\s*(?<Rule>[\w%]+)\s*>")
        Private Shared rxMajor As Regex = New Regex("<\s*(?<Rule>[\w%]+)\s*<")
        Private Shared rxEqual As Regex = New Regex("=\s*(?<Rule>.+?)\s*=")
        Private Shared rxFormat As Regex = New Regex _
            ("#\s*(?<Rule>[\w%]+)\s*:\s*(?<Num>\d+)(?=\W+)(?=\s*#)|#\s*(?<Num>\d+)(?=\W)\s*:\s*(?<Rule>[\w%]+)(?=\s*#)|#\s*(?<Num>\d+)\s*(?=#)|#\s*$")
        Private Shared rxNumber As Regex = New Regex _
            ("^\s*(?<Rule>[\w%]+)\s*:\s*(?<Num>\d+)\s*$|^\s*(?<Num>\d+)\s*:\s*(?<Rule>[\w%]+)\s*$")
        Private Shared rxFreeFormat As Regex = New Regex("\b(?<Rule>([A-Za-z])\1+)\b")

        ReadOnly Property Value() As Double
            Get
                Return Base
            End Get
        End Property
        ReadOnly Property Factor() As Double
            Get
                Return Divisor
            End Get
        End Property
        ReadOnly Property Type() As SubstitutionType
            Get
                Return SubType
            End Get
        End Property

        <Flags()> Public Enum SubstitutionType
            Minor = 1
            Major = 2
            Both = Minor Or Major
        End Enum

        Public Sub New(ByVal NumberBase As Double, ByVal NumberDivisor As Double, ByVal RuleString As String)
            Dim mCollection As MatchCollection
            Dim m As Match
            Dim _Base, _Divisor As Double
            Dim _DigitNum, _Index, _Number As Integer
            Dim _RuleName As Group
            Dim _Digits As Group

            Base = NumberBase
            Divisor = NumberDivisor
            Text.Append(RuleString)

            ' Check for the correct combinations of the special symbols:
            '  <...<, >...> and =...= should be matched - we do the simplest syntax check here

            ' Find all Major <...< substitutions
            mCollection = rxMajor.Matches(RuleString)
            For Each m In mCollection
                SubType = SubType Or SubstitutionType.Major
                Substitutions.Add(New Substitution(m.Index, m.Value, m.Groups("Rule").Value))
            Next

            ' Find all Minor >...> substitutions
            mCollection = rxMinor.Matches(RuleString)
            For Each m In mCollection
                SubType = SubType Or SubstitutionType.Minor
                Substitutions.Add(New Substitution(m.Index, m.Value, m.Groups("Rule").Value))
            Next

            ' Find all Equal =...= substitutions
            mCollection = rxEqual.Matches(RuleString)
            For Each m In mCollection
                Dim NumberMatch As Match = rxNumber.Match(m.Groups("Rule").Value)
                If NumberMatch.Success AndAlso NumberMatch.Groups("Num").Value <> String.Empty Then
                    Substitutions.Add(New Substitution(m.Index, m.Value, NumberMatch.Groups("Rule").Value, Substitution.GetNumber(NumberMatch.Groups("Num").Value), 0))
                Else
                    Substitutions.Add(New Substitution(m.Index, m.Value, m.Groups("Rule").Value))
                End If
            Next

            ' Find all Format #...# substitutions
            mCollection = rxFormat.Matches(RuleString)
            _DigitNum = 0
            For _Index = mCollection.Count - 1 To 0 Step -1
                m = mCollection.Item(_Index)
                _Base = Math.Pow(10.0, _DigitNum)

                _Digits = m.Groups("Num")
                If _Digits.Success Then
                    _Number = CInt(Substitution.GetNumber(_Digits.Value.Trim))
                    _Divisor = Math.Pow(10.0, _Number)
                    _DigitNum += _Number
                Else
                    _Divisor = 1
                End If

                _RuleName = m.Groups("Rule")
                Substitutions.Add(New Substitution(m.Index, m.Value, m.Groups("Rule").Value, _Base, _Divisor))
            Next

            ' Find all FreeFormat   DDD YYYYY MM hh:ss   substitutions
            mCollection = rxFreeFormat.Matches(RuleString)
            For _Index = 0 To mCollection.Count - 1
                m = mCollection.Item(_Index)
                Substitutions.Add(New Substitution(m.Index, m.Value, m.Groups("Rule").Value))
            Next
            Substitutions.Sort()    ' All substitutions will be executed Left-to-right
        End Sub

        Public Function CompareTo(ByVal obj As Object) As Integer Implements IComparable.CompareTo
            Return Base.CompareTo(CType(obj, ConversionRule).Base)
        End Function

        Public Function Convert(ByVal Number As Double, ByVal ConversionFunc As Conversion) As String
            Dim _res As New StringBuilder
            Dim _subst As Substitution
            Dim _subResult As String
            Dim _Parameter As Double
            Dim _number As Double
            Dim Index As Integer
            Dim _subType As Char

            _res.Append(Me.Text)

            Index = 0
            For Each _subst In Substitutions
                _subType = _subst.Text.Chars(0)

                '   Check that string -
                ' we can have 5 possible cases:
                ' <%name< or <name<     - call the ruleset with Div or Mod number
                ' <<                    - call our own ruleset with Div or Mod Number
                ' <#format< or <0<      - Print the Div or Mod number with specified number of digits 
                ' <(file)<              - Expand the filename

                Select Case _subType
                    Case "<"c
                        _number = Math.Floor(Number / Me.Divisor)
                        _Parameter = _number

                    Case ">"c
                        _number = Number - Math.Floor(Number / Me.Divisor) * Me.Divisor
                        ' Special case - Fractional Conversion should always 
                        '  produce fractional Minor shifts
                        If (Base = Double.MinValue) Then
                            If (_number > 0.5) Then
                                _number -= Double.Epsilon
                            Else
                                _number += Double.Epsilon
                            End If
                        ElseIf Base = Double.MaxValue Then
                            _number = Number
                        Else    ' Force it to be integer
                            _number = Math.Floor(_number)
                        End If
                        _Parameter = _number

                    Case "="c
                        If _subst.NumberDivisor = 0 Then
                            _Parameter = _subst.NumberBase
                        Else
                            _Parameter = Number
                        End If

                    Case Else   ' If it is #...# or free format (like DDD, YYYY, SSSS)
                        _number = Math.Floor(Number / _subst.NumberBase)
                        If (_subst.NumberDivisor > 1) Then
                            _number = _number - Math.Floor(_number / _subst.NumberDivisor) * _subst.NumberDivisor
                        End If
                        _Parameter = _number
                End Select

                If (Not _subst.FunctionName Is Nothing) Then
                    _subResult = ConversionFunc(_subst.FunctionName, _Parameter, ConversionFunc)
                Else
                    _subResult = _subst.Convert(_Parameter)
                End If

                If (Not _subResult Is Nothing) Then
                    _res.Replace(_subst.Text, _subResult, _subst.Index + Index, _subst.Text.Length)
                    ' Adjust the offset for the substitution - subst.Text was removed and _subResult was added
                    Index -= _subst.Text.Length
                    Index += _subResult.Length
                End If
            Next
            Return _res.ToString()
        End Function

    End Class

    Friend Class ConversionRuleSet
        Private Name As String
        Private NumberBase As Double
        Private NumberDivisor As Double

        Private ReadOnly GlobalConversion As Conversion

        Private FormatRule As ConversionRule
        Private NegativeRule As ConversionRule
        Private FractionRule As ConversionRule
        Private IntegerRule As ConversionRule
        Private MasterRule As ConversionRule
        Private ConversionRules As New Collections.ArrayList
        Private Shared rxBase As New Regex("^\s*(?<Base>[\d\s\-\(\),.]+)?(?<Div>/[\d\s\-\(\),.]+)?(?<Shift>\s*>[>\s]*)?:")
        Private Shared rxNegative As New Regex("^\s*-[\sxX]*:")
        Private Shared rxMaster As New Regex("^\s*([xX]\s*\.\s*[xX]|\.)\s*:")
        Private Shared rxFraction As New Regex("^\s*(0?\s*\.\s*[Xx]|0\s*\.)\s*:")
        Private Shared rxInteger As New Regex("^\s*([xX]\s*\.\s*0?|\.\s*0)\s*:")
        Private Shared rxExpand As New Regex("\[[^\[]+\]")
        Private Shared rxFormat As New Regex("^\s*#\s*:")

        ' Here we receive already tokenized set of rules
        Public Sub New(ByVal RuleSetName As String, ByVal RuleSetTokens() As String, ByVal GlobalFunction As Conversion)
            Dim CurrentToken As String
            Dim _base As Double
            Dim _divisor As Double
            Dim _shift As Integer

            Name = RuleSetName
            NumberBase = 1.0
            NumberDivisor = 1.0
            GlobalConversion = GlobalFunction
            ' If the name has a format "NAME/Operation" we need to select params
            If (Name.IndexOf("/") <> -1) Then
                Dim _split() As String = Name.Split("/"c)
                Name = _split(0).Trim
                Select Case _split(1).Trim.ToUpper
                    Case "SECOND"
                        NumberBase = 1.0
                        NumberDivisor = 100.0
                    Case "MINUTE"
                        NumberBase = 100.0
                        NumberDivisor = 100.0
                    Case "HOUR"
                        NumberBase = 10000.0
                        NumberDivisor = 100.0
                    Case "WEEK"
                        NumberBase = 1000000.0
                        NumberDivisor = 10.0
                    Case "DAY"
                        NumberBase = 10000000.0
                        NumberDivisor = 100.0
                    Case "MONTH"
                        NumberBase = 1000000000.0
                        NumberDivisor = 100.0
                    Case "YEAR"
                        NumberBase = 100000000000.0
                        NumberDivisor = 10000.0
                    Case Else
                        _base = Substitution.GetNumber(_split(1))
                        If _base > 0 Then NumberBase = _base
                        If (_split.Length > 2) Then
                            _divisor = Substitution.GetNumber(_split(2))
                            If _divisor > 1 Then NumberDivisor = _divisor
                        End If
                End Select
            End If

            _base = 0
            _divisor = 10
            _shift = 0
            ' We have to go thru every token and extract base, divisor and applicable rule
            For Each CurrentToken In RuleSetTokens
                If (rxFormat.IsMatch(CurrentToken)) Then
                    FormatRule = New ConversionRule(Double.MinValue, 1, rxFormat.Split(CurrentToken)(1))
                ElseIf (rxNegative.IsMatch(CurrentToken)) Then
                    NegativeRule = New ConversionRule(Double.MaxValue, 1, rxNegative.Split(CurrentToken)(1))
                ElseIf (rxMaster.IsMatch(CurrentToken)) Then
                    MasterRule = New ConversionRule(Double.MinValue, 1, rxMaster.Split(CurrentToken)(2))
                ElseIf (rxInteger.IsMatch(CurrentToken)) Then
                    IntegerRule = New ConversionRule(Double.MinValue, 1, rxInteger.Split(CurrentToken)(2))
                ElseIf (rxFraction.IsMatch(CurrentToken)) Then
                    FractionRule = New ConversionRule(Double.MinValue, 1, rxFraction.Split(CurrentToken)(2))
                ElseIf (rxBase.IsMatch(CurrentToken)) Then
                    Dim m As Match = rxBase.Match(CurrentToken)

                    If m.Groups("Base").Success Then
                        ' We detected the Base is present in the token - get it
                        _base = 0
                        _divisor = 10
                        _shift = 0
                        _base = Substitution.GetNumber(m.Groups("Base").Value)
                    End If

                    If m.Groups("Div").Success Then
                        _divisor = 0
                        _shift = 0
                        _divisor = Substitution.GetNumber(m.Groups("Div").Value)
                    End If

                    If m.Groups("Shift").Success Then
                        _shift = m.Groups("Shift").Value.Split(">"c).Length - 1
                    End If
                    AddConversionRule(_base, CalculateDivisor(_base, _divisor, _shift), CurrentToken.Substring(m.Length))
                    _base += 1
                Else
                    AddConversionRule(_base, CalculateDivisor(_base, _divisor, _shift), CurrentToken)
                    _base += 1
                End If
            Next
            ConversionRules.Sort()
        End Sub

        Private Function CalculateDivisor(ByVal Base As Double, ByVal Div As Double, ByVal Shift As Integer) As Double
            Dim Divisor As Double
            If Div <= 1.0 Then Div = 10.0
            If Base < Div Then
                Divisor = 1.0
            Else
                Divisor = Math.Pow(Div, Math.Floor(Math.Log10(Base) / Math.Log10(Div))) / _
                    Math.Pow(Div, Shift)
                If Divisor < 1.0 Then Divisor = 1.0
            End If
            Return Divisor
        End Function

        Private Sub AddConversionRule(ByRef RuleBase As Double, ByVal RuleDivisor As Double, ByVal RuleText As String)
            ' We utilize here the following algorithm:
            '   if the rule added has the [...] constracts, then the rule is created with
            '   all such constracts removed
            '   After that an additional rule is created that has a Base = Base + 1 and
            '   everything inside [...] visible. 
            ' That rule is useful to shorten the notation for cases like:
            '   20: Twenty[->>];    , instead of
            '   20: Twenty; Twenty->>;
            '   
            If rxExpand.IsMatch(RuleText) Then
                Dim _split() As String = rxExpand.Split(RuleText)
                Dim _str As String = String.Concat(_split)
                ConversionRules.Add(New ConversionRule(RuleBase, RuleDivisor, _str))
                _str = RuleText.Replace("[", "").Replace("]", "")
                RuleBase += 1
                ConversionRules.Add(New ConversionRule(RuleBase, RuleDivisor, _str))
            Else
                ConversionRules.Add(New ConversionRule(RuleBase, RuleDivisor, RuleText))
            End If
        End Sub

        Public Function Convert(ByVal RuleSetName As String, ByVal Number As Double, ByVal ConvFunction As Conversion) As String
            Dim _result As String
            ' If we don't have any rules - end it here
            If (ConversionRules.Count = 0) Then
                _result = String.Empty
            Else
                ' Save the Original number 
                If (Me.NumberDivisor > 1) Then
                    Number = Math.Floor(Number / Me.NumberBase)
                    Number = Number - Math.Floor(Number / Me.NumberDivisor) * Me.NumberDivisor
                End If
                _result = Me.LocalConvert(RuleSetName, Number, ConvFunction)
            End If
            Return _result
        End Function

        Private Function LocalConvert(ByVal RuleSetName As String, ByVal Number As Double, ByVal ConvFunction As Conversion) As String
            Dim _result As String

            If (RuleSetName.Equals("") OrElse Me.Name.Equals(RuleSetName)) Then
                'We need a three separate sets of rules - For Negative number, 
                '   for the Fractional part and for the Integer part
                If (Number < 0) Then
                    Number = Math.Abs(Number)
                    If Not (Me.NegativeRule Is Nothing) Then
                        _result = Me.NegativeRule.Convert(Number, AddressOf Me.LocalConvert)
                    Else
                        _result = Me.LocalConvert(RuleSetName, Number, AddressOf Me.LocalConvert)
                    End If
                ElseIf Not Me.FormatRule Is Nothing Then
                    Number = Math.Floor(Number)
                    _result = Me.FormatRule.Convert(Number, AddressOf Me.IntegerConvert)
                ElseIf Number = Math.Round(Number) AndAlso Not Me.IntegerRule Is Nothing Then
                    _result = Me.IntegerRule.Convert(Number, AddressOf Me.IntegerConvert)
                ElseIf Number <> Math.Round(Number) AndAlso Number < 1.0 AndAlso Not Me.FractionRule Is Nothing Then
                    _result = Me.FractionRule.Convert(Number, AddressOf Me.FractionalConvert)
                ElseIf Number <> Math.Round(Number) AndAlso Not Me.MasterRule Is Nothing Then
                    _result = Me.MasterRule.Convert(Number, AddressOf Me.FractionalConvert)
                Else
                    _result = Me.FractionalConvert(RuleSetName, Number, AddressOf Me.FractionalConvert)
                End If
            Else
                _result = Me.GlobalConversion(RuleSetName, Number, ConvFunction)
            End If
            Return _result
        End Function

        Private Function IntegerConvert(ByVal RuleSetName As String, ByVal Number As Double, ByVal ConvFunction As Conversion) As String
            Dim _match As ConversionRule
            Dim _conv As ConversionRule
            Dim _result As String

            If (RuleSetName.Equals("") OrElse Me.Name.Equals(RuleSetName)) Then
                'Initial rule will be the last one in the array, so if we don't find
                ' anything - we will use this one
                _match = CType(ConversionRules(ConversionRules.Count - 1), ConversionRule)
                For Each _conv In ConversionRules
                    If (_conv.Value <= Number) Then
                        _match = _conv
                    Else
                        Exit For
                    End If
                Next
                ' Here is the special rule:
                '   if this found match has 2 substitutions - major and minor,
                ' and the remainder of division of the number to the base is 0
                ' and there is another rule with the same factor - we will use it
                If ((_match.Value Mod _match.Factor) <> 0) AndAlso _
                        ((Number Mod _match.Factor) = 0) AndAlso _
                            (_match.Type = ConversionRule.SubstitutionType.Both) Then
                    Dim index As Integer = ConversionRules.IndexOf(_match)
                    Dim _factor As Double = _match.Factor
                    While (index > 0)
                        index -= 1
                        _conv = CType(ConversionRules(index), ConversionRule)
                        If _conv.Factor <> _match.Factor Then
                            index += 1
                            Exit While
                        End If
                        If _conv.Value Mod _conv.Factor = 0 Then Exit While
                    End While
                    _match = CType(ConversionRules(index), ConversionRule)
                End If
                _result = _match.Convert(Number, AddressOf Me.IntegerConvert)
            Else
                _result = Me.GlobalConversion(RuleSetName, Number, ConvFunction)
            End If
            Return _result
        End Function


        Private Function FractionalConvert(ByVal RuleSetName As String, ByVal Number As Double, ByVal ConvFunction As Conversion) As String
            Dim _match As ConversionRule
            Dim _conv As ConversionRule
            Dim IntegerPart As Double
            Dim _result As String

            If (RuleSetName.Equals("") OrElse Me.Name.Equals(RuleSetName)) Then
                If (Math.Round(Number) = Number) Then
                    ' If the number is integer - use regular Integer conversion part
                    _result = Me.IntegerConvert(RuleSetName, Number, AddressOf Me.IntegerConvert)
                ElseIf FractionRule Is Nothing AndAlso MasterRule Is Nothing Then
                    ' This is a specialized fractional ruleset with a special syntax:
                    ' 0 : ........  - Handles 0 fractional part
                    ' aa:
                    ' bbb:      - we go thru each rule, multiplying Number by aa, bbb, cccc
                    ' cccc:     until we get the integer number. Then this rule will  be used
                    Dim _temp As Double
                    ' If no match will be found - use the last rule
                    _match = CType(ConversionRules(ConversionRules.Count - 1), ConversionRule)
                    For Each _conv In ConversionRules
                        If (_conv.Value = 0) Then
                            _temp = Number * _match.Value
                            If (_temp < 1.0) Then
                                _match = _conv
                                Exit For
                            End If
                        Else
                            _temp = Number * _conv.Value
                            IntegerPart = Math.Floor(_temp)
                            If ((_temp - IntegerPart) <= Double.Epsilon) Then
                                _match = _conv
                                Exit For
                            End If
                        End If
                    Next
                    IntegerPart = Math.Floor(Number * _match.Value * _match.Factor)
                    _result = _match.Convert(IntegerPart, AddressOf Me.IntegerConvert)
                Else
                    ' OK, here is a deal. If we got here, that means that we have to convert
                    ' fractional part only. If we came here from the general ruleset - then it should
                    ' have a fractional fule -> and we cannot use the ruleset for handling
                    ' fractional part - we have to use a default digit-by-digit.
                    ' On the other hand - if we are here and there is no fractional ruleset -
                    ' then our ruleset should be used as special fractions conversion ruleset.
                    _result = ""
                    Number = Number - Math.Floor(Number) ' Get fractional part
                    ' Spell each digit
                    Dim _IntNum As Integer
                    _IntNum = CInt(Number * 1000000.0)
                    While _IntNum <> 0
                        IntegerPart = _IntNum \ 100000
                        _IntNum = CInt((_IntNum - IntegerPart * 100000) * 10)
                        _result &= Me.IntegerConvert(RuleSetName, IntegerPart, AddressOf Me.IntegerConvert)
                    End While
                End If
            Else
                _result = Me.GlobalConversion(RuleSetName, Number, ConvFunction)
            End If
            Return _result
        End Function
    End Class

    Friend Class LanguageRuleSet
        Private LangName As String
        Private Shared rxTokens As New Regex("(?<!\\);")
        Private Shared rxComment As New Regex("//[^\n]*\n+")
        Private Shared rxVTab As New Regex("\t+")
        Private RuleSets As New Collections.Hashtable

        Public Sub New(ByVal LanguageName As String, ByVal RulesText As String)
            Dim CurrentRuleSetName As String = ""
            LangName = LanguageName
            RulesText = rxComment.Replace(RulesText, String.Empty)
            RulesText = rxVTab.Replace(RulesText, String.Empty)
            RulesText = RulesText.Replace(Environment.NewLine, String.Empty).Replace("'", " ")
            Dim _split() As String = rxTokens.Split(RulesText)
            Dim _nameToken As String = String.Empty
            Dim _tokens As New Collections.ArrayList

            For Each _currToken As String In _split
                _nameToken = _currToken.Trim
                If _nameToken.StartsWith("%") Then              ' Ruleset name starts with %
                    Dim _colonPosition As Integer = _currToken.IndexOf(":")
                    ' If we already accumulated some tokens - add them to the previous rule
                    If _tokens.Count <> 0 AndAlso CurrentRuleSetName <> String.Empty Then
                        AddNewRuleset(CurrentRuleSetName, _tokens)
                    End If
                    _tokens.Clear()

                    If _colonPosition <> -1 Then
                        _tokens.Add(_currToken.Substring(_colonPosition + 1))
                        CurrentRuleSetName = _currToken.Substring(0, _colonPosition)
                    Else
                        CurrentRuleSetName = _currToken
                    End If
                Else
                    _currToken = _currToken.Replace("\;", ";")
                    _tokens.Add(_currToken)
                End If
            Next
            If (_tokens.Count > 0) Then
                If _nameToken = String.Empty Then
                    _tokens.RemoveAt(_tokens.Count - 1)
                End If
                AddNewRuleset(CurrentRuleSetName, _tokens)
            End If
        End Sub

        Private Sub AddNewRuleset(ByVal RuleSetName As String, ByVal Rules As Collections.ArrayList)
            Dim _Name As String
            RuleSetName = RuleSetName.Trim.ToUpper.Substring(1)
            If (RuleSetName.IndexOf("/") <> -1) Then
                _Name = RuleSetName.Split("/"c)(0)
            Else
                _Name = RuleSetName
            End If
            RuleSets(_Name) = New ConversionRuleSet(RuleSetName, CType(Rules.ToArray(GetType(String)), String()), AddressOf Me.GlobalConvert)
        End Sub

        Private Overloads Function GlobalConvert(ByVal RuleSetName As String, ByVal Parameter As Double, ByVal ConvFunction As Conversion) As String
            Dim _result As String
            RuleSetName = RuleSetName.Trim.ToUpper
            If (RuleSetName.StartsWith("%")) Then RuleSetName = RuleSetName.Substring(1)
            If RuleSets.Contains(RuleSetName) Then
                _result = CType(RuleSets(RuleSetName), ConversionRuleSet).Convert(RuleSetName, Parameter, ConvFunction)
            Else
                _result = Nothing
            End If
            Return _result
        End Function

        Public Overloads Function Convert(ByVal RuleSetName As String, ByVal Parameter As Object) As String
            Dim _par As Double

            If Parameter Is Nothing Then Return Nothing
            While (RuleSetName.StartsWith("%"))
                RuleSetName = RuleSetName.Substring(1)
            End While

            If (Parameter.GetType.Equals(GetType(DateTime))) Then
                Dim dt As DateTime = CType(Parameter, DateTime)
                _par = Math.Floor((((((dt.Year * 100.0 + dt.Month) * 100.0 + dt.Day) * 10 + dt.DayOfWeek) * 100.0 + dt.Hour) * 100.0 + dt.Minute) * 100.0 + dt.Second)
            ElseIf (Parameter.GetType.Equals(GetType(TimeSpan))) Then
                Dim ts As TimeSpan = CType(Parameter, TimeSpan)
                _par = Math.Floor(((ts.Days * 100.0 + ts.Hours) * 100.0 + ts.Minutes) * 100.0 + ts.Seconds)
            Else
                _par = CDbl(Parameter)
            End If
            Return GlobalConvert(RuleSetName, _par, AddressOf Me.GlobalConvert)
        End Function
    End Class

    Friend Class LanguageSets
        Private LanguageRuleSets As New Collections.Hashtable

        Public Sub New()
            Dim ReadBuffer As String
            Dim LangName As String

            ' First get the default set from the resource file - note the Upper case
            ' To get the full name of the resources - look into the Assembly manifest
            Try
                LangName = "DEFAULT"
                Dim res As System.Resources.ResourceManager = New System.Resources.ResourceManager("StateMachine", [Assembly].GetCallingAssembly())
                Dim ba As Byte() = CType(res.GetObject("default.config"), Byte())
                Dim utf8Reader As System.Text.UTF8Encoding = New System.Text.UTF8Encoding
                ReadBuffer = utf8Reader.GetString(ba)

                If Not ReadBuffer Is Nothing Then
                    DBG("Adding default Ruleset : " & "default.config" & ":" & vbCrLf)
                    LanguageRuleSets(LangName) = New LanguageRuleSet(LangName, ReadBuffer)
                    DBG(ReadBuffer)
                End If
            Catch ex As Exception
                DBGEX(ex)
            End Try
        End Sub



        Public Sub Add(ByVal DirectoryName As String, Optional ByVal ExtensionName As String = ".config")
            Dim FileName As String
            Dim ReadBuffer As String
            Dim LangName As String
            Dim sr As System.IO.StreamReader = Nothing

            ' The extension name should have "." in it
            If (Not ExtensionName.StartsWith(".")) Then ExtensionName = "." & ExtensionName

            ' Iterate thru all files in that directory with extension "ExtensionName" and add them as rules
            For Each FileName In System.IO.Directory.GetFiles(DirectoryName, "*" & ExtensionName)
                DBG("Added new Ruleset : " & FileName & " :" & vbCrLf)
                Try
                    sr = New System.IO.StreamReader(FileName, System.Text.Encoding.ASCII)
                    ReadBuffer = sr.ReadToEnd()
                    LangName = System.IO.Path.GetFileNameWithoutExtension(FileName).Trim.ToUpper
                    LanguageRuleSets(LangName) = New LanguageRuleSet(LangName, ReadBuffer)
                    DBG(ReadBuffer)
                Catch ex As Exception
                    DBGEX(ex)
                Finally
                    sr.Close()
                End Try
            Next
        End Sub

        Default ReadOnly Property Item(ByVal LangName As String) As LanguageRuleSet
            Get
                LangName = LangName.Trim.ToUpper
                If (LanguageRuleSets.Contains(LangName)) Then
                    Return CType(LanguageRuleSets(LangName), LanguageRuleSet)
                Else
                    Return Nothing
                End If
            End Get
        End Property
    End Class

End Namespace

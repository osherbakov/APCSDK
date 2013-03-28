Option Explicit On 
Option Strict On
Namespace Diacom
    ''' <summary>
    ''' Represents the data record retrieved from the SQL datbase.
    ''' </summary>
    Public Class DataRecord : Implements IEnumerable : Implements IEnumerator
        ''' <summary>
        ''' Collection of elements in record.
        ''' </summary>
        Private m_Records As Collections.ArrayList
        ''' <summary>
        ''' Collection of field names.
        ''' </summary>
        Private m_FieldNames As Collections.Specialized.StringCollection
        ''' <summary>
        ''' Index for NextRecord interface.
        ''' </summary>
        Private m_Index As Integer
        ''' <summary>
        ''' Index for IEnumerator interface.
        ''' </summary>
        Private m_Enum As Integer
        ''' <summary>
        ''' Current data.
        ''' </summary>
        Private m_Data As Collections.ArrayList

        ''' <summary>
        ''' Creates a new instance of <see cref="DataRecord"/> class with default parameters.
        ''' </summary>
        Public Sub New()
            m_Records = New Collections.ArrayList
            m_FieldNames = New Collections.Specialized.StringCollection
            m_Data = New Collections.ArrayList
            Reset()
        End Sub

        ''' <summary>
        ''' Creates a new instance of <see cref="DataRecord"/> class with Field names as parameters.
        ''' </summary>
        ''' <param name="fieldNames">An array of field names to initialize the field names.</param>
        Public Sub New(ByVal fieldNames() As String)
            m_Records = New Collections.ArrayList
            m_FieldNames = New Collections.Specialized.StringCollection
            m_Data = New Collections.ArrayList
            For Each _name As String In fieldNames
                m_FieldNames.Add(_name.Trim.ToUpper)
            Next
            Reset()
        End Sub

        ''' <summary>
        ''' Resets the Data Record, implements <see cref="System.Collections.IEnumerator.Reset"/> method of <see cref="System.Collections.IEnumerator"/> interface.
        ''' Sets the enumerator to its initial position, which is before the first element in the collection.
        ''' </summary>
        ''' <exception cref="System.InvalidOperationException">The collection was modified after the 
        ''' enumerator was created.</exception>
        ''' <remarks>
        ''' <para>An enumerator remains valid as long as the collection remains unchanged. If changes are made 
        ''' to the collection, such as adding, modifying or deleting elements, the enumerator is irrecoverably 
        ''' invalidated and the next call to <see cref="MoveNext"/> or <see cref="Reset"/> throws an 
        ''' <see cref="System.InvalidOperationException"/>.</para>
        ''' <para><b>Notes to Implementers:</b></para>
        ''' <para>All calls to <see cref="Reset"/> must result in the same state for the enumerator. The preferred 
        ''' implementation is to move the enumerator to the beginning of the collection, before the first element. 
        ''' This invalidates the enumerator if the collection has been modified since the enumerator was created, 
        ''' which is consistent with <see cref="MoveNext"/> and <see cref="Current"/>.</para>   
        ''' </remarks>
        Public Sub Reset() Implements IEnumerator.Reset
            m_Index = 0
            m_Enum = -1
            If (m_Records.Count <> 0) Then m_Data = CType(m_Records.Item(0), Collections.ArrayList)
        End Sub

        ''' <summary>
        ''' Adds specified data to the record.
        ''' </summary>
        ''' <param name="oRow">The data to add.</param>
        Public Sub Add(ByVal oRow As Collections.ArrayList)
            If (m_Records.Count = 0) Then m_Data = oRow
            m_Records.Add(oRow)
        End Sub

        ''' <summary>
        ''' Gets the number of elements actually contained in record.
        ''' </summary>
        ''' <value>Number of data fields in record.</value>
        Public ReadOnly Property Count() As Integer
            Get
                Return m_Records.Count
            End Get
        End Property

        ''' <summary>
        ''' Returns an enumerator that can iterate through a collection.
        ''' </summary>
        ''' <returns>An <see cref="System.Collections.IEnumerator"/> that can be used to iterate 
        ''' through the collection.</returns>
        Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Return Me
        End Function

        ''' <summary>
        ''' Gets the current element in the collection.
        ''' </summary>
        ''' <value>The current element in the collection.</value>
        ''' <exception cref="System.InvalidOperationException">The enumerator is positioned before the 
        ''' first element of the collection or after the last element.</exception>
        ''' <remarks>
        ''' <para>After an enumerator is created or after a <see cref="Reset"/>, <see cref="MoveNext"/> must be 
        ''' called to advance the enumerator to the first element of the collection before reading the value 
        ''' of <see cref="Current"/>; otherwise, <see cref="Current"/> is undefined.</para>
        ''' <para><see cref="Current"/> also throws an exception if the last call to 
        ''' <see cref="MoveNext"/> returned false, which indicates the end of the collection.</para>
        ''' <para><see cref="Current"/> does not move the position of the enumerator and consecutive calls to 
        ''' <see cref="Current"/> return the same object until either <see cref="MoveNext"/> or 
        ''' <see cref="Reset"/> is called.</para>
        ''' <para>An enumerator remains valid as long as the collection remains unchanged. If changes are made 
        ''' to the collection, such as adding, modifying or deleting elements, the enumerator is irrecoverably 
        ''' invalidated and the next call to <see cref="MoveNext"/> or <see cref="Reset"/> throws an 
        ''' <see cref="System.InvalidOperationException"/>. If the collection is modified between 
        ''' <see cref="MoveNext"/> and <see cref="Current"/>, <see cref="Current"/> will return the element 
        ''' that it is set to, even if the enumerator is already invalidated.</para>
        ''' </remarks>
        Public ReadOnly Property Current() As Object Implements IEnumerator.Current
            Get
                If ((m_Enum >= 0) AndAlso (m_Enum < Count)) Then
                    Return Me
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Advances the enumerator to the next element of the collection.
        ''' </summary>
        ''' <returns><c>Nothing</c> if the enumerator was successfully advanced to the next element;
        ''' <c>False</c> if the enumerator has passed the end of the collection.</returns>
        ''' <remarks>
        ''' <para>After an enumerator is created or after a call to Reset, an enumerator is positioned before 
        ''' the first element of the collection, and the first call to <see cref="MoveNext"/> moves the enumerator 
        ''' over the first element of the collection.</para>
        ''' <para>After the end of the collection is passed, subsequent calls to <see cref="MoveNext"/> return 
        ''' <c>False</c> until <see cref="Reset"/> is called.</para>
        ''' <para>An enumerator remains valid as long as the collection remains unchanged. If changes are 
        ''' made to the collection, such as adding, modifying or deleting elements, the enumerator is 
        ''' irrecoverably invalidated and the next call to MoveNext or Reset throws an 
        ''' <see cref="InvalidOperationException"/>.</para>
        ''' </remarks>
        Public Function MoveNext() As Boolean Implements IEnumerator.MoveNext
            If ((Count > 0) AndAlso (m_Enum < (Count - 1))) Then
                m_Enum += 1
                m_Data = CType(m_Records(m_Enum), Collections.ArrayList)
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' Gets the element at the specified index.
        ''' </summary>
        ''' <param name="index">The zero-based index of the element to get.</param>
        ''' <exception cref="System.ArgumentOutOfRangeException">Index is less 
        ''' than zero or index is equal to or greater than <see cref="Count"/>.</exception>
        ''' <value>The element at the specified index.</value>
        Default Public Overloads ReadOnly Property Item(ByVal index As Integer) As Object
            Get
                If ((m_Index < m_Records.Count) AndAlso (index < m_Data.Count)) Then
                    Return m_Data.Item(index)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Searches for the element with specified field name and returns it if found.
        ''' </summary>
        ''' <param name="Key">Name of the field to get the element from.</param>
        ''' <value>The element with the specified field name.</value>
        Default Public Overloads ReadOnly Property Item(ByVal Key As String) As Object
            Get
                Dim _index As Integer = m_FieldNames.IndexOf(Key.Trim.ToUpper)
                If ((m_Index < m_Records.Count) AndAlso (_index <> -1) AndAlso (_index < m_Data.Count)) Then
                    Return m_Data.Item(_index)
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' Gets the element that positiones after the current and advances position.
        ''' </summary>
        Public Sub NextRecord()
            If (m_Index < (Count - 1)) Then
                m_Index += 1
                m_Data = CType(m_Records(m_Index), Collections.ArrayList)
            End If
        End Sub

        ''' <summary>
        ''' Checks if current element is empty or not.
        ''' </summary>
        ''' <value>The current element is empty or not.</value>
        Public ReadOnly Property IsEmpty() As Boolean
            Get
                Return (m_Index >= Count)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return "DataRecord : Fields : " & CStr(m_FieldNames.Count) & " Records : " & CStr(m_FieldNames.Count)
        End Function
    End Class
End Namespace

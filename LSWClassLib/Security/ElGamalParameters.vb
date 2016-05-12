Namespace Security
    <Serializable()> _
    Public Structure ElGamalParameters
        Public P() As Byte
        Public G() As Byte
        Public Y() As Byte
        <NonSerialized()> _
        Public X() As Byte
    End Structure
End Namespace
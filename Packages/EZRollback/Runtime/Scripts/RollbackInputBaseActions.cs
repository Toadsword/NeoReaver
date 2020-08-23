using System;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

/**
 * \brief Utility struct that accept all button press inputs, stored inputs in a single bit per actions.
 * It includes two axis : Horizontal and Vertical.
 */
[Serializable]
public struct RollbackInputBaseActions : IEquatable<RollbackInputBaseActions> {

    const float AxisChangeRatio = 64.0f;

    [SerializeField] private sbyte _horizontalValue;
    [SerializeField] private sbyte _verticalValue;
    [SerializeField] private byte[] _bits;

    /**
     * \brief Constructor of the class.
     * \param defaultSize Number of inputs the BaseActions should be able to handle.
     */
    public RollbackInputBaseActions(int defaultSize = 1) {
        _bits = new byte[1 + defaultSize/8];
        _horizontalValue = 0;
        _verticalValue = 0;
    }

    /**
     * \brief Get the value of the input at the requested index.
     * \param i Index of the input.
     * \return true if input is pressed, false otherwise.
     */
    public bool GetValueBit(int i) {
        return (_bits[i/8] & (1 << (i%8))) != 0;
    }

    /**
     * \brief Utility function to choose between setting or clearing a bit.
     * \param i Index of the input.
     * \param setIt true if input is pressed, false otherwise.
     */
    public void SetOrClearBit(int i, bool setIt) {
        if (setIt) 
            SetBit(i);
        else 
            ClearBit(i);
    }
    
    /**
     * \brief Set the value of the input at the requested index. (Set to 1)
     * \param i Index of the input.
     */
    public void SetBit(int i) {
        _bits[i/8] |= (byte)(1 << (i%8));
    }
    
    /**
     * \brief Clear the value of the input at the requested index. (Set to 0)
     * \param i Index of the input.
     */
    public void ClearBit(int i) {
        _bits[i/8] &= (byte)~(1 << (i%8));
    }
    
    /**
     * \brief Set the verticalAxis from a float value
     * \param value
     */
    public void SetVerticalAxis(float value) {
        _verticalValue = TransformAxisValueToSByte(value);
    }
    
    /**
     * \brief Get the verticalAxis value in float
     * \return value
     */
    public float GetVerticalAxis() {
        return TransformSByteToAxisValue(_verticalValue);
    }
    
    /**
     * \brief Get the Horizontal Axis value in float
     * \param value
     */
    public void SetHorizontalAxis(float value) {
        _horizontalValue = TransformAxisValueToSByte(value);
    }
    
    /**
     * \brief Get the Horizontal axis value in float
     * \return value
     */
    public float GetHorizontalAxis() {
        return TransformSByteToAxisValue(_horizontalValue);
    }

    /**
     * \brief Transform a float value (axis) to an sbyte
     * \return value
     */
    private sbyte TransformAxisValueToSByte(float value) {
        return (sbyte) (value * AxisChangeRatio);
    }
    
    /**
     * \brief Transform a sbyte value to a float (axis)
     * \param value Value to transform
     * \return value Result
     */
    private float TransformSByteToAxisValue(sbyte value) {
        return value / AxisChangeRatio;
    }

    public byte[] PackBits() {
        byte[] bytes = new byte[2 + _bits.Length];

        bytes[0] = (byte)_horizontalValue;
        bytes[1] = (byte)_verticalValue;
        
        for(int i = 0; i < _bits.Length; i++) {
            bytes[2 + i] = _bits[i];
        }

        return bytes;
    }

    public void UnpackBits(byte[] bytes) {
        _horizontalValue = (sbyte)bytes[0];
        _verticalValue = (sbyte)bytes[1];
        
        _bits = new byte[bytes.Length - 2];
        for(int i = 0; i < bytes.Length - 2; i++) {
            _bits[i] = bytes[2 + i];
        }
    }

    public override string ToString() {
        return "Horizontal : " + TransformSByteToAxisValue(_horizontalValue)
                               + "; Vertical : " + TransformSByteToAxisValue(_verticalValue)
                               + "; Bits value  : " + _bits;
    }
    
    public bool IsWellInitialized() {
        return _bits != null;
    }

    public override bool Equals(object obj) {
        return obj is RollbackInputBaseActions other && Equals(other);
    }
    
    public bool Equals(RollbackInputBaseActions other) {
        return _horizontalValue == other._horizontalValue && 
               _verticalValue == other._verticalValue && 
               BitsEquals(other._bits);
    }

    //Had to implement our own way to compare an array of byte, because the Equal always returns false.
    private bool BitsEquals(byte[] other) {
            if (_bits.Length != other.Length) return false;

        bool result = true;
        for (int i = 0; i < _bits.Length; i++) {
            result = _bits[i] == other[i];
        }
        return result;
    }

    public override int GetHashCode() {
        unchecked {
            int hashCode = _horizontalValue.GetHashCode();
            hashCode = (hashCode * 397) ^ _verticalValue.GetHashCode();
            hashCode = (hashCode * 397) ^ (_bits != null ? _bits.GetHashCode() : 0);
            return hashCode;
        }
    }
}

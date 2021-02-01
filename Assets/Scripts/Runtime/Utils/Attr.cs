using System;
using System.Collections.Generic;

public class Attr
{
    public Attr ShallowCopy() {
        Attr copy = (Attr)this.MemberwiseClone();
        return copy;
    }
    public enum DataType {
        FLOAT = 0,
        INT,
        CUSTOM,
        FORMULA,
        EMPTY,
    }
    public Attr(){
        Type = DataType.EMPTY;
    }
    public Attr(int value) {
        SetValue(value);
    }
    public Attr(float value) {
        SetValue(value);
    }
    public Attr(string value) {
        SetValue(value);
    }
    private DataType _type = DataType.CUSTOM;
    public DataType Type {
        get{ return _type; }
        set{ _type = value; }
    }
    private object _value;
    public void SetValue<T>(T value)
    {
        if (typeof(T) == typeof(float)){
            Type = DataType.FLOAT;
        } else if (typeof(T) == typeof(int)){
            Type = DataType.INT;
        } else if (typeof(T) == typeof(string)){
            Type = DataType.FORMULA;
        } else {
            Type = DataType.CUSTOM;
        }
        _value = value;
    }
    public T GetValue<T>()
    {
        object value = null;
        if (!(_value is T)){
            if (typeof(T) == typeof(int) && _value is float) {
                value = Convert.ToInt32(_value);
            } else if (typeof(T) == typeof(float) && _value is int) {
                value = Convert.ToSingle(_value);
            } else if (typeof(T) == typeof(float) && _value is string) {
                value = FormulaSystem.I.CalcFormula((string)_value);
            } else if (typeof(T) == typeof(int) && _value is string) {
                value = (int)FormulaSystem.I.CalcFormula((string)_value); 
            } else if (typeof(T).IsEnum){
                try {
                    var intVal = Convert.ToInt32(_value);
                    value = Enum.ToObject(typeof(T), intVal);
                } catch {}
            } else {
                try { value = (T)_value; } catch {}
            }
        } else {
            value = _value;
        }
        if (value == null) return default;
        return (T)value;
    }
    
    public static implicit operator int(Attr value)
    {
        return Convert.ToInt32(value._value);
    }

    public static implicit operator Attr(int value)
    {
        return new Attr(value);
    }

    public static implicit operator float(Attr value)
    {
        return Convert.ToSingle(value._value);
    }

    public static implicit operator Attr(float value)
    {
        return new Attr(value);
    }

    public static implicit operator string(Attr value)
    {
        return (string)value._value;
    }

    public static implicit operator Attr(string value)
    {
        return new Attr(value);
    }
}
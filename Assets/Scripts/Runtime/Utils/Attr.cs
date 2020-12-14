using System;
using System.Collections.Generic;

public class Attr
{
    public enum DataType {
        FLOAT = 0,
        INT,
        CUSTOM,
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
        } else {
            Type = DataType.CUSTOM;
        }
        _value = value;
    }
    public T GetValue<T>()
    {
        return (T)_value;
    }
    public void SetCustomValue<T>(T value) where T:class
    {
        _value = (object)value;
        Type = DataType.CUSTOM;
    }

    public T GetCustomValue<T>() where T:class
    {
        return _value as T;
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
}
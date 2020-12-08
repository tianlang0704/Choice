using System;
using System.Collections;
using System.Collections.Generic;


public interface IIDAble {
    int Id { get; set; }
}

public interface IIDAbleProfileContent
{
    Type DataType { get; }
    IIDAble[] DataArray { get; }
}


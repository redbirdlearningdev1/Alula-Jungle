using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReferenceObj<T>
{
    private T actualObject;
    public T Value
    {
        get
        {
            return actualObject;
        }
        set
        {
            actualObject = value;
        }
    }

    public ReferenceObj(T obj)
    {
        actualObject = obj;
    }
}

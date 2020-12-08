using UnityEngine;
 using System;
 using System.Collections;
 
 public static class GameUtil {
     static public Type GetStaticType<T>(T v)
     {
         return typeof(T);
     }

     static public Type GetElementTaype<T>(T v)
     {
         var type = GetStaticType(v);
         if (type.IsArray) {
             type = type.GetElementType();
         }
         return type;
     }
 }
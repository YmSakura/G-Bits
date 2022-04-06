using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 泛型基类:用于创建单例模式
/// </summary>
/// <typeparam name="T">类型</typeparam>
public class BaseManager<T> where T : new()
{
    private static T _Instance;

    public static T GetInstance()
    {
        if (_Instance == null)
        {
            _Instance = new T();
        }
        return _Instance;
    }
}
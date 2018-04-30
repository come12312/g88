using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PoolingManager : MonoSingleton<PoolingManager>
{
    private Transform m_transHide;
    private Dictionary<Type, Stack<MonoBehaviour>> m_dicPoolingItem = new Dictionary<Type, Stack<MonoBehaviour>>();

    public void Init()
    {
        GameObject objHide = new GameObject("Hide");
        objHide.SetActive(false);
        m_transHide = objHide.transform;
        m_transHide.SetParent(transform);
    }

    public T PopItem<T>() where T : MonoBehaviour
    {
        Type type = typeof(T);
        Stack<MonoBehaviour> stackItem = GetStackByType(type);
        if (stackItem.Count == 0)
            CreateItem<T>();

        return stackItem.Pop() as T;
    }

    public void PushItem<T>(T t) where T : MonoBehaviour
    {
        Type type = typeof(T);
        Stack<MonoBehaviour> stackItem = GetStackByType(type);
        t.transform.SetParent(m_transHide);
        if (!stackItem.Contains(t))
            stackItem.Push(t);
    }

    public void CreateItem<T>() where T : MonoBehaviour
    {
        Type type = typeof(T);
        GameObject objItem = Instantiate(Resources.Load<GameObject>("Prefab/" + type.Name));
        PushItem(objItem.GetComponent<T>());
    }

    Stack<MonoBehaviour> GetStackByType(Type t)
    {
        if (!m_dicPoolingItem.ContainsKey(t))
            m_dicPoolingItem[t] = new Stack<MonoBehaviour>();

        return m_dicPoolingItem[t];
    }
}
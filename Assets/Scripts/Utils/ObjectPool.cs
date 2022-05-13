using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

public class ObjectPool
{
    public static ObjectPool Instance => BaseManager<ObjectPool>.GetInstance(); //建立公有实例以便提取调用
    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    private GameObject pool;
    private GameObject uiCanvas;



    public GameObject GetObject(GameObject prefab)
    {
        GameObject _object;
        string _name = prefab.name + "Pool";
        if (!objectPool.ContainsKey(_name)||objectPool[_name].Count==0)  
            //如果不存在对应池或可用数量为0，则实例化后推入池中
        {
            _object = GameObject.Instantiate(prefab);
            PushObject(_object);
            if(pool == null)                                            //如果无父池则创建父池存储子池
            {
                pool = new GameObject("ObjectPool");
            }
            GameObject childPool =GameObject.Find(_name);               //寻找子池
            if (!childPool)                                             //如果不存在对应物体的子池则创建子池并将父子池绑定父子关系
            {
                childPool = new GameObject(_name);
                childPool.transform.SetParent(pool.transform);
            }
            _object.transform.SetParent(childPool.transform);           //将物体设置为子池的子物品
        }
        _object = objectPool[_name].Dequeue();                          //从子池中取出物体
        _object.SetActive(true);                                        //使实例激活
        return _object;                                                 //返回实例以供使用
    }

    public GameObject GetUI(GameObject prefab)
    {
        GameObject _object;
        string _name = prefab.name + "Pool";
        if (!objectPool.ContainsKey(_name)||objectPool[_name].Count==0)  
            //如果不存在对应池或可用数量为0，则实例化后推入池中
        {
            _object = GameObject.Instantiate(prefab);
            PushObject(_object);
            CreateUIPool();
            GameObject childPool =GameObject.Find(_name);               //寻找子池
            if (!childPool)                                             //如果不存在对应物体的子池则创建子池并将父子池绑定父子关系
            {
                childPool = new GameObject(_name);
                childPool.transform.SetParent(uiCanvas.transform);
            }
            _object.transform.SetParent(childPool.transform);           //将物体设置为子池的子物品
        }
        _object = objectPool[_name].Dequeue();                          //从子池中取出物体
        _object.SetActive(true);                                        //使实例激活
        return _object;                                                 //返回实例以供使用
    }
    

    //将实例推到对象池当中
    public void PushObject(GameObject prefab)
    {
        string _name=prefab.name.Replace("(Clone)","Pool");
        if (!objectPool.ContainsKey(_name))                             //查找是否存在对应对象池，不存在则创建
        {
            objectPool.Add(_name,new Queue<GameObject>());
        }
        objectPool[_name].Enqueue(prefab);                              //重新放到队列中
        prefab.SetActive(false);                                        //使实例失活
    }

    private void CreateUIPool()
    {
        if (uiCanvas != null) return;
        uiCanvas = new GameObject("UIPool");
        Canvas canvas = uiCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        // uiCanvas=GameObject.Find("Canvas");
    }

    public GameObject GetUIPool()
    {
        if (uiCanvas) return uiCanvas;
        CreateUIPool();
        return uiCanvas;
    }
}

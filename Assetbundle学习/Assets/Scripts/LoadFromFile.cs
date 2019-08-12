using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
public class LoadFromFile : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //FirstLoad();
        //ReplyLoad();
        LoadReply();
        //StartCoroutine(Memory());
        //StartCoroutine(NetWork());
        StartCoroutine(DownloadFromServer());
        //StartCoroutine(WebRequest());
    }

    /// <summary>
    /// 从本地加载资源的方法
    /// </summary>
    void FirstLoad()
    {
        /*第一种方法：加载一个资源*/
        AssetBundle ab = AssetBundle.LoadFromFile("AssetBundles/prefabs/wood.corner");//使用静态方法加载ab包到AssetBundles实例上
        //从ab包里获得类型为GameObject的名叫Corner-Wood-Model-01的物体，需要填好资源后缀
        GameObject g = ab.LoadAsset<GameObject>("Corner-Wood-Model-01.prefab");
        Instantiate(g);  //实例化这个对象到场景中

        /*第二种方法：加载一个AB包里的所有资源*/
        object[] objs = ab.LoadAllAssets();   //获取一个AB包里的所有资源
        //实例化所有的资源
        foreach (Object o in objs)
        {
            Instantiate(o);
        }
    }

    /// <summary>
    /// 先加载所依赖的包，如材质
    /// </summary>
    void ReplyLoad()
    {
        AssetBundle ab1 = AssetBundle.LoadFromFile("AssetBundles/share_material");//使用静态方法加载ab包到AssetBundles实例上
        AssetBundle ab2 = AssetBundle.LoadFromFile("AssetBundles/reply/cubestone");//使用静态方法加载ab包到AssetBundles实例上

        //从ab包里获得类型为GameObject的名叫Corner-Wood-Model-01的物体，需要填好资源后缀
        GameObject g = ab2.LoadAsset<GameObject>("CubeStone.prefab");
        Instantiate(g);  //实例化这个对象到场景中
    }

    /// <summary>
    ///从内存加载资源的方法。这里用了异步方式
    /// </summary>
    IEnumerator Memory()
    {
        //将AB包读取成字节流之后（此时在内存中），再将字节流转换成AssetBundle
        //AssetBundleCreateRequest是创建一个AB包的请求，因为后面是异步的方法，需要等待其完成。使用协程而不卡进程
        AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes("AssetBundles/reply/spherestone"));
        yield return request;   //等待其请求完成
        AssetBundle ab = request.assetBundle;    //获取请求完成的ab包

        //使用获得的资源
        GameObject g = ab.LoadAsset<GameObject>("SphereStone.prefab");
        Instantiate(g);  //实例化这个对象到场景中
    }

    /// <summary>
    /// 从本机上下载加载资源
    /// </summary>
    /// <returns></returns>
    IEnumerator NetWork()
    {
        while (Caching.ready == false) //查看是否缓存是否准备好了
        {
            yield return null; //暂停一帧
        }

        //需要写完整的路径，如果是本地则是file，注意是三或两个斜杠
        System.String path= @"file:///F:\Chenhuada Unity Project\Assetbundle学习\AssetBundles\reply\spherestone";  

        WWW www= WWW.LoadFromCacheOrDownload(path,1);     //第二个参数是版本
        yield return www;    //等待返回，即等待下载完成

        if (string.IsNullOrEmpty(www.error)==false)  //如果下载有错误则显示错误
        {
            Debug.Log(www.error);
            yield break;    //结束协程
        }

        AssetBundle ab = www.assetBundle;  //获得从网络下载的资源
        //使用获得的资源
        GameObject g = ab.LoadAsset<GameObject>("SphereStone.prefab");
        Instantiate(g);  //实例化这个对象到场景中
    }

    /// <summary>
    /// 从远程服务器上下载加载资源
    /// </summary>
    /// <returns></returns>
    IEnumerator DownloadFromServer()
    {
        while (Caching.ready == false) //查看是否缓存是否准备好了
        {
            yield return null; //暂停一帧
        }

        //需要写完整的路径，如果是远程服务器则是http协议，写好ip地址，注意是三或两个斜杠
        System.String path = @"http://localhost/AssetBundles/reply/spherestone";

        WWW www = WWW.LoadFromCacheOrDownload(path, 1);     //第二个参数是版本
        yield return www;    //等待返回，即等待下载完成

        if (string.IsNullOrEmpty(www.error) == false)  //如果下载有错误则显示错误
        {
            Debug.Log(www.error);
            yield break;    //结束协程
        }

        AssetBundle ab = www.assetBundle;  //获得从网络下载的资源
        //使用获得的资源
        
        AssetBundleRequest g = ab.LoadAssetAsync<GameObject>("SphereStone.prefab");
        yield return g;
        Instantiate(g.asset);  //实例化这个对象到场景中
    }

    /// <summary>
    /// 新方法从服务器下载assetbundle
    /// </summary>
    /// <returns></returns>
    IEnumerator WebRequest()
    {
        string url = @"http://localhost/AssetBundles/reply/spherestone";

        UnityWebRequest request = UnityWebRequest.GetAssetBundle(url);//写入url准备向服务器发送请求
        yield return request.SendWebRequest();      //调用方法向服务器发送请求，并等待返回结果

        if (request.error != null)   //如果发生错误则结束协程
        {
            Debug.LogError("网络连接发生错误"+request.error);
            yield break;
        }

        //将下载的资源保存本地起来
        //File.WriteAllBytes(1, request.downloadedBytes);
        AssetBundle ab = DownloadHandlerAssetBundle.GetContent(request);  //从返回的结果中获得AssetBundle包
        //AssetBundle ab2 = (request.downloadHandler as DownloadHandlerAssetBundle).assetBundle;  //第二种获得ab包的方法

        //使用获得的资源
        GameObject g = ab.LoadAsset<GameObject>("SphereStone.prefab");
        Instantiate(g);  //实例化这个对象到场景中
    }
    /// <summary>
    /// 加载依赖的包
    /// </summary>
    void LoadReply()
    {
        AssetBundle manifestAB = AssetBundle.LoadFromFile("AssetBundles/AssetBundles");  //找到总包
        AssetBundleManifest manifest = manifestAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");//获得总包的manifest文件

        string[] str = manifest.GetAllDependencies("reply/spherestone");  //获得这个包的所有依赖包的名字及其路径
        //根据名字将所有的依赖包加载出来
        foreach(var name in str)
        {
            print(name);
            AssetBundle.LoadFromFile("AssetBundles/" + name);
        }
    }
}

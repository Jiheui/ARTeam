using System;
using System.Threading;
using hiscene;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class CloudRecoBehaiour : CloudRecognition,ICloudRecoEventHandler
{
    Text timeText;
    Text addressText;
    Text linkText;

    new void Start()
    {
        base.Start();
        RegisterCloudRecoEventHandler(this);
        timeText = GameObject.Find("Time").GetComponent<Text>();
        addressText = GameObject.Find("Address").GetComponent<Text>();
        linkText = GameObject.Find("Web Link").GetComponent<Text>();
    }

	public override void OnCloudReco(RecoResult recoResult){
        GameObject gameObject = null;

        gameObject = createGameObject(recoResult);

        if (gameObject != null)
        {
            bindingGameObject(gameObject, recoResult.KeyId);
        }
    }

    public override GameObject createGameObject(RecoResult recoResult)
    {
        GameObject gameObject = new GameObject();
        if (recoResult.keyType == KeyType.IMAGE)
        {
            gameObject.AddComponent<ImageTargetBehaviour>();
        }
        gameObject.transform.parent = transform.parent;
        return gameObject;
    }

    public void OnCloudStart()
    {
    }

    public void OnCloudComplete(CloudReco.CloudRecoResult result)
    {
    }
}

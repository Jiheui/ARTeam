using System;
using System.Threading;
using hiscene;
using Models;
using UnityEngine;
using UnityEngine.UI;

public class CloudRecoBehaiour : CloudRecognition,ICloudRecoEventHandler
{

    new void Start()
    {
        base.Start();
        RegisterCloudRecoEventHandler(this);
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

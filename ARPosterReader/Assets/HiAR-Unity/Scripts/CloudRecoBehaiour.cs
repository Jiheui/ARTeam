using System;
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

        Poster poster = new Poster();
        poster.keygroup = recoResult.KeyGroup;
        poster.keyid = recoResult.KeyId;
        poster.GetPoster();
        showDetail(poster.detail);
    }

    public void showDetail(string detail)
    {
        string detailRaw = detail;
        string[] detailList = detailRaw.Split(';');

        timeText.text = detailList[0];
        addressText.text = detailList[1];
        linkText.text = detailList[2];
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

using UnityEngine;
using hiscene;

public class TartgetDynamicBehaviour : TartgetDynamic
{

    public override void OnDynamicReco(RecoResult recoResult)
    {
        GameObject gameObject = null;
        gameObject = new GameObject();
        if (recoResult.keyType == KeyType.IMAGE)
        {
            gameObject.AddComponent<ImageTargetBehaviour>();
        }

        Target target = gameObject.GetComponent<Target>();
        target.PixelWidth = recoResult.Width * 0.01f;
        target.PixelHeight = recoResult.Height * 0.01f;
        gameObject.transform.parent = transform.parent;
        gameObject.SetActive(true);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = gameObject.transform;
        cube.transform.localScale = new Vector3(1.0f, 1.0f, 5.4f);
        cube.transform.parent = gameObject.transform;
        bindingGameObject(gameObject, recoResult.KeyId);
    }

}

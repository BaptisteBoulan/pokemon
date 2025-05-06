using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEngine.RuleTile.TilingRuleOutput;


[System.Serializable]
public class MoveObjectAction : CutSceneAction
{
    [SerializeField] bool isCamera;
    [SerializeField] GameObject obj;
    [SerializeField] List<Vector2> movePatern;
    [SerializeField] float speed;

    public override IEnumerator Play()
    {
        if (isCamera)
            obj = GameController.Instance.Player.GetComponentInChildren<Camera>().gameObject;

        foreach (var moveVec in movePatern)
        {
            var targetPos = obj.transform.position;
            targetPos.x += moveVec.x;
            targetPos.y += moveVec.y;

            while ((targetPos - obj.transform.position).sqrMagnitude > 1e-10)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }
        }
    }
}

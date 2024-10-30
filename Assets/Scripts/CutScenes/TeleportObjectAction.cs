using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportObjectAction : CutSceneAction
{
    [SerializeField] GameObject obj;
    [SerializeField] Vector2 position;
    [SerializeField] bool spinAtStart;
    [SerializeField] bool spinAtEnd;

    public override IEnumerator Play()
    {
        Character character = obj.GetComponent<Character>();

        if (spinAtStart && character != null)
            yield return Spin(character);

        obj.transform.position = position;

        if (spinAtEnd && character != null)
            yield return Spin(character);
    }

    IEnumerator Spin(Character character, float time=0.05f)
    {
        for (int i = 0; i < 3 ; i++)
        {
            character.Animator.SetFacingDirection(FacingDirection.Up);
            yield return new WaitForSeconds(time); 
            character.Animator.SetFacingDirection(FacingDirection.Right);
            yield return new WaitForSeconds(time);
            character.Animator.SetFacingDirection(FacingDirection.Down);
            yield return new WaitForSeconds(time);
            character.Animator.SetFacingDirection(FacingDirection.Left);
            yield return new WaitForSeconds(time);
        }
    }
}

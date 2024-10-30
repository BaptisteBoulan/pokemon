using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> surfSprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;

    // Parameters
    public float MoveX {  get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }

    public bool IsSurfing { get; set; }

    // States
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkLeftAnim;
    SpriteAnimator walkRightAnim;

    SpriteAnimator currentAnim;

    SpriteRenderer spriteRenderer;
    bool wasPrevMoving;

    public FacingDirection FacingDirection { get { return defaultDirection; } }

    private void Start()
    {
        spriteRenderer= GetComponent<SpriteRenderer>();
        walkDownAnim = new SpriteAnimator(spriteRenderer, walkDownSprites);
        walkUpAnim = new SpriteAnimator(spriteRenderer, walkUpSprites);
        walkLeftAnim = new SpriteAnimator(spriteRenderer, walkLeftSprites);
        walkRightAnim = new SpriteAnimator(spriteRenderer, walkRightSprites);

        SetFacingDirection(defaultDirection);
        currentAnim = walkDownAnim;


    }

    private void Update()
    {


        if (IsSurfing)
        {
            if (MoveX == 1) spriteRenderer.sprite = surfSprites[2];
            else if (MoveX == -1) spriteRenderer.sprite = surfSprites[3];
            else if (MoveY == 1) spriteRenderer.sprite = surfSprites[1];
            else if (MoveY == -1) spriteRenderer.sprite = surfSprites[0];
        } 
        else
        {
            var prevAnim = currentAnim;

            if (MoveX == 1) currentAnim = walkRightAnim;
            else if (MoveX == -1) currentAnim = walkLeftAnim;
            else if (MoveY == 1) currentAnim = walkUpAnim;
            else if (MoveY == -1) currentAnim = walkDownAnim;

            if (currentAnim != prevAnim  || IsMoving != wasPrevMoving)
            {
                currentAnim.Start();
            }

            if (IsMoving) currentAnim.HandleUpdate();
            else spriteRenderer.sprite = currentAnim.Frames[0];

            wasPrevMoving = IsMoving;
        }

    }

    public void SetFacingDirection(FacingDirection facingDirection)
    {
        if (facingDirection == FacingDirection.Right)
        {
            MoveX = 1;
            MoveY = 0;
        }
        else if (facingDirection == FacingDirection.Left)
        {
            MoveX = -1;
            MoveY = 0;
        }
        else if (facingDirection == FacingDirection.Up)
        {
            MoveX = 0;
            MoveY = 1;
        }
        else if (facingDirection == FacingDirection.Down)
        {
            MoveX = 0;
            MoveY = -1;
        }
    }
}

public enum FacingDirection
{
    Up,
    Down,
    Left,
    Right
}
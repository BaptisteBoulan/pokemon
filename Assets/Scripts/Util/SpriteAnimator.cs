using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator
{
    SpriteRenderer spriteRenderer;
    List<Sprite> frames;
    float frameRate;

    int currentFrame;
    float timer;

    public SpriteAnimator(SpriteRenderer spriteRenderer, List<Sprite> frames, float frameRate=0.12f)
    {
        this.frames = frames;
        this.spriteRenderer = spriteRenderer;
        this.frameRate = frameRate;
    }

    public void Start()
    {
        currentFrame = 0;
        timer = 0;
        spriteRenderer.sprite = frames[0];
    }

    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if (timer> frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer = 0;
        }
    }

    public List<Sprite> Frames
    {
        get { return frames; }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasAnimator
{
    Image image;
    List<Sprite> frames;
    float frameRate;

    int currentFrame;
    float timer;

    public CanvasAnimator(Image image, List<Sprite> frames, float frameRate = 0.12f)
    {
        this.frames = frames;
        this.image = image;
        this.frameRate = frameRate;
    }

    public void Start()
    {
        currentFrame = 0;
        timer = 0;
        image.sprite = frames[0];
    }

    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            image.sprite = frames[currentFrame];
            timer = 0;
        }
    }

    public List<Sprite> Frames
    {
        get { return frames; }
    }
}

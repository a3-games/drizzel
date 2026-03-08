using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StoryLine
{
    [TextArea]
    public string line;
    public Color lineColor;
    public float fadeDuration;
    public float advanceDelay;
    public float intermissionDuration;
    public UnityEvent onLineStart;
    public UnityEvent onLineEnd;
}

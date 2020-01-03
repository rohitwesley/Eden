using UnityEditor;

[CustomEditor(typeof(AnimationIntReaction))]
public class AnimationIntReactionEditor : ReactionEditor
{
    protected override string GetFoldoutLabel ()
    {
        return "Animation Reaction";
    }
}

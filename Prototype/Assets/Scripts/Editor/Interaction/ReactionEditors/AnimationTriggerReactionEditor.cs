using UnityEditor;

[CustomEditor(typeof(AnimationTriggerReaction))]
public class AnimationTriggerReactionEditor : ReactionEditor
{
    protected override string GetFoldoutLabel ()
    {
        return "Animation Reaction";
    }
}

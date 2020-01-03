using UnityEditor;

[CustomEditor(typeof(HookedReaction))]
public class HookedReactionReactionEditor : ReactionEditor
{
    protected override string GetFoldoutLabel ()
    {
        return "Is Hooked Reaction";
    }
}

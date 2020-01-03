using UnityEditor;

[CustomEditor(typeof(MovementStateReaction))]
public class MovementStateReactionEditor : ReactionEditor
{
    protected override string GetFoldoutLabel ()
    {
        return "Agent Movement State Change";
    }
}

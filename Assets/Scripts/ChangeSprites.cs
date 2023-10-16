#if(UNITY_EDITOR)

using UnityEngine;
using UnityEditor;

public class ChangeSprites : EditorWindow {
    private static readonly Vector2Int size = new Vector2Int(250, 500);
    public Conversation[] convos;
    private Sprite arthurNeutralNew;
    private Sprite arthurStaffCloseNew;
    private Sprite arthurStaffRaisedNew;
    private Sprite arthurEyesClosedNew;
    private Sprite arthurEyesWideNew;


    [MenuItem("Tools/Change Sprites")]
    public static void ShowWindow() {
        EditorWindow window = GetWindow<ChangeSprites>();
        window.minSize = size;
        //window.maxSize = new Vector2Int(250, 1500);
    }
    private void OnGUI() {
        arthurNeutralNew = EditorGUILayout.ObjectField("Arthur Neutral New", arthurNeutralNew, typeof(Sprite), false) as Sprite;
        arthurStaffCloseNew = EditorGUILayout.ObjectField("Arthur StaffClose New", arthurStaffCloseNew, typeof(Sprite), false) as Sprite;
        arthurStaffRaisedNew = EditorGUILayout.ObjectField("Arthur StaffRaise New", arthurStaffRaisedNew, typeof(Sprite), false) as Sprite;
        arthurEyesClosedNew = EditorGUILayout.ObjectField("Arthur EyesClosed New", arthurEyesClosedNew, typeof(Sprite), false) as Sprite;
        arthurEyesWideNew = EditorGUILayout.ObjectField("Arthur EyesWide New", arthurEyesWideNew, typeof(Sprite), false) as Sprite;

        //convo = EditorGUILayout.ObjectField(convo, typeof(Conversation), false) as Conversation;

        if(GUILayout.Button("Change sprites")){
            foreach(Conversation convo in convos){
                foreach(Dialogue block in convo.dialogueBlocks){
                    if(block.npcSprite.name.ToLower() == "arthur_neutral"){
                        block.npcSprite = arthurNeutralNew;
                    }
                    if(block.npcSprite.name.ToLower() == "arthur_eyeswide"){
                        block.npcSprite = arthurEyesWideNew;
                    }
                    if(block.npcSprite.name.ToLower() == "arthur_eyesclosed"){
                        block.npcSprite = arthurEyesClosedNew;
                    }
                    if(block.npcSprite.name.ToLower() == "arthur_staffclose"){
                        block.npcSprite = arthurStaffCloseNew;
                    }
                    if(block.npcSprite.name.ToLower() == "arthur_staffraised"){
                        block.npcSprite = arthurStaffRaisedNew;
                    }
                }
            }
        }
        //convos = EditorGUILayout.ObjectField(convos, typeof(Conversation), false) as Conversation;
    }
}

#endif
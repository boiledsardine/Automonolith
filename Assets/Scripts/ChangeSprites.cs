#if(UNITY_EDITOR)

using UnityEngine;
using UnityEditor;

public class ChangeSprites : EditorWindow {
    private static readonly Vector2Int size = new Vector2Int(250, 500);
    public Conversation convo;
    private Sprite arthurNeutralNew, arthurStaffCloseNew, arthurStaffRaisedNew, arthurEyesClosedNew, arthurEyesWideNew;
    private Sprite morganNeutralNew, morganLeftNew, morganSmileNew, morganClosedSmugNew, morganClosedSmileNew;


    [MenuItem("Tools/Change Sprites")]
    public static void ShowWindow() {
        EditorWindow window = GetWindow<ChangeSprites>();
        window.minSize = size;
        //window.maxSize = new Vector2Int(250, 1500);
    }
    private void OnGUI() {
        
        arthurNeutralNew = EditorGUILayout.ObjectField("Arthur Neutral", arthurNeutralNew, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
        arthurStaffCloseNew = EditorGUILayout.ObjectField("Arthur StaffClose", arthurStaffCloseNew, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
        arthurStaffRaisedNew = EditorGUILayout.ObjectField("Arthur StaffRaise", arthurStaffRaisedNew, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
        arthurEyesClosedNew = EditorGUILayout.ObjectField("Arthur EyesClosed", arthurEyesClosedNew, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
        arthurEyesWideNew = EditorGUILayout.ObjectField("Arthur EyesWide", arthurEyesWideNew, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;

        morganNeutralNew = EditorGUILayout.ObjectField("Morgan Neutral", morganNeutralNew, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
        morganLeftNew = EditorGUILayout.ObjectField("Morgan Left", morganLeftNew, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
        morganSmileNew = EditorGUILayout.ObjectField("Morgan Smile", morganSmileNew, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
        morganClosedSmugNew = EditorGUILayout.ObjectField("Morgan Smug", morganClosedSmugNew, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;
        morganClosedSmileNew = EditorGUILayout.ObjectField("Morgan ClosedSmile", morganClosedSmileNew, typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight)) as Sprite;

        convo = EditorGUILayout.ObjectField(convo, typeof(Conversation), false) as Conversation;

        if(GUILayout.Button("Change sprites")){
            //foreach(Conversation convo in convos){
                foreach(Dialogue block in convo.dialogueBlocks){
                    if(block.npcSprite.name.ToLower() == "old_arthur_neutral"){
                        block.npcSprite = arthurNeutralNew;
                    }
                    else if(block.npcSprite.name.ToLower() == "old_arthur_eyeswide"){
                        block.npcSprite = arthurEyesWideNew;
                    }
                    else if(block.npcSprite.name.ToLower() == "old_arthur_eyesclosed"){
                        block.npcSprite = arthurEyesClosedNew;
                    }
                    else if(block.npcSprite.name.ToLower() == "old_arthur_staffclose"){
                        block.npcSprite = arthurStaffCloseNew;
                    }
                    else if(block.npcSprite.name.ToLower() == "old_arthur_staffraised"){
                        block.npcSprite = arthurStaffRaisedNew;
                    }
                    else if(block.npcSprite.name.ToLower() == "old_morgan_neutral"){
                        block.npcSprite = morganNeutralNew;
                    }
                    else if(block.npcSprite.name.ToLower() == "old_morgan_left"){
                        block.npcSprite = morganLeftNew;
                    }
                    else if(block.npcSprite.name.ToLower() == "old_morgan_smile"){
                        block.npcSprite = morganSmileNew;
                    }
                    else if(block.npcSprite.name.ToLower() == "old_morgan_closedsmug"){
                        block.npcSprite = morganClosedSmugNew;
                    }
                    else if(block.npcSprite.name.ToLower() == "old_morgan_closedsmile"){
                        block.npcSprite = morganClosedSmileNew;
                    }
                }
            //}
        }
        //convos = EditorGUILayout.ObjectField(convos, typeof(Conversation), false) as Conversation;
    }
}

#endif
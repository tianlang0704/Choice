using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
///
public class ProfileChoiceContentAssetPostprocessor : AssetPostprocessor 
{
    private static readonly string filePath = "Assets/Resources/Excels/1CardProfiles.xls";
    private static readonly string assetFilePath = "Assets/Resources/Excels/ProfileChoiceContent.asset";
    private static readonly string sheetName = "ProfileChoiceContent";
    
    static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets) 
        {
            if (!filePath.Equals (asset))
                continue;
                
            ProfileChoiceContent data = (ProfileChoiceContent)AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(ProfileChoiceContent));
            if (data == null) {
                data = ScriptableObject.CreateInstance<ProfileChoiceContent> ();
                data.SheetName = filePath;
                data.WorksheetName = sheetName;
                AssetDatabase.CreateAsset ((ScriptableObject)data, assetFilePath);
                //data.hideFlags = HideFlags.NotEditable;
            }
            
            //data.dataArray = new ExcelQuery(filePath, sheetName).Deserialize<ProfileChoiceContentData>().ToArray();		

            //ScriptableObject obj = AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(ScriptableObject)) as ScriptableObject;
            //EditorUtility.SetDirty (obj);

            ExcelQuery query = new ExcelQuery(filePath, sheetName);
            if (query != null && query.IsValid())
            {
                data.dataArray = query.Deserialize<ProfileChoiceContentData>().ToArray();
                ScriptableObject obj = AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(ScriptableObject)) as ScriptableObject;
                EditorUtility.SetDirty (obj);
            }
        }
    }
}

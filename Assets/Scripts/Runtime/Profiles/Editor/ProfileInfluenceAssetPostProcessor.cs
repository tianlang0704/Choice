using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using UnityQuickSheet;

///
/// !!! Machine generated code !!!
///
public class ProfileInfluenceAssetPostprocessor : AssetPostprocessor 
{
    private static readonly string filePath = "Assets/Resources/Excels/2InfluenceProfiles.xls";
    private static readonly string assetFilePath = "Assets/Resources/Excels/ProfileInfluence.asset";
    private static readonly string sheetName = "ProfileInfluence";
    
    static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string asset in importedAssets) 
        {
            if (!filePath.Equals (asset))
                continue;
                
            ProfileInfluence data = (ProfileInfluence)AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(ProfileInfluence));
            if (data == null) {
                data = ScriptableObject.CreateInstance<ProfileInfluence> ();
                data.SheetName = filePath;
                data.WorksheetName = sheetName;
                AssetDatabase.CreateAsset ((ScriptableObject)data, assetFilePath);
                //data.hideFlags = HideFlags.NotEditable;
            }
            
            //data.dataArray = new ExcelQuery(filePath, sheetName).Deserialize<ProfileInfluenceData>().ToArray();		

            //ScriptableObject obj = AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(ScriptableObject)) as ScriptableObject;
            //EditorUtility.SetDirty (obj);

            ExcelQuery query = new ExcelQuery(filePath, sheetName);
            if (query != null && query.IsValid())
            {
                data.dataArray = query.Deserialize<ProfileInfluenceData>().ToArray();
                ScriptableObject obj = AssetDatabase.LoadAssetAtPath (assetFilePath, typeof(ScriptableObject)) as ScriptableObject;
                EditorUtility.SetDirty (obj);
            }
        }
    }
}

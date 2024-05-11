using Cr7Sund.Server.UI.Impl;
using Cr7Sund.UGUI.Impls;
using UnityEditor;
using UnityEngine.UIElements;

namespace Cr7Sund.UGUI.Editor
{
    [CustomEditor(typeof(UIScreenNavigatorSettings))]
    public class UIScreenNavigatorSettingsEditor : UnityEditor.Editor
    {
        private const string pushEnterAnimDefine = "_pagePushEnterAnimation";
        private const string pushExitAnimDefine = "_pagePushExitAnimation";
        private const string popEnterAnimDefine = "_pagePopEnterAnimation";
        private const string popExitAnimDefine = "_pagePopExitAnimation";
        private const string PageConfigPath = "Assets/Preresources/Config/UI/Page";

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement myInspector = new VisualElement();
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI/UIScreenNavigatorSettings.uxml");
            var config = target as UIScreenNavigatorSettings;

            visualTree.CloneTree(myInspector);

            var allButtons = myInspector.Query<Button>();
            allButtons.ForEach((btn) =>
            {
                btn.clicked += () =>
                {
                    if (btn.name == "enterPushBtn")
                    {
                        var simpleTransitionAnimationObject = SimpleUITransitionAnimationObject.CreateInstance(
                                                    beforeAlignment: SheetAlignment.Right, afterAlignment: SheetAlignment.Center);
                        AssetDatabase.CreateAsset(simpleTransitionAnimationObject,
                                                 $"{PageConfigPath}/{nameof(config.PagePushEnterAnimation)}.asset");
                        config.PagePushEnterAnimation = simpleTransitionAnimationObject;

                    }
                    else if (btn.name == "enterPopBtn")
                    {
                        var simpleTransitionAnimationObject = SimpleUITransitionAnimationObject.CreateInstance(beforeAlignment: SheetAlignment.Left,
                                        afterAlignment: SheetAlignment.Center);
                        AssetDatabase.CreateAsset(simpleTransitionAnimationObject,
                                                 $"{PageConfigPath}/{nameof(config.PagePopEnterAnimation)}.asset");
                        config.PagePopEnterAnimation = simpleTransitionAnimationObject;
                    }
                    else if (btn.name == "exitPushBtn")
                    {
                        var simpleTransitionAnimationObject = SimpleUITransitionAnimationObject.CreateInstance(beforeAlignment: SheetAlignment.Center,
                                        afterAlignment: SheetAlignment.Left);
                        AssetDatabase.CreateAsset(simpleTransitionAnimationObject,
                                                 $"{PageConfigPath}/{nameof(config.PagePushExitAnimation)}.asset");
                        config.PagePushExitAnimation = simpleTransitionAnimationObject;
                    }
                    else if (btn.name == "exitPopBtn")
                    {
                        var simpleTransitionAnimationObject = SimpleUITransitionAnimationObject.CreateInstance(beforeAlignment: SheetAlignment.Center,
                                        afterAlignment: SheetAlignment.Right);
                        AssetDatabase.CreateAsset(simpleTransitionAnimationObject,
                                                 $"{PageConfigPath}/{nameof(config.PagePopExitAnimation)}.asset");
                        config.PagePopExitAnimation = simpleTransitionAnimationObject;
                    }

                    EditorUtility.SetDirty(target);
                    AssetDatabase.SaveAssetIfDirty(target);
                };
            });


            return myInspector;
        }
    }
}
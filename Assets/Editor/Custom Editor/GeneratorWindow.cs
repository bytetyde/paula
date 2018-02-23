using Assets.Scripts.Level_Generator.Generator;
using Assets.Scripts.Level_Generator.Gramatical_Textreplacer;
using Assets.Scripts.Level_Generator.Infinite;
using Assets.Scripts.Level_Generator.Instantiater;
using Assets.Scripts.Level_Generator.Populator;
using Assets.Scripts.Level_Generator.Setup;
using Assets.Scripts.Level_Generator.Textreplacer_PreSetup;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AnimatedValues;
#endif

namespace Assets.Editor.Custom_Editor
{
    public class GeneratorWindow : EditorWindow
    {
        [MenuItem("PAULA/PAULA Generator")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = (GeneratorWindow) GetWindow(typeof (GeneratorWindow));
            window.titleContent = new GUIContent("PAULA Generator");
            window.minSize = new Vector2(750,400);
        }

        private Vector2 scrollPosition = new Vector2(0f, 0f);
        private AnimBool _showSetupSettings;
        private AnimBool _showInfiniteSettings;
        private AnimBool _showStringParser;
        private AnimBool _showGteEvolution;
        private AnimBool _showLevelInstantiater;
        private AnimBool _showLevelPopulator;

        void OnEnable()
        {
            _showSetupSettings = new AnimBool(true);
            _showSetupSettings.valueChanged.AddListener(Repaint);

            _showInfiniteSettings = new AnimBool(true);
            _showInfiniteSettings.valueChanged.AddListener(Repaint);

            _showStringParser = new AnimBool(true);
            _showStringParser.valueChanged.AddListener(Repaint);

            _showGteEvolution = new AnimBool(true);
            _showGteEvolution.valueChanged.AddListener(Repaint);

            _showLevelInstantiater = new AnimBool(true);
            _showLevelInstantiater.valueChanged.AddListener(Repaint);

            _showLevelPopulator= new AnimBool(true);
            _showLevelPopulator.valueChanged.AddListener(Repaint);

        }

        private void OnGUI()
        {
            var levelGeneratorObj = Selection.activeGameObject;

            if (levelGeneratorObj == null || levelGeneratorObj.GetComponent<LevelGenerator>() == null) return;

            var setupSettings = levelGeneratorObj.GetComponent<PCGSetup>();
            var infiniteLevelSettings = levelGeneratorObj.GetComponent<InfiniteLevelSettings>();
            var stringParser = levelGeneratorObj.GetComponent<StringParser>();
            var geHolder = levelGeneratorObj.GetComponent<StringReplacerHolder>();
            var levelGenerator = levelGeneratorObj.GetComponent<LevelGenerator>();
            var levelInstantiater = levelGeneratorObj.GetComponent<LevelInstantiater>();
            var levelPopulator = levelGeneratorObj.GetComponent<LevelPopulator>();
        
            var scrollViewX = 0f;
            var scrollViewY = 0f;
            var scrollViewW = Screen.width;
            var scrollViewH = Screen.height - 20;

            GUILayout.BeginArea(new Rect(scrollViewX, scrollViewY, scrollViewW, scrollViewH));
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            _showSetupSettings.target = EditorGUILayout.ToggleLeft("Show Setup Settings", _showSetupSettings.target);
            if (EditorGUILayout.BeginFadeGroup(_showSetupSettings.faded))
            {
                GUILayout.BeginVertical();
                if (setupSettings != null)
                {
                    var editor = UnityEditor.Editor.CreateEditor(setupSettings);
                    editor.OnInspectorGUI();
                }
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFadeGroup();

            _showInfiniteSettings.target = EditorGUILayout.ToggleLeft("Show Infinite Settings", _showInfiniteSettings.target);
            if (EditorGUILayout.BeginFadeGroup(_showInfiniteSettings.faded))
            {
                GUILayout.BeginVertical();
                if (infiniteLevelSettings != null)
                {
                    var editor = UnityEditor.Editor.CreateEditor(infiniteLevelSettings);
                    editor.OnInspectorGUI();
                }

                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFadeGroup();

            _showStringParser.target = EditorGUILayout.ToggleLeft("Show Level Grammar", _showStringParser.target);
            if (EditorGUILayout.BeginFadeGroup(_showStringParser.faded))
            {
                GUILayout.BeginVertical();
                if (stringParser != null)
                {
                    var editor = UnityEditor.Editor.CreateEditor(stringParser);
                    editor.OnInspectorGUI();
                }

                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFadeGroup();

            _showGteEvolution.target = EditorGUILayout.ToggleLeft("Show GE Evolution Options", _showGteEvolution.target);
            if (EditorGUILayout.BeginFadeGroup(_showGteEvolution.faded))
            {
                GUILayout.BeginVertical();

                if (geHolder != null)
                {
                    var editor = UnityEditor.Editor.CreateEditor(geHolder);
                    editor.OnInspectorGUI();
                }

                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFadeGroup();
        
            _showLevelInstantiater.target = EditorGUILayout.ToggleLeft("Show Level Instantiater Options", _showLevelInstantiater.target);
            if (EditorGUILayout.BeginFadeGroup(_showLevelInstantiater.faded))
            {
                GUILayout.BeginVertical();

                if (levelInstantiater != null)
                {
                    var editor = UnityEditor.Editor.CreateEditor(levelInstantiater);
                    editor.OnInspectorGUI();
                }
            
                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFadeGroup();

            _showLevelPopulator.target = EditorGUILayout.ToggleLeft("Show Level Populator Options", _showLevelPopulator.target);
            if (EditorGUILayout.BeginFadeGroup(_showLevelPopulator.faded))
            {
                GUILayout.BeginVertical();

                if (levelPopulator != null)
                {
                    var editor = UnityEditor.Editor.CreateEditor(levelPopulator);
                    editor.OnInspectorGUI();
                }

                GUILayout.EndVertical();
            }
            EditorGUILayout.EndFadeGroup();

            GUILayout.BeginHorizontal();
       
            GUILayout.BeginVertical();

            if (levelGenerator != null)
            {
                var editor = UnityEditor.Editor.CreateEditor(levelGenerator);
                editor.OnInspectorGUI();
            }
            GUILayout.EndVertical();
        
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(10);
            GUI.EndScrollView();
            GUILayout.EndArea();
        }
    }
}
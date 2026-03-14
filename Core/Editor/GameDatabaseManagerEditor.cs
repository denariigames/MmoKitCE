using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MultiplayerARPG
{
    public class GameDatabaseManagerEditor : EditorWindow
    {
        private List<GameDataListSection> sections;
        private Vector2 menuScrollPosition;
        private int selectedMenuIndex;
        private GameDatabase selectedDatabase;

        [MenuItem(EditorMenuConsts.GAME_DATABASE_MENU, false, EditorMenuConsts.GAME_DATABASE_ORDER)]
        public static void CreateNewEditor()
        {
            GetWindow<GameDatabaseManagerEditor>();
        }

        private void OnEnable()
        {
            selectedDatabase = null;
            sections = new List<GameDataListSection>()
            {
                new GameDataListSection<Attribute>("attributes", "Attributes"),
                new GameDataListSection<Currency>("currencies", "Currencies"),
                new GameDataListSection<DamageElement>("damageElements", "Damage Elements"),
                new GameDataListSection<BaseItem>("items", "Items"),
                new GameDataListSection<ItemCraftFormula>("itemCraftFormulas", "Item Crafts"),
                new GameDataListSection<ArmorType>("armorTypes", "Armor Types"),
                new GameDataListSection<WeaponType>("weaponTypes", "Weapon Types"),
                new GameDataListSection<AmmoType>("ammoTypes", "Ammo Types"),
                new GameDataListSection<StatusEffect>("statusEffects", "Status Effects"),
                new GameDataListSection<BaseSkill>("skills", "Skills"),
                new GameDataListSection<GuildSkill>("guildSkills", "Guild Skills"),
                new GameDataListSection<GuildIcon>("guildIcons", "Guild Icons"),
                new GameDataListSection<PlayerCharacter>("playerCharacters", "Player Characters\n(aka Character Class)"),
                new GameDataListSection<MonsterCharacter>("monsterCharacters", "Monster Characters"),
                new GameDataListSection<Harvestable>("harvestables", "Harvestables"),
                new GameDataListSection<BaseMapInfo>("mapInfos", "Map Infos"),
                new GameDataListSection<Quest>("quests", "Quests"),
                new GameDataListSection<Faction>("factions", "Factions"),
            };
        }

        private void OnDisable()
        {
            EditorGlobalData.WorkingDatabase = null;
        }

        private void OnGUI()
        {
            titleContent = new GUIContent("Game Database", null, "Game Database");
            if (EditorGlobalData.WorkingDatabase == null)
            {
                Vector2 wndRect = new Vector2(500, 100);
                minSize = wndRect;

                GUILayout.BeginVertical("Game Database", "window");
                {
                    GUILayout.BeginVertical("box");
                    {
                        if (EditorGlobalData.WorkingDatabase == null)
                            EditorGUILayout.HelpBox("Select the game database that you want to edit", MessageType.Info);
                        EditorGlobalData.WorkingDatabase = EditorGUILayout.ObjectField("Game database", EditorGlobalData.WorkingDatabase, typeof(GameDatabase), true, GUILayout.ExpandWidth(true)) as GameDatabase;
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
                return;
            }
            else
            {
                Vector2 wndRect = new Vector2(500, 500);
                minSize = wndRect;
            }

            if (EditorGlobalData.WorkingDatabase != selectedDatabase)
            {
                selectedDatabase = EditorGlobalData.WorkingDatabase;
                selectedDatabase.LoadReferredData();
            }

            // Prepare GUI styles
            GUIStyle leftMenuButton = new GUIStyle(EditorStyles.label);
            leftMenuButton.fontSize = 12;
            leftMenuButton.alignment = TextAnchor.MiddleLeft;

            GUIStyle selectedLeftMenuButton = new GUIStyle(EditorStyles.label);
            selectedLeftMenuButton.fontSize = 12;
            selectedLeftMenuButton.alignment = TextAnchor.MiddleLeft;
            var background = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            background.SetPixel(0, 0, EditorGUIUtility.isProSkin ? new Color(0.243f, 0.373f, 0.588f) : new Color(0.247f, 0.494f, 0.871f));
            background.Apply();
            selectedLeftMenuButton.active.background = selectedLeftMenuButton.normal.background = background;

            // Left menu
            GUILayout.BeginArea(new Rect(0, 0, 145, position.height), string.Empty, "box");
            {
                menuScrollPosition = GUILayout.BeginScrollView(menuScrollPosition);
                GUILayout.BeginVertical();
                {
                    for (int i = 0; i < sections.Count; ++i)
                    {
                        if (GUILayout.Button(sections[i].MenuTitle, (i == selectedMenuIndex ? selectedLeftMenuButton : leftMenuButton), GUILayout.Height(32)))
                        {
                            selectedMenuIndex = i;
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();

            // Content
            GUILayout.BeginArea(new Rect(145, 0, position.width - 145, position.height), string.Empty);
            {
                sections[selectedMenuIndex].OnGUI(position.width - 145, position.height);
            }
            GUILayout.EndArea();
        }
    }
}

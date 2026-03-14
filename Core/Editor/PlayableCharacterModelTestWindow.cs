using Playables = MultiplayerARPG.GameData.Model.Playables;
using UnityEditor;
using UnityEngine;

namespace MultiplayerARPG
{
    public class PlayableCharacterModelTestWindow : EditorWindow
    {
        [System.Flags]
        public enum MoveDir1
        {
            None = 0,
            Forward = 1 << 0,
            Backward = 1 << 1,
        }

        [System.Flags]
        public enum MoveDir2
        {
            None = 0,
            Left = 1 << 0,
            Right = 1 << 1,
        }

        public enum Grounding
        {
            Grounded,
            Jumping,
            Falling,
            Landed,
            UnderWater,
        }

        public enum Action
        {
            None,
            Hurt,
            Dead,
            Pickup,
            AttackMainHand,
            AttackOffHand,
            SkillCast,
            SkillActivate,
        }

        protected GameObject go;
        protected AnimationClip animationClip;
        protected float time = 0.0f;
        protected bool lockSelection = false;
        protected bool animationMode = false;
        protected int attackIndex = 0;
        protected WeaponType weaponType;
        protected BaseSkill skill;
        protected MoveDir1 moveDir1;
        protected MoveDir2 moveDir2;
        protected Grounding grounding;
        protected ExtraMovementState extraMovementState;
        protected Action action;
        protected Playables.PlayableCharacterModel model;

        [MenuItem("MMORPG KIT/Test Playable Character Model Animation", false, 10100)]
        public static void CreateNewEditor()
        {
            PlayableCharacterModelTestWindow window = GetWindowWithRect<PlayableCharacterModelTestWindow>(new Rect(0, 0, 300, 200));
            window.Show();
        }

        public void OnSelectionChange()
        {
            if (!lockSelection)
            {
                go = null;
                GameObject activeGameObject = Selection.activeGameObject;
                if (activeGameObject != null && activeGameObject.TryGetComponent(out model) && model != null && model.animator != null)
                {
                    Animator animator = model.animator != null ? model.animator : model.GetComponentInChildren<Animator>();
                    if (animator != null)
                    {
                        go = animator.gameObject;
                        animationClip = model.defaultAnimations.idleState.clip;
                    }
                }
                Repaint();
            }
        }

        private void OnDestroy()
        {
            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();
        }

        // Main editor window
        public void OnGUI()
        {
            // Wait for user to select a GameObject
            if (go == null)
            {
                EditorGUILayout.HelpBox("Please select a entity which already has animator component setup", MessageType.Info);
                return;
            }

            // Animate and Lock buttons.  Check if Animate has changed
            GUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            GUILayout.Toggle(AnimationMode.InAnimationMode(), "Animate");
            if (EditorGUI.EndChangeCheck())
                ToggleAnimationMode();

            GUILayout.FlexibleSpace();
            lockSelection = GUILayout.Toggle(lockSelection, "Lock");
            GUILayout.EndHorizontal();

            // Slider to use when Animate has been ticked
            EditorGUILayout.BeginVertical();

            weaponType = EditorGUILayout.ObjectField(weaponType, typeof(WeaponType), false) as WeaponType;
            skill = EditorGUILayout.ObjectField(skill, typeof(BaseSkill), false) as BaseSkill;
            moveDir1 = (MoveDir1)EditorGUILayout.EnumPopup("Movement Dir1", moveDir1);
            moveDir2 = (MoveDir2)EditorGUILayout.EnumPopup("Movement Dir2", moveDir2);
            grounding = (Grounding)EditorGUILayout.EnumPopup("Grounding", grounding);
            extraMovementState = (ExtraMovementState)EditorGUILayout.EnumPopup("Extra Movement State", extraMovementState);
            action = (Action)EditorGUILayout.EnumPopup("Action", action);

            if (action == Action.AttackMainHand || action == Action.AttackOffHand)
                attackIndex = EditorGUILayout.IntField("Attack Index", attackIndex);

            if (grounding == Grounding.Grounded)
            {
                if (extraMovementState == ExtraMovementState.IsCrawling)
                {
                    if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims))
                        SetMovementClip(anims.crawlIdleState, anims.crawlMoveStates);
                    else
                        SetMovementClip(model.defaultAnimations.crawlIdleState, model.defaultAnimations.crawlMoveStates);
                }
                else if (extraMovementState == ExtraMovementState.IsCrouching)
                {
                    if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims))
                        SetMovementClip(anims.crouchIdleState, anims.crouchMoveStates);
                    else
                        SetMovementClip(model.defaultAnimations.crouchIdleState, model.defaultAnimations.crouchMoveStates);
                }
                else if (extraMovementState == ExtraMovementState.IsWalking)
                {
                    if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims))
                        SetMovementClip(anims.idleState, anims.walkStates);
                    else
                        SetMovementClip(model.defaultAnimations.idleState, model.defaultAnimations.walkStates);
                }
                else if (extraMovementState == ExtraMovementState.IsSprinting)
                {
                    if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims))
                        SetMovementClip(anims.idleState, anims.sprintStates);
                    else
                        SetMovementClip(model.defaultAnimations.idleState, model.defaultAnimations.sprintStates);
                }
                else
                {
                    if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims))
                        SetMovementClip(anims.idleState, anims.moveStates);
                    else
                        SetMovementClip(model.defaultAnimations.idleState, model.defaultAnimations.moveStates);
                }
            }
            else if (grounding == Grounding.UnderWater)
            {
                if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims))
                    SetMovementClip(anims.swimIdleState, anims.swimMoveStates);
                else
                    SetMovementClip(model.defaultAnimations.swimIdleState, model.defaultAnimations.swimMoveStates);
            }
            else if (grounding == Grounding.Jumping)
            {
                if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims))
                    animationClip = anims.jumpState.clip;
                else
                    animationClip = model.defaultAnimations.jumpState.clip;
            }
            else if (grounding == Grounding.Landed)
            {
                if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims))
                    animationClip = anims.landedState.clip;
                else
                    animationClip = model.defaultAnimations.landedState.clip;
            }
            else
            {
                if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims))
                    animationClip = anims.fallState.clip;
                else
                    animationClip = model.defaultAnimations.fallState.clip;
            }
            if (action == Action.Hurt)
            {
                if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims))
                    animationClip = anims.hurtState.clip;
                else
                    animationClip = model.defaultAnimations.hurtState.clip;
            }
            else if (action == Action.Dead)
            {
                if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims))
                    animationClip = anims.deadState.clip;
                else
                    animationClip = model.defaultAnimations.deadState.clip;
            }
            else if (action == Action.Pickup)
            {
                if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims))
                    animationClip = anims.pickupState.clip;
                else
                    animationClip = model.defaultAnimations.pickupState.clip;
            }
            else if (action == Action.AttackMainHand)
            {
                if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims) && anims.rightHandAttackAnimations != null && attackIndex < anims.rightHandAttackAnimations.Length)
                    animationClip = anims.rightHandAttackAnimations[attackIndex].state.clip;
                else if (attackIndex < model.defaultAnimations.rightHandAttackAnimations.Length)
                    animationClip = model.defaultAnimations.rightHandAttackAnimations[attackIndex].state.clip;
            }
            else if (action == Action.AttackOffHand)
            {
                if (TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims) && anims.leftHandAttackAnimations != null && attackIndex < anims.leftHandAttackAnimations.Length)
                    animationClip = anims.leftHandAttackAnimations[attackIndex].state.clip;
                else if (attackIndex < model.defaultAnimations.leftHandAttackAnimations.Length)
                    animationClip = model.defaultAnimations.leftHandAttackAnimations[attackIndex].state.clip;
            }
            else if (action == Action.SkillCast)
            {
                if (TryGetSkillAnims(out Playables.SkillAnimations anims) && anims.castState.clip != null)
                    animationClip = anims.castState.clip;
                else
                    animationClip = model.defaultAnimations.skillCastState.clip;
            }
            else if (action == Action.SkillActivate)
            {
                if (TryGetSkillAnims(out Playables.SkillAnimations anims) && anims.activateAnimation.state.clip != null)
                    animationClip = anims.activateAnimation.state.clip;
                else
                    animationClip = model.defaultAnimations.skillActivateAnimation.state.clip;
            }

            animationClip = EditorGUILayout.ObjectField(animationClip, typeof(AnimationClip), false) as AnimationClip;
            if (animationClip != null)
            {
                float startTime = 0.0f;
                float stopTime = animationClip.length;
                time = EditorGUILayout.Slider(time, startTime, stopTime);
            }
            else if (AnimationMode.InAnimationMode())
            {
                AnimationMode.StopAnimationMode();
            }
            EditorGUILayout.EndVertical();
        }

        void SetMovementClip(Playables.AnimState idleState, Playables.MoveStates moveStates)
        {
            if (moveDir1 == MoveDir1.Forward)
            {
                if (moveDir2 == MoveDir2.Right)
                {
                    animationClip = moveStates.forwardRightState.clip;
                }
                else if (moveDir2 == MoveDir2.Left)
                {
                    animationClip = moveStates.forwardLeftState.clip;
                }
                else
                {
                    animationClip = moveStates.forwardState.clip;
                }
            }
            else if (moveDir1 == MoveDir1.Backward)
            {
                if (moveDir2 == MoveDir2.Right)
                {
                    animationClip = moveStates.backwardRightState.clip;
                }
                else if (moveDir2 == MoveDir2.Left)
                {
                    animationClip = moveStates.backwardLeftState.clip;
                }
                else
                {
                    animationClip = moveStates.backwardState.clip;
                }
            }
            else if (moveDir2 == MoveDir2.Right)
            {
                animationClip = moveStates.rightState.clip;
            }
            else if (moveDir2 == MoveDir2.Left)
            {
                animationClip = moveStates.leftState.clip;
            }
            else
            {
                animationClip = idleState.clip;
            }
        }

        bool TryGetWeaponTypeBasedAnims(out Playables.WeaponAnimations anims)
        {
            anims = default;
            if (weaponType == null)
                return false;
            for (int i = 0; i < model.weaponAnimations.Length; ++i)
            {
                if (model.weaponAnimations[i].weaponType == weaponType)
                {
                    anims = model.weaponAnimations[i];
                    return true;
                }
            }
            return false;
        }

        bool TryGetSkillAnims(out Playables.SkillAnimations anims)
        {
            anims = default;
            if (skill == null)
                return false;
            for (int i = 0; i < model.skillAnimations.Length; ++i)
            {
                if (model.skillAnimations[i].skill == skill)
                {
                    anims = model.skillAnimations[i];
                    return true;
                }
            }
            return false;
        }

        void Update()
        {
            if (go == null)
                return;

            if (animationClip == null)
                return;

            // Animate the GameObject
            if (!EditorApplication.isPlaying && AnimationMode.InAnimationMode())
            {
                time += Time.deltaTime;
                AnimationMode.BeginSampling();
                AnimationMode.SampleAnimationClip(go, animationClip, time);
                AnimationMode.EndSampling();
                if (time > animationClip.length)
                    time = 0f;
                Repaint();
            }
        }

        void ToggleAnimationMode()
        {
            if (AnimationMode.InAnimationMode())
                AnimationMode.StopAnimationMode();
            else
                AnimationMode.StartAnimationMode();
        }
    }
}

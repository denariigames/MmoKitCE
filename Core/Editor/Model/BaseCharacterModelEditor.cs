using Insthync.UnityEditorUtils.Editor;
using UnityEditor;

namespace MultiplayerARPG
{
    [CustomEditor(typeof(BaseCharacterModel), true)]
    [CanEditMultipleObjects]
    public class BaseCharacterModelEditor : BaseCustomEditor
    {
        protected override void SetFieldCondition()
        {
            BaseCharacterModel model = target as BaseCharacterModel;
            CharacterModelManager manager = model.GetComponent<CharacterModelManager>();
            if (manager == null)
                manager = model.GetComponentInParent<CharacterModelManager>();
            if (manager != null)
            {
                model.MainModel = manager.MainTpsModel;
                if (manager.MainFpsModel == model)
                    model.MainModel = model;
            }
            else
            {
                model.MainModel = model;
            }
            ShowOnBool(nameof(model.IsMainModel), true, "hiddingObjects");
            ShowOnBool(nameof(model.IsMainModel), true, "hiddingRenderers");
            ShowOnBool(nameof(model.IsMainModel), true, "fpsHiddingObjects");
            ShowOnBool(nameof(model.IsMainModel), true, "fpsHiddingRenderers");
            ShowOnBool(nameof(model.IsMainModel), true, "effectContainers");
            ShowOnBool(nameof(model.IsMainModel), true, "setEffectContainersBySetters");
            ShowOnBool(nameof(model.IsMainModel), true, "equipmentContainers");
            ShowOnBool(nameof(model.IsMainModel), true, "setEquipmentContainersBySetters");
            ShowOnBool(nameof(model.IsMainModel), true, "deactivateInstantiatedObjects");
            ShowOnBool(nameof(model.IsMainModel), true, "activateInstantiatedObject");
            ShowOnBool(nameof(model.IsMainModel), true, "vehicleModels");
        }
    }
}

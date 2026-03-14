using UnityEngine;

namespace MultiplayerARPG
{
    public interface ITargetableEntity
    {
        Transform EntityTransform { get; }
        GameObject EntityGameObject { get; }
        /// <summary>
        /// This must return `TRUE`, if you want controller to set this entity as target immediately in one click. if it is `FALSE`, first click will select it for information viewing, second will set it as target.
        /// </summary>
        /// <returns></returns>
        bool SetAsTargetInOneClick();
        /// <summary>
        /// This must return `TRUE`, if you want controller to *NOT* set this entity as selecting entity. For some entities such as building foundation may allow character to move pass it so set it to `TRUE` to allow to move pass it.
        /// </summary>
        /// <returns></returns>
        bool NotBeingSelectedOnClick();
    }
}

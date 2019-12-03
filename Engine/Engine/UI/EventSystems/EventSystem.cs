using System.Collections.Generic;
using Engine.Core;
using Engine.Debug;
using OpenTK;

namespace Engine.UI.EventSystems
{
    /// <summary>
    /// EventSystem handles all Selectable UI Elements in the Game
    /// </summary>
    public class EventSystem
    {


        private List<ISelectable> selectables = new List<ISelectable>();

        /// <summary>
        /// Registers an element
        /// </summary>
        /// <param name="element">Element to register</param>
        public void Register(ISelectable element)
        {
            if (!selectables.Contains(element))
            {
                selectables.Add(element);
            }
        }

        /// <summary>
        /// Unregisters an element
        /// </summary>
        /// <param name="element">Element to unregister</param>
        public void Unregister(ISelectable element)
        {
            if (selectables.Contains(element))
            {
                selectables.Remove(element);
            }
        }

        /// <summary>
        /// Updates the Event System
        /// And Changes the states of the Registered elements
        /// </summary>
        public void Update()
        {
            Vector2 mpos = GameEngine.Instance.MousePosition - GameEngine.Instance.WindowSize / 2;
            mpos.X /= GameEngine.Instance.Width;
            mpos.Y /= GameEngine.Instance.Height;
            mpos.Y *= -1;
            mpos *= 2;
            for (int i = 0; i < selectables.Count; i++)
            {
                if (selectables[i].BoundingBox.Contains(mpos))
                {
                    if (Input.LeftMb)
                    {
                        selectables[i].SetState(SelectableState.Selected);
                        Logger.Log("Clicking", DebugChannel.Log, 10);
                    }
                    else
                    {
                        selectables[i].SetState(SelectableState.Hovered);
                    }
                }
                else
                {
                    selectables[i].SetState(SelectableState.None);
                }
            }
        }
    }
}
using System.Collections.Generic;
using Engine.Core;
using Engine.Debug;
using OpenTK;

namespace Engine.UI.EventSystems
{
    public class EventSystem
    {
        private List<ISelectable> selectables = new List<ISelectable>();

        public void Register(ISelectable element)
        {
            if (!selectables.Contains(element))
                selectables.Add(element);
        }

        public void Unregister(ISelectable element)
        {
            if (selectables.Contains(element))
                selectables.Remove(element);
        }


        public void Update()
        {
            Vector2 mpos = GameEngine.Instance.MousePosition - GameEngine.Instance.WindowSize / 2;
            mpos.X /= GameEngine.Instance.Width;
            mpos.Y /= GameEngine.Instance.Height;
            mpos.Y *= -1;
            mpos *= 2;
            //Logger.Log("MousePos: "+ mpos, DebugChannel.Log, 10);
            for (int i = 0; i < selectables.Count; i++)
            {
                if (selectables[i].BoundingBox.Contains(mpos))
                {
                    if (Input.LeftMB)
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
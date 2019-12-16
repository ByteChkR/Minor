namespace Engine.UI.EventSystems
{
    /// <summary>
    /// Selectable States a UI Element Can be in
    /// </summary>
    public enum SelectableState
    {
        /// <summary>
        /// Idle State, the UI Element is not interacted with
        /// </summary>
        None,
        /// <summary>
        /// The cursor is currently hovering the UI Element
        /// </summary>
        Hovered,
        /// <summary>
        /// The user clicked on the UI Element
        /// </summary>
        Selected
    }
}
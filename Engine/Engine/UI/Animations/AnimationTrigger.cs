namespace Engine.UI.Animations
{
    /// <summary>
    /// All possible Animation Triggers
    /// </summary>
    public enum AnimationTrigger
    {
        /// <summary>
        /// No Animation Trigger
        /// Can only be Triggered by manually triggering the ISelectable
        /// </summary>
        None,
        /// <summary>
        /// When Added to the Event System
        /// Can be used to make the UI have a fly in animation
        /// </summary>
        OnLoad,
        /// <summary>
        /// Animation To play when entering the hover state
        /// </summary>
        OnHover,
        /// <summary>
        /// Animation To play when entering the bounding box of the ISelectable
        /// </summary>
        OnEnter,
        /// <summary>
        /// Animation To play when clicked on
        /// </summary>
        OnClick,
        /// <summary>
        /// Animation To play when leaving the bounding box of the ISelectable
        /// </summary>
        OnLeave
    }
}
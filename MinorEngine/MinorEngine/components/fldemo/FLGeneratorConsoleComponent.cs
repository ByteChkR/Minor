using System.Text;
using FilterLanguage;
using GameEngine.engine.components;
using GameEngine.engine.core;
using GameEngine.engine.ui;
using OpenTK;
using OpenTK.Input;

namespace GameEngine.components.fldemo
{
    public class FLGeneratorConsoleComponent : AbstractComponent
    {
        private const string HelpText = "Press C to Open the FL Console";

        private GameObject _quadDisplay;
        private GameObject _sphereDisplay;
        private UITextRendererComponent _textRenderer;
        private StringBuilder _sb;
        

        private bool _textInputActive;
        private bool _blinkActive;
        private float _blinkMaxTime = 0.5f;
        private float _blinkTime;

        public delegate void OnLineFinished(string line);

        private OnLineFinished _currentDel;

        protected override void Awake()
        {
            base.Awake();

            _quadDisplay = AbstractGame.Instance.World.GetChildWithName("QuadDisplay", true);
            _sphereDisplay = AbstractGame.Instance.World.GetChildWithName("SphereDisplay", true);
            _textRenderer = Owner.GetComponent<UITextRendererComponent>();
            _sb = new StringBuilder();
        }

        protected override void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (_textInputActive)
            {
                _sb.Append(e.KeyChar);
            }

        }

        protected override void OnKeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _currentDel?.Invoke(_sb.ToString());
                _currentDel = null;
                _textInputActive = false;
            }
            else if (_sb.Length > 0 && e.Key == Key.BackSpace)
            {
                _sb.Remove(_sb.Length - 1, 1);
            }
            else if (e.Key == Key.Escape)
            {
                _sb.Clear();
            }
        }

        public void RequestInput(OnLineFinished callback)
        {
            _textInputActive = true;
            _sb.Clear();
            _currentDel = callback;
        }

        protected override void Update(float deltaTime)
        {
            _blinkTime += deltaTime;
            if (_blinkTime >= _blinkMaxTime)
            {
                _blinkTime = 0;
                _blinkActive = !_blinkActive;
            }

            if (_textInputActive)
            {
                _textRenderer.Text = _sb.ToString() + (_blinkActive ? "_" : "");
            }
            else
            {
                _textRenderer.Text = HelpText;
            }

        }
    }
}
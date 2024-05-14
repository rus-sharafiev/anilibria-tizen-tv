using System;
using Tizen.NUI.BaseComponents;
using Tizen.NUI;

namespace AnilibriaAppTizen.Views
{
    internal class Error
    {
        private View _errorView;

        public Error(Exception error)
        {
            ShowError(error);
        }

        public void ShowError(Exception error)
        {
            _errorView = new View
            {
                Size = Window.Instance.Size,
                BackgroundColor = Color.Black,
            };

            TextLabel errorLabel = new TextLabel
            {
                Text = error.InnerException?.Message ?? error.Message,
                MultiLine = true,
                TextColor = Color.White,
                PointSize = 32.0f,

                SizeModeFactor = new Vector3(0.8f, 0.8f, 1.0f),
                WidthResizePolicy = ResizePolicyType.SizeRelativeToParent,
                HeightResizePolicy = ResizePolicyType.SizeRelativeToParent,
                ParentOrigin = new Position(0.1f, 0.1f),

                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };

            _errorView.Add(errorLabel);
            Window.Instance.Add(errorLabel);
        }

        public void Disppose()
        {
            _errorView.Unparent();
            _errorView.Dispose();
        }
    }
}

using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class LoadingIndicator
    {
        private View _loading;

        public LoadingIndicator() 
        {

        }

        public LoadingIndicator AddTo(View parent = null) 
        {
            _loading = new View
            {
                ParentOrigin = Position.ParentOriginBottomRight,
                PivotPoint = Position.PivotPointBottomRight,
                PositionUsesPivotPoint = true,
            };
            Window.Instance.Add(_loading);

            if (parent != null)
                _loading.Size = parent.Size;
            else
                _loading.Size = Window.Instance.Size;

            TextLabel loading = new TextLabel
            {
                Text = "Loading...",
                MultiLine = true,
                TextColor = Color.White,
                PointSize = 32.0f,
                Size = _loading.Size,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            _loading.Add(loading);

            return this;
        }

        public void Remove() 
        {
            if (_loading != null)
            {
                _loading.Unparent();
                _loading.Dispose();
                _loading = null;
            }
        }
    }
}

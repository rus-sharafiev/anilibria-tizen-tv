using System.Data.Common;
using Tizen.NUI;
using Tizen.NUI.BaseComponents;

namespace AnilibriaAppTizen.Views
{
    internal class ScrollContainer
    {
        private View _scrollContainer;
        private Animation _scrollAnimation;
        private int _animationDuration;
        private AlphaFunction _alphaFunction;
        private float _scrollPosition;
        private float _topScrollIndentation;
        private float _bottomScrollIndentation;

        private View[,] _focusMatrix;
        private View _leftView;
        private View _topView;

        public View View
        {
            get => _scrollContainer;
        }
        public AlphaFunction AlphaFunction
        {
            get => _alphaFunction;
            set => _alphaFunction = value;
        }
        public int AnimationDuration
        {
            get => _animationDuration;
            set => _animationDuration = value;
        }
        public float TopScrollIndentation
        {
            get => _topScrollIndentation;
            set => _topScrollIndentation = value;
        }
        public float BottomScrollIndentation
        {
            get => _bottomScrollIndentation;
            set => _bottomScrollIndentation = value;
        }
        public View LeftFocusableView
        {
            set => _leftView = value;
        }
        public View UpFocusableView
        {
            set => _topView = value;
        }

        public ScrollContainer()
        {
            _animationDuration = 280;
            _scrollAnimation = new Animation(_animationDuration);
            _alphaFunction = new AlphaFunction(AlphaFunction.BuiltinFunctions.EaseOutSquare);

            Initialize();
        }

        private void Initialize()
        {
            _scrollPosition = 0;
            _scrollContainer = new View
            {
                //ClippingMode = ClippingModeType.ClipChildren
            };
        }

        public void ScrollTo(View focusedView)
        {
            float focusedViewTop = focusedView.PositionY;
            float focusedViewBottom = focusedView.PositionY + focusedView.SizeHeight;

            var parentView = _scrollContainer.GetParent() as View;

            float visibleRectangleTop = -_scrollContainer.PositionY + _topScrollIndentation;
            float visibleRectangleBottom = -_scrollContainer.PositionY + parentView.SizeHeight - _bottomScrollIndentation;

            if (focusedViewTop < visibleRectangleTop)
            {
                _scrollPosition += visibleRectangleTop - focusedViewTop;
                AnimateTo(_scrollPosition);
            }
            else if (focusedViewBottom > visibleRectangleBottom)
            {
                _scrollPosition -= focusedViewBottom - visibleRectangleBottom;
                AnimateTo(_scrollPosition);
            }
        }

        public void AnimateTo(float destination)
        {
            if (_scrollAnimation != null)
            {
                if (_scrollAnimation.State == Animation.States.Playing) _scrollAnimation.Stop();
                _scrollAnimation.Clear();
            }
            _scrollAnimation = new Animation(_animationDuration);
            _scrollAnimation.AnimateTo(_scrollContainer, "PositionY", destination, _alphaFunction);
            _scrollAnimation.Play();
        }

        public void CreateFocusMatrix(int rows, int columns)
        {
            _focusMatrix = new View[rows, columns];
        }

        public void AddToFocusMatrix(int row, int column, View view)
        {
            _focusMatrix[row, column] = view;
        }

        public void GenerateFocusMatrix()
        {
            for (int r = 0; r < _focusMatrix.GetLength(0); r++)
                for (int c = 0; c < _focusMatrix.GetLength(1); c++)
                    if (_focusMatrix[r, c] != null)
                    {
                        if (r > 0)
                            _focusMatrix[r, c].UpFocusableView = GetClothestView(r - 1, c);
                        else if (r == 0)
                            _focusMatrix[r, c].UpFocusableView = _topView;

                        if (r < _focusMatrix.GetLength(0) - 1)
                            _focusMatrix[r, c].DownFocusableView = GetClothestView(r + 1, c);

                        if (c > 0)
                            _focusMatrix[r, c].LeftFocusableView = _focusMatrix[r, c - 1];
                        else if (c == 0)
                            _focusMatrix[r, c].LeftFocusableView = _leftView;

                        if (c < _focusMatrix.GetLength(1) - 1)
                            _focusMatrix[r, c].RightFocusableView = _focusMatrix[r, c + 1];
                    }
        }

        private View GetClothestView(int r, int c)
        {
            if (_focusMatrix[r, c] != null)
                return _focusMatrix[r, c];
            else if (c > 0)
                for (int i = c - 1; i > 0; i--)
                {
                    if (_focusMatrix[r, i] != null)
                    {
                        return _focusMatrix[r, i];
                    }
                }

            return null;
        }
    }
}

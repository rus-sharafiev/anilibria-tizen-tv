using ElmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _scrollContainer = new View();
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
    }
}

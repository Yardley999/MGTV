using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace SharedFx.UI.Animations
{
    public abstract class AnimationBase
    {
        #region Constructor

        public AnimationBase()
        {
        }

        #endregion

        #region Properties

        protected Storyboard _Storyboard = null;
        protected FrameworkElement AnimationTarget = null;

        #endregion

        #region Animation

        public virtual void Stop()
        {
            this._Storyboard.Stop();
        }

        #endregion

        #region Events

        protected Action<FrameworkElement> AnimationCompleted = null;

        #endregion

    }
}

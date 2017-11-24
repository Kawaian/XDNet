using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XDNet
{
    public class LayoutManager
    {
        public double HeightRatio { get; set; }

        private double _width = 0;
        private double _height = 0;
        private double _left = 0;
        private double _top = 0;

        public double Width { get { return _width; } }
        public double Height { get { return _height; } }
        public double Left { get { return _left; } }
        public double Top { get { return _top; } }

        public LayoutManager(double heightRatio = 0.15)
        {
            HeightRatio = heightRatio;
            Update();
        }

        public void Update()
        {
            double displayWidth = SystemParameters.PrimaryScreenWidth;
            double displayHeight = SystemParameters.PrimaryScreenHeight;

            switch (TaskbarInfo.Position)
            {
                // TODO(matyas): Support taskbar resize / update layout
                // TODO(matyas): Support notification location specification
                case TaskbarPosition.Top:
                    _width = SystemParameters.PrimaryScreenWidth;
                    _height = SystemParameters.PrimaryScreenHeight * HeightRatio;
                    _top = TaskbarInfo.DisplayBounds.Height;
                    _left = 0;

                    break;

                case TaskbarPosition.Bottom:
                    _width = SystemParameters.PrimaryScreenWidth;
                    _height = SystemParameters.PrimaryScreenHeight * HeightRatio;
                    _top = SystemParameters.PrimaryScreenHeight - TaskbarInfo.DisplayBounds.Height - _height;
                    _left = 0;

                    break;

                case TaskbarPosition.Left:
                    _width = SystemParameters.PrimaryScreenWidth - TaskbarInfo.DisplayBounds.Width;
                    _height = SystemParameters.PrimaryScreenHeight * HeightRatio;
                    _top = 0;
                    _left = TaskbarInfo.DisplayBounds.Width;
                    break;

                case TaskbarPosition.Right:
                    _width = SystemParameters.PrimaryScreenWidth - TaskbarInfo.DisplayBounds.Width;
                    _height = SystemParameters.PrimaryScreenHeight * HeightRatio;
                    _top = 0;
                    _left = SystemParameters.PrimaryScreenWidth - TaskbarInfo.DisplayBounds.Width - _width;
                    break;

                default:
                    break;
            }
        }
    }
}

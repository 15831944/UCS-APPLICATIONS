using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UCS_Road_Lines
{
    /// <summary>
    /// Interaction logic for SixLaneRoads.xaml
    /// </summary>
    public partial class SixLaneRoads : UserControl
    {
        public SixLaneRoads()
        {
            InitializeComponent();
            WidthsChanged();
        }

        private void SixL_W_NO_MED_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "6L-W-NO-MED-W";
            utils.nrLanes = 6;
            SetSelectedButton("6L-W-NO-MED-W");
            utils.currentlyActiveStyle = "Current Style: Six Lanes - No Median, 3 Lanes on each side";
        }

        private void SixL_NO_MED_NO_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "6L-NO-MED-NO-W";
            utils.nrLanes = 6;
            SetSelectedButton("6L-NO-MED-NO-W");
            utils.currentlyActiveStyle = "Current Style: Six Lanes - No Median, 3 Lanes on each side, No White Edge Lines";
        }

        private void SixL_W_MED_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "6L-W-MED-W";
            utils.nrLanes = 6;
            SetSelectedButton("6L-W-MED-W");
            utils.currentlyActiveStyle = "Current Style: Six Lanes - Divided Highway, 3 Lanes on each side";
        }

        private void SixL_MED_NO_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "6L-MED-NO-W";
            utils.nrLanes = 6;
            SetSelectedButton("6L-MED-NO-W");
            utils.currentlyActiveStyle = "Current Style: Six Lanes - Divided Highway, 3 Lanes each side, No White Edge Lines";
        }

        public void SetSelectedButton(string currentStyle)
        {
            /* Six Lane Styles
             * Button Name          roadStyle
             * SixL_W_NO_MED_W      6L-W-NO-MED-W   Six Lanes - No Median, 3 Lanes on each side
             * SixL_NO_MED_NO_W     6L-NO-MED-NO-W  Six Lanes - No Median, 3 Lanes on each side, No White Edge Lines
             * SixL_W_MED_W         6L-W-MED-W      Six Lanes - Divided Highway, 3 Lanes on each side
             * SixL_MED_NO_W        6L-MED-NO-W     Six Lanes - Divided Highway, 3 Lanes each side, No White Edge Lines
             */
             switch(currentStyle)
            {
                case "6L-W-NO-MED-W":
                    {
                        SixL_W_NO_MED_W.BorderBrush = Brushes.Red;
                        SixL_NO_MED_NO_W.BorderBrush = Brushes.Transparent;
                        SixL_W_MED_W.BorderBrush = Brushes.Transparent;
                        SixL_MED_NO_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "6L-NO-MED-NO-W":
                    {
                        SixL_W_NO_MED_W.BorderBrush = Brushes.Transparent;
                        SixL_NO_MED_NO_W.BorderBrush = Brushes.Red;
                        SixL_W_MED_W.BorderBrush = Brushes.Transparent;
                        SixL_MED_NO_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "6L-W-MED-W":
                    {
                        SixL_W_NO_MED_W.BorderBrush = Brushes.Transparent;
                        SixL_NO_MED_NO_W.BorderBrush = Brushes.Transparent;
                        SixL_W_MED_W.BorderBrush = Brushes.Red;
                        SixL_MED_NO_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "6L-MED-NO-W":
                    {
                        SixL_W_NO_MED_W.BorderBrush = Brushes.Transparent;
                        SixL_NO_MED_NO_W.BorderBrush = Brushes.Transparent;
                        SixL_W_MED_W.BorderBrush = Brushes.Transparent;
                        SixL_MED_NO_W.BorderBrush = Brushes.Red;
                        break;
                    }
                default:
                    {
                        SixL_W_NO_MED_W.BorderBrush = Brushes.Transparent;
                        SixL_NO_MED_NO_W.BorderBrush = Brushes.Transparent;
                        SixL_W_MED_W.BorderBrush = Brushes.Transparent;
                        SixL_MED_NO_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
            }
        }

        public void WidthsChanged()
        {
            var_LaneWidth_6LN.Content = utils.laneWidth.ToString();

            if (utils.drawEOP)
            {
                double leftShoulderWidth = Math.Abs(utils.leftShoulderWidth);
                var_LeftShoulderWidth_6LN.Content = leftShoulderWidth.ToString();
                double leftMedianWidth = Math.Abs(utils.leftMedianWidth);
                var_LeftMedianWidth_6LN.Content = leftMedianWidth.ToString();

                double rightShoulderWidth = Math.Abs(utils.rightShoulderWidth);
                var_RightShoulderWidth_6LN.Content = rightShoulderWidth.ToString();
                double rightMedianWidth = Math.Abs(utils.rightMedianWidth);
                var_RightMedianWidth_6LN.Content = rightMedianWidth.ToString();
            }
            else
            {
                var_LeftShoulderWidth_6LN.Content = "None";
                var_RightShoulderWidth_6LN.Content = "None";
                var_LeftMedianWidth_6LN.Content = "None";
                var_RightMedianWidth_6LN.Content = "None";
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            WidthsChanged();
            SetSelectedButton(utils.roadStyle);
            lbl_instructions6L.ToolTip = utils.currentlyActiveStyle;
        }
    }
}

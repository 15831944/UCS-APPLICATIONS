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
    /// Interaction logic for FourLaneRoads.xaml
    /// </summary>
    public partial class FourLaneRoads : UserControl
    {
        public FourLaneRoads()
        {
            InitializeComponent();
            WidthsChanged();
        }

        private void FourL_W_DW_DBY_DW_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "4L-W-DW-DBY-DW-W";
            utils.nrLanes = 4;
            SetSelectedButton("4L-W-DW-DBY-DW-W");
            utils.currentlyActiveStyle = "Current Style: Four Lanes - Double Yellow Centerline and White Side Lines";
        }

        private void FourL_DW_DBY_DW_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "4L-DW-DBY-DW";
            utils.nrLanes = 4;
            SetSelectedButton("4L-DW-DBY-DW");
            utils.currentlyActiveStyle = "Current Style: Four Lanes - Double Yellow Centerline and No White Side Lines";
        }

        private void FourLDH_W_DW_DBY_DW_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "4LDH-W-DW-DBY-DW-W";
            utils.nrLanes = 4;
            SetSelectedButton("4LDH-W-DW-DBY-DW-W");
            utils.currentlyActiveStyle = "Current Style: Four Lanes - Divided Highway with White Side Lines";
        }

        private void FourLDH_DW_DBY_DW_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "4LDH-DW-DBY-DW";
            utils.nrLanes = 4;
            SetSelectedButton("4LDH-DW-DBY-DW");
            utils.currentlyActiveStyle = "Current Style: Four Lanes - Divided Highway without White Side Lines";
        }

        public void SetSelectedButton(string currentStyle)
        {
            /* Four Lane Styles
             * Button Name              roadStyle
             * FourL_W_DW_DBY_DW_W      4L-W-DW-DBY-DW-W    Four Lanes - Double Yellow Centerline and White Side Lines
             * FourL_DW_DBY_DW          4L-DW-DBY-DW        Four Lanes - Double Yellow Centerline and No White Side Lines
             * FourLDH_W_DW_DBY_DW_W    4LDH-W-DW-DBY-DW-W  Four Lanes - Divided Highway with White Side Lines
             * FourLDH_DW_DBY_DW        4LDH-DW-DBY-DW      Four Lanes - Divided Highway without White Side Lines
             */
             switch(currentStyle)
            {
                case "4L-W-DW-DBY-DW-W":
                    {
                        FourL_W_DW_DBY_DW_W.BorderBrush = Brushes.Red;
                        FourL_DW_DBY_DW.BorderBrush = Brushes.Transparent;
                        FourLDH_W_DW_DBY_DW_W.BorderBrush = Brushes.Transparent;
                        FourLDH_DW_DBY_DW.BorderBrush = Brushes.Transparent;
                        break;
                    }

                case "4L-DW-DBY-DW":
                    {
                        FourL_W_DW_DBY_DW_W.BorderBrush = Brushes.Transparent;
                        FourL_DW_DBY_DW.BorderBrush = Brushes.Red;
                        FourLDH_W_DW_DBY_DW_W.BorderBrush = Brushes.Transparent;
                        FourLDH_DW_DBY_DW.BorderBrush = Brushes.Transparent;
                        break;
                    }

                case "4LDH-W-DW-DBY-DW-W":
                    {
                        FourL_W_DW_DBY_DW_W.BorderBrush = Brushes.Transparent;
                        FourL_DW_DBY_DW.BorderBrush = Brushes.Transparent;
                        FourLDH_W_DW_DBY_DW_W.BorderBrush = Brushes.Red;
                        FourLDH_DW_DBY_DW.BorderBrush = Brushes.Transparent;
                        break;
                    }

                case "4LDH-DW-DBY-DW":
                    {
                        FourL_W_DW_DBY_DW_W.BorderBrush = Brushes.Transparent;
                        FourL_DW_DBY_DW.BorderBrush = Brushes.Transparent;
                        FourLDH_W_DW_DBY_DW_W.BorderBrush = Brushes.Transparent;
                        FourLDH_DW_DBY_DW.BorderBrush = Brushes.Red;
                        break;
                    }
                default:
                    {
                        FourL_W_DW_DBY_DW_W.BorderBrush = Brushes.Transparent;
                        FourL_DW_DBY_DW.BorderBrush = Brushes.Transparent;
                        FourLDH_W_DW_DBY_DW_W.BorderBrush = Brushes.Transparent;
                        FourLDH_DW_DBY_DW.BorderBrush = Brushes.Transparent;
                        break;
                    }
            }
        }

        public void WidthsChanged()
        {
            var_LaneWidth_4LN.Content = utils.laneWidth.ToString();

            if (utils.drawEOP)
            {
                double leftShoulderWidth = Math.Abs(utils.leftShoulderWidth);
                var_LeftShoulderWidth_4LN.Content = leftShoulderWidth.ToString();
                double leftMedianWidth = Math.Abs(utils.leftMedianWidth);
                var_LeftMedianWidth_4LN.Content = leftMedianWidth.ToString();

                double rightShoulderWidth = Math.Abs(utils.rightShoulderWidth);
                var_RightShoulderWidth_4LN.Content = rightShoulderWidth.ToString();
                double rightMedianWidth = Math.Abs(utils.rightMedianWidth);
                var_RightMedianWidth_4LN.Content = rightMedianWidth.ToString();
            }
            else
            {
                var_LeftShoulderWidth_4LN.Content = "None";
                var_RightShoulderWidth_4LN.Content = "None";
                var_LeftMedianWidth_4LN.Content = "None";
                var_RightMedianWidth_4LN.Content = "None";
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            WidthsChanged();
            SetSelectedButton(utils.roadStyle);
            lbl_instructions4L.ToolTip = utils.currentlyActiveStyle;
        }
    }
}

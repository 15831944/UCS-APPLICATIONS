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
    /// Interaction logic for FiveLaneRoads.xaml
    /// </summary>
    public partial class FiveLaneRoads : UserControl
    {
        public FiveLaneRoads()
        {
            InitializeComponent();
            WidthsChanged();
        }

        private void FiveL_W_DW_Y_DY_DY_Y_DW_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "5L-W-DW-Y-DY-DY-Y-DW-W";
            utils.nrLanes = 5;
            SetSelectedButton("5L-W-DW-Y-DY-DY-Y-DW-W");
            utils.currentlyActiveStyle = "Current Style: Five Lanes - (Four Lane Road) with Center Turnout";
        }

        private void FiveL_DW_Y_DY_DY_Y_DW_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "5L-DW-Y-DY-DY-Y-DW";
            utils.nrLanes = 5;
            SetSelectedButton("5L-DW-Y-DY-DY-Y-DW");
            utils.currentlyActiveStyle = "Current Style: Five Lanes - (Four Lane Road) with Center Turnout but without White Edge Lines";
        }

        /* Five Lane Styles
         * Button Name                  roadStyle
         * FiveL_W_DW_Y_DY_DY_Y_DW      5L-W-DW-Y-DY-DY-Y-DW-W  Five Lanes - (Four Lane Road) with Center Turnout
         * FiveL_DW_Y_DY_DY_Y_DW        5L-DW-Y-DY-DY-Y-DW      Five Lanes - (Four Lane Road) with Center Turnout but without White Edge Lines
         */

        public void SetSelectedButton(string currentStyle)
        {
            switch (currentStyle)
            {
                case "5L-W-DW-Y-DY-DY-Y-DW-W":
                    {
                        FiveL_W_DW_Y_DY_DY_Y_DW.BorderBrush = Brushes.Red;
                        FiveL_DW_Y_DY_DY_Y_DW.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "5L-DW-Y-DY-DY-Y-DW":
                    {
                        FiveL_W_DW_Y_DY_DY_Y_DW.BorderBrush = Brushes.Transparent;
                        FiveL_DW_Y_DY_DY_Y_DW.BorderBrush = Brushes.Red;
                        break;
                    }
                default:
                    {
                        FiveL_W_DW_Y_DY_DY_Y_DW.BorderBrush = Brushes.Transparent;
                        FiveL_DW_Y_DY_DY_Y_DW.BorderBrush = Brushes.Transparent;
                        break;
                    }
            }
        }

        public void WidthsChanged()
        {
            var_LaneWidth_5LN.Content = utils.laneWidth.ToString();

            if (utils.drawEOP)
            {
                double leftShoulderWidth = Math.Abs(utils.leftShoulderWidth);
                var_LeftShoulderWidth_5LN.Content = leftShoulderWidth.ToString();

                double rightShoulderWidth = Math.Abs(utils.rightShoulderWidth);
                var_RightShoulderWidth_5LN.Content = rightShoulderWidth.ToString();
            }
            else
            {
                var_LeftShoulderWidth_5LN.Content = "None";
                var_RightShoulderWidth_5LN.Content = "None";
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            WidthsChanged();
            SetSelectedButton(utils.roadStyle);
            lbl_instructions5L.ToolTip = utils.currentlyActiveStyle;
        }
    }
}

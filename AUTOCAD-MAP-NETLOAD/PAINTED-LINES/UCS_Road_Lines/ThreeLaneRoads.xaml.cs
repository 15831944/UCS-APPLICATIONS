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
    /// Interaction logic for ThreeLaneRoads.xaml
    /// </summary>
    public partial class ThreeLaneRoads : UserControl
    {
        public ThreeLaneRoads()
        {
            InitializeComponent();
            WidthsChanged();
        }

        private void ThreeL_W_CLT_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "3L-W-CLT-W";
            utils.nrLanes = 3;
            SetSelectedButton("3L-W-CLT-W");
            utils.currentlyActiveStyle = "Current Style: Three Lane - Center Lane for Turns";
        }

        private void ThreeL_W_DAW_DBY_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "3L-W-DAW-DBY-W";
            utils.nrLanes = 3;
            SetSelectedButton("3L-W-DAW-DBY-W");
            utils.currentlyActiveStyle = "Current Style: Three Lane - Two Lanes Left, No Passing Right";
        }

        private void ThreeL_W_DBY_DAW_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "3L-W-DBY-DAW-W";
            utils.nrLanes = 3;
            SetSelectedButton("3L-W-DBY-DAW-W");
            utils.currentlyActiveStyle = "Current Style: Three Lane - Two Lanes Left, No Passing Right";
        }

        private void ThreeL_W_DAW_Y_DAY_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "3L-W-DAW-Y-DAY-W";
            utils.nrLanes = 3;
            SetSelectedButton("3L-W-DAW-Y-DAY-W");
            utils.currentlyActiveStyle = "Current Style: Three Lane - Two Lanes Left, Restricted Passing Right";
        }

        private void ThreeL_W_DAY_Y_DAW_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "3L-W-DAY-Y-DAW-W";
            utils.nrLanes = 3;
            SetSelectedButton("3L-W-DAY-Y-DAW-W");
            utils.currentlyActiveStyle = "Current Style: Three Lane - Restricted Passing Left, Two Lanes Right";
        }

        /* Three Lane Styles
         Button Name          roadStyle
         ThreeL_W_CLT_W       3L-W-CLT-W          Three Lane - Center Lane for Turns
         ThreeL_W_DAW_DBY_W   3L-W-DAW-DBY-W      Three Lane - Two Lanes Left, No Passing Right
         ThreeL_W_DBY_DAW_W   3L-W-DBY-DAW-W      Three Lane - No Passing Left, Two Lanes Right
         ThreeL_W_DAW_Y_DAY_W 3L-W-DAW-Y-DAY-W    Three Lane - Two Lanes Left, Restricted Passing Right
         ThreeL_W_DAY_Y_DAW_W 3L-W-DAY-Y-DAW-W    Three Lane - Restricted Passing Left, Two Lanes Right
         */

        public void SetSelectedButton(string currentStyle)
        {
            switch (currentStyle)
            {
                case "3L-W-CLT-W":
                    {                        
                        ThreeL_W_CLT_W.BorderBrush = Brushes.Red;
                        ThreeL_W_DAW_DBY_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DBY_DAW_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DAW_Y_DAY_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DAY_Y_DAW_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "3L-W-DAW-DBY-W":
                    {
                        ThreeL_W_CLT_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DAW_DBY_W.BorderBrush = Brushes.Red;
                        ThreeL_W_DBY_DAW_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DAW_Y_DAY_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DAY_Y_DAW_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "3L-W-DBY-DAW-W":
                    {
                        ThreeL_W_CLT_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DAW_DBY_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DBY_DAW_W.BorderBrush = Brushes.Red;
                        ThreeL_W_DAW_Y_DAY_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DAY_Y_DAW_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "3L-W-DAW-Y-DAY-W":
                    {
                        ThreeL_W_CLT_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DAW_DBY_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DBY_DAW_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DAW_Y_DAY_W.BorderBrush = Brushes.Red;
                        ThreeL_W_DAY_Y_DAW_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "3L-W-DAY-Y-DAW-W":
                    {
                        ThreeL_W_CLT_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DAW_DBY_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DBY_DAW_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DAW_Y_DAY_W.BorderBrush = Brushes.Transparent;
                        ThreeL_W_DAY_Y_DAW_W.BorderBrush = Brushes.Red;
                        break;
                    }
            }
        }

        public void WidthsChanged()
        {
            var_LaneWidth_3LN.Content = utils.laneWidth.ToString();

            if (utils.drawEOP)
            {
                double leftShoulderWidth = Math.Abs(utils.leftShoulderWidth);
                var_LeftShoulderWidth_3LN.Content = leftShoulderWidth.ToString();

                double rightShoulderWidth = Math.Abs(utils.rightShoulderWidth);
                var_RightShoulderWidth_3LN.Content = rightShoulderWidth.ToString();
            }
            else
            {
                var_LeftShoulderWidth_3LN.Content = "None";
                var_RightShoulderWidth_3LN.Content = "None";
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            WidthsChanged();
            SetSelectedButton(utils.roadStyle);
            lbl_instructions3L.ToolTip = utils.currentlyActiveStyle;
        }


    }
}

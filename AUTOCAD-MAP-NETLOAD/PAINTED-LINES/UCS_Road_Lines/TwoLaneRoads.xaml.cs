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
    /// Interaction logic for TwoLaneRoads.xaml
    /// </summary>
    public partial class TwoLaneRoadsUserControl : UserControl
    {
        public TwoLaneRoadsUserControl()
        {
            InitializeComponent();
            WidthsChanged();
        }

        /* Two Lane Road Info
        Button Name         roadStyle
        TwoL_No_Markings    2L-NM 
        TwoL_CL             2L-CL
        TwoL_W_CL_W         2L-W-CL-W
        TwoL_DY             2L-DY
        TwoL_W_DY_W         2L-W-DY-W
        TwoL_W_YCL_Y_W      2L-W-YCL-Y-W
        TwoL_W_Y_YCL_W      2L-W-Y-YCL-W
        */

        private void TwoL_No_Markings_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "2L-NM";
            utils.nrLanes = 2;
            SetSelectedButton("2L-NM");
            utils.currentlyActiveStyle = "Current Style: Two Lanes - No Markings";
        }

        private void TwoL_CL_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "2L-CL";
            utils.nrLanes = 2;
            SetSelectedButton("2L-CL");
            utils.currentlyActiveStyle = "Current Style: Two Lanes - Dashed White Centerline Marking";
        }

        private void TwoL_W_CL_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "2L-W-CL-W";
            utils.nrLanes = 2;
            SetSelectedButton("2L-W-CL-W");
            utils.currentlyActiveStyle = "Current Style: Two Lane - White-Dashed White-White Lane Markings";
        }

        private void TwoL_DY_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "2L-DY";
            utils.nrLanes = 2;
            SetSelectedButton("2L-DY");
            utils.currentlyActiveStyle = "Current Style: Two Lane - Double Yellow Lane Markings";
        }

        private void TwoL_W_DY_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "2L-W-DY-W";
            utils.nrLanes = 2;
            SetSelectedButton("2L-W-DY-W");
            utils.currentlyActiveStyle = "Current Style: Two Lane - White-Double Yellow-White Lane Markings";
        }

        private void TwoL_W_YCL_Y_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "2L-W-YCL-Y-W";
            utils.nrLanes = 2;
            SetSelectedButton("2L-W-YCL-Y-W");
            utils.currentlyActiveStyle = "Current Style: Two Lane - White-Yellow-Dashed Yellow-White Lane Markings";
        }

        private void TwoL_W_Y_YCL_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "2L-W-Y-YCL-W";
            utils.nrLanes = 2;
            SetSelectedButton("2L-W-Y-YCL-W");
            utils.currentlyActiveStyle = "Current Style: Two Lane - White-Dashed Yellow-Yellow-White Lane Markings";
        }

        public void SetSelectedButton(string currentStyle)
        {
            /* One Lane Road Info
            Button Name         roadStyle
            TwoL_No_Markings    2L-NM 
            TwoL_CL             2L-CL
            TwoL_W_CL_W         2L-W-CL-W
            TwoL_DY             2L-DY
            TwoL_W_DY_W         2L-W-DY-W
            TwoL_W_YCL_Y_W      2L-W-YCL-Y-W
            TwoL_W_Y_YCL_W      2L-W-Y-YCL-W
            */
            switch (currentStyle)
            {
                case "2L-NM":
                    {
                        TwoL_No_Markings.BorderBrush = Brushes.Red;
                        TwoL_CL.BorderBrush = Brushes.Transparent;
                        TwoL_W_CL_W.BorderBrush = Brushes.Transparent;
                        TwoL_DY.BorderBrush = Brushes.Transparent;
                        TwoL_W_DY_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_YCL_Y_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_Y_YCL_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "2L-CL":
                    {
                        TwoL_No_Markings.BorderBrush = Brushes.Transparent;
                        TwoL_CL.BorderBrush = Brushes.Red;
                        TwoL_W_CL_W.BorderBrush = Brushes.Transparent;
                        TwoL_DY.BorderBrush = Brushes.Transparent;
                        TwoL_W_DY_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_YCL_Y_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_Y_YCL_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "2L-W-CL-W":
                    {
                        TwoL_No_Markings.BorderBrush = Brushes.Transparent;
                        TwoL_CL.BorderBrush = Brushes.Transparent;
                        TwoL_W_CL_W.BorderBrush = Brushes.Red;
                        TwoL_DY.BorderBrush = Brushes.Transparent;
                        TwoL_W_DY_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_YCL_Y_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_Y_YCL_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "2L-DY":
                    {
                        TwoL_No_Markings.BorderBrush = Brushes.Transparent;
                        TwoL_CL.BorderBrush = Brushes.Transparent;
                        TwoL_W_CL_W.BorderBrush = Brushes.Transparent;
                        TwoL_DY.BorderBrush = Brushes.Red;
                        TwoL_W_DY_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_YCL_Y_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_Y_YCL_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "2L-W-DY-W":
                    {
                        TwoL_No_Markings.BorderBrush = Brushes.Transparent;
                        TwoL_CL.BorderBrush = Brushes.Transparent;
                        TwoL_W_CL_W.BorderBrush = Brushes.Transparent;
                        TwoL_DY.BorderBrush = Brushes.Transparent;
                        TwoL_W_DY_W.BorderBrush = Brushes.Red;
                        TwoL_W_YCL_Y_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_Y_YCL_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "2L-W-YCL-Y-W":
                    {
                        TwoL_No_Markings.BorderBrush = Brushes.Transparent;
                        TwoL_CL.BorderBrush = Brushes.Transparent;
                        TwoL_W_CL_W.BorderBrush = Brushes.Transparent;
                        TwoL_DY.BorderBrush = Brushes.Transparent;
                        TwoL_W_DY_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_YCL_Y_W.BorderBrush = Brushes.Red;
                        TwoL_W_Y_YCL_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "2L-W-Y-YCL-W":
                    {
                        TwoL_No_Markings.BorderBrush = Brushes.Transparent;
                        TwoL_CL.BorderBrush = Brushes.Transparent;
                        TwoL_W_CL_W.BorderBrush = Brushes.Transparent;
                        TwoL_DY.BorderBrush = Brushes.Transparent;
                        TwoL_W_DY_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_YCL_Y_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_Y_YCL_W.BorderBrush = Brushes.Red;
                        break;
                    }
                default:
                    {
                        TwoL_No_Markings.BorderBrush = Brushes.Transparent;
                        TwoL_CL.BorderBrush = Brushes.Transparent;
                        TwoL_W_CL_W.BorderBrush = Brushes.Transparent;
                        TwoL_DY.BorderBrush = Brushes.Transparent;
                        TwoL_W_DY_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_YCL_Y_W.BorderBrush = Brushes.Transparent;
                        TwoL_W_Y_YCL_W.BorderBrush = Brushes.Transparent;
                        break;
                    }
            }
        }

        public void WidthsChanged()
        {
            var_LaneWidth_2LN.Content = utils.laneWidth.ToString();

            if (utils.drawEOP)
            {
                double leftShoulderWidth = Math.Abs(utils.leftShoulderWidth);
                var_LeftShoulderWidth_2LN.Content = leftShoulderWidth.ToString();

                double rightShoulderWidth = Math.Abs(utils.rightShoulderWidth);
                var_RightShoulderWidth_2LN.Content = rightShoulderWidth.ToString();
            }
            else
            {
                var_LeftShoulderWidth_2LN.Content = "None";
                var_RightShoulderWidth_2LN.Content = "None";
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            WidthsChanged();
            SetSelectedButton(utils.roadStyle);
            lbl_instructions2L.ToolTip = utils.currentlyActiveStyle;
        }

    }
}

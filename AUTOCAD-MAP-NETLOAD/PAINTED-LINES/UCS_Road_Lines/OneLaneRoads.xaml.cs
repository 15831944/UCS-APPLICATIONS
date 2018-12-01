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
    /// Interaction logic for OneLaneRoad.xaml
    /// </summary>
    public partial class OneLaneRoadUserControl : UserControl
    {
        public OneLaneRoadUserControl()
        {
            InitializeComponent();
            WidthsChanged();
        }

        private void OneL_No_Markings_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "1L-NM";
            utils.nrLanes = 1;
            SetSelectedButton("1L-NM");
            utils.currentlyActiveStyle = "Current Style: One Lane - No Markings";
        }

        private void OneL_W_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "1L-W-W";
            utils.nrLanes = 1;
            SetSelectedButton("1L-W-W");
            utils.currentlyActiveStyle = "Current Style: One Lane - White-White Markings";
        }

        private void OneL_Y_W_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "1L-Y-W";
            utils.nrLanes = 1;
            SetSelectedButton("1L-Y-W");
            utils.currentlyActiveStyle = "Current Style: One Lane - Yellow-White Markings";
        }

        private void OneL_W_Y_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "1L-W-Y";
            utils.nrLanes = 1;
            SetSelectedButton("1L-W-Y");
            utils.currentlyActiveStyle = "Current Style: One Lane - White-Yellow Markings";
        }

        private void OneL_Y_Y_Click(object sender, RoutedEventArgs e)
        {
            utils.roadStyle = "1L-Y-Y";
            utils.nrLanes = 1;
            SetSelectedButton("1L-Y-Y");
            utils.currentlyActiveStyle = "Current Style: One Lane - Yellow-Yellow Markings";
        }

        public void SetSelectedButton(string currentStyle)
        {
            /* One Lane Road Info
            Button Name         roadStyle
            OneL_No_Markings    1L-NM 
            OneL_W_W            1L-W-W
            OneL_Y_W            1L-Y-W
            OneL_W_Y            1L-W-Y
            OneL_Y_Y            1l-Y-Y
            */
            switch (currentStyle)
            {
                case "1L-NM":
                    {
                        OneL_No_Markings.BorderBrush = Brushes.Red;
                        OneL_W_W.BorderBrush = Brushes.Transparent;
                        OneL_Y_W.BorderBrush = Brushes.Transparent;
                        OneL_W_Y.BorderBrush = Brushes.Transparent;
                        OneL_Y_Y.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "1L-W-W":
                    {
                        OneL_No_Markings.BorderBrush = Brushes.Transparent;
                        OneL_W_W.BorderBrush = Brushes.Red;
                        OneL_Y_W.BorderBrush = Brushes.Transparent;
                        OneL_W_Y.BorderBrush = Brushes.Transparent;
                        OneL_Y_Y.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "1L-Y-W":
                    {
                        OneL_No_Markings.BorderBrush = Brushes.Transparent;
                        OneL_W_W.BorderBrush = Brushes.Transparent;
                        OneL_Y_W.BorderBrush = Brushes.Red;
                        OneL_W_Y.BorderBrush = Brushes.Transparent;
                        OneL_Y_Y.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "1L-W-Y":
                    {
                        OneL_No_Markings.BorderBrush = Brushes.Transparent;
                        OneL_W_W.BorderBrush = Brushes.Transparent;
                        OneL_Y_W.BorderBrush = Brushes.Transparent;
                        OneL_W_Y.BorderBrush = Brushes.Red;
                        OneL_Y_Y.BorderBrush = Brushes.Transparent;
                        break;
                    }
                case "1L-Y-Y":
                    {
                        OneL_No_Markings.BorderBrush = Brushes.Transparent;
                        OneL_W_W.BorderBrush = Brushes.Transparent;
                        OneL_Y_W.BorderBrush = Brushes.Transparent;
                        OneL_W_Y.BorderBrush = Brushes.Transparent;
                        OneL_Y_Y.BorderBrush = Brushes.Red;
                        break;
                    }
                default:
                    {
                        OneL_No_Markings.BorderBrush = Brushes.Transparent;
                        OneL_W_W.BorderBrush = Brushes.Transparent;
                        OneL_Y_W.BorderBrush = Brushes.Transparent;
                        OneL_W_Y.BorderBrush = Brushes.Transparent;
                        OneL_Y_Y.BorderBrush = Brushes.Transparent;
                        break;
                    }
                }
        }

        public void WidthsChanged()
        {
            var_LaneWidth_1LN.Content = utils.laneWidth.ToString();

            if (utils.drawEOP)
            {
                double leftShoulderWidth = Math.Abs(utils.leftShoulderWidth);
                var_LeftShoulderWidth_1LN.Content = leftShoulderWidth.ToString();

                double rightShoulderWidth = Math.Abs(utils.rightShoulderWidth);
                var_RightShoulderWidth_1LN.Content = rightShoulderWidth.ToString();
            }
            else
            {
                var_LeftShoulderWidth_1LN.Content = "None";
                var_RightShoulderWidth_1LN.Content = "None";
            }
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            WidthsChanged();
            SetSelectedButton(utils.roadStyle);
            lbl_instructions1L.ToolTip = utils.currentlyActiveStyle;
            
        }
    }
}

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

namespace Elevator_Emulator
{

    class Event
    {
        public static void OnUpButtonClicked(object sender, MouseButtonEventArgs e)
        {
            ((UI.UpButton)sender).TurnOn();
            int floor = ((UI.UpButton)sender).Floor;
            Scheduling.Job.AddJob(floor, 1);
        }

        public static void OnDownButtonClicked(object sender, MouseButtonEventArgs e)
        {
            ((UI.DownButton)sender).TurnOn();
            int floor = ((UI.DownButton)sender).Floor;
            Scheduling.Job.AddJob(floor, -1);
        }

        public static void OnPanelButtonClicked(object sender, MouseButtonEventArgs e)
        {
            int line = (((UI.Panel)((Grid)((Label)sender).Parent).Parent)).Line;
            int floor = int.Parse(((Label)sender).Content.ToString());
            UI.Elevators[line].Job.AddJob(floor, 0);
        }

        internal static void OnSOSButtonClicked(object sender, MouseButtonEventArgs e)
        {
            int line = ((UI.Panel)((Label)sender).Parent).Line;
            UI.Elevators[line].OnSOSButtonClicked();
        }

        internal static void OnCloseButtonClicked(object sender, MouseButtonEventArgs e)
        {
            int line = ((UI.Panel)((Label)sender).Parent).Line;
            UI.Elevators[line].OnCloseButtonClicked();
        }

        internal static void OnOpenButtonClicked(object sender, MouseButtonEventArgs e)
        {
            int line = ((UI.Panel)((Label)sender).Parent).Line;
            UI.Elevators[line].OnOpenButtonClicked();
        }
    }
}
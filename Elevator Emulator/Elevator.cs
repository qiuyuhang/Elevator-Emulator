using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Elevator_Emulator
{
    public class JobType
    {
        public int[] Up = new int[21];
        public int[] Down = new int[21];
        public int[] Inside = new int[21];
        public int JobNum()
        {
            int jobNum = 0;
            for (int i = 1; i < 21; ++i)
            {
                if (Up[i] != 0)
                    ++jobNum;
                if (Down[i] != 0)
                    ++jobNum;
                if (Inside[i] != 0)
                    ++jobNum;
            }
            return jobNum;
        }
        public int JobNum(int floor)
        {
            int jobNum = 0;
            if (Up[floor] != 0)
                ++jobNum;
            if (Down[floor] != 0)
                ++jobNum;
            if (Inside[floor] != 0)
                ++jobNum;
            return jobNum;
        }
        public void AddJob(int floor, int direction)
        {
            if (direction == 1)
                Up[floor]++;
            if (direction == -1)
                Down[floor]++;
            if (direction == 0)
                Inside[floor]++;
        }
    }
    public class Elevator
    {
        public JobType Job = new JobType();
        public Rectangle elevator;
        public int Line;
        public int Floor = 1;
        int Timeout = 0;
        public enum ElevatorState
        {
            OpenWaiting,
            CloseWaiting,
            Moving,
            Stopping,
            SOS
        };
        private ElevatorState state = ElevatorState.CloseWaiting;
        public ElevatorState State
        {
            set
            {
                if (value == ElevatorState.OpenWaiting)
                {
                    TurnColor(50, 200, 50, 0.5);
                    if (Direction != ElevatorDirection.Up && Floor != 1)
                    {
                        for (int i = 1; i < 6; ++i)
                            UI.DownButtons[i, Floor].TurnOff();
                    }
                    if (Direction != ElevatorDirection.Down && Floor != 20)
                    {
                        for (int i = 1; i < 6; ++i)
                            UI.UpButtons[i, Floor].TurnOff();
                    }
                }
                if (value == ElevatorState.CloseWaiting)
                    TurnColor(200, 200, 0, 0.5);
                if (value == ElevatorState.Moving)
                    TurnColor(200, 50, 50, 0.5);
                if (value == ElevatorState.Stopping)
                    TurnColor(200, 100, 0, 0.5);
                if (value == ElevatorState.SOS)
                    TurnColor(255, 0, 0, 1);
                state = value;
            }
            get
            {
                return state;
            }
        }
        public enum ElevatorDirection
        {
            Up,
            Down,
            None
        };
        public ElevatorDirection Direction;

        public Elevator(int line)
        {
            State = ElevatorState.CloseWaiting;
            Direction = ElevatorDirection.None;
            elevator = new Rectangle()
            {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10 + line * 50, 10 + (20 - 1) * 40, 0, 0),
                Width = 24,
                Height = 36,
                Fill = new SolidColorBrush(Color.FromRgb(200, 200, 0)),
                Opacity = 0.5
            };
            Line = line;
            Floor = 1;
            Thread thread = new Thread(Scheduling);
            thread.Start();
        }

        void Scheduling()
        {
            while (true)
            {
                if (Job.JobNum() == 0 || (Job.JobNum() == 1 && (Job.Up[Floor] != 0 || Job.Down[Floor] != 0 || Job.Inside[Floor] != 0)))
                    Direction = ElevatorDirection.None;
                switch (State)
                {
                    case ElevatorState.CloseWaiting:
                        {
                            if (Job.JobNum() != 0 && Timeout >= 10)
                            {
                                if (Direction == ElevatorDirection.None)
                                {
                                    if (JobDirection() == 1000)
                                    {
                                        State = ElevatorState.OpenWaiting;
                                        Job.Up[Floor] = 0;
                                        continue;
                                    }
                                    if (JobDirection() == -1000)
                                    {
                                        State = ElevatorState.OpenWaiting;
                                        Job.Down[Floor] = 0;
                                        continue;
                                    }
                                    if (JobDirection() > 0)
                                        Direction = ElevatorDirection.Up;
                                    if (JobDirection() < 0)
                                        Direction = ElevatorDirection.Down;
                                }
                                if (Direction == ElevatorDirection.Up)
                                {
                                    Job.Up[Floor] = 0;
                                }
                                if (Direction == ElevatorDirection.Down)
                                {
                                    Job.Down[Floor] = 0;
                                }
                                State = ElevatorState.Moving;
                                Timeout = 0;
                            }
                            else
                            {
                                Thread.Sleep(100);
                                Timeout++;
                            }
                            break;
                        }
                    case ElevatorState.OpenWaiting:
                        {
                            if (Timeout >= 20)
                            {
                                State = ElevatorState.CloseWaiting;
                                Timeout = 0;
                            }
                            else
                            {
                                Thread.Sleep(100);
                                Timeout++;
                            }
                            break;
                        }
                    case ElevatorState.Moving:
                        {
                            if (Direction == ElevatorDirection.Up)
                            {
                                if (Floor == 20)
                                {
                                    State = ElevatorState.Stopping;
                                    Direction = ElevatorDirection.Down;
                                    continue;
                                }
                                if (Floor != 19)
                                {
                                    if (Job.JobNum(Floor + 1) != 0)
                                        State = ElevatorState.Stopping;
                                }
                                MoveUp();
                            }
                            if (Direction == ElevatorDirection.Down)
                            {
                                if (Floor == 1)
                                {
                                    State = ElevatorState.Stopping;
                                    Direction = ElevatorDirection.Up;
                                    continue;
                                }
                                if (Floor != 2)
                                {
                                    if (Job.JobNum(Floor - 1) != 0)
                                        State = ElevatorState.Stopping;
                                }
                                MoveDown();
                            }
                            break;
                        }
                    case ElevatorState.Stopping:
                        {
                            State = ElevatorState.OpenWaiting;
                            break;
                        }
                    case ElevatorState.SOS:
                        {
                            Thread.Sleep(10000);
                            break;
                        }
                }

            }
        }
        int MoveUp()
        {

            elevator.Dispatcher.Invoke(new Action(() =>
            {
                ThicknessAnimation ta = new ThicknessAnimation()
                {
                    From = GetThickness(Floor),
                    To = GetThickness(Floor + 1),
                    Duration = TimeSpan.FromSeconds(2)
                };
                elevator.BeginAnimation(TextBlock.MarginProperty, ta);
                Floor++;
            }));
            Job.Up[Floor] = 0;
            Job.Inside[Floor] = 0;
            Thread.Sleep(1000);
            UI.Displays[Line].Dispatcher.Invoke(new Action(() =>
            {
                UI.Displays[Line].Content = Floor;
            }));
            Thread.Sleep(1000);
            return Floor;
        }

        int MoveDown()
        {
            elevator.Dispatcher.Invoke(new Action(() =>
            {
                ThicknessAnimation ta = new ThicknessAnimation()
                {
                    From = GetThickness(Floor),
                    To = GetThickness(Floor - 1),
                    Duration = TimeSpan.FromSeconds(2)
                };
                elevator.BeginAnimation(TextBlock.MarginProperty, ta);
                Floor--;
            }));
            Job.Down[Floor] = 0;
            Job.Inside[Floor] = 0;
            Thread.Sleep(1000);
            UI.Displays[Line].Dispatcher.Invoke(new Action(() =>
            {
                UI.Displays[Line].Content = Floor;
            }));
            Thread.Sleep(1000);
            return Floor;
        }

        void TurnColor(Byte R, Byte G, Byte B, double Opacity)
        {
            if (elevator != null)
                elevator.Dispatcher.Invoke(new Action(() =>
                {
                    elevator.Fill = new SolidColorBrush(Color.FromRgb(R, G, B));
                    elevator.Opacity = 1;
                }));
        }

        Thickness GetThickness(int floor)
        {
            return new Thickness(10 + Line * 50, 10 + (20 - floor) * 40, 0, 0);
        }

        void ResetTimeout()
        {
            Timeout = 0;
        }


        public void OnOpenButtonClicked()
        {
            if (State == ElevatorState.OpenWaiting || State == ElevatorState.CloseWaiting)
            {
                State = ElevatorState.OpenWaiting;
                ResetTimeout();
            }
        }
        public void OnCloseButtonClicked()
        {
            if (State == ElevatorState.OpenWaiting)
            {
                State = ElevatorState.CloseWaiting;
                ResetTimeout();
            }
        }
        public void OnSOSButtonClicked()
        {
            State = ElevatorState.SOS;
        }

        int JobDirection()
        {
            if (Job.Up[Floor] != 0)
                return 1000;
            if (Job.Down[Floor] != 0)
                return -1000;
            int up = 21;
            int down = 0;
            for (int i = 1; i < Floor; ++i)
            {
                if (Job.Up[i] != 0 || Job.Down[i] != 0 || Job.Inside[i] != 0)
                    down = i;
            }
            for (int i = 20; i > Floor; --i)
            {
                if (Job.Up[i] != 0 || Job.Down[i] != 0 || Job.Inside[i] != 0)
                    up = i;
            }
            if (up != 21 && down == 0)
                return 3;
            if (up == 21 && down != 0)
                return -3;
            if (up == 21 && down == 0)
                return 0;
            if (up - Floor > Floor - down)
                return -1;
            if (up - Floor > Floor - down)
                return 1;
            if (up - Floor == Floor - down)
            {
                if (Direction == ElevatorDirection.Down)
                    return -1;
            }
            return 1;
        }
    }

}

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
    public class UI : Grid
    {
        public class Door
        {
            public int Line;
            public int Floor;
            public Rectangle door = new Rectangle();
            public Door(int line, int floor)
            {
                door.HorizontalAlignment = HorizontalAlignment.Left;
                door.VerticalAlignment = VerticalAlignment.Top;
                door.Stroke = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                door.Fill = new SolidColorBrush(Color.FromRgb(144, 144, 144));
                door.Width = 24;
                door.Height = 36;
                door.Margin = new Thickness(10 + line * 50, 10 + (20 - floor) * 40, 0, 0);


                Doors[line, floor] = this;
                Line = line;
                Floor = floor;
            }
        }

        public class UpButton : Image
        {
            public int Line;
            public int Floor;
            public UpButton(int line, int floor)
            {
                HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Top;
                Height = 9;
                Width = 16;
                Margin = new Thickness(39 + line * 50, 16 + (20 - floor) * 40, 0, 0);
                Source = new BitmapImage(new Uri(@"\res\up.png", UriKind.Relative));

                MouseLeftButtonDown += Event.OnUpButtonClicked;
                UpButtons[line, floor] = this;
                Line = line;
                Floor = floor;
            }

            public void TurnOn()
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    Source = new BitmapImage(new Uri(@"\res\up_on.png", UriKind.Relative));
                }));
            }

            public void TurnOff()
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    Source = new BitmapImage(new Uri(@"\res\up.png", UriKind.Relative));
                }));
            }
        }
        public class DownButton : Image
        {
            public int Line;
            public int Floor;
            public DownButton(int line, int floor)
            {
                HorizontalAlignment = HorizontalAlignment.Left;
                VerticalAlignment = VerticalAlignment.Top;
                Height = 9;
                Width = 16;
                Margin = new Thickness(39 + line * 50, 32 + (20 - floor) * 40, 0, 0);
                Source = new BitmapImage(new Uri(@"\res\down.png", UriKind.Relative));

                MouseLeftButtonDown += Event.OnDownButtonClicked;
                DownButtons[line, floor] = this;
                Line = line;
                Floor = floor;
            }

            public void TurnOn()
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    Source = new BitmapImage(new Uri(@"\res\down_on.png", UriKind.Relative));
                }));
            }

            public void TurnOff()
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    Source = new BitmapImage(new Uri(@"\res\down.png", UriKind.Relative));
                }));
            }
        }
        public static Door[,] Doors = new Door[6, 21];
        public static UpButton[,] UpButtons = new UpButton[6, 21];
        public static DownButton[,] DownButtons = new DownButton[6, 21];
        public static Elevator[] Elevators = new Elevator[6];
        public static Label[] Displays = new Label[6];

        public UI()
        {
            for (int j = 0; j < 20; ++j)
            {
                Label label = new Label()
                {
                    Content = 20 - j,
                    Margin = new Thickness(10, 10 + j * 40, 0, 0),
                    FontSize = 25
                };
                Children.Add(label);
            }
            for (int i = 0; i < 5; ++i)
            {
                for (int j = 0; j < 20; ++j)
                {
                    Door unit = new Door(i + 1, 20 - j);
                    Children.Add(unit.door);
                    if (j != 0)
                        Children.Add(new UpButton(i + 1, 20 - j));
                    if (j != 19)
                        Children.Add(new DownButton(i + 1, 20 - j));
                }
            }

            for (int j = 0; j < 5; ++j)
            {
                Panel panel = new Panel(j + 1)
                {
                    Margin = new Thickness(350 + 150 * j, 10, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };
                Children.Add(panel);
            }


            for (int i = 0; i < 5; ++i)
            {
                Elevators[i + 1] = new Elevator(i + 1);
                Children.Add(Elevators[i + 1].elevator);
            }
        }
        public class Panel : Grid
        {
            public int Line;
            public Panel(int line)
            {
                Line = line;
                Margin = new Thickness(0, 0, 0, 0);
                Width = 125;
                Height = 250;
                Background = new SolidColorBrush(Color.FromRgb(200, 200, 200));

                Grid floorPanel = new Grid()
                {
                    Width = 125,
                    Height = 155,
                    Margin = new Thickness(0, 60, 0, 25)
                };
                Children.Add(floorPanel);
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 4; ++j)
                    {
                        Label button = new Label()
                        {
                            Content = 4 * i + j + 1,
                            Width = 25,
                            Height = 25,
                            BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                            BorderThickness = new Thickness(1, 1, 1, 1),
                            Margin = new Thickness(5 + 30 * j, 125 - 30 * i, 95 - 30 * j, 5 + 30 * i),
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center
                        };
                        button.MouseLeftButtonDown += Event.OnPanelButtonClicked;
                        floorPanel.Children.Add(button);
                    }
                }

                Label display = new Label()
                {
                    Content = 1,
                    Height = 55,
                    Width = 115,
                    FontSize = 30,
                    Background = new SolidColorBrush(Color.FromRgb(50, 200, 50)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    Margin = new Thickness(5, 5, 5, 190),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    Name = "display" + Line
                };
                Displays[Line] = display;
                Children.Add(display);

                Label open = new Label()
                {
                    Content = "开",
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    Margin = new Thickness(5, 220, 95, 5),
                    Width = 25,
                    Height = 25,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,

                };
                Children.Add(open);
                open.MouseLeftButtonDown += Event.OnOpenButtonClicked;

                Label close = new Label()
                {
                    Content = "关",
                    Width = 25,
                    Height = 25,
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    Margin = new Thickness(35, 220, 65, 5),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                Children.Add(close);
                close.MouseLeftButtonDown += Event.OnCloseButtonClicked;

                Label sos = new Label()
                {
                    Content = "SOS",
                    Background = new SolidColorBrush(Color.FromRgb(200, 50, 50)),
                    Width = 55,
                    Height = 25,
                    BorderThickness = new Thickness(1, 1, 1, 1),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0)),
                    Margin = new Thickness(65, 220, 5, 5),
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                Children.Add(sos);
                sos.MouseLeftButtonDown += Event.OnSOSButtonClicked;
            }
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elevator_Emulator
{

    public class Scheduling
    {
        public static JobType Job = new JobType();
        public static void StartScheduling()
        {
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(100);
                    for (int i = 1; i < 21; ++i)
                    {
                        if (Job.Up[i] != 0)
                            DispatchJob(i, 1);
                        if (Job.Down[i] != 0)
                            DispatchJob(i, -1);
                        Job.Up[i] = 0;
                        Job.Down[i] = 0;
                    }
                }
            });
            thread.Start();
        }

        public static void DispatchJob(int floor, int direction)
        {
            int[] Score = new int[6];
            for (int i = 1; i < 6; ++i)
            {
                Elevator elevator = UI.Elevators[i];
                if (elevator.Job.JobNum() == 0)
                    Score[i] += 100 * Math.Abs(20 - (floor - elevator.Floor));
                else
                {
                    if (floor >= elevator.Floor && direction == 1 && elevator.Direction == Elevator.ElevatorDirection.Up)
                        Score[i] += 50 * Math.Abs(20 - (floor - elevator.Floor));
                    if (floor <= elevator.Floor && direction == -1 && elevator.Direction == Elevator.ElevatorDirection.Down)
                        Score[i] += 50 * Math.Abs(20 - (floor - elevator.Floor));
                    if (floor >= elevator.Floor && direction == -1 && elevator.Direction == Elevator.ElevatorDirection.Up)
                        Score[i] -= 50 * Math.Abs(20 - (floor - elevator.Floor));
                    if (floor <= elevator.Floor && direction == 1 && elevator.Direction == Elevator.ElevatorDirection.Down)
                        Score[i] -= 50 * Math.Abs(20 - (floor - elevator.Floor));
                    if (floor >= elevator.Floor && direction == 1 && elevator.Direction == Elevator.ElevatorDirection.Down)
                        Score[i] -= 50 * Math.Abs(20 - (floor - elevator.Floor));
                    if (floor <= elevator.Floor && direction == -1 && elevator.Direction == Elevator.ElevatorDirection.Up)
                        Score[i] -= 50 * Math.Abs(20 - (floor - elevator.Floor));
                }
            }
            int min = Score[1];
            for (int i = 2; i < 6; ++i)
                if (min > Score[i])
                    min = Score[i];
            if (min == 0)
                return;
            int line = 1;
            for (int i = 2; i < 6; ++i)
                if (Score[line] < Score[i])
                    line = i;
            UI.Elevators[line].Job.AddJob(floor, direction);
        }
    }
}

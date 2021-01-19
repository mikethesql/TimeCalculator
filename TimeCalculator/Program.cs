using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime newDay;
            double newMinutes;
            DateTime answer;
            //get the start date and time
            Console.WriteLine("Start Date and Time: ");
            string startDate = Console.ReadLine();
            Console.WriteLine("");
            //get the number of minutes for this task
            Console.WriteLine("Minutes: ");
            string minutes = Console.ReadLine();
            //Add some extra spacing for readability
            Console.WriteLine("");
            Console.WriteLine("");

            //parse out the date and time to make sure it's actually a datetime value
            if (DateTime.TryParse(startDate, out newDay))
            {
                //convert the minutes string to double
                newMinutes = Convert.ToDouble(minutes);
                //Format the entry for readability
                Console.WriteLine("Start = " + newDay.ToString("F", CultureInfo.CreateSpecificCulture("en-US")));
                Console.WriteLine("Minutes = " + newMinutes);
                //Call getEndDate to calculate the ending datetime value
                answer = getEndDate(newDay, newMinutes);
                //Display the value returned
                Console.WriteLine("Answer = " + answer.ToString("F", CultureInfo.CreateSpecificCulture("en-US")));
            }
            else
            {
                //Display an error if the date and time was not entered correctly
                Console.WriteLine("Invalid DateTime entered");
            }

            Console.Read();
        }

        static DateTime getEndDate(DateTime startDate, double minToWork)
        {
            DateTime Answer = new DateTime();
            DateTime currBusinessDay = startDate;
            double remainingMinutes = minToWork;
            TimeSpan amStart = new TimeSpan(08, 0, 0); // 8 am

            //check if start date is a weekday during business hours and not during break
            //if during business hours, start calculating time (240 min in 4 hours, 480 per work day)
            //if not during business hours, determine the next valid time period

            bool x = false;
            while (x == false)
            {
                //calc time beginning with the start date before entering the loop
                if (isWeekday(currBusinessDay) && !isHoliday(currBusinessDay))
                {
                    //If the time is before business hours, set it to the start of business
                    if(currBusinessDay.TimeOfDay < amStart)
                    {
                        currBusinessDay = new DateTime(currBusinessDay.Year,currBusinessDay.Month, currBusinessDay.Day, 8,0,0);
                    }

                    //it's a work day during business hours
                    if (isBusinessHoursAm(currBusinessDay))
                    {
                        //figure out a way to calc min without exceeding time limit for period
                        // calc time remaining in period and determine if time exceeds
                        DateTime endOfPeriod = new DateTime(currBusinessDay.Year, currBusinessDay.Month, currBusinessDay.Day, 12, 0, 0);
                        TimeSpan ts = endOfPeriod - currBusinessDay;
                        // minutes are less than or equal to remaining period
                        if (ts.TotalMinutes >= remainingMinutes)
                        {
                            Answer = currBusinessDay.AddMinutes(remainingMinutes);
                            x = true;
                        }
                        else // minutes are greater than remaining period
                        {
                            //calc remaining minutes
                            remainingMinutes = remainingMinutes - ts.TotalMinutes;

                            //
                            DateTime endOfPeriod2 = new DateTime(currBusinessDay.Year, currBusinessDay.Month, currBusinessDay.Day, 17, 0, 0);
                            DateTime startOfPeriod2 = new DateTime(currBusinessDay.Year, currBusinessDay.Month, currBusinessDay.Day, 13, 0, 0);
                            TimeSpan ts2 = endOfPeriod2 - startOfPeriod2;
                            if (ts2.TotalMinutes >= remainingMinutes)
                            {
                                Answer = startOfPeriod2.AddMinutes(remainingMinutes);
                                x = true;
                            }
                            else
                            {
                                remainingMinutes = remainingMinutes - ts2.TotalMinutes;
                                //calculate the next business day
                                currBusinessDay = currBusinessDay.AddDays(1).Date;
                            }

                        }
                    }
                    else if (isBusinessHoursPm(currBusinessDay))
                    {
                        DateTime endOfPeriod = new DateTime(currBusinessDay.Year, currBusinessDay.Month, currBusinessDay.Day, 17, 0, 0);
                        TimeSpan ts = endOfPeriod - currBusinessDay;
                        // minutes are less than or equal to remaining period
                        if (ts.TotalMinutes >= remainingMinutes)
                        {
                            Answer = currBusinessDay.AddMinutes(remainingMinutes);
                            x = true;
                        }
                        else // minutes are greater than remaining period
                        {
                            remainingMinutes = remainingMinutes - ts.TotalMinutes;
                            //calculate the next business day
                            currBusinessDay = currBusinessDay.AddDays(1).Date;
                        }
                    }
                    else
                    {
                        //calculate the next business day
                        currBusinessDay = currBusinessDay.AddDays(1).Date;
                    }
                }
                else
                {
                    //calculate the next business day
                    currBusinessDay = currBusinessDay.AddDays(1).Date;
                }
            }


            return Answer;
        }

        static bool isBusinessHoursAm(DateTime currDate)
        {
            //determine if time of currDate is within working hours
            TimeSpan amStart = new TimeSpan(08,0,0); // 8 am
            TimeSpan amEnd = new TimeSpan(12,0,0); // 12 pm
            TimeSpan currTime = currDate.TimeOfDay;

            if((currTime >= amStart) && (currTime <= amEnd))
            {
                return true;
            }
            
            return false;
        }

        static bool isBusinessHoursPm(DateTime currDate)
        {
            //determine if time of currDate is within working hours
            TimeSpan pmStart = new TimeSpan(13, 0, 0); // 1 pm
            TimeSpan pmEnd = new TimeSpan(17, 0, 0); // 5 pm
            TimeSpan currTime = currDate.TimeOfDay;

            if ((currTime >= pmStart) && (currTime <= pmEnd))
            {
                return true;
            }

            return false;
        }

        static bool isWeekday(DateTime selectedDate)
        {
            bool weekday = true;
            DayOfWeek day = selectedDate.DayOfWeek;

            if((day == DayOfWeek.Saturday) || (day == DayOfWeek.Sunday))
            {
                weekday = false;
            }
            return weekday;
        }

        //used for determining which week of the month certain holidays occur.
        public enum monthWeeks
        {
            FirstWeek = 1,
            SecondWeek = 2,
            ThirdWeek = 3,
            FourthWeek = 4,
            LastWeek = 5
        }

        static bool isHoliday(DateTime selectedDate)
        {
            //Determine if the date being passed is a holiday

            //New Year's Day
            if (selectedDate.Month == 1 && selectedDate.Day == 1) return true;

            //Memorial Day (Last Monday in May)
            if (selectedDate.Date == GetNthDayOfNthWeek(new DateTime(selectedDate.Year, 5, 1), (int)DayOfWeek.Monday, (int)monthWeeks.LastWeek).Date) return true;

            //Independence Day
            if (selectedDate.Month == 7 && selectedDate.Day == 4) return true;

            //Labor Day (First Monday in Sept.)
            if (selectedDate.Date == GetNthDayOfNthWeek(new DateTime(selectedDate.Year, 9, 1), (int)DayOfWeek.Monday, (int)monthWeeks.FirstWeek).Date) return true;

            //Thanksgiving (4th Thursday in November)
            if (selectedDate.Date == GetNthDayOfNthWeek(new DateTime(selectedDate.Year, 11, 1), (int)DayOfWeek.Thursday, (int)monthWeeks.FourthWeek).Date) return true;

            //Christmas
            if (selectedDate.Month == 12 && selectedDate.Day == 25) return true;


            return false;
        }

        static DateTime GetNthDayOfNthWeek(DateTime dt, int DayofWeek, int WhichWeek)
        {
            // specify which day of which week of a month and this function will get the date
            // this function uses the month and year of the date provided

            // get first day of the given date
            var dtFirst = new DateTime(dt.Year, dt.Month, 1);

            // get first DayOfWeek of the month
            var dtRet = dtFirst.AddDays(6 - (int)dtFirst.AddDays(-(DayofWeek + 1)).DayOfWeek);

            // get which week
            dtRet = dtRet.AddDays((WhichWeek - 1) * 7);

            // if day is past end of month then adjust backwards a week
            if (dtRet >= dtFirst.AddMonths(1))
            {
                dtRet = dtRet.AddDays(-7);
            }

            // return
            return dtRet;
        }

    }
}

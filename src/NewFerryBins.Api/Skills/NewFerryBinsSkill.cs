using AlexaSkillsKit.Speechlet;
using AlexaSkillsKit.UI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace NewFerryBins.Api.Skills
{
    public class NewFerryBinsSkill : SpeechletBase, ISpeechletWithContextAsync
    {
        public Task<SpeechletResponse> OnIntentAsync(IntentRequest intentRequest, Session session, Context context)
        {
            SpeechletResponse response = null;

            if (intentRequest.Intent == null)
            {
                response = new SpeechletResponse()
                {
                    OutputSpeech = new PlainTextOutputSpeech() { Text = "Sorry, I could not recognise that." },
                    ShouldEndSession = true
                };
            }

            int weekNumber = this.GetWeekNumber();
            BinWeek binWeek = null;
            switch (intentRequest.Intent.Name)
            {
                case "ThisWeeksBins":
                    binWeek = this.GetBinWeek(weekNumber);
                    response = new SpeechletResponse()
                    {
                        OutputSpeech = new PlainTextOutputSpeech() { Text = $"This weeks bins are on {binWeek.Day} and are {binWeek.Type}, the {binWeek.Colour} ones." },
                        ShouldEndSession = true
                    };
                    break;

                case "NextWeeksBins":
                    binWeek = this.GetBinWeek(weekNumber == 52 ? 1 : weekNumber + 1);
                    response = new SpeechletResponse()
                    {
                        OutputSpeech = new PlainTextOutputSpeech() { Text = $"Next weeks bins are on {binWeek.Day} and are {binWeek.Type}, the {binWeek.Colour} ones." },
                        ShouldEndSession = true
                    };
                    break;

                case "LastWeeksBins":
                    binWeek = this.GetBinWeek(weekNumber == 1 ? 52 : weekNumber - 1);
                    response = new SpeechletResponse()
                    {
                        OutputSpeech = new PlainTextOutputSpeech() { Text = $"Last weeks bins were on {binWeek.Day} and were {binWeek.Type}, the {binWeek.Colour} ones." },
                        ShouldEndSession = true
                    };
                    break;

                default:
                    response = new SpeechletResponse()
                    {
                        OutputSpeech = new PlainTextOutputSpeech() { Text = "Sorry, I could not recognise that." },
                        ShouldEndSession = true
                    };
                    break;
            }

            return Task.FromResult(response);
        }

        public Task<SpeechletResponse> OnLaunchAsync(LaunchRequest launchRequest, Session session, Context context)
        {
            var response = new SpeechletResponse()
            {
                OutputSpeech = new PlainTextOutputSpeech() { Text = "Would you like to know this weeks, last weeks or next weeks?" }
            };

            return Task.FromResult(response);
        }

        public Task OnSessionEndedAsync(SessionEndedRequest sessionEndedRequest, Session session, Context context)
        {
            return Task.CompletedTask;
        }

        public Task OnSessionStartedAsync(SessionStartedRequest sessionStartedRequest, Session session, Context context)
        {
            return Task.CompletedTask;
        }

        private BinWeek GetBinWeek(int week)
        {
            using (StreamReader r = new StreamReader(HostingEnvironment.MapPath(@"~/App_Data/bin-data.json")))
            {
                string json = r.ReadToEnd();
                List<BinWeek> weeks = JsonConvert.DeserializeObject<List<BinWeek>>(json);
                return weeks.Find(bw => bw.Week == week);
            }
        }

        public int GetWeekNumber()
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }

        private class BinWeek
        {
            public int Week { get; set; }

            public string Colour { get; set; }

            public string Type { get; set; }

            public string Day { get; set; }
        }
    }
}
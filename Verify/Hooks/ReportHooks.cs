﻿using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using TechTalk.SpecFlow;

namespace Verify.Hooks
{
    [Binding]
    public sealed class ReportHooks
    {

        private static ExtentTest featureName;
        private static ExtentTest scenario;
        private static ExtentReports extent;

        private ScenarioContext _scenarioContext;
        public static String testStepText;

        [BeforeTestRun]
        public static void ReportIni()
        {
            String workingDirectory = Environment.CurrentDirectory;

            String path = Directory.GetParent(workingDirectory).Parent.Parent.FullName + @"\Reports\ExtentReport.html";

            var htmlReporter = new ExtentHtmlReporter(path);

            extent =  new ExtentReports();
            extent.AttachReporter(htmlReporter);
        }

        [AfterTestRun]
        public static void TearDownReport()
        {
            extent.Flush();
        }


        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            featureName = extent.CreateTest<Feature>(featureContext.FeatureInfo.Title);
        }

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext scenarioContext)
        {
            scenario = featureName.CreateNode<Scenario>(scenarioContext.ScenarioInfo.Title);
        }

        [AfterStep]
        public void InsertreportingSteps(ScenarioContext scenarioContext)
        {
            String stepType = ScenarioStepContext.Current.StepInfo.StepDefinitionType.ToString();

            _scenarioContext = scenarioContext;

            if (_scenarioContext.TestError == null)
            {
                if (stepType.Equals("Given"))
                    scenario.CreateNode<Given>("<b>"+ScenarioStepContext.Current.StepInfo.Text+"</b>").Info(testStepText);
                else if (stepType.Equals("When"))
                    scenario.CreateNode<When>("<b>" + ScenarioStepContext.Current.StepInfo.Text + "</b>").Info(testStepText);
                else if (stepType.Equals("Then"))
                    scenario.CreateNode<Then>("<b>" + ScenarioStepContext.Current.StepInfo.Text + "</b>").Info(testStepText);
                else
                    scenario.CreateNode<And>("<b>" + ScenarioStepContext.Current.StepInfo.Text + "</b>").Info(testStepText);
            }
            else
            {
                String errorMessage = _scenarioContext.TestError.Message;
                if (stepType.Equals("Given"))
                    scenario.CreateNode<Given>(ScenarioStepContext.Current.StepInfo.Text).Fail("<i>" + errorMessage + "</i>");
                else if (stepType.Equals("When"))
                    scenario.CreateNode<When>(ScenarioStepContext.Current.StepInfo.Text).Fail("<i>" + errorMessage + "</i>");
                else if (stepType.Equals("Then"))
                    scenario.CreateNode<Then>(ScenarioStepContext.Current.StepInfo.Text).Fail("<i>" + errorMessage + "</i>");
                else
                    scenario.CreateNode<And>(ScenarioStepContext.Current.StepInfo.Text).Fail("<i>" + errorMessage + "</i>");
            }

            testStepText = "";



        }

        public static void defineTestText(String log)
        {
            testStepText += "&nbsp;&nbsp;<i>->" + log+ "</i><br>";
        }

    }
}
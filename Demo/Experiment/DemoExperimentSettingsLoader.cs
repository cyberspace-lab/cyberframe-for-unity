using cyberframe.Experiment;
using cyberframe.FileHandeling;

namespace cyberframe.Demo
{
    public class DemoExperimentSettingsLoader : ExperimentSettingsLoader
    {
        public override ExperimentSettings LoadSettings(string experimentName, string content)
        {
            return ParseSettings<DemoExperimentSettings>(content);
        }
    }
}

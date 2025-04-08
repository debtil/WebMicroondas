namespace Business.Models
{
    public abstract class HeatingProgram
    {
        public string Name { get; set; }
        public string Food { get; set; }
        public int TimeSeconds { get; set; }
        public int Power { get; set; }
        public char HeatingChar { get; set; }
        public string Instructions { get; set; }
    }

    public class PredefinedProgram : HeatingProgram
    {
        public bool IsPredefined => true;
    }

    public class CustomProgram : HeatingProgram
    {
        public bool IsPredefined => false;
    }
}


namespace WebMicroondas.Domain.Entities
{
    public class MicrowaveProgram
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Food { get; set; }
        public int TimeSeconds { get; set; }
        public int Power { get; set; } // 1..10
        public char HeatingChar { get; set; }
        public string Instructions { get; set; }
        public bool IsPredefined { get; set; }
    }
}

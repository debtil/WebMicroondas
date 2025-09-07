using System.Text.Json;
using WebMicroondas.Domain.Entities;
using WebMicroondas.Domain.Interfaces;

namespace WebMicroondas.Infra.Persistence
{
    public class ProgramJsonRepository : IProgramRepository
    {
        private readonly string _path;
        private readonly object _gate = new();

        public ProgramJsonRepository(IWebHostEnvironment env)
        {
            _path = Path.Combine(env.ContentRootPath, "App_Data", "programs.json");
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            if (!File.Exists(_path)) File.WriteAllText(_path, "[]");
        }

        public IEnumerable<MicrowaveProgram> LoadCustomPrograms()
        {
            lock (_gate)
            {
                var json = File.ReadAllText(_path);
                return JsonSerializer.Deserialize<List<MicrowaveProgram>>(json) ?? new List<MicrowaveProgram>();
            }
        }

        public MicrowaveProgram Add(MicrowaveProgram program)
        {
            lock (_gate)
            {
                var list = LoadCustomPrograms().ToList();
                list.Add(program);
                File.WriteAllText(_path, JsonSerializer.Serialize(list));
                return program;
            }
        }

        public void Delete(int id)
        {
            lock (_gate)
            {
                var list = LoadCustomPrograms().Where(p => p.Id != id).ToList();
                File.WriteAllText(_path, JsonSerializer.Serialize(list));
            }
        }

        public bool AnyByHeatingChar(char c)
        {
            lock (_gate)
            {
                return LoadCustomPrograms().Any(p => p.HeatingChar == c);
            }
        }

        public int GetNextId()
        {
            lock (_gate)
            {
                var custom = LoadCustomPrograms();
                var maxCustom = custom.Any() ? custom.Max(p => p.Id) : 1000; // custom ids a partir de 1000
                return maxCustom + 1;
            }
        }
    }
}

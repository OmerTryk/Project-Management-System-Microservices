using System.Reflection;
using UserApi.Models.ViewModels;
using ProjectApi.Models.ViewModels;
using TaskApi.Models.ViewModels;
using Xunit;
using Xunit.Abstractions;

namespace PMS.Tests
{
    public class ModelDiscoveryTests
    {
        private readonly ITestOutputHelper _output;

        public ModelDiscoveryTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DiscoverUserModels()
        {
            _output.WriteLine("=== USER MODELLERİ ===");
            InspectAssembly(typeof(UserApi.Models.User).Assembly);
            Assert.True(true, "Model keşfi tamamlandı");
        }
        
        [Fact]
        public void DiscoverTaskModels()
        {
            _output.WriteLine("=== TASK MODELLERİ ===");
            InspectAssembly(typeof(TaskApi.Models.Task).Assembly);
            Assert.True(true, "Model keşfi tamamlandı");
        }
        
        [Fact]
        public void DiscoverProjectModels()
        {
            _output.WriteLine("=== PROJECT MODELLERİ ===");
            InspectAssembly(typeof(ProjectApi.Models.Project).Assembly);
            Assert.True(true, "Model keşfi tamamlandı");
        }
        
        private void InspectAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.Contains("ViewModels"))
                .OrderBy(t => t.Name)
                .ToList();
            
            _output.WriteLine($"Toplam model sayısı: {types.Count}");
            
            foreach (var type in types)
            {
                _output.WriteLine($"\nModel: {type.Name}");
                
                var properties = type.GetProperties();
                _output.WriteLine($"Özellikler ({properties.Length}):");
                
                foreach (var prop in properties)
                {
                    _output.WriteLine($" - {prop.Name}: {prop.PropertyType.Name}");
                }
            }
        }
    }
}
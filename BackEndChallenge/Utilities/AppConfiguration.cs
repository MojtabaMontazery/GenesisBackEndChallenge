using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BackEndChallenge.Utilities
{
    public class AppConfiguration
    {
        public string JWTSecret { get; set; }

        public AppConfiguration()
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();

            var appSetting = root.GetSection("AppSettings");
            JWTSecret = appSetting["JWTSecretKey"];
        }
    }
}

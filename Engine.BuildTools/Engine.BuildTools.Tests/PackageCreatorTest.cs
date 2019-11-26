using System;
using System.IO;
using Engine.BuildTools.PackageCreator;
using Engine.BuildTools.PackageCreator.Versions;
using Xunit;
using Assert = Xunit.Assert;

namespace Engine.BuildTools.Tests
{
    public class PackageCreatorTest
    {
        [Fact]
        public void CreateAndReadPackage()
        {
            Directory.CreateDirectory("content");
            Directory.CreateDirectory("content/assets");
            Random rnd = new Random();
            byte[] buffer = new byte[1024 * 1024];
            for (int i = 0; i < 100; i++)
            {
                rnd.NextBytes(buffer);
                File.WriteAllBytes("content/assets/" + i + ".blob", buffer);
            }

            File.WriteAllBytes("content/TestPackage.dll", buffer);
            File.WriteAllBytes("content/TestPackage.runtimeconfig.json", buffer);
            File.WriteAllBytes("content/TestPackage.deps.json", buffer);
            Directory.CreateDirectory("output");


            //Creator.CreateGamePackage("TestPackage", "NoExecutable", "");
            Builder.Builder.RunCommand("--create-package ./content TestPackage ./output/TestPackage.game False False --packager-version legacy --packer-override-engine-version 9.9.9.9");
            Builder.Builder.RunCommand("--create-package ./content TestPackage ./output/v1Test.game False False --packager-version v1 --packer-override-engine-version 9.9.9.9");
            Builder.Builder.RunCommand("--create-package ./content TestPackage ./output/v2Test.game False False --packager-version v2 --packer-override-engine-version 9.9.9.9");


            IPackageManifest manifestL = Creator.ReadManifest("./output/TestPackage.game");
            IPackageManifest manifestV1 = Creator.ReadManifest("./output/v1Test.game");
            IPackageManifest manifestV2 = Creator.ReadManifest("./output/v2Test.game");

            Assert.True(manifestL.PackageVersion == "legacy");
            Assert.True(manifestV1.PackageVersion == "v1");
            Assert.True(manifestV2.PackageVersion == "v2");
            Assert.True(manifestV2.Version == manifestL.Version && manifestV2.Version == manifestV1.Version);
            Assert.True(manifestV2.Title == manifestL.Title && manifestV2.Title == manifestV1.Title);
            Assert.True(manifestV2.StartCommand == manifestL.StartCommand && manifestV2.StartCommand == manifestV1.StartCommand);

            Directory.Delete("content", true);
            Directory.Delete("output", true);


        }
    }
}

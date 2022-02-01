using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vit.Ioc.MsTest
{
    [TestClass]
    public class DisposableTest
    {
        #region TestScope


        [TestMethod]
        public void TestScope()
        {
            int[] hash = new int[10];

            IocHelp.AddScoped<Disposable>();
            IocHelp.Update();
            {
                using var serviceScope = IocHelp.CreateScope();
                hash[0] = IocHelp.Create<Disposable>().GetHashCode();
                hash[1] = IocHelp.Create<Disposable>().GetHashCode();

                using var serviceScope2 = IocHelp.CreateScope();
                hash[2] = IocHelp.Create<Disposable>().GetHashCode();

                Assert.AreEqual(hash[0], hash[1]);
                Assert.AreNotEqual(hash[0], hash[2]);
            }

            {
                using var serviceScope = IocHelp.CreateScope();
                hash[3] = IocHelp.Create<Disposable>().GetHashCode();
                hash[4] = IocHelp.Create<Disposable>().GetHashCode();

                using var serviceScope2 = IocHelp.CreateScope();
                hash[5] = IocHelp.Create<Disposable>().GetHashCode();

                Assert.AreEqual(hash[3], hash[4]);
                Assert.AreNotEqual(hash[3], hash[5]);

                Assert.AreNotEqual(hash[1], hash[3]);
                Assert.AreNotEqual(hash[1], hash[5]);

                Assert.AreNotEqual(hash[2], hash[3]);
                Assert.AreNotEqual(hash[2], hash[5]);

            }
 
            

        }
        #endregion





        #region class for test
        public class Disposable : IDisposable
        {
            public Disposable()
            {
                Console.WriteLine("structor:" + GetHashCode());
            }
            ~Disposable()
            {
                Console.WriteLine("destructor:" + GetHashCode());
            }
            public void Dispose()
            {
                Console.WriteLine("Dispose:" + GetHashCode());
            }
        }

        #endregion

    }
}

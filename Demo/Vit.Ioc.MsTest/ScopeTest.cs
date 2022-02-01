using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vit.Ioc.MsTest
{
    [TestClass]
    public class ScopeTest
    {
        #region TestScope


        [TestMethod]
        public void TestScope()
        {
            int[] hash = new int[10];

            IocHelp.AddScoped<Disposable>();
            IocHelp.Update();
            {
                using var scope = new Scope();
                hash[0] = scope.Create<Disposable>().GetHashCode();
                hash[1] = scope.Create<Disposable>().GetHashCode();

                using var scope2 = scope.CreateScope();
                hash[2] = scope2.Create<Disposable>().GetHashCode();

                Assert.AreEqual(hash[0], hash[1]);
                Assert.AreNotEqual(hash[0], hash[2]);
            }

            {
                using var scope = new Scope();
                hash[0] = scope.Create<Disposable>().GetHashCode();

                using var scope5 = new Scope();
                hash[5] = scope5.Create<Disposable>().GetHashCode();


                using var scope1 = scope.CreateScope();
                hash[1] = scope1.Create<Disposable>().GetHashCode();

                using var scope6 = new Scope();
                hash[6] = scope6.Create<Disposable>().GetHashCode();

                Assert.AreEqual(hash[0], scope.Create<Disposable>().GetHashCode());
                Assert.AreEqual(hash[5], scope5.Create<Disposable>().GetHashCode());
                Assert.AreEqual(hash[1], scope1.Create<Disposable>().GetHashCode());
                Assert.AreEqual(hash[6], scope6.Create<Disposable>().GetHashCode());
                Assert.AreNotEqual(hash[0], hash[1]);

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

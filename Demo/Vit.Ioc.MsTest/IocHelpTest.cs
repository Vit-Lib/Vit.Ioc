using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Vit.Ioc.MsTest
{
    [TestClass]
    public class IocHelpTest
    {
     



        #region TestSingletonAndTransient


        [TestMethod]
        public void TestSingletonAndTransient()
        {            
            //(x.1)Singleton
            IocHelp.AddSingleton<InterfaceA, ClassA>();
            IocHelp.Update();
            var obj0_1 = IocHelp.Create<InterfaceA>();
            var obj0_1_HashCode = obj0_1.GetHashCode();


            var obj0_2 = IocHelp.Create<InterfaceA>();
            var obj0_2_HashCode = obj0_2.GetHashCode();

            Assert.AreEqual(obj0_1_HashCode, obj0_2_HashCode);



            //(x.2)Transient
            IocHelp.AddTransient<InterfaceB, ClassA>();
            IocHelp.Update();

            var obj1_1 = IocHelp.Create<InterfaceB>();
            var obj1_1_HashCode = obj1_1.GetHashCode();


            var obj1_2 = IocHelp.Create<InterfaceB>();
            var obj1_2_HashCode = obj1_2.GetHashCode();

            Assert.AreNotEqual(obj1_1_HashCode, obj1_2_HashCode);


        }
        #endregion




        #region TestScope


        [TestMethod]
        public void TestScope()
        {
            //IocHelp.AddSingleton<InterfaceA, ClassA>();
            IocHelp.AddTransient<InterfaceB, ClassA>();
            //var ia1 = IocHelp.Create<InterfaceA>();

           

            IocHelp.AddScoped<InterfaceC, ClassA>();

            IocHelp.Update();


            var obj0_1 = IocHelp.Create<InterfaceC>();
            var obj0_1_HashCode = obj0_1.GetHashCode();


            var obj0_2 = IocHelp.Create<InterfaceC>();
            var obj0_2_HashCode = obj0_2.GetHashCode();

            Assert.AreEqual(obj0_1_HashCode, obj0_2_HashCode);



            using (var scope1 = IocHelp.CreateScope())
            {
                var obj1_1 = IocHelp.Create<InterfaceC>();
                var obj1_1_HashCode = obj1_1.GetHashCode();


                var obj1_2 = IocHelp.Create<InterfaceC>();
                var obj1_2_HashCode = obj1_2.GetHashCode();

                Assert.AreEqual(obj1_1_HashCode, obj1_2_HashCode);


                var objB = IocHelp.Create<InterfaceB>();
                var objB_HashCode = objB.GetHashCode();

                Assert.AreNotEqual(obj1_2_HashCode, objB_HashCode);


                using (var scope2 = IocHelp.CreateScope())
                {

                    var obj2_1 = IocHelp.Create<InterfaceC>();
                    var obj2_1_HashCode = obj2_1.GetHashCode();


                    var obj2_2 = IocHelp.Create<InterfaceC>();
                    var obj2_2_HashCode = obj2_2.GetHashCode();

                    Assert.AreEqual(obj2_1_HashCode, obj2_2_HashCode);


                    Assert.AreNotEqual(obj1_2_HashCode, obj2_1_HashCode);

                }

                var obj1_3 = IocHelp.Create<InterfaceC>();
                var obj1_3_HashCode = obj1_3.GetHashCode();

                Assert.AreEqual(obj1_1_HashCode, obj1_3_HashCode);
            }

            var obj0_3 = IocHelp.Create<InterfaceC>();
            var obj0_3_HashCode = obj0_3.GetHashCode();

            Assert.AreEqual(obj0_1_HashCode, obj0_3_HashCode);
        }
        #endregion


        #region TestAsyncLocalTask


        [TestMethod]
        public void TestAsyncLocalTask()
        {
            IocHelp.AddScoped<InterfaceC, ClassA>();

            IocHelp.Update();

            TestAsyncLocalTask_Event = new AutoResetEvent(false);


            builder.Clear();
            // ����һ��ί��ʵ��
            Action act = async () =>
            {
                await RunAsync();
            };

            using (var scope = IocHelp.CreateScope())
            { 
              
                act();

                TestAsyncLocalTask_Event.WaitOne();
            }         


            string console = builder.ToString();

        }
        static AutoResetEvent TestAsyncLocalTask_Event;

        static async Task RunAsync()
        {
            // �����ǰ�̵߳�ID
            builder.AppendLine($"�첽�ȴ�ǰ����ǰ�߳�ID��{Thread.CurrentThread.ManagedThreadId}");

            var obj0_1 = IocHelp.Create<InterfaceC>();
            var obj0_1_HashCode = obj0_1.GetHashCode();

            // ��ʼִ���첽���������ȴ����
            await Task.Delay(50);
            // �첽�ȴ���ɺ��ٴ������ǰ�̵߳�ID
            builder.AppendLine($"�첽�ȴ��󣬵�ǰ�߳�ID��{Thread.CurrentThread.ManagedThreadId}");


            var obj0_2 = IocHelp.Create<InterfaceC>();
            var obj0_2_HashCode = obj0_2.GetHashCode();

            Assert.AreEqual(obj0_1_HashCode, obj0_2_HashCode);

            TestAsyncLocalTask_Event.Set();

        }
        #endregion


        #region class for test
        interface InterfaceA
        {
            void MethodA();
        }

        interface InterfaceB
        {
            void MethodB();
        }

        interface InterfaceC
        {
            void MethodC();
        }

        static StringBuilder builder = new StringBuilder();

        class ClassA : IDisposable, InterfaceA, InterfaceB, InterfaceC
        {


            public ClassA()
            {
                builder.Append("[ClassA.ClassA]");
            }
            public void Dispose()
            {
                builder.Append("[ClassA.Dispose]");
            }

            public void MethodA()
            {
                builder.Append("[ClassA.MethodA]");
            }
            public void MethodB()
            {
                builder.Append("[ClassA.MethodB]");
            }
            public void MethodC()
            {
                builder.Append("[ClassA.MethodC]");
            }
        }
        #endregion

    }
}

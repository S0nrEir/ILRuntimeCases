using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ILRuntimeDemo
{   
    public class TestClassBaseAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Int32> mget_Value_0 = new CrossBindingFunctionInfo<System.Int32>("get_Value");
        static CrossBindingMethodInfo<System.Int32> mset_Value_1 = new CrossBindingMethodInfo<System.Int32>("set_Value");
        static CrossBindingMethodInfo<System.String> mTestVirtual_2 = new CrossBindingMethodInfo<System.String>("TestVirtual");
        static CrossBindingMethodInfo<System.Int32> mTestAbstract_3 = new CrossBindingMethodInfo<System.Int32>("TestAbstract");

        //该适配器要适配的类型（针对哪一个类型的适配器）
        public override Type BaseCLRType
        {
            get
            {
                return typeof(global::TestClassBase);
            }
        }

        //该适配器的实际类型
        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        //对于所有要被热更工程继承的类型，需要写一个对应的Adapter，并继承该类型和实现CrossBindingAdaptorType接口
        //这实际上就是热更工程中要继承类型在主工程中的真实类型
        public class Adapter : global::TestClassBase, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public override void TestVirtual(System.String str)
            {
                if (mTestVirtual_2.CheckShouldInvokeBase(this.instance))
                    base.TestVirtual(str);
                else
                    mTestVirtual_2.Invoke(this.instance, str);
            }

            public override void TestAbstract(System.Int32 gg)
            {
                mTestAbstract_3.Invoke(this.instance, gg);
            }

            public override System.Int32 Value
            {
            get
            {
                if (mget_Value_0.CheckShouldInvokeBase(this.instance))
                    return base.Value;
                else
                    return mget_Value_0.Invoke(this.instance);

            }
            set
            {
                if (mset_Value_1.CheckShouldInvokeBase(this.instance))
                    base.Value = value;
                else
                    mset_Value_1.Invoke(this.instance, value);

            }
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}


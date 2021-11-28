namespace SSock.Core.Infrastructure
{
    public class Ref<T>
    {
        public T Value { get; set; }

        public Ref() { }

        public Ref(T value) 
        {
            Value = value;
        }       

        public static implicit operator T(Ref<T> r) 
        { 
            return r.Value; 
        }

        public static implicit operator Ref<T>(T value)
        {
            return new Ref<T>(value);
        }
    }
}

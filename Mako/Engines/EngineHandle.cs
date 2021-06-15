using JetBrains.Annotations;

namespace Mako.Engines
{
    [PublicAPI]
    public struct EngineHandle : ICancellable
    {
        public string Id { get; }
        
        public bool IsCanceled { get; set; }

        public EngineHandle(string id)
        {
            Id = id;
            IsCanceled = false;
        }

        public void Cancel() => IsCanceled = true;

        public static bool operator ==(EngineHandle lhs, EngineHandle rhs)
        {
            return lhs.Id == rhs.Id && lhs.IsCanceled == rhs.IsCanceled;
        }

        public static bool operator !=(EngineHandle lhs, EngineHandle rhs)
        {
            return !(lhs == rhs);
        }
    }
}
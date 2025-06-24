namespace Machinarius.Custom3dEngine.GLAbstractions;

public class EngineContainer : IDisposable
{
    private readonly Dictionary<Type, object> _services = new();
    private readonly List<IDisposable> _disposables = new();
    
    public void RegisterSingleton<T>(T instance)
    {
        if (instance == null)
            throw new ArgumentNullException(nameof(instance));
            
        _services[typeof(T)] = instance;
        
        if (instance is IDisposable disposable)
        {
            _disposables.Add(disposable);
        }
    }
    
    public T Get<T>()
    {
        if (!_services.TryGetValue(typeof(T), out var service))
        {
            throw new InvalidOperationException($"Service of type {typeof(T).Name} is not registered.");
        }
        
        return (T)service;
    }
    
    public bool TryGet<T>(out T service)
    {
        if (_services.TryGetValue(typeof(T), out var serviceObj))
        {
            service = (T)serviceObj;
            return true;
        }
        
        service = default(T)!;
        return false;
    }
    
    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable?.Dispose();
        }
        
        _disposables.Clear();
        _services.Clear();
    }
}
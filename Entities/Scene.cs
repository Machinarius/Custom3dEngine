namespace Machinarius.Custom3dEngine.Entities;

public class Scene : IDisposable {
  private readonly Camera camera;
  private readonly List<SceneObject> contents;

  public Scene(Camera camera) {
    this.camera = camera ?? throw new ArgumentNullException(nameof(camera));

    contents = new List<SceneObject>();
  }

  public int ObjectCount => contents.Count;

  public void Add(SceneObject newObject) {
    if (newObject == null)
      throw new ArgumentNullException(nameof(newObject));

    contents.Add(newObject);
  }

  public bool Remove(SceneObject objectToRemove) {
    return contents.Remove(objectToRemove);
  }

  public void Clear() {
    contents.Clear();
  }

  public void Draw(double deltaTime, double absoluteTime) {
    foreach (var sObject in contents) {
      sObject.Draw(deltaTime, absoluteTime, camera);
    }
  }

  public void Dispose() {
    foreach (var sObject in contents) {
      sObject.Dispose();
    }
  }
}
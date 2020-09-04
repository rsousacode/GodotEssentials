# Godot Unity Compatibility 

This project allows  enabling and disabling nodes virtually. This means methods such as `Process`, `LateUpdate` and `FixedUpdate`, will not run when the object is not visible, and therefore inactive.
It is mainly a fork of: [unity-godot-compat](https://github.com/NathanWarden/unity-godot-compat) with many deletions and modifications.

## Getting Started

1) Add the `EssentialsAutoLoad.cs` to a node in your global scene ([singleton](https://docs.godotengine.org/en/latest/getting_started/step_by_step/singletons_autoload.html)).
2) Create a new TestScript.cs
3) Write a simple "Hello Godot".

```csharp
using Bigmonte.Essentials;

[Extended]
public class TestScript : Node
{
    private void Start()
    {
        GD.Print("Hello Godot");
    }

    private void OnEnable()
    {
   
    }

    private void OnDisable()
    {

    }

}

```

#### Common methods ####

Some of the most commonly used methods of Unity are implemented. Such as:

- `void Awake () {} `
- `void Start () {}`
- `void Process () {} ` Previously called Update, but renamed to Process to avoid conflict with an Engine function called "Update". 
- `void LateUpdate () {} `
- `void OnEnable () {}`  requires the use of `SetActive` method
- `void OnDisable () {} ` requires the use of method `node.SetActive (bool)` or  `node.Destroy()`


#### Coroutines  ####

Example:

```csharp
[Extended]
public class Ultra : Node
{
    private void Start()
    {
        this.StartCoroutine(MyCoroutine());
    }

    private IEnumerator MyCoroutine()
    {
        yield return new WaitForSeconds(1f);
        GD.Print("1" );
        yield return new WaitForSeconds(1f);
        GD.Print("2" );
    }

}

```

#### Notes ####

* It is discouraged to change the Visible variable of Spatial or CanvasItem type nodes. Use for example `node.SetActive(false);` or `this.SetActive(false);`
*  It is discouraged to Use Free or QueueFree, use for example `this.Destroy();` or `node.Destroy();` for destroying objects.

### Prerequisites
Godot 3+

## Contributing

Feel free to point issues and help the development of this asset. 

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Acknowledgments

- Most of the intention of providing Unity portability was removed in this fork. 
- This extension relies on one singleton to work.
- It may contain experimental/unsafe code 


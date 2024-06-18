# ImmutableSharp

## C# analyzer & library for deep immutability

```cs
using ImmutableSharp;

class C
{
	[Immutable] // annotate members as immutable
	public int Property { get; }

	public int MutableProperty { get; set; }

	[Immutable] // annotate methods as immutable
	public void Method()
	{
		MutableProperty = 3; // error: cannot assign fields or properties in an immutable method
		MutableMethod(); // error: cannot call non-immutable methods in an immutable method
	}

	public void MutableMethod()
	{
		MutableProperty = 4;
	}
}
```

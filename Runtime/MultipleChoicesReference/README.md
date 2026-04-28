# MultipleChoicesReference

`MultipleChoiceReference` stores a small typed set of Unity object references that are configured by attributes on derived classes.

## Mental model

- A derived class defines slots with `MultipleChoiceReferenceFieldAttribute`.
- Each slot has:
  - a display label
  - a type key (`TypeName`)
  - an assigned `Object` reference
- At runtime, slots are rebuilt into a dictionary (`Type -> Object`) for fast lookup.

## How initialization works

1. `EnsureInitializedFromAttributes()` reads slot attributes from the derived class.
2. Existing assigned references are preserved by matching each slot `TypeName`.
3. The serialized list is rebuilt so the inspector and runtime stay in sync.

This method is called before and after serialization, and before lookups.

## Single vs multiple mode

- If the owner type has `AllowMultipleAttribute`, multiple assigned slots are allowed.
- Otherwise, the structure behaves as single-choice:
  - only the first assigned slot is kept
  - all later assigned slots are cleared

## Runtime API

- `TryGet(Type, out Object)`
  - returns the reference for the exact configured type key
- `TryGet<T>(out T)`
  - typed helper around `TryGet(Type, out Object)`
- `ReferenceMap`
  - read-only dictionary view of the rebuilt map
- `ToTypeMap()`
  - copy of the current map

## Equality behavior

Equality is order-independent.

Two instances are equal when they contain the same `(type, reference)` pairs with the same multiplicity, regardless of list order.

Implementation detail:

- entries are converted into a multiset (`Dictionary<EntryKey, count>`)
- `EntryKey` is based on `TypeName` + Unity object instance id

## Example pattern

```csharp
[Serializable]
[AllowMultiple]
[MultipleChoiceReferenceField(typeof(AudioClip), "Hit SFX")]
[MultipleChoiceReferenceField(typeof(Sprite), "Preview Icon")]
public class ExampleReference : MultipleChoiceReference
{
}
```

## Related files

- `MultipleChoiceReference.cs` - runtime structure and equality logic
- `MultipleChoiceReferenceDrawer.cs` - inspector UI
- `MultipleChoiceReferenceFieldAttribute.cs` - slot declaration attribute
- `AllowMultipleAttribute.cs` - mode attribute


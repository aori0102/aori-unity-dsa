# Matrix2

Represents a two-dimensional matrix of elements of type `T`. Provides methods for accessing and manipulating the
elements in the matrix, as well as utility methods for working with the matrix dimensions and rotations.

### Namespace

`Aori.DSA.Generic`

### Declaration

```csharp
public sealed class Matrix2<T> : UnityEngine.ISerializationCallbackReceiver, System.Collection.Generic.IEnumerable<T>
```

### Generic Parameters

| Generic Parameter | Description                            | Notes                          |
|:------------------|:---------------------------------------|:-------------------------------|
| `T`               | Type of elements stored in the matrix. | Must be serializable by Unity. |

## Table of Contents

- [Description](#description)
- [Constructors](#constructors)
- [Properties](#properties)
- [Operators](#operators)
- [Methods](#methods)
- [Explicit Interface Implementations](#explicit-interface-implementations)
- [Overrides](#overrides)

***

## Constructors

## Matrix2(int maxRow, int maxColumn)

### Declaration

```csharp
public Matrix2(int maxRow, int maxColumn)
```

### Description

Initializes a new instance of the `Matrix2<T>` class with `maxRow` rows and `maxColumn` columns.

### Parameters

| Parameter   | Type  | Description                                  |
|:------------|:------|:---------------------------------------------|
| `maxRow`    | `int` | The maximum number of rows in the matrix.    |
| `maxColumn` | `int` | The maximum number of columns in the matrix. |

### Exception

| Exception Type                       | Description                                      |
|:-------------------------------------|:-------------------------------------------------|
| `System.ArgumentOutOfRangeException` | Thrown when `maxRow` or `maxColumn` is negative. |

***

## Matrix2(Matrix2 other)

### Declaration

```csharp
public Matrix2(Matrix2<T> other)
```

### Description

Initializes a new instance of the `Matrix2<T>` class by copying the dimensions and elements from `other`.

### Parameters

| Parameter | Type         | Description              |
|:----------|:-------------|:-------------------------|
| `other`   | `Matrix2<T>` | The matrix to copy from. |

### Exception

| Exception Type                 | Description                    |
|:-------------------------------|:-------------------------------|
| `System.ArgumentNullException` | Thrown when `other` is `null`. |

***

## Properties

- [MaxColumn](#maxcolumn)
- [MaxRow](#maxrow)

## MaxColumn

### Declaration

```csharp
public int MaxColumn { get; }
```

### Description

Gets the maximum number of columns in the matrix.

***

## MaxRow

### Declaration

```csharp
public int MaxRow { get; }
```

### Description

Gets the maximum number of rows in the matrix.

***

## Operators

- [operator []](#operator-)

## operator []

### Declaration

```csharp
public T this[int row, int column] { get; set; }
```

### Description

Gets or sets the element at the specified `row` and `column` indices.

### Parameters

| Parameter | Type  | Description                  |
|:----------|:------|:-----------------------------|
| `row`     | `int` | The zero-based row index.    |
| `column`  | `int` | The zero-based column index. |

### Returns

The element at the specified `row` and `column` indices.

### Exception

| Exception Type                       | Description                                               |
|:-------------------------------------|:----------------------------------------------------------|
| `System.ArgumentOutOfRangeException` | Thrown when `row` or `column` is outside the valid range. |

***

## Methods

- [Clone](#matrix2clone)
- [Fill](#matrix2fill)
- [GetCellAfterRotatingClockwise](#matrix2getcellafterrotatingclockwise)
- [GetCellAfterRotatingCounterClockwise](#matrix2getcellafterrotatingcounterclockwise)
- [GetCellAfterRotatingOneEighty](#matrix2getcellafterrotatingoneeighty)
- [IsValid](#matrix2isvalid)
- [RotateClockwise](#matrix2rotateclockwise)
- [RotateCounterClockwise](#matrix2rotatecounterclockwise)
- [RotateOneEighty](#matrix2rotateoneeighty)

## Matrix2.Clone

### Declaration

```csharp
public Matrix2<T> Clone()
```

### Description

Creates a new `Matrix2<T>` instance that is a copy of the current instance, including its dimensions and elements.

### Returns

A new `Matrix2<T>` instance that is a copy of the current instance.

***

## Matrix2.Fill

### Declaration

```csharp
public void Fill(T value)
```

### Description

Fills all cells in the matrix with the specified `value`.

### Parameters

| Parameter | Type | Description                        |
|:----------|:-----|:-----------------------------------|
| `value`   | `T`  | The value to fill the matrix with. |

***

## Matrix2.GetCellAfterRotatingClockwise

### Declaration

```csharp
public (int row, int column) GetCellAfterRotatingClockwise(int row, int column)
``` 

### Description

Returns the row and column indices of the cell that would be in the same position as the cell at `row` and `column`
after rotating the matrix 90 degrees clockwise.

### Parameters

| Parameter | Type  | Description                  |
|:----------|:------|:-----------------------------|
| `row`     | `int` | The zero-based row index.    |
| `column`  | `int` | The zero-based column index. |  

### Returns

A tuple containing the row and column indices of the cell after rotating the matrix 90 degrees clockwise.

### Notes

This method uses a formula that works with any matrix dimension in general. The resulting tuple can be pointing to an
invalid cell of the rotated matrix.

***

## Matrix2.GetCellAfterRotatingCounterClockwise

### Declaration

```csharp
public (int row, int column) GetCellAfterRotatingCounterClockwise(int row, int column)
```

### Description

Returns the row and column indices of the cell that would be in the same position as the cell at `row` and `column`
after rotating the matrix 90 degrees counterclockwise.

### Parameters

| Parameter | Type  | Description                  |
|:----------|:------|:-----------------------------|
| `row`     | `int` | The zero-based row index.    |
| `column`  | `int` | The zero-based column index. |

### Returns

A tuple containing the row and column indices of the cell after rotating the matrix 90 degrees counterclockwise.

### Notes

This method uses a formula that works with any matrix dimension in general. The resulting tuple can be pointing to an
invalid cell of the rotated matrix.

***

## Matrix2.GetCellAfterRotatingOneEighty

### Declaration

```csharp
public (int row, int column) GetCellAfterRotatingOneEighty(int row, int column)
```

### Description

Returns the row and column indices of the cell that would be in the same position as the cell at `row` and `column`
after rotating the matrix 180 degrees.

### Parameters

| Parameter | Type  | Description                  |
|:----------|:------|:-----------------------------|
| `row`     | `int` | The zero-based row index.    |
| `column`  | `int` | The zero-based column index. |

### Returns

A tuple containing the row and column indices of the cell after rotating the matrix 180 degrees.

### Notes

This method uses a formula that works with any matrix dimension in general. The resulting tuple can be pointing to an
invalid cell of the rotated matrix.

***

## Matrix2.IsValid

### Declaration

```csharp
public bool IsValid(int row, int column)
```

### Description

Determines whether the specified `row` and `column` indices are within the valid range of the matrix dimensions.

### Parameters

| Parameter | Type  | Description                  |
|:----------|:------|:-----------------------------|
| `row`     | `int` | The zero-based row index.    |  
| `column`  | `int` | The zero-based column index. |

### Returns

`true` if the specified `row` and `column` indices are valid; otherwise, `false`.

***

## Matrix2.RotateClockwise

### Declaration

```csharp
public Matrix2<T> RotateClockwise()
```

### Description

Rotates the matrix 90 degrees clockwise and returns the resulting matrix as a new instance.

### Returns

The current instance after being rotated 90 degrees clockwise.

***

### Declaration

```csharp
public static Matrix2<T> RotateClockwise(Matrix2<T> matrix)
```

### Description

Rotates the specified `matrix` 90 degrees clockwise and returns the resulting matrix as a new instance.

### Parameters

| Parameter | Type         | Description           |
|:----------|:-------------|:----------------------|
| `matrix`  | `Matrix2<T>` | The matrix to rotate. |

### Returns

The specified `matrix` after being rotated 90 degrees clockwise.

***

## Matrix2.RotateCounterClockwise

### Declaration

```csharp
public Matrix2<T> RotateCounterClockwise()
```

### Description

Rotates the matrix 90 degrees counterclockwise and returns the resulting matrix as a new instance.

### Returns

The current instance after being rotated 90 degrees counterclockwise.

***

### Declaration

```csharp
public static Matrix2<T> RotateCounterClockwise(Matrix2<T> matrix)
```

### Description

Rotates the specified `matrix` 90 degrees counterclockwise and returns the resulting matrix as a new instance.

### Parameters

| Parameter | Type         | Description           |
|:----------|:-------------|:----------------------|
| `matrix`  | `Matrix2<T>` | The matrix to rotate. |

### Returns

The specified `matrix` after being rotated 90 degrees counterclockwise.

***

## Matrix2.RotateOneEighty

### Declaration

```csharp
public Matrix2<T> RotateOneEighty()
```

### Description

Rotates the matrix 180 degrees and returns the resulting matrix as a new instance.

### Returns

The current instance after being rotated 180 degrees.

***

### Declaration

```csharp
public static Matrix2<T> RotateOneEighty(Matrix2<T> matrix)
```

### Description

Rotates the specified `matrix` 180 degrees and returns the resulting matrix as a new instance.

### Parameters

| Parameter | Type         | Description           |
|:----------|:-------------|:----------------------|
| `matrix`  | `Matrix2<T>` | The matrix to rotate. | 

### Returns

The specified `matrix` after being rotated 180 degrees.

***

## Explicit Interface Implementations

### UnityEngine.ISerializationCallbackReceiver

| Method Name                                                                                                           | Description                              |
|:----------------------------------------------------------------------------------------------------------------------|:-----------------------------------------|
| [OnAfterDeserialize](https://docs.unity3d.com/ScriptReference/ISerializationCallbackReceiver.OnAfterDeserialize.html) | Called after the object is deserialized. |
| [OnBeforeSerialize](https://docs.unity3d.com/ScriptReference/ISerializationCallbackReceiver.OnBeforeSerialize.html)   | Called before the object is serialized.  |

### System.Collections.Generic.IEnumerable<T>

| Method Name                                                                                                                          | Description                                                 |
|:-------------------------------------------------------------------------------------------------------------------------------------|:------------------------------------------------------------|
| [GetEnumerator](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1.getenumerator?view=net-10.0)   | Returns an enumerator that iterates through the collection. |
| [IEnumerable.GetEnumerator](https://learn.microsoft.com/en-us/dotnet/api/system.collections.ienumerable.getenumerator?view=net-10.0) | Returns an enumerator that iterates through a collection.   |

***

## Overrides

### System.Object

| Method Name                                                                                   | Description                                          |
|:----------------------------------------------------------------------------------------------|:-----------------------------------------------------|
| [ToString](https://learn.microsoft.com/en-us/dotnet/api/system.object.tostring?view=net-10.0) | Returns a string that represents the current object. |
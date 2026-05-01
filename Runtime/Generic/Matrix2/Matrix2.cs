using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

namespace Aori.DSA.Generic
{
    /// <summary>
    /// A data structure representing 2D matrix.
    /// </summary>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <remarks>This data structure is serializable only if <typeparamref name="T"/> is
    /// a serializable type.</remarks>
    [Serializable]
    public sealed class Matrix2<T> :
        ISerializationCallbackReceiver,
        IEnumerable<T>
    {
        [SerializeField]
        [HideInInspector]
        private int _serialized_max_row;

        [SerializeField]
        [HideInInspector]
        private int _serialized_max_column;

        [SerializeField]
        [HideInInspector]
        private T[] _serialized_grid;

        private T[] m_grid;

        public int MaxRow { get; private set; }
        public int MaxColumn { get; private set; }
        private int Size => MaxRow * MaxColumn;

        public T this[int row, int column]
        {
            get => m_grid[IndexFrom(row, column)];
            set => m_grid[IndexFrom(row, column)] = value;
        }

        /// <summary>
        /// Get the list index of the element at <paramref name="row"/> and <paramref name="column"/>.
        /// </summary>
        /// <param name="row">The row of the element.</param>
        /// <param name="column">The column of the element.</param>
        /// <returns>The list index of the element.</returns>
        private int IndexFrom(int row, int column)
            => row * MaxColumn + column;

        /// <summary>
        /// Validate <paramref name="row"/> and <paramref name="column"/>.
        /// </summary>
        /// <param name="row">The row to validate.</param>
        /// <param name="column">The column to validate.</param>
        /// <returns><c>true</c> if <paramref name="row"/> and <paramref name="column"/> is within this
        /// matrix, otherwise <c>false</c>.</returns>
        public bool IsValid(int row, int column)
            => row >= 0 && row < MaxRow && column >= 0 && column < MaxColumn;

        /// <summary>
        /// Get a pair (newRow, newColumn) from <param name="row"></param> and <paramref name="column"/>
        /// when rotating this matrix 180 degrees.
        /// </summary>
        /// <param name="row">The source row.</param>
        /// <param name="column">The source row.</param>
        /// <returns>A tuple pair of new row and column after the rotation.</returns>
        public (int Row, int Column) GetCellAfterRotatingOneEighty(int row, int column)
            => (MaxRow - row - 1, MaxColumn - column - 1);

        /// <summary>
        /// Get a pair (newRow, newColumn) from <param name="row"></param> and <paramref name="column"/>
        /// when rotating this matrix 90 degrees counter-clockwise.
        /// </summary>
        /// <param name="row">The source row.</param>
        /// <param name="column">The source row.</param>
        /// <returns>A tuple pair of new row and column after the rotation.</returns>
        public (int Row, int Column) GetCellAfterRotatingCounterclockwise(int row, int column)
            => (MaxColumn - 1 - column, row);

        /// <summary>
        /// Get a pair (newRow, newColumn) from <param name="row"></param> and <paramref name="column"/>
        /// when rotating this matrix 90 degrees clockwise.
        /// </summary>
        /// <param name="row">The source row.</param>
        /// <param name="column">The source row.</param>
        /// <returns>A tuple pair of new row and column after the rotation.</returns>
        public (int Row, int Column) GetCellAfterRotatingClockwise(int row, int column)
            => (column, MaxRow - 1 - row);

        /// <summary>
        /// Fill the matrix with the given value.
        /// </summary>
        /// <param name="value">The value to be filled.</param>
        public void Fill(T value)
            => Array.Fill(m_grid, value);

        public IEnumerator<T> GetEnumerator()
            => new Matrix2Enumerator<T>(m_grid);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        /// The row corresponding to the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index">List index.</param>
        /// <returns>The corresponding row.</returns>
        private int RowOf(int index)
            => index / MaxColumn;

        /// <summary>
        /// The column corresponding to the given <paramref name="index"/>.
        /// </summary>
        /// <param name="index">List index.</param>
        /// <returns>The corresponding column.</returns>
        private int ColumnOf(int index)
            => index % MaxColumn;

        /// <summary>
        /// Make a copy of this matrix.
        /// </summary>
        /// <returns>A copy of this matrix.</returns>
        public Matrix2<T> Clone()
            => new(this);

        /// <summary>
        /// Rotate this matrix 180 degrees. 
        /// </summary>
        /// <returns>The new rotated matrix.</returns>
        public Matrix2<T> RotateOneEighty()
            => RotateOneEighty(this);

        /// <summary>
        /// Rotate this matrix 90 degrees clockwise.
        /// </summary>
        /// <returns>The rotated matrix.</returns>
        public Matrix2<T> RotateClockwise()
            => RotateClockwise(this);

        /// <summary>
        /// Rotate this matrix 90 degrees counterclockwise.
        /// </summary>
        /// <returns>The rotated matrix.</returns>
        public Matrix2<T> RotateCounterClockwise()
            => RotateCounterClockwise(this);

        public Matrix2(int maxRow, int maxColumn)
        {
            MaxRow = maxRow;
            MaxColumn = maxColumn;
            m_grid = new T[Size];
            Array.Fill(m_grid, default);
        }

        public Matrix2(Matrix2<T> other)
        {
            MaxRow = other.MaxRow;
            MaxColumn = other.MaxColumn;
            m_grid = new T[Size];
            Array.Copy(other.m_grid, m_grid, Size);
        }

        public override string ToString()
        {
            var data = new StringBuilder();
            data.Append("----------------------------------------\nMatrix#")
                .Append(GetHashCode())
                .Append('\n')
                .Append("Size: ")
                .Append(MaxRow)
                .Append('x')
                .Append(MaxColumn)
                .Append('\n');
            for (var i = 0; i < Size; i++)
            {
                data.Append(m_grid[i])
                    .Append('\t');
                if (ColumnOf(i) == MaxColumn - 1)
                {
                    data.Append("\n");
                }
            }

            return data
                .Append("----------------------------------------")
                .ToString();
        }

        public void OnBeforeSerialize()
        {
            _serialized_max_row = MaxRow;
            _serialized_max_column = MaxColumn;
            var size = MaxRow * MaxColumn;
            _serialized_grid = new T[size];

            if (m_grid != null)
            {
                Array.Copy(m_grid, _serialized_grid, size);
            }
        }

        public void OnAfterDeserialize()
        {
            // Validate null
            if (_serialized_grid == null)
            {
                throw new SerializationException("Serialized grid is null!");
            }

            // Validate mismatch count
            var serializedSize = _serialized_grid.Length;
            var expectedCount = _serialized_max_column * _serialized_max_row;
            if (serializedSize != expectedCount)
            {
                var error = "Mismatch in grid's size with its serialized row and column." +
                            $"Expected: {expectedCount}, Actual: {serializedSize}";
                throw new SerializationException(error);
            }

            // Finalize data
            MaxRow = _serialized_max_row;
            MaxColumn = _serialized_max_column;
            m_grid = new T[Size];
            Array.Copy(_serialized_grid, m_grid, Size);
        }

        /// <summary>
        /// Rotate this matrix 90 degrees clockwise.
        /// </summary>
        /// <returns>The rotated matrix.</returns>
        public static Matrix2<T> RotateClockwise(Matrix2<T> matrix)
        {
            var maxRow = matrix.MaxRow;
            var maxColumn = matrix.MaxColumn;
            var rotated = new Matrix2<T>(maxColumn, maxRow);
            for (var row = 0; row < maxRow; row++)
            {
                for (var column = 0; column < maxColumn; column++)
                {
                    var (newRow, newColumn) = matrix.GetCellAfterRotatingClockwise(row, column);
                    rotated[newRow, newColumn] = matrix[row, column];
                }
            }

            return rotated;
        }

        /// <summary>
        /// Rotate this matrix 90 degrees counter-clockwise.
        /// </summary>
        /// <returns>The rotated matrix.</returns>
        public static Matrix2<T> RotateCounterClockwise(Matrix2<T> matrix)
        {
            var maxRow = matrix.MaxRow;
            var maxColumn = matrix.MaxColumn;
            var rotated = new Matrix2<T>(maxColumn, maxRow);
            for (var row = 0; row < maxRow; row++)
            {
                for (var column = 0; column < maxColumn; column++)
                {
                    var (newRow, newColumn) = matrix.GetCellAfterRotatingCounterclockwise(row, column);
                    rotated[newRow, newColumn] = matrix[row, column];
                }
            }

            return rotated;
        }

        /// <summary>
        /// Rotate this matrix 180 degrees. 
        /// </summary>
        /// <returns>The new rotated matrix.</returns>
        public static Matrix2<T> RotateOneEighty(Matrix2<T> matrix)
        {
            var maxRow = matrix.MaxRow;
            var maxColumn = matrix.MaxColumn;
            var rotated = new Matrix2<T>(maxRow, maxColumn);
            for (var row = 0; row < maxRow; row++)
            {
                for (var column = 0; column < maxColumn; column++)
                {
                    var (newRow, newColumn) = matrix.GetCellAfterRotatingOneEighty(row, column);
                    rotated[newRow, newColumn] = matrix[row, column];
                }
            }

            return rotated;
        }
    }
}
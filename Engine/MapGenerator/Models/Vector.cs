using System;
using System.Collections;
using System.Linq;

namespace Engine.MapGenerator.Models {
    public class Vector : IEnumerable, IComparable {
        /// <summary>
        ///     Global precision for any calculation
        /// </summary>
        private const int Precision = 10;

        private readonly double[] _data;
        public object Tag = null;

        /// <summary>
        ///     Build a new vector
        /// </summary>
        /// <param name="dim">The dimension</param>
        public Vector(int dim) {
            _data = new double[dim];
        }

        /// <summary>
        ///     Build a new vector
        /// </summary>
        /// <param name="vector">The elements of the vector</param>
        public Vector(params double[] vector) {
            _data = new double[vector.Length];
            vector.CopyTo(_data, 0);
        }

        /// <summary>
        ///     Build a new vector as a copy of an existing one
        /// </summary>
        /// <param name="vector">The existing vector</param>
        public Vector(Vector vector)
            : this(vector._data) {}

        /// <summary>
        ///     Gets or sets the value of the vector at the given index
        /// </summary>
        private double this[int i] {
            get { return _data[i]; }
            set { _data[i] = Math.Round(value, Precision); }
        }


        /// <summary>
        ///     First coordinate of vector, in euclidian geometry it's x value
        /// </summary>
        public double X {
            get { return _data[0]; }
            set { _data[0] = Math.Round(value, Precision); }
        }

        /// <summary>
        ///     Second coordinate of vector, in euclidian geometry it's y value
        /// </summary>
        public double Y {
            get { return _data[1]; }
            set { _data[1] = Math.Round(value, Precision); }
        }

        /// <summary>
        ///     Third coordinate of vector, in euclidian geometry it's y value
        /// </summary>
        public double Z {
            get { return _data[2]; }
            set { _data[2] = Math.Round(value, Precision); }
        }

        /// <summary>
        ///     The dimension of the vector
        /// </summary>
        private int Dimension => _data.Length;

        /// <summary>
        ///     The squared length of the vector
        /// </summary>
        private double SquaredLength => this * this;

        /// <summary>
        ///     The sum of all elements in the vector
        /// </summary>
        public double ElementSum {
            get {
                int i;
                double sum = 0;
                for (i = 0; i < Dimension; i++) {
                    sum += _data[i];
                }
                return sum;
            }
        }

        /// <summary>
        ///     Compare two vectors
        /// </summary>
        public int CompareTo(object obj) {
            var a = this;
            var b = obj as Vector;

            if (b == null) {
                return 0;
            }

            if (a.SquaredLength > b.SquaredLength) {
                return 1;
            }
            if (a.SquaredLength < b.SquaredLength) {
                return -1;
            }

            for (var i = 0; i < a.Dimension; i++) {
                if (a[i] > b[i]) {
                    return 1;
                }
                if (a[i] < b[i]) {
                    return -1;
                }
            }
            return 0;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _data.GetEnumerator();
        }

        /// <summary>
        ///     Scale all elements by scalar
        /// </summary>
        /// <param name="scalar">The scalar</param>
        public void Multiply(double scalar) {
            for (var i = 0; i < Dimension; i++) {
                this[i] *= scalar;
            }
        }

        /// <summary>
        ///     Add another vector
        /// </summary>
        /// <param name="vector">V</param>
        public void Add(Vector vector) {
            for (var i = 0; i < Dimension; i++) {
                this[i] += vector[i];
            }
        }

        /// <summary>
        ///     Add a constant to all elements
        /// </summary>
        /// <param name="constant">The constant</param>
        public void Add(double constant) {
            for (var i = 0; i < Dimension; i++) {
                this[i] += constant;
            }
        }

        /// <summary>
        ///     Convert the vector into a reconstructable string representation
        /// </summary>
        /// <returns>A string from which the vector can be rebuilt</returns>
        public override string ToString() {
            var str = "(";
            for (var i = 0; i < Dimension; i++) {
                str += this[i].ToString("G4");
                if (i < Dimension - 1) {
                    str += ";";
                }
            }
            str += ")";
            return str;
        }

        /// <summary>
        ///     Compares this vector with another one
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            var a = this;
            var b = obj as Vector;
            if (b == null || Dimension != b.Dimension) {
                return false;
            }
            return !_data.Where((t, i) => Math.Abs(t - b._data[i]) > 1e-10).Any();
        }

        /// <summary>
        ///     Retrieves a hashcode that is dependent on the elements
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode() {
            return _data.Aggregate(0, (current, value) => current ^ Math.Round(value, Precision).GetHashCode());
        }

        /// <summary>
        ///     Subtract two vectors
        /// </summary>
        public static Vector operator -(Vector a, Vector b) {
            if (a.Dimension != b.Dimension) {
                throw new Exception("Vectors of different dimension!");
            }
            var vector = new Vector(a.Dimension);
            for (var i = 0; i < a.Dimension; i++) {
                vector[i] = a[i] - b[i];
            }
            return vector;
        }

        /// <summary>
        ///     Add two vectors
        /// </summary>
        public static Vector operator +(Vector a, Vector b) {
            if (a.Dimension != b.Dimension) {
                throw new Exception("Vectors of different dimension!");
            }
            var vector = new Vector(a.Dimension);
            for (var i = 0; i < a.Dimension; i++) {
                vector[i] = a[i] + b[i];
            }
            return vector;
        }

        /// <summary>
        ///     Get the scalar product of two vectors
        /// </summary>
        public static double operator *(Vector a, Vector b) {
            if (a.Dimension != b.Dimension) {
                throw new Exception("Vectors of different dimension!");
            }
            double scalar = 0;
            int i;
            for (i = 0; i < a.Dimension; i++) {
                scalar += a[i] * b[i];
            }
            return scalar;
        }

        /// <summary>
        ///     Scale one vector
        /// </summary>
        public static Vector operator *(Vector vector, double scalar) {
            var newVector = new Vector(vector.Dimension);
            for (var i = 0; i < vector.Dimension; i++) {
                newVector[i] = vector[i] * scalar;
            }
            return newVector;
        }

        /// <summary>
        ///     Scale one vector
        /// </summary>
        public static Vector operator *(double scalar, Vector vector) {
            return vector * scalar;
        }

        /// <summary>
        ///     Interprete the vector as a double-array
        /// </summary>
        public static explicit operator double[](Vector vector) {
            return vector._data;
        }

        /// <summary>
        ///     Get the distance of two vectors
        /// </summary>
        public static double Dist(Vector a, Vector b) {
            if (a.Dimension != b.Dimension) {
                return -1;
            }
            var distance = 0.0d;
            for (var i = 0; i < a.Dimension; i++) {
                distance += Math.Pow(a[i] - b[i], 2);
            }
            return distance;
        }

        /// <summary>
        ///     Get a copy of one vector
        /// </summary>
        /// <returns></returns>
        public virtual Vector Clone() {
            return new Vector(_data);
        }
    }
}
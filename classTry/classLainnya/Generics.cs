namespace classLainnya;

// namespace HANYA BOLEH berisi TYPE, yaitu:

// class

// interface

// struct

// record

// enum




public class Stack<T> // Declares a type parameter T
{
    int position;
    T[] data = new T[100]; // Array of type T

    public void Push(T obj) => data[position++] = obj; // Accepts type T
    public T Pop() => data[--position];             // Returns type T
}


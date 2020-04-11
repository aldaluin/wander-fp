using System;

namespace Goldraven.Generic {
  class ArrayOfAnything<T> {

    /*
     *  cmcb 2019/10/27
     *
     *  Direct copy of a generic C# indexer from ms docs.
     */

    // Declare an array to store the data elements.
    private T[] arr;

    // Define the indexer to allow client code to use [] notation.
    public T this [int i] {
      get { return arr[i]; }
      set { arr[i] = value; }
    }

    public ArrayOfAnything (int cnt) {
      arr = new T[cnt];
    }

    public ArrayOfAnything () {
      arr = new T[10];
    }
  }
}
# Editor Architecture

How does Repository achieve such fast highlighting for large files? The answer lies in the way the editor is implemented.

## Background

The usual way to setup a code editor in Android is:

- Declare an `EditText` in your activity
- Get an instance of the `Spannable` interface (typically `SpannableString` or `SpannableStringBuilder`)
- Call `spannable.SetSpan(new ForegroundColorSpan(color), ...)` as needed to color regions of text
- Call `editText.SetText(spannable, TextView.BufferType.Editable)` to display the colored text

Unfortunately, this approach is no good for large files.

## Problem: Too many global references

The above approach hangs indefinitely for large files, in fact. The issue is with `new ForegroundColorSpan(color)` being called too many times, once for each region of colored text. `ForegroundColorSpan` is a Java type, and each time a Java object is instantiated from C# it must be registered with the Mono.Android runtime. There's a limit on the number of objects you can register, and once you exceed that limit your app will start to hang. See this answer for more info: https://stackoverflow.com/a/44621666/4077294

### Solution: Creating `ForegroundColorSpan`s in Java code

To prevent Mono from registering each `ForegroundColorSpan`, all of them are instantiated in Java code. This is the primary reason why a small portion of Repository is written in Java.

## Problem: Frequent interop calls

Since `ForegroundColorSpan` has to be instantiated in Java, `SetSpan` also has to be called from Java. Suppose we want to call `SetSpan` every time the text colorer colors a new region. Then we would have to call our custom Java method that calls `SetSpan` every time we color a region. Calling Java methods thousands of times from C# can become a bottleneck due to interop logic, so this isn't ideal.

### Solution: Buffering writes to `ColoredText`

Instead of calling the Java method each time a region is colored, we store the relevant info in a buffer. Adding info to the buffer doesn't require calling any Java code. When the buffer is full, we 'flush' the data, invoking a single Java method that uses info from the buffer to call `SetSpan` in bulk.

## Problem: Buffer copies

Previously, a `long[]` was used to send info to the Java code about 1) which regions to color, and 2) what to color them. (Both values can be encoded in `int`s, so each `long` contains the color in its top half and the number of chars to color in its bottom half.) Unfortunately, Java arrays are not the same as .NET arrays, so each time the Java method was invoked, the Mono.Android runtime transparently 1) allocated a Java array and 2) copied the data. This amounted to significant overhead for large files.

### Solution: `ByteBuffer`

Java offers a `ByteBuffer` class that can point to a random block of memory. No copies are made of its contents, even when a `ByteBuffer` object is passed from non-Java code. Repository takes advantage of this by storing each `long` in a `ByteBuffer` instead of a `long[]`. Storing each `long` does not require calling `ByteBuffer.PutLong` (which would be slow since `PutLong` is a Java method). Instead, each `long` is written to the underlying memory of the `ByteBuffer` using pointer manipulation, which can be done entirely in C#.

## Problem: Highlighting blocks UI thread

The `IHighlighter` interface has (had) no notion of saving state. Once it begins, the highlighter must run against the entire string passed to it. For large files, this is problematic: highlighting them can take tens of seconds. During those tens of seconds, the UI is frozen, because the UI thread is completely preoccupied with highlighting the source code.

### Solution: Async highlighting

TODO

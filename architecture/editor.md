# Editor Architecture

How does Repository achieve such fast highlighting for large files? The answer lies in the way the editor is implemented.

## Background

The usual way to setup a code editor in Android is:

- Declare an `EditText` in your activity
- Get an instance of the `Spannable` interface (typically `SpannableString` or `SpannableStringBuilder`)
- Call `spannable.SetSpan(new ForegroundColorSpan(color), ...)` as needed to color regions of text
- Call `editText.SetText(spannable, TextView.BufferType.Editable)` to show the colored text

Unfortunately, this approach is no good for large files.

## Problem #1: Too many global references #

The above approach may hang indefinitely for large files, in fact. The issue is with `new ForegroundColorSpan(color)` being called too many times, once for each region of colored text. `ForegroundColorSpan` is a Java type, and each time a Java object is instantiated from C# it must be registered with the Mono.Android runtime. There's a limit on the number of objects you can register, and once you exceed that limit your app will start to hang. See this answer for more info: https://stackoverflow.com/a/44621666/4077294

### Solution: Creating `ForegroundColorSpan`s in Java code

To prevent Mono from registering each `ForegroundColorSpan`, all of them are instantiated in Java code. This is the primary reason why a small portion of Repository is written in Java.

## Problem #2: Frequent interop calls #

Since `ForegroundColorSpan` has to be instantiated in Java, `SetSpan` also has to be called from Java. Suppose we want to call `SetSpan` every time the text colorer colors a new region. Then we would have to call our custom Java method that called `SetSpan` every time we color a region. Java interop calls can easily become a bottleneck when they are called thousands of times, so this isn't ideal.

### Solution: Buffering writes to `ColoredText`

Instead of calling the Java method each time a region is colored, we add the relevant info to a buffer. Only when the buffer is full do we flush the data, and tell a Java method to call `SetSpan` in bulk.

## Problem #3: Buffer copies #

Previously, a `long[]` was used to send info to the Java code about 1) which regions to color, and 2) what to color them. (Both values can be encoded in `int`s, so each `long` contains the color in its top half and the count in its bottom half.) Unfortunately, Java arrays are not the same as .NET arrays, so the Mono.Android runtime transparently 1) allocates a Java array and 2) copies the data during interop calls. This can result in significant overhead for large files.

### Solution: `ByteBuffer`

Java offers a `ByteBuffer` class that can point to a random block of memory. No copies are made of its contents, even when a `ByteBuffer` object is passed from non-Java code. Repository takes advantage of this by storing each `long` in a `ByteBuffer` instead of a `long[]`.

## Problem #4: Highlighting blocks UI thread #

The `IHighlighter` interface has no notion of saving state. Once it begins, the highlighter must run against the entire text passed to it. For large files, this is problematic since highlighting them can take tens of seconds.

### Solution: Async highlighting

TODO

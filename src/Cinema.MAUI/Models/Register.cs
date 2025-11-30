using System;
using System.Collections.Generic;
using System.Text;

namespace Cinema.MAUI.Models;

// init-only setters allow setting properties only during object initialization
// (constructor or object initializer).
//
// After initialization completes, properties become immutable (setters are no
// longer usable).
//
// This helps enforce required configuration at creation while keeping objects
// read-only thereafter.
internal class Register
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}

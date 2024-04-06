﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace KSoft.Blam.Megalo.Proto
{
	[System.Reflection.Obfuscation(Exclude=false, ApplyToMembers=false)]
	public enum MegaloScriptValueBaseType : byte
	{
		None,

		/// <summary>A standard boolean value</summary>
		Bool,

		/// <summary>A standard signed integer</summary>
		/// <remarks>When decoded, the sign is always extended</remarks>
		[SuppressMessage("Microsoft.Design", "CA1720:IdentifiersShouldNotContainTypeNames")]
		Int,
		/// <summary>A standard unsigned integer</summary>
		[SuppressMessage("Microsoft.Design", "CA1720:IdentifiersShouldNotContainTypeNames")]
		UInt,
		/// <summary>A value whose proto TypeParameter is an index to a SingleEncoding</summary>
		[SuppressMessage("Microsoft.Design", "CA1720:IdentifiersShouldNotContainTypeNames")]
		Single,

		Point3d,

		/// <summary>An enumeration whose members represent individual bits</summary>
		/// <remarks>BitLength is autogenerated when absent from the prototype</remarks>
		Flags,
		/// <summary>An enumeration whose members represent sequential values starting from -1 or 0</summary>
		/// <remarks>BitLength is autogenerated when absent from the prototype</remarks>
		Enum,
		/// <summary>A reference (by index) to static, variant, or runtime data</summary>
		/// <remarks>BitLength is autogenerated when absent from the prototype</remarks>
		Index,

		/// <remarks>If used as a parameter, it is assumed the value is encoded as PointerHasValue</remarks>
		Var,			// BitLength (Autogenerated) = Database.GetVariableIndexBitLength
		/// <summary>A reference to any or a specific group of variables or their variable-typed attributes</summary>
		VarReference,
		/// <summary>Optional values (0-n) which can be used as input in a formatted string in the variant string table</summary>
		/// <remarks>BitLength is autogenerated and represents the amount of bits to encode TokenCount</remarks>
		Tokens,

		// Composite
		/// <summary>Represents a branch to a set of conditions and actions</summary>
		VirtualTrigger,
		/// <summary>Represents a shape type and one or more variable references representing shape attributes</summary>
		Shape,
		/// <summary>Represents a target type and, if not none, an optional target variable reference</summary>
		TargetVar,
		TeamFilterParameters,
		NavpointIconParameters,
		WidgetMeterParameters,
		ObjectReferenceWithPlayerVarIndex,

		/// <remarks>5 bits</remarks>
		[Obsolete(EnumBitEncoderBase.kObsoleteMsg, true)] kNumberOf,
	};
}

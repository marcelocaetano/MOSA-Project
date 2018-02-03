// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// MovCRStore
	/// </summary>
	/// <seealso cref="Mosa.Platform.x86.X86Instruction" />
	public sealed class MovCRStore : X86Instruction
	{
		public static readonly LegacyOpCode LegacyOpcode = new LegacyOpCode(new byte[] { 0x0F, 0x22 } );

		internal MovCRStore()
			: base(0, 2)
		{
		}

		public override bool IsMemoryWrite { get { return true; } }

		public override bool ThreeTwoAddressConversion { get { return false; } }

		internal override void EmitLegacy(InstructionNode node, X86CodeEmitter emitter)
		{
			System.Diagnostics.Debug.Assert(node.ResultCount == DefaultResultCount);
			System.Diagnostics.Debug.Assert(node.OperandCount == DefaultOperandCount);

			emitter.Emit(LegacyOpcode, node.Operand1, node.Operand2);
		}

		// The following is used by the automated code generator.

		public override LegacyOpCode __legacyopcode { get { return LegacyOpcode; } }

		public override string __legacyOpcodeOperandOrder { get { return "12"; } }
	}
}


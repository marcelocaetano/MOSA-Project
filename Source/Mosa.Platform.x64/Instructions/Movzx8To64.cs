// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x64.Instructions
{
	/// <summary>
	/// Movzx8To64
	/// </summary>
	/// <seealso cref="Mosa.Platform.x64.X64Instruction" />
	public sealed class Movzx8To64 : X64Instruction
	{
		public override int ID { get { return 473; } }

		internal Movzx8To64()
			: base(1, 1)
		{
		}

		public override void Emit(InstructionNode node, BaseCodeEmitter emitter)
		{
			System.Diagnostics.Debug.Assert(node.ResultCount == 1);
			System.Diagnostics.Debug.Assert(node.OperandCount == 1);

			emitter.OpcodeEncoder.SuppressByte(0x40);
			emitter.OpcodeEncoder.AppendNibble(0b0100);
			emitter.OpcodeEncoder.AppendBit(0b1);
			emitter.OpcodeEncoder.AppendBit((node.Result.Register.RegisterCode >> 3) & 0x1);
			emitter.OpcodeEncoder.AppendBit(0b0);
			emitter.OpcodeEncoder.AppendBit((node.Operand1.Register.RegisterCode >> 3) & 0x1);
			emitter.OpcodeEncoder.AppendByte(0x0F);
			emitter.OpcodeEncoder.AppendNibble(0b1011);
			emitter.OpcodeEncoder.AppendNibble(0b0110);
			emitter.OpcodeEncoder.Append2Bits(0b11);
			emitter.OpcodeEncoder.Append3Bits(node.Result.Register.RegisterCode);
			emitter.OpcodeEncoder.Append3Bits(node.Operand1.Register.RegisterCode);
		}
	}
}

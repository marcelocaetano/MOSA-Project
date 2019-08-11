// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.ARMv8A32.Instructions
{
	/// <summary>
	/// Ldr32 - Single Data Transfer
	/// </summary>
	/// <seealso cref="Mosa.Platform.ARMv8A32.ARMv8A32Instruction" />
	public sealed class Ldr32 : ARMv8A32Instruction
	{
		public override int ID { get { return 641; } }

		internal Ldr32()
			: base(1, 3)
		{
		}

		public override void Emit(InstructionNode node, BaseCodeEmitter emitter)
		{
			System.Diagnostics.Debug.Assert(node.ResultCount == 1);
			System.Diagnostics.Debug.Assert(node.OperandCount == 3);

			if (node.Operand1.IsCPURegister && node.Operand2.IsCPURegister && node.Operand3.IsConstant)
			{
				emitter.OpcodeEncoder.Append4Bits(GetConditionCode(node.ConditionCode));
				emitter.OpcodeEncoder.Append2Bits(0b01);
				emitter.OpcodeEncoder.Append1Bit(0b0);
				emitter.OpcodeEncoder.Append1Bit(0b0);
				emitter.OpcodeEncoder.Append1BitImmediate(node.Operand3);
				emitter.OpcodeEncoder.Append1Bit(0b0);
				emitter.OpcodeEncoder.Append1Bit(0b0);
				emitter.OpcodeEncoder.Append1Bit(0b1);
				emitter.OpcodeEncoder.Append4Bits(node.Operand1.Register.RegisterCode);
				emitter.OpcodeEncoder.Append4Bits(node.Result.Register.RegisterCode);
				emitter.OpcodeEncoder.Append4Bits(0b0000);
				emitter.OpcodeEncoder.Append1Bit(0b0);
				emitter.OpcodeEncoder.Append2Bits(0b00);
				emitter.OpcodeEncoder.Append1Bit(0b0);
				emitter.OpcodeEncoder.Append4Bits(node.Operand2.Register.RegisterCode);
				return;
			}

			throw new Compiler.Common.Exceptions.CompilerException("Invalid Opcode");
		}
	}
}

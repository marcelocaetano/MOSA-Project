// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common.Exceptions;
using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Platform;
using System.Diagnostics;

namespace Mosa.Platform.ARMv8A32
{
	/// <summary>
	/// BaseTransformationStage
	/// </summary>
	public abstract class BaseTransformationStage : BasePlatformTransformationStage
	{
		protected override string Platform { get { return "ARMv8A32"; } }

		#region Helper Methods

		protected void SwapFirstTwoOperandsIfFirstConstant(Context context)
		{
			var operand1 = context.Operand1;

			if (operand1.IsConstant)
			{
				var operand2 = context.Operand2;

				context.Operand1 = operand2;
				context.Operand2 = operand1;
			}
		}

		protected void UpdateInstruction(Context context, BaseInstruction virtualInstruction, BaseInstruction immediateInstruction, Operand result, StatusRegister statusRegister, Operand operand1)
		{
			if (operand1.IsConstant)
			{
				operand1 = CreateImmediateOperand(context, operand1);
			}

			if (operand1.IsVirtualRegister || operand1.IsCPURegister)
			{
				context.SetInstruction(virtualInstruction, result, operand1);
			}
			else if (operand1.IsResolvedConstant)
			{
				context.SetInstruction(immediateInstruction, result, operand1);
			}
			else
			{
				throw new CompilerException("Error at {context} in {Method}");
			}
		}

		protected void UpdateInstruction(Context context, BaseInstruction virtualInstruction, BaseInstruction immediateInstruction, Operand result, StatusRegister statusRegister, Operand operand1, Operand operand2)
		{
			if (operand1.IsConstant)
			{
				if (virtualInstruction.IsCommutative && !operand2.IsConstant)
				{
					var temp = operand1;
					operand1 = operand2;
					operand2 = temp;
				}
				else
				{
					var v1 = AllocateVirtualRegister(TypeSystem.BuiltIn.I4);

					var before = context.InsertBefore();

					if (operand1.IsResolvedConstant)
					{
						before.SetInstruction(ARMv8A32.MovImm, v1, CreateConstant(operand1.ConstantUnsignedInteger & 0xFFFF));
						before.AppendInstruction(ARMv8A32.MovtImm, v1, v1, CreateConstant(operand1.ConstantUnsignedInteger >> 16));
					}
					else
					{
						before.SetInstruction(ARMv8A32.MovImm, v1, operand1);
						before.AppendInstruction(ARMv8A32.MovtImm, v1, v1, operand1);
					}

					operand1 = v1;
				}
			}

			if (operand2.IsConstant)
			{
				operand2 = CreateImmediateOperand(context, operand2);
			}

			Debug.Assert(operand1.IsVirtualRegister || operand1.IsCPURegister);

			if (operand2.IsVirtualRegister || operand2.IsCPURegister)
			{
				context.SetInstruction(virtualInstruction, statusRegister, result, operand1, operand2);
			}
			else if (operand2.IsResolvedConstant)
			{
				context.SetInstruction(immediateInstruction, statusRegister, result, operand1, operand2);
			}
			else
			{
				throw new CompilerException("Error at {context} in {Method}");
			}
		}

		protected Operand CreateImmediateOperand(Context context, Operand operand)
		{
			if (!operand.IsConstant)
				return operand;

			if (operand.IsResolvedConstant)
			{
				if (ARMHelper.CalculateImmediateValue(operand.ConstantUnsignedInteger, out uint immediate, out _, out _))
				{
					if (operand.ConstantUnsignedLongInteger == immediate)
					{
						return operand;
					}

					return CreateConstant(immediate);
				}
				else
				{
					var v1 = AllocateVirtualRegister(TypeSystem.BuiltIn.I4);

					var before = context.InsertBefore();

					before.SetInstruction(ARMv8A32.MovImm, v1, CreateConstant(operand.ConstantUnsignedInteger & 0xFFFF));
					before.AppendInstruction(ARMv8A32.MovtImm, v1, CreateConstant(operand.ConstantUnsignedInteger >> 16));

					return v1;
				}
			}
			else if (operand.IsUnresolvedConstant)
			{
				var v1 = AllocateVirtualRegister(TypeSystem.BuiltIn.I4);

				var before = context.InsertBefore();

				before.SetInstruction(ARMv8A32.MovImm, v1, operand);
				before.AppendInstruction(ARMv8A32.MovtImm, v1, v1, operand);

				return v1;
			}
			else
			{
				throw new CompilerException("Error at {context} in {Method}");
			}
		}

		#endregion Helper Methods
	}
}

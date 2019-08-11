// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;
using System.Collections.Generic;

namespace Mosa.Platform.ARMv8A32
{
	/// <summary>
	/// ARMv8A32 Instruction Map
	/// </summary>
	public static class ARMv8A32Instructions
	{
		public static readonly List<BaseInstruction> List = new List<BaseInstruction> {
			ARMv8A32.Adc,
			ARMv8A32.AdcImm,
			ARMv8A32.AdcImmShift,
			ARMv8A32.AdcRegShift,
			ARMv8A32.Add,
			ARMv8A32.AddImm,
			ARMv8A32.AddImmShift,
			ARMv8A32.AddRegShift,
			ARMv8A32.And,
			ARMv8A32.AndImm,
			ARMv8A32.AndImmShift,
			ARMv8A32.AndRegShift,
			ARMv8A32.Bic,
			ARMv8A32.BicImm,
			ARMv8A32.BicImmShift,
			ARMv8A32.BicRegShift,
			ARMv8A32.Cmn,
			ARMv8A32.CmnImm,
			ARMv8A32.CmnImmShift,
			ARMv8A32.CmnRegShift,
			ARMv8A32.Cmp,
			ARMv8A32.CmpImm,
			ARMv8A32.CmpImmShift,
			ARMv8A32.CmpRegShift,
			ARMv8A32.Eor,
			ARMv8A32.EorImm,
			ARMv8A32.EorImmShift,
			ARMv8A32.EorRegShift,
			ARMv8A32.Mvn,
			ARMv8A32.MvnImm,
			ARMv8A32.MvnImmShift,
			ARMv8A32.MvnRegShift,
			ARMv8A32.Orr,
			ARMv8A32.OrrImm,
			ARMv8A32.OrrImmShift,
			ARMv8A32.OrrRegShift,
			ARMv8A32.Rsb,
			ARMv8A32.RsbImm,
			ARMv8A32.RsbImmShift,
			ARMv8A32.RsbRegShift,
			ARMv8A32.Rsc,
			ARMv8A32.RscImm,
			ARMv8A32.RscImmShift,
			ARMv8A32.RscRegShift,
			ARMv8A32.Sbc,
			ARMv8A32.SbcImm,
			ARMv8A32.SbcImmShift,
			ARMv8A32.SbcRegShift,
			ARMv8A32.Sub,
			ARMv8A32.SubImm,
			ARMv8A32.SubImmShift,
			ARMv8A32.SubRegShift,
			ARMv8A32.Teq,
			ARMv8A32.TeqImm,
			ARMv8A32.TeqImmShift,
			ARMv8A32.TeqRegShift,
			ARMv8A32.Tst,
			ARMv8A32.TstImm,
			ARMv8A32.TstImmShift,
			ARMv8A32.TstRegShift,
			ARMv8A32.Bl,
			ARMv8A32.B,
			ARMv8A32.Bx,
			ARMv8A32.Bkpt,
			ARMv8A32.Dmb,
			ARMv8A32.Dsb,
			ARMv8A32.Isb,
			ARMv8A32.Ldm,
			ARMv8A32.Ldmfd,
			ARMv8A32.Ldmia,
			ARMv8A32.Ldr32,
			ARMv8A32.LdrUp32,
			ARMv8A32.LdrDown32,
			ARMv8A32.Ldr8,
			ARMv8A32.LdrUp8,
			ARMv8A32.LdrDown8,
			ARMv8A32.LdrImm32,
			ARMv8A32.LdrUpImm32,
			ARMv8A32.LdrDownImm32,
			ARMv8A32.LdrImm8,
			ARMv8A32.LdrUpImm8,
			ARMv8A32.LdrDownImm8,
			ARMv8A32.Ldr16,
			ARMv8A32.LdrUp16,
			ARMv8A32.LdrDown16,
			ARMv8A32.LdrImm16,
			ARMv8A32.LdrUpImm16,
			ARMv8A32.LdrDownImm16,
			ARMv8A32.LdrS16,
			ARMv8A32.LdrUpS16,
			ARMv8A32.LdrDownS16,
			ARMv8A32.LdrImmS16,
			ARMv8A32.LdrUpImmS16,
			ARMv8A32.LdrDownImmS16,
			ARMv8A32.LdrS8,
			ARMv8A32.LdrUpS8,
			ARMv8A32.LdrDownS8,
			ARMv8A32.LdrImmS8,
			ARMv8A32.LdrUpImmS8,
			ARMv8A32.LdrDownImmS8,
			ARMv8A32.Mrs,
			ARMv8A32.Msr,
			ARMv8A32.Mul,
			ARMv8A32.Nop,
			ARMv8A32.Pop,
			ARMv8A32.Push,
			ARMv8A32.Rev,
			ARMv8A32.Rev16,
			ARMv8A32.Revsh,
			ARMv8A32.Sev,
			ARMv8A32.Stm,
			ARMv8A32.Stmea,
			ARMv8A32.Stmia,
			ARMv8A32.Str,
			ARMv8A32.Strb,
			ARMv8A32.Strh,
			ARMv8A32.Svc,
			ARMv8A32.Swi,
			ARMv8A32.Sxtb,
			ARMv8A32.Sxth,
			ARMv8A32.Uxtb,
			ARMv8A32.Uxth,
			ARMv8A32.Wfe,
			ARMv8A32.Wfi,
			ARMv8A32.Yield,
			ARMv8A32.MovtImm,
			ARMv8A32.MovkImm,
			ARMv8A32.Mov,
			ARMv8A32.MovImm,
			ARMv8A32.MovImmShift,
			ARMv8A32.MovRegShift,
			ARMv8A32.Lsl,
			ARMv8A32.LslImm,
			ARMv8A32.Lsr,
			ARMv8A32.LsrImm,
			ARMv8A32.Asr,
			ARMv8A32.AsrImm,
			ARMv8A32.Ror,
			ARMv8A32.RorImm,
		};
	}
}

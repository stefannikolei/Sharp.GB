using System;
using System.Collections.Generic;
using Sharp.GB.Cpu;
using Sharp.GB.Cpu.Op.ArgumentImplementations;
using Sharp.GB.Memory.Interface;

public abstract class Argument
{
    private string _label;

    private int _operandLength;

    private bool _memory;

    private DataType _dataType;

    protected Argument(string name)
        : this(name, 0, false, DataType.D8) { }

    protected Argument(string label, int operandLength, bool memory, DataType dataType)
    {
        // TODO: "" must be replaced with the Implementationname
        _label = label == null ? "" : label;
        _operandLength = operandLength;
        _memory = memory;
        _dataType = dataType;
    }

    public int GetOperandLength()
    {
        return _operandLength;
    }

    public bool IsMemory()
    {
        return _memory;
    }

    public abstract int Read(Registers registers, IAddressSpace addressSpace, int[] args);

    public abstract void Write(
        Registers registers,
        IAddressSpace addressSpace,
        int[] args,
        int value
    );

    public DataType GetDataType()
    {
        return _dataType;
    }

    private static List<Argument> s_values =
    [
        new A82(),
        new A162(),
        new Bc2(),
        new C2(),
        new De2(),
        new Hl2(),
        new A(),
        new A16(),
        new Af(),
        new B(),
        new Bc(),
        new C(),
        new D(),
        new D8(),
        new D16(),
        new De(),
        new E(),
        new H(),
        new Hl(),
        new L(),
        new Pc(),
        new R8(),
        new Sp(),
    ];

    public static Argument Parse(string value)
    {
        foreach (Argument a in s_values)
        {
            if (a._label.Equals(value))
            {
                return a;
            }
        }

        throw new ArgumentException("Unknown argument: " + value);
    }

    public string GetLabel()
    {
        return _label;
    }
}

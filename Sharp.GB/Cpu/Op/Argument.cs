using System;
using System.Collections.Generic;
using Sharp.GB.Cpu;
using Sharp.GB.Cpu.Op.ArgumentImplementations;
using Sharp.GB.Memory.Interface;

public abstract class Argument
{
    private string label;

    private int operandLength;

    private bool memory;

    private DataType dataType;

    protected Argument(string name) :
        this(name, 0, false, DataType.D8)
    {
    }

    protected Argument(string label, int operandLength, bool memory, DataType dataType)
    {
        // TODO: "" must be replaced with the Implementationname
        this.label = label == null ? "" : label;
        this.operandLength = operandLength;
        this.memory = memory;
        this.dataType = dataType;
    }

    public int getOperandLength()
    {
        return operandLength;
    }

    public bool isMemory()
    {
        return memory;
    }

    public abstract int read(Registers registers, IAddressSpace addressSpace, int[] args);

    public abstract void write(Registers registers, IAddressSpace addressSpace, int[] args, int value);

    public DataType getDataType()
    {
        return dataType;
    }

    private static List<Argument> Values =
    [
        new _A8(), new _A16(), new _BC(), new _C(), new _DE(), new _HL(), new A(), new A16(), new AF(), new B(),
        new BC(), new C(), new D(), new D8(), new D16(), new DE(), new E(), new H(), new HL(), new L(), new PC(),
        new R8(), new SP()
    ];

    public static Argument parse(string value)
    {
        foreach (Argument a in Values)
        {
            if (a.label.Equals(value))
            {
                return a;
            }
        }

        throw new ArgumentException("Unknown argument: " + value);
    }

    public String getLabel()
    {
        return label;
    }
}

#if V_1

#type1 To#type1(#type0 val)
{
    return (#type1)(val);
}

#type0 From#type1(#type1 val)
{
    return (#type0)(val);
}

#elseif V_2

#type1 To#type1(#type0 val)
{
    return (#type1)(val.s0, val.s1);
}

#type0 From#type1(#type1 val)
{
    return (#type0)(val.s0, val.s1);
}

#elseif V_3

#type1 To#type1(#type0 val)
{
    return (#type1)(val.s0, val.s1, val.s2);
}

#type0 From#type1(#type1 val)
{
    return (#type0)(val.s0, val.s1, val.s2);
}

#elseif V_4

#type1 To#type1(#type0 val)
{
    return (#type1)(val.s0, val.s1, val.s2, val.s3);
}

#type0 From#type1(#type1 val)
{
    return (#type0)(val.s0, val.s1, val.s2, val.s3);
}

#elseif V_8

#type1 To#type1(#type0 val)
{
    return (#type1)(val.s0, val.s1, val.s2, val.s3, val.s4, val.s5, val.s6, val.s7);
}
#type0 From#type1(#type1 val)
{
    return (#type0)(val.s0, val.s1, val.s2, val.s3, val.s4, val.s5, val.s6, val.s7);
}

#elseif V_16

#type1 To#type1(#type0 val)
{
    return (#type1)(val.s0, val.s1, val.s2, val.s3, val.s4, val.s5, val.s6, val.s7, val.s8, val.s9, val.sA, val.sB, val.sC, val.sD, val.sE, val.sF);
}
#type0 From#type1(#type1 val)
{
    return (#type0)(val.s0, val.s1, val.s2, val.s3, val.s4, val.s5, val.s6, val.s7, val.s8, val.s9, val.sA, val.sB, val.sC, val.sD, val.sE, val.sF);
}
#endif
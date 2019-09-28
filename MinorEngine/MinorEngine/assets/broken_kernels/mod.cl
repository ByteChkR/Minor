#if V_1
#type0 Modulus(#type0 a, #type0 m)
{
	float val = fmod((float)a, (float)m);
    return (#type0)(val);
}

#elseif V_2
#type0 Modulus(#type0 a, #type0 m)
{
	float2 val = fmod((float2)(a.s0, a.s1), 
		(float2)(m.s0,m.s1));
    return (#type0)(val.s0, val.s1);
}
#elseif V_3
#type0 Modulus(#type0 a, #type0 m)
{
	float3 val = fmod((float3)(a.s0, a.s1, a.s2), (float3)(m.s0, m.s1, m.s2));
    return (#type0)(val.s0, val.s1, val.s2);
}
#elseif V_4
#type0 Modulus(#type0 a, #type0 m)
{
	float4 val = fmod((float4)(a.s0, a.s1, a.s2, a.s3), (float4)(m.s0, m.s1, m.s2, m.s3));
    return (#type0)(val.s0, val.s1, val.s2, val.s3);
}
#elseif V_8
#type0 Modulus(#type0 a, #type0 m)
{
	float8 val = fmod((float8)(a.s0, a.s1, a.s2, a.s3, a.s4, a.s5, a.s6, a.s7), 
		(float8)(m.s0, m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7));
    return (#type0)(val.s0, val.s1, val.s2, val.s3, val.s4, val.s5, val.s6, val.s7);
}
#elseif V_16
#type0 Modulus(#type0 a, #type0 m)
{
	float16 val = fmod((float16)(a.s0, a.s1, a.s2, a.s3, a.s4, a.s5, a.s6, a.s7, a.s8, a.s9, a.sA, a.sB, a.sC, a.sD, a.sE, a.sF), 
		(float16)(m.s0, m.s1, m.s2, m.s3, m.s4, m.s5, m.s6, m.s7, m.s8, m.s9, m.sA, m.sB, m.sC, m.sD, m.sE, m.sF));
    return (#type0)(val.s0, val.s1, val.s2, val.s3, val.s4, val.s5, val.s6, val.s7, val.s8, val.s9, val.sA, val.sB, val.sC, val.sD, val.sE, val.sF);
}
#endif
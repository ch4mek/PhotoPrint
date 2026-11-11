export const formatNullableNumber = (value?: number | null) =>
	value == null ? null : String(value);

export const checkIfForwardProps =
	(propNames: string | string[]) => (prop: string) =>
		Array.isArray(propNames) ? !propNames.includes(prop) : propNames !== prop;

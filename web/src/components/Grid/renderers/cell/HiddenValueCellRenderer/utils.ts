import { DEFAULT_MASK_SYMBOL } from './constants';
import { MaskSensitiveValueOptions } from './types';

export const getMaskedDisplayValue = (
	rawValue: unknown,
	{
		maskSymbol = DEFAULT_MASK_SYMBOL,
		maskRepeatCount,
	}: MaskSensitiveValueOptions = {}
) => {
	const stringValue = String(rawValue ?? '');
	const repeatLength = maskRepeatCount ?? stringValue.length;

	return maskSymbol.repeat(Math.max(0, repeatLength));
};

import { RenderCellProps } from 'react-data-grid';

import { GridCellDefaultProps } from '@/components/Grid';

import { HiddenValueCellWrapper } from './styles';
import { HiddenValueCellRendererOptions } from './types';
import { getMaskedDisplayValue } from './utils';

export const HiddenValueCellRenderer = <T extends GridCellDefaultProps>(
	{ row, column: { key } }: RenderCellProps<T, unknown>,
	{ fieldToDisplay, options }: HiddenValueCellRendererOptions<T> = {}
) => {
	const { maskSymbol, maskRepeatCount } = options ?? {};
	const rawValue = row[fieldToDisplay ?? (key as keyof T)];

	const maskedText = getMaskedDisplayValue(rawValue, {
		maskSymbol,
		maskRepeatCount,
	});

	return <HiddenValueCellWrapper>{maskedText}</HiddenValueCellWrapper>;
};

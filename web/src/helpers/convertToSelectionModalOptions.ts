import { SelectionModalOption } from '@/components/Grid/renderers/header/types';

export const convertToSelectionModalOptions = <
	T extends Pick<SelectionModalOption, 'id'>
>(
	items: Array<T>,
	field: keyof T
): Array<SelectionModalOption> =>
	items.map((item) => ({
		id: item.id,
		description: String(item[field]),
	}));

export const convertConstantToSelectionModalOption = (
	constant: string
): SelectionModalOption => ({
	id: constant,
	description: constant,
});

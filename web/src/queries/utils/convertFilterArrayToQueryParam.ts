import { ITEM_ROOT } from 'odata-query';

import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';

export const convertFilterArrayToQueryParam = (
	filterArray: string[],
	allFilterValuesLength: number,
	emptyFilterValue: null | '' = null
) => {
	if (!filterArray.length || filterArray.length === allFilterValuesLength) {
		return undefined;
	}

	const filterArrayWithoutNoValueOption = filterArray.filter(
		(filterValue) => filterValue !== NO_VALUE_FILTER_OPTION.name
	);

	if (filterArrayWithoutNoValueOption.length === filterArray.length) {
		return {
			in: filterArrayWithoutNoValueOption,
		};
	}

	if (filterArrayWithoutNoValueOption.length) {
		return {
			or: [
				{
					[ITEM_ROOT]: {
						in: filterArrayWithoutNoValueOption,
					},
				},
				{
					[ITEM_ROOT]: {
						eq: emptyFilterValue,
					},
				},
			],
		};
	}

	return { eq: emptyFilterValue };
};

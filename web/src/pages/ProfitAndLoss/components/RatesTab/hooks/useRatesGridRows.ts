import { useMemo } from 'react';

import { ConvertRatesItemsIntoRowsParams } from '../types';
import { convertRatesItemsIntoRows } from '../utils/convertRatesDataIntoRows';
import { patchRowsExpandedState } from '../utils/patchRowsExpandedState';

export const useRatesGridRows = ({
	items,
	expandedIds,
	departmentTypes,
	canOpenProfile,
	handleOpenRateHistory,
}: ConvertRatesItemsIntoRowsParams) => {
	const baseRows = useMemo(
		() =>
			convertRatesItemsIntoRows({
				items,
				expandedIds: new Set(),
				departmentTypes,
				canOpenProfile,
				handleOpenRateHistory,
			}),
		[items, departmentTypes, canOpenProfile, handleOpenRateHistory]
	);

	return useMemo(
		() => patchRowsExpandedState(baseRows, expandedIds),
		[baseRows, expandedIds]
	);
};

import { useMemo } from 'react';

import { MOCK_ROW } from '../constants';
import { EquipmentHistoryRow } from '../types';
import { getMockRows } from '../utils/getMockRows';
import { mapEquipmentHistoryRows } from '../utils/mapEquipmentHistoryRows';

import { DEFAULT_GRID_SKELETON_ROWS_AMOUNT } from '@/components/Grid/constants';
import { useConfiguration } from '@/context/Configuration';
import { EquipmentHistoryResponse } from '@/types/equipment';

export const useGetEquipmentHistoryRows = (
	data: EquipmentHistoryResponse,
	showSkeleton: boolean
): EquipmentHistoryRow[] => {
	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const isMockRowVisible = showSkeleton || !data?.items;

	return useMemo(() => {
		if (isMockRowVisible) {
			const mockRowsCount =
				data?.items?.length ?? DEFAULT_GRID_SKELETON_ROWS_AMOUNT / 2;

			return getMockRows(mockRowsCount, MOCK_ROW);
		}

		return mapEquipmentHistoryRows(data.items, departmentTypes);
	}, [data?.items, departmentTypes, isMockRowVisible]);
};

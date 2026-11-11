import { useMemo } from 'react';

import { MOCK_ROW } from '../constants';

import { getSkeletonRows } from '@/components/Grid/utils/getSkeletonRows';
import { formatDateString } from '@/helpers/dateToISOString';
import { GetUserContractsResponse } from '@/queries/teamboard/types';
import { PersistentConfigurationItem } from '@/types/configuration';

const MOCK_ROWS_AMOUNT = 1;

export const useContractsRow = (
	data: GetUserContractsResponse | undefined,
	handleItemEditClick: (id?: number | string) => void,
	showSkeleton: boolean,
	{ fireTypes }: { fireTypes: Array<PersistentConfigurationItem> }
) =>
	useMemo(() => {
		if (showSkeleton) {
			return getSkeletonRows(MOCK_ROWS_AMOUNT, MOCK_ROW);
		}

		return (data?.items || []).map((contract) => ({
			id: contract.id,
			contractFrom: formatDateString(contract.contractFrom, ''),
			contractTo: formatDateString(contract.contractTo, ''),
			fireDate: formatDateString(contract.fireDate, ''),
			fireType:
				fireTypes.find(({ name }) => name === contract.fireType)?.description ??
				'',
			fireReason: contract.fireReason ?? '',
			fireDescription: contract.fireDescription ?? '',
			handleEditClick: handleItemEditClick,
		}));
	}, [data?.items, showSkeleton]);

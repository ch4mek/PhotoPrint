import { useMemo } from 'react';

import { formatDateString } from '@/helpers/dateToISOString';
import { GetUserContractsResponse } from '@/queries/teamboard/types';

export const useContractsRow = (data: GetUserContractsResponse | undefined) =>
	useMemo(() => {
		if (!data?.items) {
			return [];
		}

		return data.items.map((contract) => ({
			id: contract.id,
			contractFrom: formatDateString(contract.contractFrom, ''),
			contractTo: formatDateString(contract.contractTo, ''),
		}));
	}, [data?.items]);

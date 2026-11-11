import { ClaimsRow } from '../types';

import { splitValueByCapitalLetters } from '@/pages/Settings/utils';
import {
	GetAdminDepartmentClaimsResponse,
	GetAdminRoleClaimsResponse,
} from '@/queries/types';

export const convertRows = <
	TData extends GetAdminDepartmentClaimsResponse | GetAdminRoleClaimsResponse,
	TKey extends keyof TData
>(
	keyFieldName: TKey,
	data?: TData[]
): ClaimsRow[] => {
	if (!data || data.length === 0) {
		return [];
	}

	const claimNames = data[0].claims.map((claim) => claim.name);

	return claimNames.map((name) => ({
		name: splitValueByCapitalLetters(name),
		...data.reduce((acc, dataItem) => {
			const { claims } = dataItem;

			if (claims && claims.length > 0) {
				const key = String(dataItem[keyFieldName]);
				const claim = claims.find((claimName) => claimName.name === name);

				return { ...acc, [key]: Boolean(claim?.value) };
			}

			return acc;
		}, {}),
	}));
};

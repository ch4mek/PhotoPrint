import { UserClaimsRow, User } from '../types';

import { getDepartmentName } from '@/helpers/getDepartmentName';
import { splitValueByCapitalLetters } from '@/pages/Settings/utils';
import { GetAdminUsersClaimsItemsResponse } from '@/queries/types';
import { ComplexConfigurableConfigurationItem } from '@/types/configuration';

export const convertUsersRows = <
	TData extends GetAdminUsersClaimsItemsResponse
>(
	columns: string[],
	handleEdit: (user: User) => () => void,
	departments: ComplexConfigurableConfigurationItem[],
	data?: TData[]
): UserClaimsRow[] => {
	if (!data || data.length === 0) {
		return [];
	}

	return data.map((dataItem) => {
		const { groups, userId, userFullName, color, departmentId } = dataItem;

		const row: UserClaimsRow = {
			userId,
			color,
			departmentId: getDepartmentName(departments, departmentId),
			name: splitValueByCapitalLetters(userFullName),
			handleEditClick: () => handleEdit({ userId, userFullName }),
		};

		if (groups && groups.length > 0) {
			groups.forEach((claim) => {
				if (columns.includes(claim.name)) {
					row[claim.name] = claim.status;
				}
			});
		}

		return row;
	});
};

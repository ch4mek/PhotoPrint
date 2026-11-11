import { useCallback, useMemo } from 'react';

import { DEFAULT_GRID_SKELETON_ROWS_AMOUNT } from '@/components/Grid/constants';
import { isIntern, Roles } from '@/constants/roles';
import { useConfiguration } from '@/context/Configuration';
import { mergeUserFullNames } from '@/helpers/utils';
import { UsersRowsParams } from '@/pages/AdminPanel/components/UsersTab/types';
import { DepartmentType } from '@/types/configuration';
import { UsersGridRow } from '@/types/userInfo';

const mockRow: UsersGridRow = {
	color: '',
	departmentfullname: '',
	email: '',
	fullName: '',
	firstName: '',
	lastName: '',
	location: '',
	position: '',
	positionKey: '',
	group: '',
	showSkeleton: true,
	isActive: true,
	disabled: false,
	id: 0,
};

export const useUsersRows = ({
	items,
	dataUpdatedAt,
	page,
	rowsPerPage,
	showSkeleton,
	handleEdit,
	handlePageOpen,
	hasUsersModuleAccess,
	hasManageAllInternsClaim,
}: UsersRowsParams): UsersGridRow[] => {
	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const mapDepartments = useCallback((options: Array<DepartmentType>) => {
		const res: {
			[key: string]: string;
		} = {};

		if (!options) {
			return {};
		}

		options.forEach((item) => {
			res[item.id] = item.fullName;
		});

		return res;
	}, []);

	const correctDepartments = useMemo(
		() => mapDepartments(departmentTypes),
		[departmentTypes]
	);

	return useMemo(
		() =>
			showSkeleton || !items
				? new Array(items?.length ?? DEFAULT_GRID_SKELETON_ROWS_AMOUNT)
						.fill(mockRow)
						.map((row, index) => ({ ...row, id: -index * 9999 }))
				: items.map(
						({
							id,
							color,
							fullName,
							fullNameRu,
							firstName,
							lastName,
							email,
							rightPosition,
							position,
							departmentId,
							location,
							isActive,
							resourceGroupName,
						}) => ({
							fullName: mergeUserFullNames(fullName, fullNameRu),
							id,
							firstName,
							lastName,
							email,
							position: rightPosition,
							positionKey: position,
							departmentfullname: correctDepartments[departmentId as string],
							location,
							color,
							handleEditClick:
								hasUsersModuleAccess ||
								(hasManageAllInternsClaim && isIntern(position as Roles))
									? handlePageOpen
									: handleEdit,
							showSkeleton,
							isActive,
							disabled: !isActive,
							resourceGroupName: resourceGroupName ?? '',
						})
				  ),
		[
			dataUpdatedAt,
			page,
			rowsPerPage,
			showSkeleton,
			hasUsersModuleAccess,
			hasManageAllInternsClaim,
			correctDepartments,
			handleEdit,
			handlePageOpen,
		]
	);
};

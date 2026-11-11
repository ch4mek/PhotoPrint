import { useMemo } from 'react';

import { EMPTY_GRID_CELL_VALUE } from '@/components/Grid/constants';
import { useConfiguration } from '@/context/Configuration';
import { formatDateString } from '@/helpers/dateToISOString';
import { getFullName } from '@/helpers/utils';
import { ByPageResponse } from '@/queries/types';
import {
	AssignmentHistoryGridRow,
	AssignmentsHistoryItem,
} from '@/types/equipment';

export const useAssignmentsHistoryRows = (
	data: ByPageResponse<AssignmentsHistoryItem>,
	page: number,
	rowsPerPage: number,
	handleEditClick: (equipmentId?: string | number) => () => void
): AssignmentHistoryGridRow[] => {
	const {
		configurableValues: { equipmentTypes, departmentTypes },
	} = useConfiguration();

	return useMemo(
		() =>
			data?.items
				? data.items.map(
						({
							id,
							userId,
							equipmentId,
							equipmentTypeId,
							equipmentName,
							isAssigned,
							userDepartmentId,
							serialNumber,
							inventoryNumber,
							userLastName,
							userFirstName,
							issueDate,
							returnDate,
						}) => {
							const equipmentType = equipmentTypes.find(
								(type) => type.id === equipmentTypeId
							)?.fullName;

							const department = departmentTypes.find(
								(dep) => dep.id === userDepartmentId
							)?.fullName;

							return {
								id,
								userId,
								equipmentId,
								serialNumber,
								inventoryNumber,
								equipmentName,
								isAssigned,
								equipmentTypeId: equipmentType ?? equipmentTypeId,
								userFullName: getFullName(userFirstName, userLastName),
								userDepartmentId: department ?? EMPTY_GRID_CELL_VALUE,
								issueDate: formatDateString(issueDate, EMPTY_GRID_CELL_VALUE),
								returnDate: formatDateString(returnDate, EMPTY_GRID_CELL_VALUE),
								handleEditClick,
							};
						}
				  )
				: [],
		[data, page, rowsPerPage, equipmentTypes, departmentTypes]
	);
};

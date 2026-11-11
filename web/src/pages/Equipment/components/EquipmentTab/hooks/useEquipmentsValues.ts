import numeral from 'numeral';
import { useMemo } from 'react';

import { EMPTY_GRID_CELL_VALUE } from '@/components/Grid/constants';
import { NUMERAL_PRICE_FORMAT } from '@/constants/equipment';
import { Roles } from '@/constants/roles';
import { useConfiguration } from '@/context/Configuration';
import { formatDateString } from '@/helpers/dateToISOString';
import { Dayjs } from '@/lib/dayjs';
import { ByPageResponse } from '@/queries/types';
import { EquipmentGridRow, EquipmentItemResponse } from '@/types/equipment';

export const useEquipmentsRows = (
	data: ByPageResponse<EquipmentItemResponse>,
	dataUpdatedAt: Dayjs | Date | number,
	page: number,
	rowsPerPage: number,
	handleEdit: (equipmentId?: string | number) => () => void,
	role: Roles
): EquipmentGridRow[] => {
	const {
		configurableValues: { equipmentTypes, departmentTypes },
	} = useConfiguration();

	return useMemo(
		() =>
			data?.items
				? data.items.map(
						({
							id,
							name,
							equipmentTypeId,
							purchasePriceUsd,
							approver,
							comment,
							activeHolder,
							guaranteeDate,
							location,
							serialNumber,
							inventoryNumber,
							invoiceNumber,
							characteristics,
							purchaseDate,
							purchasePlace,
							status,
							jiraTask,
							storage,
							repairPlace,
						}) => {
							const equipmentType = equipmentTypes.find(
								(type) => type.id === equipmentTypeId
							)?.fullName;

							const department = departmentTypes.find(
								(dep) => dep.id === activeHolder?.departmentId
							)?.fullName;

							return {
								id,
								serialNumber,
								inventoryNumber,
								invoiceNumber,
								status,
								name,
								storage,
								equipmentTypeId: equipmentType ?? equipmentTypeId,
								purchasePriceUsd:
									numeral(purchasePriceUsd).format(NUMERAL_PRICE_FORMAT),
								activeHolderFullName:
									activeHolder?.fullName ?? EMPTY_GRID_CELL_VALUE,
								activeHolderDepartmentId:
									department ??
									activeHolder?.departmentId ??
									EMPTY_GRID_CELL_VALUE,
								equipmentCharacteristics: characteristics,
								purchasePlace,
								approverFullName: approver.fullName ?? EMPTY_GRID_CELL_VALUE,
								purchaseDate: formatDateString(purchaseDate),
								guaranteeDate: formatDateString(guaranteeDate),
								location,
								comment: comment || EMPTY_GRID_CELL_VALUE,
								handleEditClick: handleEdit,
								role,
								jiraTask,
								repairPlace,
							};
						}
				  )
				: [],
		[dataUpdatedAt, page, rowsPerPage, equipmentTypes, departmentTypes]
	);
};

import { useMemo } from 'react';

import { EMPTY_GRID_CELL_VALUE } from '@/components/Grid/constants';
import { AssignStatus, assignStatusLabel } from '@/constants/equipment';
import { formatDateString } from '@/helpers/dateToISOString';
import { UserEquipmentGridRow, UserEquipmentItem } from '@/types/equipment';

export function useEquipmentsRows(
	data?: UserEquipmentItem[]
): UserEquipmentGridRow[] {
	const rows: UserEquipmentGridRow[] = useMemo(() => {
		if (!data || !data.length) {
			return [];
		}

		const resRows = data.map(
			({ isAssigned, issueDate, returnDate, ...resProps }) => ({
				...resProps,
				issueDate: formatDateString(issueDate, EMPTY_GRID_CELL_VALUE),
				returnDate: formatDateString(returnDate, EMPTY_GRID_CELL_VALUE),
				isAssigned:
					assignStatusLabel[
						isAssigned ? AssignStatus.Assigned : AssignStatus.Returned
					],
				disabled: !isAssigned,
			})
		);

		return resRows;
	}, [data]);

	return rows;
}

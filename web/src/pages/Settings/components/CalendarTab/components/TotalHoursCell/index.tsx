import { RenderCellProps } from 'react-data-grid';

import { PERIOD } from '@/constants/date';
import { WorkingHoursCalendarRow } from '@/types/settings';

import { TotalCellWrapper } from './styles';

export function TotalHoursCell({
	row: { timeMap, disabled },
}: RenderCellProps<WorkingHoursCalendarRow, unknown>) {
	const totalHours = PERIOD.reduce(
		(acc, curr, index) => acc + (timeMap.get(index + 1)?.hours || 0),
		0
	);

	return <TotalCellWrapper disabled={disabled}>{totalHours}</TotalCellWrapper>;
}

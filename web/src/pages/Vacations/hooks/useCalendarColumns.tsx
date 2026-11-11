import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import { nameCellRenderer } from '../renderers/nameCellRenderer';
import { vacationCellRenderer } from '../renderers/vacationCellRenderer';

import { getColumnFactory } from '@/components/Grid';

export const useCalendarColumns = (year: number, month: number) => {
	const daysInMonth = useMemo(
		() => new Date(year, month, 0).getDate(),
		[year, month]
	);

	const headers: Array<Column<any, unknown>> = useMemo(
		() => [
			getColumnFactory({
				key: 'name',
				name: 'Employee',
				renderCell: (props) => nameCellRenderer(props),
				width: 200,
			}),
			...Array.from({ length: daysInMonth }, (_, i) => {
				const day = i + 1;
				const date = new Date(year, month - 1, day);
				const dayName = date.toLocaleDateString('en-US', { weekday: 'short' });

				return getColumnFactory({
					key: `day${day}`,
					name: `${day} ${dayName}`,
					renderCell: vacationCellRenderer(day, year, month),
				});
			}),
		],
		[daysInMonth]
	);

	return headers;
};

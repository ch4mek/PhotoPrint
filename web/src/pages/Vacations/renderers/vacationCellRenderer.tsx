import { RenderCellProps } from 'react-data-grid';

import { LEAVE_TYPE_COLORS, LEAVE_TYPE_LABELS } from '../constants';
import { VacationTypeText, VacationTypeWrapper } from '../styles';
import { EmployeeVacations, LeaveDay } from '../types';
import { darkenColor } from '../utils';

export const vacationCellRenderer = (
	day: number,
	year: number,
	month: number
) =>
	function ({ row }: RenderCellProps<EmployeeVacations, unknown>) {
		const currentDate = `${String(day).padStart(2, '0')}.${String(
			month
		).padStart(2, '0')}.${year}`;

		const leaveDay = row.leaveDays?.find(
			(leave: LeaveDay) => leave.date === currentDate
		);

		if (!leaveDay) {
			return null;
		}

		const isFirstDayInBlock = (() => {
			const currentIndex = row.leaveDays.findIndex(
				(leave: LeaveDay) => leave.date === currentDate
			);

			if (currentIndex === -1) {
				return false;
			}

			const previousDate = row.leaveDays[currentIndex - 1]?.date;
			const previousType = row.leaveDays[currentIndex - 1]?.type;

			if (!previousDate || !previousType || previousType !== leaveDay.type) {
				return true;
			}

			const [prevDay, prevMonth, prevYear] = previousDate
				.split('.')
				.map(Number);

			return (
				new Date(prevYear, prevMonth - 1, prevDay).getTime() !==
				new Date(year, month - 1, day - 1).getTime()
			);
		})();

		const backgroundColor = LEAVE_TYPE_COLORS[leaveDay.type] || '#d3d3d3';
		const textColor = darkenColor(backgroundColor, 50);

		return (
			<VacationTypeWrapper
				data-vacation="true"
				backgroundColor={backgroundColor}
				textColor={textColor}
			>
				{isFirstDayInBlock && (
					<VacationTypeText>
						{LEAVE_TYPE_LABELS[leaveDay.type] || leaveDay.type}
					</VacationTypeText>
				)}
			</VacationTypeWrapper>
		);
	};

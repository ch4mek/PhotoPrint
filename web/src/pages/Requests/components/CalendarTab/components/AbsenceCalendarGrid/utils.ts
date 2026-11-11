import { checkIsDocumentRequest } from '@/helpers/absences';
import { getDepartmentName } from '@/helpers/getDepartmentName';
import dayjs, { Dayjs } from '@/lib/dayjs';
import {
	InquiryCalendarItem,
	InquiryCalendarItemWithLevel,
	InquiryCalendarGroupRow,
	InquiryCalendarInfo,
} from '@/types/absences';
import { ComplexConfigurableConfigurationItem } from '@/types/configuration';

import { GRID_SCROLLBAR_WIDTH } from './constants';

export const getRowKey = (row: InquiryCalendarGroupRow) => row.userId;

export const getMinimalRequiredGridLength = (
	periodLength: number,
	minimalColumnWidth: number,
	fixedColumnsWidth: number
) => fixedColumnsWidth + periodLength * minimalColumnWidth;

export const getColumnWidthFromGridOffsetWidth = (
	gridOffsetWidth: number,
	periodLength: number,
	fixedColumnsWidth: number
) =>
	(gridOffsetWidth - GRID_SCROLLBAR_WIDTH - fixedColumnsWidth) / periodLength;

function assignRowLevels(
	inquiries: InquiryCalendarItem[]
): InquiryCalendarItemWithLevel[] {
	const sortedInquiries = [...inquiries].sort(
		(a, b) => dayjs(a.dateFrom).valueOf() - dayjs(b.dateFrom).valueOf()
	);

	const occupiedSlots: Record<number, Set<number>> = {};

	const findAvailableLevel = (days: number[]): number => {
		let level = 0;

		const isLevelOccupied = (lvl: number) =>
			days.some((day) => occupiedSlots[day]?.has(lvl));

		while (isLevelOccupied(level)) {
			level += 1;
		}

		return level;
	};

	return sortedInquiries.reduce<InquiryCalendarItemWithLevel[]>(
		(acc, inquiry) => {
			const startDay = dayjs(inquiry.dateFrom).date();
			const endDay = dayjs(inquiry.dateTo).date();

			const days = Array.from(
				{ length: endDay - startDay + 1 },
				(_, i) => startDay + i
			);

			const level = findAvailableLevel(days);

			days.forEach((day) => {
				if (!occupiedSlots[day]) {
					occupiedSlots[day] = new Set();
				}
				occupiedSlots[day].add(level);
			});

			acc.push({ ...inquiry, level });

			return acc;
		},
		[]
	);
}

function clampInquiryToMonth(
	inquiry: InquiryCalendarItem,
	monthStart: Dayjs,
	monthEnd: Dayjs
): (InquiryCalendarItem & { isStartPart: boolean; isEndPart: boolean }) | null {
	const start = dayjs(inquiry.dateFrom);
	const end = dayjs(inquiry.dateTo);

	const startsBeforeMonth = start.isBefore(monthStart);
	const endsAfterMonth = end.isAfter(monthEnd);

	const clampedStart = startsBeforeMonth ? monthStart : start;
	const clampedEnd = endsAfterMonth ? monthEnd : end;

	if (clampedStart.isAfter(clampedEnd)) {
		return null;
	}

	return {
		...inquiry,
		dateFrom: clampedStart.toISOString(),
		dateTo: clampedEnd.toISOString(),
		isStartPart: endsAfterMonth,
		isEndPart: startsBeforeMonth,
	};
}

export function prepareRow(
	row: InquiryCalendarInfo<InquiryCalendarItem>,
	period: Date[],
	departments: ComplexConfigurableConfigurationItem[]
): InquiryCalendarGroupRow {
	const monthStart = dayjs(period[0]).startOf('day');
	const monthEnd = dayjs(period[period.length - 1]).endOf('day');

	const filteredInquiries = row.inquiries.filter(
		({ inquiryType }) => !checkIsDocumentRequest(inquiryType)
	);

	const normalizedInquiries = filteredInquiries
		.map((inquiry) => clampInquiryToMonth(inquiry, monthStart, monthEnd))
		.filter(Boolean) as InquiryCalendarItem[];

	const inquiries = assignRowLevels(normalizedInquiries);

	const maxLevel =
		inquiries.length > 0 ? Math.max(...inquiries.map((i) => i.level)) + 1 : 1;

	return {
		...row,
		inquiries,
		maxLevel,
		department: getDepartmentName(departments, row.departmentId),
	};
}

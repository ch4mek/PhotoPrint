import { PERIOD } from '@/constants/date';
import { TOTAL_ROW_ID } from '@/pages/Payload/contants';
import {
	KpiColors,
	PayloadGridRow,
	TotalMonth,
	TotalRow,
} from '@/pages/Payload/types';

import { formatTotalHoursAndFTEValues } from './formatHoursAndFTEValues';

export function getTotalRow(totalData: TotalMonth[], isShowSkeleton: boolean) {
	if (!totalData) {
		return {} as PayloadGridRow;
	}

	const formattedTotalData = totalData.map(formatTotalHoursAndFTEValues);

	const months = PERIOD.reduce((acc, month, index) => {
		const totalDetailForMonth = formattedTotalData.find(
			(item) => item.monthOfYear === index + 1
		);
		const key = month.toLowerCase() as keyof TotalRow;
		acc[key] = totalDetailForMonth || {
			monthOfYear: index + 1,
			totalActualFte: 0,
			totalFactBillableInHours: 0,
			totalKpi: 0,
			totalPlannedFte: 1,
			totalPlannedWorkloadInHours: 0,
			totalTimePerUsers: 0,
			usersCount: 0,
			color: KpiColors.Red,
		};

		return acc;
	}, {} as TotalRow);

	return {
		jan: months.jan,
		feb: months.feb,
		mar: months.mar,
		apr: months.apr,
		may: months.may,
		jun: months.jun,
		jul: months.jul,
		aug: months.aug,
		sep: months.sep,
		oct: months.oct,
		nov: months.nov,
		dec: months.dec,
		color: '',
		comments: null,
		department: '',
		employee: {
			firstName: '',
			lastName: '',
			firstNameRu: '',
			lastNameRu: '',
			specializationName: '',
			specializationSeniority: '',
			isActive: false,
		},
		id: TOTAL_ROW_ID,
		needProject: false,
		plan: null,
		projects: [],
		year: 0,
		isShowSkeleton,
		group: null,
		disabled: false,
		isLastRow: false,
		...months,
	} as PayloadGridRow;
}

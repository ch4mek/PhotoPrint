import { DateRangeFilter } from '@/components/Grid/modals/types';
import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';

export const convertDateRangeToQueryParam = (
	dateRange: DateRangeFilter,
	paramName: string
) => {
	const params: string[] = [];
	const { includeNull, fromDate, toDate } = dateRange;

	if (includeNull) {
		params.push(`${paramName} eq null`);
	}

	if (toDate) {
		params.push(
			`date(${paramName}) le ${dayjs(toDate).format(
				YYYY_MM_DD_DAYJS_DATE_FORMAT
			)}`
		);
	}

	if (fromDate) {
		params.push(
			`date(${paramName}) ge ${dayjs(fromDate).format(
				YYYY_MM_DD_DAYJS_DATE_FORMAT
			)}`
		);
	}

	return {
		and: params,
	};
};

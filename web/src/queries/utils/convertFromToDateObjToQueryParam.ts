import { FromToProps } from '@/components/Grid/modals/types';
import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';

export const convertFromToDateObjToQueryParam = (
	date: FromToProps<Date> | undefined,
	paramName: string
) => {
	const params: string[] = [];

	if (!date) {
		return params;
	}

	const { from: fromDate, to: toDate } = date;

	if (fromDate) {
		params.push(
			`date(${paramName}) ge ${dayjs(fromDate).format(
				YYYY_MM_DD_DAYJS_DATE_FORMAT
			)}`
		);
	}

	if (toDate) {
		params.push(
			`date(${paramName}) le ${dayjs(toDate).format(
				YYYY_MM_DD_DAYJS_DATE_FORMAT
			)}`
		);
	}

	return params;
};

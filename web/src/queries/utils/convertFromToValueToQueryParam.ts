import { FromToProps } from '@/components/Grid';
import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';

export const convertDateFromToValueToQueryParam = (
	fromToValue: FromToProps<Date> | undefined,
	propertyName: string
) => [
	fromToValue?.from &&
		`date(${propertyName}) ge ${dayjs(fromToValue.from).format(
			YYYY_MM_DD_DAYJS_DATE_FORMAT
		)}`,
	fromToValue?.to &&
		`date(${propertyName}) le ${dayjs(fromToValue.to).format(
			YYYY_MM_DD_DAYJS_DATE_FORMAT
		)}`,
];

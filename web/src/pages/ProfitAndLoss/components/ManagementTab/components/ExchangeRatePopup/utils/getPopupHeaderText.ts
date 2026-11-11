import { DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';

export const getPopupHeaderText = (exchangeRateDate: string | null) => {
	const formattedDate = exchangeRateDate
		? dayjs(exchangeRateDate).format(DATE_FORMAT)
		: dayjs().format(DATE_FORMAT);

	return `The official exchange rate set by the National Bank of the Republic of Belarus on ${formattedDate}`;
};

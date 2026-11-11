import { Dayjs } from '@/lib/dayjs';

export interface ExchangeRatePopupProps {
	date: Dayjs;
	handleClose: VoidFunction;
}

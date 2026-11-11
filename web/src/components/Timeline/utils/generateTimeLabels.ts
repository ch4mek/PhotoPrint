import { isNeedZero } from '@/helpers/dateToISOString';

export const generateTimeLabels = (startHour: number, amount: number) =>
	Array(amount)
		.fill('')
		.map((_, index) => {
			const hour = startHour + index;
			const formattedHour = isNeedZero(hour);

			return `${formattedHour}:00`;
		});

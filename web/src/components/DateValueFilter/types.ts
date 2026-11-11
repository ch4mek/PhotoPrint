import { Dayjs } from '@/lib/dayjs';

export type DateValueFilterProps = {
	date: Dayjs;
	onDateChange: (date: Dayjs) => void;
	onChange?: VoidFunction;
	shouldBeClosedOnOutsideScroll?: boolean;
};

import { Dayjs } from '@/lib/dayjs';

export type DateValueFilterControlsProps = {
	date: Dayjs;
	onDateChange: (date: Dayjs) => void;
	onReset: VoidFunction;
	isResetDisabled: boolean;
};

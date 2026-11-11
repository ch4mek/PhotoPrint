import { WithMobileView } from '@/types/filter';
import { TimePeriod } from '@/types/index';
import { NavigatorDate } from '@/types/report';

export interface DateSelectorProps extends WithMobileView {
	date: NavigatorDate;
	timePeriod: TimePeriod;
	setTimePeriod: (newValue: TimePeriod) => void;
	setDate: (value: NavigatorDate | any) => void;
	showToast: () => void;
	shouldBeClosedOnOutsideScroll?: boolean;
}

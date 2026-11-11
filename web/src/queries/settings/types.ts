import { WorkingHoursPerMonth } from '@/types/settings';

export type CreateCalendarTimeParams = Omit<WorkingHoursPerMonth, 'id'>;

export type UpdateCalendarTimeParams = Pick<
	WorkingHoursPerMonth,
	'id' | 'hours'
>;

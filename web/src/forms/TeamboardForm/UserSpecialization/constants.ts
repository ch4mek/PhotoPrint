import dayjs from '@/lib/dayjs';
import { UserSpecializationFormValues } from '@/types/teamboard';

export const DEFAULT_INITIAL_VALUES: UserSpecializationFormValues = {
	date: dayjs(),
	specialization: null,
};

export const FORM_NAME = 'Teamboard user specialization form';

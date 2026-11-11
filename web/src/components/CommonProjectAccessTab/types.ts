import { DateRangeFilter } from '../Grid/modals/types';

import { FormUserAutoselectOption } from '@/types/forms';

export type DateRangeModalFilter = DateRangeFilter & {
	areDatePickersActive: boolean;
};
export interface ProjectAccessTabProps {
	user: FormUserAutoselectOption;
	isUserTab?: boolean;
	applyDefaultSorting?: boolean;
}

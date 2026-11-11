import { SALARY_COLUMN_KEYS } from '../../SalaryTable/constants';

import { MAX_ROWS_PER_PAGE_INT } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';

export const sectionQueryParams = {
	skip: 0,
	top: MAX_ROWS_PER_PAGE_INT,
	orderBy: `${SALARY_COLUMN_KEYS.date} ${SortDirection.DESC}`,
};

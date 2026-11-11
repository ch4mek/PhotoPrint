import { useMemo } from 'react';

import { UserSalaryRow } from '../types';

import { DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';
import { UserSalary } from '@/types/userInfo';

export function useSalaryRows(data?: UserSalary[]) {
	const rows: UserSalaryRow[] = useMemo(() => {
		if (!data || !data.length) {
			return [];
		}

		return data.map(({ id, salaryFrom, salaryUsd, isCurrent }) => ({
			id,
			salaryFrom: dayjs(salaryFrom).format(DATE_FORMAT),
			salaryUsd: `${salaryUsd}$`,
			isCurrent,
		}));
	}, [data]);

	return rows;
}

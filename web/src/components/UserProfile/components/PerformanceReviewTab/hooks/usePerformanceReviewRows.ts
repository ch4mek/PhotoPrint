import { useEffect, useState } from 'react';

import {
	UserPefromanceReviewRowsHookReturn,
	UserPerformanceReviewGridRow,
} from '../types';
import { mapPerformanceReviewDataToRows } from '../utils';

import { UserPerformanceReviewData } from '@/types/teamboard';

export function usePerformanceReviewRows(
	data?: UserPerformanceReviewData[]
): UserPefromanceReviewRowsHookReturn {
	const [rows, setRows] = useState<UserPerformanceReviewGridRow[]>([]);

	useEffect(() => {
		if (data) {
			setRows(mapPerformanceReviewDataToRows(data));
		}
	}, [data]);

	return [rows, setRows];
}

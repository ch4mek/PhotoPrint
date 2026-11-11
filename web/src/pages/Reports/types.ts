import { months } from '@/constants/date';
import { TrackedTimeType } from '@/types/index';

export interface ReportsQueryParams {
	year: number;
	monthOfYear: string[];
	trackedTimeType: TrackedTimeType;
	departmentId?: string;
	file?: string;
	clientId?: number;
	projectId?: number;
	groupId?: number | string;
	userId?: number;
	filter?: string;
	pageNo?: number;
	pageSize?: string | string[];
}

export type Month = (typeof months)[number];

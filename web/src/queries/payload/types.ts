import { SortDirection } from '@/constants/sort';

export interface PayloadQueryParams {
	year: number;
	order: SortDirection;
	orderBy: string;
	specializationTitle: string;
	pageSize?: number;
}

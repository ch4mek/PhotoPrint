import { TagTypes } from '@/components/DateTag/types';

export interface PerformanceReviewUserData {
	id: number;
	fullName: string;
}

export interface PerformanceReviewDate {
	date: string;
	performanceReviewDateType: TagTypes;
	performanceReviewId: number;
}

export type HandleRowClick<T> = (data: T) => void;

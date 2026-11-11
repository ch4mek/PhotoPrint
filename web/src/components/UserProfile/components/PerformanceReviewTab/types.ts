import { Dispatch, SetStateAction } from 'react';

import { TagTypes } from '@/components/DateTag/types';
import { FromToProps } from '@/components/Grid';
import { DynamicHeightCellDefaultProps } from '@/components/Grid/renderers/types';
import { ConversationDocument } from '@/types/document';

export interface UserPerformanceReviewGridRow
	extends DynamicHeightCellDefaultProps {
	id: number;
	performanceReviewDate: string;
	performanceReviewDateType: TagTypes;
	agreements: string;
	files: ConversationDocument[];
	links: string[];
}

export type UserPefromanceReviewRowsHookReturn = [
	Array<UserPerformanceReviewGridRow>,
	Dispatch<SetStateAction<Array<UserPerformanceReviewGridRow>>>
];

type UserPerformanceReviewGridColumn = Omit<
	UserPerformanceReviewGridRow,
	'performanceReviewDateType' | 'id'
>;

export type UserPerformanceReviewGridConstants<T> = {
	[key in keyof UserPerformanceReviewGridColumn]: T;
};

export type UserPerformanceReviewGridColumnsNames = {
	key: keyof UserPerformanceReviewGridColumn;
};

export enum UserPerformanceReviewFilterAction {
	Reset = 'reset',
	Date = 'performanceReviewDate',
	Agreements = 'agreements',
}

export interface UserPerformanceReviewFilters {
	[UserPerformanceReviewFilterAction.Date]: FromToProps<Date>;
	[UserPerformanceReviewFilterAction.Agreements]: string;
}

export type UserPerformanceReviewReducerAction = {
	[K in UserPerformanceReviewFilterAction]: {
		type: K;
		payload: K extends keyof UserPerformanceReviewFilters
			? UserPerformanceReviewFilters[K]
			: UserPerformanceReviewFilters;
	};
}[UserPerformanceReviewFilterAction];

export type UserPerformanceReviewFiltersHandlers = {
	[K in UserPerformanceReviewFilterAction]: K extends keyof UserPerformanceReviewFilters
		? Dispatch<SetStateAction<UserPerformanceReviewFilters[K]>>
		: VoidFunction;
};

export type UserPerformanceReviewFiltersReducer = [
	UserPerformanceReviewFilters,
	UserPerformanceReviewFiltersHandlers
];

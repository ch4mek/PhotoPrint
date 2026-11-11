import { ReactNode } from 'react';

import { GridCellDefaultProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { GridColumnWidthsConfiguration, HandleRowClick } from '@/types/grid';
import { PerformanceReviewDate } from '@/types/performanceReview';

import { HandleTagChoose } from './components/datesRenderCell/types';

export interface PerformanceReviewGridRow extends GridCellDefaultProps {
	id: number;
	fullName: string;
	dates: Array<
		PerformanceReviewDate & {
			conversationId?: number;
		}
	>;
}

export interface PerformanceReviewData {
	date: string;
	performanceReviewId: number;
}

interface PerformanceReviewFilters {
	id: string;
	fullName: string;
	departments: string[];
}

export type PerformanceReviewTableFilters = PerformanceReviewFilters;

interface ReviewAction<Tkeys, Tstate> {
	type: Tkeys;
	payload?: string[] | string | Tstate;
}

export enum PerformanceReviewTableFiltersKeys {
	id = 'id',
	fullName = 'fullName',
	departments = 'departments',
	set = 'set',
	reset = 'reset',
}

export type PerformanceReviewAction = ReviewAction<
	PerformanceReviewTableFiltersKeys,
	PerformanceReviewTableFilters
>;

interface PerformanceReviewFilterSetter {
	id: (name: string) => void;
	fullName: (id: string) => void;
	departments: (payload: string[]) => void;
	set: (allFilters: PerformanceReviewTableFilters) => void;
	reset: VoidFunction;
}

export type PerformanceReviewsFilterSetter = PerformanceReviewFilterSetter;

export interface PerformanceReviewOptions {
	users: string[];
	departments: Array<SelectionModalOption>;
}

export interface UseGetGridColumnsProps {
	filters: PerformanceReviewTableFilters;
	filterSetter: PerformanceReviewsFilterSetter;
	options: PerformanceReviewOptions;
	gridColumnsWidths: GridColumnWidthsConfiguration;
	handleTagChoose: HandleTagChoose;
	handleIconClick: HandleRowClick<PerformanceReviewGridRow>;
	handleContentClick: HandleRowClick<PerformanceReviewGridRow>;
	enableCellClick: (row: PerformanceReviewGridRow) => boolean;
	infoDateTagContent?: ReactNode;
}

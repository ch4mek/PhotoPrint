import { Dispatch, SetStateAction } from 'react';

import { FromToProps } from '@/components/Grid';
import { GridCellDefaultProps } from '@/components/Grid/renderers/types';

export interface UserSalaryRow extends GridCellDefaultProps {
	salaryFrom: string;
	salaryUsd: string;
	isCurrent?: boolean;
}

export type UserSalaryGridConstants<T> = {
	[key in keyof UserSalaryRow]: T;
};

export enum UserSalaryFilterActions {
	RESET = 'reset',
	SALARY_FROM = 'salaryFrom',
	SALARY_USD = 'salaryUsd',
}

export interface UserSalaryFilters {
	[UserSalaryFilterActions.SALARY_FROM]: FromToProps<Date>;
	[UserSalaryFilterActions.SALARY_USD]: FromToProps<string>;
}

export type UserSalaryReducerAction = {
	[K in UserSalaryFilterActions]: {
		type: K;
		payload: K extends keyof UserSalaryFilters
			? UserSalaryFilters[K]
			: UserSalaryFilters;
	};
}[UserSalaryFilterActions];

export type UserSalaryFiltersHandlers = {
	[K in UserSalaryFilterActions]: K extends keyof UserSalaryFilters
		? Dispatch<SetStateAction<UserSalaryFilters[K]>>
		: VoidFunction;
};

export type UserSalaryFiltersReducer = [
	UserSalaryFilters,
	UserSalaryFiltersHandlers
];

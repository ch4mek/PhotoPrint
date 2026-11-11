import { Dispatch, SetStateAction } from 'react';

export interface Filter<T = string | number> {
	id: T;
	name: string;
}

export interface WithMobileView {
	isMobileView?: boolean;
}

export type FilterReducerAction<
	TActions extends string | number | symbol,
	TFilters
> = {
	[K in TActions]: {
		type: K;
		payload: K extends keyof TFilters ? TFilters[K] : TFilters;
	};
}[TActions];

export type FiltersHandlers<
	TActions extends string | number | symbol,
	TFilters
> = {
	[K in TActions]: K extends keyof TFilters
		? Dispatch<SetStateAction<TFilters[K]>>
		: VoidFunction;
};

export type FilterReducer<TFilters, THandlers> = [TFilters, THandlers];

export type PrimitiveFilterValue =
	| number
	| string
	| boolean
	| null
	| undefined
	| symbol
	| bigint;
